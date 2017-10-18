using Monkeyspeak.lexical;
using Monkeyspeak.Libraries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Monkeyspeak
{
    [Serializable]
    public class TypeNotSupportedException : Exception
    {
        public TypeNotSupportedException()
        {
        }

        public TypeNotSupportedException(string message)
            : base(message)
        {
        }

        public TypeNotSupportedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TypeNotSupportedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Used for handling triggers at runtime.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>True = Continue to the Next Trigger, False = Stop executing current block of Triggers</returns>
    public delegate bool TriggerHandler(TriggerReader reader);

    /// <summary>
    ///
    /// </summary>
    /// <param name="trigger">The trigger.</param>
    /// <param name="handler">The handler.</param>
    public delegate void TriggerAddedEventHandler(Trigger trigger, TriggerHandler handler);

    public delegate bool TriggerHandledEventHandler(Trigger trigger);

    /// <summary>
    /// Event for any errors that occur during execution
    /// If not assigned Exceptions will be thrown.
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="ex"></param>
    public delegate void TriggerHandlerErrorEvent(TriggerHandler handler, Trigger trigger, Exception ex);

    /// <summary>
    ///
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    public delegate Token TokenVisitorHandler(ref Token token);

    [Serializable]
    public sealed class Page : IDisposable
    {
        public object syncObj = new Object();

        private Parser parser;
        private List<TriggerBlock> triggerBlocks;
        private Dictionary<string, IVariable> scope;
        private HashSet<BaseLibrary> libraries;

        private Dictionary<Trigger, TriggerHandler> handlers = new Dictionary<Trigger, TriggerHandler>();
        private MonkeyspeakEngine engine;

        public event Action Resetting;

        /// <summary>
        /// Occurs when [initiating].  Best to call your variable additions in the event.
        /// </summary>
        public event Action<Page> Initiating;

        internal TokenVisitorHandler VisitingToken;

        /// <summary>
        /// Called when an Exception is raised during execution
        /// </summary>
        public event TriggerHandlerErrorEvent Error;

        /// <summary>
        /// Called when a Trigger and TriggerHandler is added to the Page
        /// </summary>
        public event TriggerAddedEventHandler TriggerAdded;

        /// <summary>
        /// Called before the Trigger's TriggerHandler is called.  If there is no TriggerHandler for that Trigger
        /// then this event is not raised.
        /// </summary>
        public event TriggerHandledEventHandler BeforeTriggerHandled;

        /// <summary>
        /// Called after the Trigger's TriggerHandler is called.  If there is no TriggerHandler for that Trigger
        /// then this event is not raised.
        /// </summary>
        public event TriggerHandledEventHandler AfterTriggerHandled;

        internal Page(MonkeyspeakEngine engine)
        {
            this.engine = engine;
            parser = new Parser(engine);
            triggerBlocks = new List<TriggerBlock>();
            scope = new Dictionary<string, IVariable>();
            libraries = new HashSet<BaseLibrary>();
            Initiate();
        }

        internal void Initiate()
        {
            SetVariable(new ConstantVariable(Variable.NoValue));
            SetVariable(new ConstantVariable("%MONKEY", engine.Banner));
            SetVariable(new ConstantVariable("%VERSION", engine.Options.Version.ToString(2)));
            LoadLibrary(Attributes.Instance);
            Initiating?.Invoke(this);
        }

        internal void GenerateBlocks(AbstractLexer lexer)
        {
            parser.VisitToken = VisitingToken;
            AddBlocks(parser.Parse(lexer));
            parser.VisitToken = null;
        }

        internal void AddBlocks(IEnumerable<TriggerBlock> blocks)
        {
            lock (syncObj)
            {
                var blocksArray = blocks.ToArray();
                for (int i = blocksArray.Length - 1; i >= 0; i--)
                    Size += blocksArray[i].Count;
                triggerBlocks.AddRange(blocksArray);
            }
            if (Size > engine.Options.TriggerLimit)
            {
                throw new MonkeyspeakException("Trigger limit exceeded.");
            }
        }

        private void TrimToLimit(int limit)
        {
            lock (syncObj)
            {
                for (int i = triggerBlocks.Count - 1; i >= 0; i--)
                {
                    for (int j = triggerBlocks[i].Count - 1; j >= 0; j--)
                    {
                        var curSize = i + j;
                        if (curSize >= limit)
                        {
                            triggerBlocks[i].RemoveAt(j);
                        }
                    }
                    if (triggerBlocks[i].Count == 0) triggerBlocks.RemoveAt(i);
                }
            }
        }

        internal bool CheckType(object value)
        {
            if (value == null) return true;

            return value is string ||
                   value is double;
        }

        internal MonkeyspeakEngine Engine
        {
            get { return engine; }
        }

        public void LoadCompiledStream(Stream stream)
        {
            try
            {
                Compiler compiler = new Compiler(engine);
                using (stream)
                    AddBlocks(compiler.DecompileFromStream(stream));
            }
            catch (Exception ex)
            {
                throw new MonkeyspeakException("Error reading compiled file.", ex);
            }
        }

        public void CompileToStream(Stream stream)
        {
            try
            {
                Compiler compiler = new Compiler(engine);
                using (stream)
                    compiler.CompileToStream(triggerBlocks, stream);
            }
            catch (Exception ex)
            {
                throw new MonkeyspeakException("Error compiling to file.", ex);
            }
        }

        public void CompileToFile(string filePath)
        {
            try
            {
                CompileToStream(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch (IOException ioex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MonkeyspeakException("Error compiling to file.", ex);
            }
        }

        /// <summary>
        /// Clears all Variables and optionally clears all TriggerHandlers from this Page.
        /// </summary>
        public void Reset(bool resetTriggerHandlers = false)
        {
            Resetting?.Invoke();
            lock (syncObj)
            {
                scope.Clear();
                foreach (var lib in libraries) lib.Unload(this);
                if (resetTriggerHandlers) handlers.Clear();
            }
            Initiate();
        }

        /// <summary>
        /// Removes the triggers.
        /// </summary>
        public void Clear()
        {
            triggerBlocks.Clear();
            Size = 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="excludeLibraryName">[true] hide library name, [false] show library name above triggers</param>
        /// <returns>IEnumerable of Triggers</returns>
        public IEnumerable<string> GetTriggerDescriptions(bool excludeLibraryName = false)
        {
            lock (syncObj)
            {
                foreach (var lib in libraries)
                {
                    yield return lib.ToString(excludeLibraryName);
                }
            }
        }

        public ReadOnlyCollection<IVariable> Scope
        {
            get { return new ReadOnlyCollection<IVariable>(scope.Values.ToArray()); }
        }

        /// <summary>
        /// Loads Monkeyspeak Sys Library into this Page
        /// <para>Used for System operations involving the Environment or Operating System</para>
        /// </summary>
        public void LoadSysLibrary()
        {
            LoadLibrary(new Libraries.Sys());
        }

        /// <summary>
        /// Loads Monkeyspeak String Library into this Page
        /// <para>Used for basic String operations</para>
        /// </summary>
        public void LoadStringLibrary()
        {
            LoadLibrary(new Libraries.StringOperations());
        }

        /// <summary>
        /// Loads Monkeyspeak IO Library into this Page
        /// <para>Used for File Input/Output operations</para>
        /// </summary>
        public void LoadIOLibrary(string authorizedPath = null)
        {
            LoadLibrary(new Libraries.IO(authorizedPath));
        }

        /// <summary>
        /// Loads Monkeyspeak Math Library into this Page
        /// <para>Used for Math operations (add, subtract, multiply, divide)</para>
        /// </summary>
        public void LoadMathLibrary()
        {
            LoadLibrary(new Libraries.Math());
        }

        /// <summary>
        /// Loads Monkeyspeak Timer Library into this Page
        /// </summary>
        public void LoadTimerLibrary(uint timersLimit = 10)
        {
            LoadLibrary(new Libraries.Timers(timersLimit));
        }

        /// <summary>
        /// Loads Monkeyspeak Debug Library into this Page
        /// <para>Used for Debug breakpoint insertion. Won't work without Debugger attached.</para>
        /// </summary>
        public void LoadDebugLibrary()
        {
            LoadLibrary(new Libraries.Debug());
        }

        /// <summary>
        /// Loads a <see cref="Libraries.BaseLibrary"/> into this Page
        /// </summary>
        /// <param name="lib"></param>
        public void LoadLibrary(Libraries.BaseLibrary lib)
        {
            if (libraries.Contains(lib)) return;
            lock (syncObj)
            {
                foreach (var kv in lib.handlers)
                {
                    SetTriggerHandler(kv.Key, kv.Value);
                }
                libraries.Add(lib);
            }
        }

        /// <summary>
        /// Loads trigger handlers from a assembly instance
        /// </summary>
        /// <param name="asm">The assembly instance</param>
        public void LoadLibraryFromAssembly(Assembly asm)
        {
            if (asm == null) return;
            foreach (var types in ReflectionHelper.GetAllTypesWithAttributeInMembers<TriggerHandlerAttribute>(asm))
                foreach (MethodInfo method in types.GetMethods().Where(method => method.IsDefined(typeof(TriggerHandlerAttribute), false)))
                {
                    foreach (TriggerHandlerAttribute attribute in ReflectionHelper.GetAllAttributesFromMethod<TriggerHandlerAttribute>(method))
                    {
                        attribute.owner = method;
                        try
                        {
                            attribute.Register(this);
                        }
                        catch (Exception ex)
                        {
                            throw new MonkeyspeakException(String.Format("Failed to load library from assembly '{0}', couldn't bind to method '{1}.{2}'", asm.FullName, method.DeclaringType.Name, method.Name), ex);
                        }
                    }
                }

            var subType = typeof(BaseLibrary);
            foreach (var type in asm.GetTypes())
            {
                if (type.IsSubclassOf(subType))
                    try
                    {
                        LoadLibrary((BaseLibrary)Activator.CreateInstance(type));
                    }
                    catch (MissingMemberException mme) { throw; }
                    catch (Exception ex) { }
            }
        }

        /// <summary>
        /// Loads trigger handlers from a assembly dll file
        /// </summary>
        /// <param name="assemblyFile">The assembly in the local file system</param>
        public void LoadLibraryFromAssembly(string assemblyFile)
        {
            Assembly asm;
            if (!File.Exists(assemblyFile))
                throw new MonkeyspeakException("Load library from file '" + assemblyFile + "' failed, file not found.");
            else if (!ReflectionHelper.TryLoad(assemblyFile, out asm))
            {
                throw new MonkeyspeakException("Load library from file '" + assemblyFile + "' failed.");
            }

            foreach (var types in ReflectionHelper.GetAllTypesWithAttributeInMembers<TriggerHandlerAttribute>(asm))
                foreach (MethodInfo method in types.GetMethods().Where(method => method.IsDefined(typeof(TriggerHandlerAttribute), false)))
                {
                    foreach (TriggerHandlerAttribute attribute in ReflectionHelper.GetAllAttributesFromMethod<TriggerHandlerAttribute>(method))
                    {
                        attribute.owner = method;
                        try
                        {
                            attribute.Register(this);
                        }
                        catch (Exception ex)
                        {
                            throw new MonkeyspeakException(String.Format("Failed to load library from file '{0}', couldn't bind to method '{1}.{2}'", assemblyFile, method.DeclaringType.Name, method.Name));
                        }
                    }
                }

            var subType = typeof(BaseLibrary);
            foreach (var type in asm.GetTypes())
            {
                if (type.BaseType == subType)
                    try
                    {
                        LoadLibrary((BaseLibrary)Activator.CreateInstance(type));
                    }
                    catch (MissingMemberException mme) { throw; }
                    catch (Exception ex) { }
            }
        }

        /// <summary>
        /// Loads all libraries.
        /// </summary>
        public void LoadAllLibraries()
        {
            foreach (var lib in BaseLibrary.GetAllLibraries())
                LoadLibrary(lib);
        }

        public bool RemoveLibrary<T>() where T : BaseLibrary
        {
            return RemoveAllTriggerHandlers(libraries.OfType<T>().FirstOrDefault()) > 0;
        }

        public T GetLibrary<T>() where T : BaseLibrary
        {
            return libraries.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Removes the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool RemoveVariable(string name)
        {
            lock (syncObj)
            {
                return scope.Remove(name);
            }
        }

        /// <summary>
        /// Sets the variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var">The variable.</param>
        /// <exception cref="Monkeyspeak.TypeNotSupportedException"></exception>
        /// <exception cref="Exception">Variable limit exceeded, operation failed.</exception>
        public void SetVariable<T>(T var) where T : IVariable
        {
            if (!CheckType(var.Value)) throw new TypeNotSupportedException(String.Format("{0} is not a supported type. Expecting string or double.", var.Value.GetType().Name));
            if (var.Name[0] != engine.Options.VariableDeclarationSymbol)
                throw new MonkeyspeakException($"Invalid variable declaration symbol '{var.Name[0]}', expected '{engine.Options.VariableDeclarationSymbol}'");

            lock (syncObj)
            {
                if (scope.ContainsKey(var.Name)) return;
                if (scope.Count + 1 > engine.Options.VariableCountLimit) throw new Exception("Variable limit exceeded, operation failed.");
                scope.Add(var.Name, var);
            }
        }

        /// <summary>
        /// Sets the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="isConstant">if set to <c>true</c> [is constant].</param>
        /// <returns></returns>
        /// <exception cref="Monkeyspeak.TypeNotSupportedException"></exception>
        /// <exception cref="Exception">Variable limit exceeded, operation failed.</exception>
        public IVariable SetVariable(string name, object value, bool isConstant)
        {
            if (!CheckType(value)) throw new TypeNotSupportedException(String.Format("{0} is not a supported type. Expecting string or double.", value.GetType().Name));
            if (name[0] != engine.Options.VariableDeclarationSymbol)
                name = engine.Options.VariableDeclarationSymbol + name;
            IVariable var;

            lock (syncObj)
            {
                if (scope.TryGetValue(name, out IVariable v))
                {
                    var = v;
                    var.Value = value;
                }
                else
                {
                    var = new Variable(name, value, isConstant);
                    if (scope.Count + 1 > engine.Options.VariableCountLimit) throw new Exception("Variable limit exceeded, operation failed.");
                    scope.Add(var.Name, var);
                }
                return var;
            }
        }

        /// <summary>
        /// Sets the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="isConstant">if set to <c>true</c> [is constant].</param>
        /// <returns></returns>
        /// <exception cref="Monkeyspeak.TypeNotSupportedException"></exception>
        /// <exception cref="Exception">Variable limit exceeded, operation failed.</exception>
        public VariableList SetVariableList(string name, object value, bool isConstant)
        {
            if (!CheckType(value)) throw new TypeNotSupportedException(String.Format("{0} is not a supported type. Expecting string or double.", value.GetType().Name));
            if (name[0] != engine.Options.VariableDeclarationSymbol)
                name = engine.Options.VariableDeclarationSymbol + name;
            VariableList var;

            lock (syncObj)
            {
                if (scope.TryGetValue(name, out IVariable v))
                {
                    var = (VariableList)v;
                    var.Value = value;
                }
                else
                {
                    var = new VariableList(name, isConstant, value);
                    if (scope.Count + 1 > engine.Options.VariableCountLimit) throw new Exception("Variable limit exceeded, operation failed.");
                    scope.Add(var.Name, var);
                }
                return var;
            }
        }

        /// <summary>
        /// Gets a Variable with Name set to <paramref name="name"/>
        /// <b>Throws Exception if Variable not found.</b>
        /// </summary>
        /// <param name="name">The name of the Variable to retrieve</param>
        /// <returns>The variable found with the specified <paramref name="name"/> or throws Exception</returns>
        public IVariable GetVariable(string name)
        {
            if (name[0] != engine.Options.VariableDeclarationSymbol)
                name = engine.Options.VariableDeclarationSymbol + name;
            if (name.IndexOf('[') != -1)
                name = name.Substring(0, name.IndexOf('[') - 1);

            lock (syncObj)
            {
                if (scope.TryGetValue(name, out IVariable var))
                    return var;
                throw new Exception("Variable \"" + name + "\" not found.");
            }
        }

        /// <summary>
        /// Checks the scope for the Variable with Name set to <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the Variable to retrieve</param>
        /// <returns>True on Variable found.  <para>False if Variable not found.</para></returns>
        public bool HasVariable(string name)
        {
            if (name[0] != engine.Options.VariableDeclarationSymbol)
                name = engine.Options.VariableDeclarationSymbol + name;
            if (name.IndexOf('[') != -1)
                name = name.Substring(0, name.IndexOf('[') - 1);
            lock (syncObj)
            {
                return scope.ContainsKey(name);
            }
        }

        /// <summary>
        /// Determines whether the specified variable exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="var">The variable.</param>
        /// <returns>
        ///   <c>true</c> if the specified variable exists; otherwise, <c>false</c>.
        /// </returns>
        public bool HasVariable(string name, out IVariable var)
        {
            if (name[0] != engine.Options.VariableDeclarationSymbol)
                name = engine.Options.VariableDeclarationSymbol + name;
            if (name.IndexOf('[') != -1)
                name = name.Substring(0, name.IndexOf('[') - 1);
            lock (syncObj)
            {
                if (!scope.TryGetValue(name, out var))
                {
                    var = Variable.NoValue;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Assigns the TriggerHandler to a trigger with <paramref name="category"/> and <paramref name="id"/>
        /// </summary>
        /// <param name="category"></param>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        /// <param name="description"></param>
        public void SetTriggerHandler(TriggerCategory category, int id, TriggerHandler handler, string description = null)
        {
            SetTriggerHandler(new Trigger(category, id), handler, description);
        }

        /// <summary>
        /// Assigns the TriggerHandler to <paramref name="trigger"/>
        /// </summary>
        /// <param name="trigger"><see cref="Monkeyspeak.Trigger"/></param>
        /// <param name="handler"><see cref="Monkeyspeak.TriggerHandler"/></param>
        /// <param name="description">optional description of the trigger, normally the human readable form of the trigger
        /// <para>Example: "(0:1) when someone says something,"</para></param>
        public void SetTriggerHandler(Trigger trigger, TriggerHandler handler, string description = null)
        {
            Attributes.Instance.AddDescription(trigger, description);
            lock (syncObj)
            {
                if (!handlers.ContainsKey(trigger))
                {
                    handlers.Add(trigger, handler);
                    TriggerAdded?.Invoke(trigger, handler);
                }
                else if (engine.Options.CanOverrideTriggerHandlers)
                {
                    handlers[trigger] = handler;
                }
                else throw new UnauthorizedAccessException($"Override of existing Trigger handler from {handler.Method} for {trigger} set to handler in {handlers[trigger].Method}");
            }
        }

        /// <summary>
        /// Removes the trigger handler.
        /// </summary>
        /// <param name="cat">The category.</param>
        /// <param name="id">The identifier.</param>
        public void RemoveTriggerHandler(TriggerCategory cat, int id)
        {
            handlers.Remove(new Trigger(cat, id));
        }

        /// <summary>
        /// Removes the trigger handler.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void RemoveTriggerHandler(Trigger trigger)
        {
            lock (syncObj)
            {
                handlers.Remove(trigger);
            }
        }

        /// <summary>
        /// Removes all trigger handlers.
        /// </summary>
        /// <param name="lib">The library.</param>
        private int RemoveAllTriggerHandlers(BaseLibrary lib)
        {
            int countRemoved = 0;
            foreach (var handler in lib.handlers)
            {
                if (handlers.Remove(handler.Key)) countRemoved++;
            }
            return countRemoved;
        }

        /// <summary>
        /// Returns the Trigger count on this Page.
        /// </summary>
        /// <returns></returns>
        public int Size
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Page"/> is in debug mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if debug; otherwise, <c>false</c>.
        /// </value>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets the libraries.
        /// </summary>
        /// <value>
        /// The libraries.
        /// </value>
        public IEnumerable<BaseLibrary> Libraries { get => libraries; }

        // Changed id to array for multiple Trigger processing.
        // This Compensates for a Design Flaw Lothus Marque spotted - Gerolkae

        /*
         * [1/7/2013 9:26:22 PM] Lothus Marque: Okay. Said feeling doesn't explain why 48 is
         * happening before 46, since your execute does them in increasing order. But what I
         * was suddenly staring at is that this has the definite potential to "run all 46,
         * then run 47, then run 48" ... and they're not all at once, in sequence.
         */

        private void ExecuteBlock(TriggerBlock triggerBlock, int causeIndex = 0, params object[] args)
        {
            var reader = new TriggerReader(this, triggerBlock)
            {
                Parameters = args
            };

            Trigger current, next;
            int j = 0;
            for (j = causeIndex; j <= triggerBlock.Count - 1; j++)
            {
                current = triggerBlock[j];
                next = triggerBlock.Count > j + 1 ? triggerBlock[j + 1] : Trigger.Undefined;
                handlers.TryGetValue(current, out TriggerHandler handler);

                reader.Trigger = current;
                reader.CurrentBlockIndex = j;
                try
                {
                    if (BeforeTriggerHandled?.Invoke(current) == false) continue;
                    bool canContinue = handler != null ? handler(reader) : false;
                    if (AfterTriggerHandled?.Invoke(current) == false) continue;
                    // if canContinue was assigned by a Condition or Cause, we care, otherwise continue
                    if (!canContinue)
                        switch (current.Category)
                        {
                            case TriggerCategory.Cause:
                                // skip ahead for another condition to meet
                                Trigger possibleCause = Trigger.Undefined;
                                for (int i = j + 1; i <= triggerBlock.Count - 1; i++)
                                {
                                    possibleCause = triggerBlock[i];
                                    if (possibleCause.Category == TriggerCategory.Cause)
                                    {
                                        j = i - 1; // set the current index of the outer loop
                                        break;
                                    }
                                    else possibleCause = Trigger.Undefined;
                                }
                                if (possibleCause.Category == TriggerCategory.Undefined) return;
                                break;

                            case TriggerCategory.Condition:
                                // skip ahead for another condition to meet
                                for (int i = j + 1; i <= triggerBlock.Count - 1; i++)
                                {
                                    Trigger possibleCondition = triggerBlock[i];
                                    if (possibleCondition.Category == TriggerCategory.Condition)
                                    {
                                        j = i - 1; // set the current index of the outer loop
                                        break;
                                    }
                                }
                                break;
                        }
                }
                catch (Exception e)
                {
                    var ex = new MonkeyspeakException(e.Message);
                    if (Error != null)
                        Error(handlers[current], current, ex);
                    else throw ex;

                    break;
                }
            }
        }

        /// <summary>
        /// Executes a trigger block containing TriggerCategory.Cause with ID equal to <paramref name="id" />
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        public void Execute(int id = 0, params object[] args)
        {
            lock (syncObj)
            {
                for (int j = 0; j <= triggerBlocks.Count - 1; j++)
                {
                    int causeIndex;
                    if ((causeIndex = triggerBlocks[j].IndexOfTrigger(TriggerCategory.Cause, id)) != -1)
                    {
                        ExecuteBlock(triggerBlocks[j], causeIndex, args);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a trigger block containing TriggerCategory.Cause with ID equal to <param name="id" />
        ///
        /// </summary>
        /// <param name="ids">I dunno</param>
        /// <param name="args"></param>
        public void Execute(int[] ids, params object[] args)
        {
            for (int i = 0; i <= ids.Length - 1; i++)
            {
                int id = ids[i];
                lock (syncObj)
                {
                    for (int j = 0; j <= triggerBlocks.Count - 1; j++)
                    {
                        int causeIndex;
                        if ((causeIndex = triggerBlocks[j].IndexOfTrigger(TriggerCategory.Cause, id)) != -1)
                        {
                            ExecuteBlock(triggerBlocks[j], causeIndex, args);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken, int id = 0, params object[] args)
        {
            await Task.Run(() =>
            {
                lock (syncObj)
                {
                    for (int j = 0; j <= triggerBlocks.Count - 1; j++)
                    {
                        int causeIndex;
                        if ((causeIndex = triggerBlocks[j].IndexOfTrigger(TriggerCategory.Cause, id)) != -1)
                        {
                            ExecuteBlock(triggerBlocks[j], causeIndex, args);
                        }
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="ids">The ids.</param>
        /// <param name="args">todo: describe args parameter on ExecuteAsync</param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken, int[] ids, params object[] args)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i <= ids.Length - 1; i++)
                {
                    int id = ids[i];
                    lock (syncObj)
                    {
                        for (int j = 0; j <= triggerBlocks.Count - 1; j++)
                        {
                            int causeIndex;
                            if ((causeIndex = triggerBlocks[j].IndexOfTrigger(TriggerCategory.Cause, id)) != -1)
                            {
                                ExecuteBlock(triggerBlocks[j], causeIndex, args);
                            }
                        }
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Executes the specified Cause asynchronously.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(int id = 0, params object[] args)
        {
            await Task.Run(() =>
            {
                lock (syncObj)
                {
                    for (int j = 0; j <= triggerBlocks.Count - 1; j++)
                    {
                        int causeIndex;
                        if ((causeIndex = triggerBlocks[j].IndexOfTrigger(TriggerCategory.Cause, id)) != -1)
                        {
                            ExecuteBlock(triggerBlocks[j], causeIndex, args);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Executes the specified Cause asynchronously.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(int[] ids, params object[] args)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i <= ids.Length - 1; i++)
                {
                    int id = ids[i];
                    lock (syncObj)
                    {
                        for (int j = 0; j <= triggerBlocks.Count - 1; j++)
                        {
                            int causeIndex;
                            if ((causeIndex = triggerBlocks[j].IndexOfTrigger(TriggerCategory.Cause, id)) != -1)
                            {
                                ExecuteBlock(triggerBlocks[j], causeIndex, args);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var lib in libraries)
            {
                lib.Unload(this);
            }
            Clear();
            Reset(true);
        }
    }
}