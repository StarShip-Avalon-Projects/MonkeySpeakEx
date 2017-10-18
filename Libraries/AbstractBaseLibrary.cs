using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Monkeyspeak.Libraries
{
    public abstract class BaseLibrary
    {
        internal Dictionary<Trigger, TriggerHandler> handlers;
        internal Dictionary<Trigger, string> descriptions;

        /// <summary>
        /// Base abstract class for Monkeyspeak Libraries
        /// </summary>
        protected BaseLibrary()
        {
            handlers = new Dictionary<Trigger, TriggerHandler>();
            descriptions = new Dictionary<Trigger, string>();
        }

        /// <summary>
        /// Raises a MonkeyspeakException
        /// </summary>
        /// <param name="reason">Reason for the error</param>
        public virtual void RaiseError(string reason)
        {
            throw new MonkeyspeakException(reason);
        }

        /// <summary>
        /// Registers a Trigger to the TriggerHandler with optional description
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="handler"></param>
        /// <param name="description"></param>
        public void Add(Trigger trigger, TriggerHandler handler, string description = null)
        {
            if (description != null && !descriptions.ContainsKey(trigger)) descriptions.Add(trigger, description);
            if (!handlers.ContainsKey(trigger))
                handlers.Add(trigger, handler);
            else throw new UnauthorizedAccessException($"Override of existing Trigger {trigger}'s handler with handler in {handler.Method}.");
        }

        /// <summary>
        /// Registers a Trigger to the TriggerHandler with optional description
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        /// <param name="description"></param>
        public void Add(TriggerCategory cat, int id, TriggerHandler handler, string description = null)
        {
            Trigger trigger = new Trigger(cat, id);
            if (description != null) descriptions.Add(trigger, description);
            if (!handlers.ContainsKey(trigger))
                handlers.Add(trigger, handler);
            else throw new UnauthorizedAccessException($"Override of existing Trigger {trigger}'s handler with handler in {handler.Method}.");
        }

        /// <summary>
        /// Called when page is disposing or resetting.
        /// </summary>
        /// <param name="page">The page.</param>
        public abstract void Unload(Page page);

        /// <summary>
        /// Builds a string representation of the descriptions of each trigger.
        /// </summary>
        /// <returns></returns>
        public string ToString(bool excludeLibraryName = false, bool excludeDescriptions = false)
        {
            StringBuilder sb = new StringBuilder();
            if (!excludeLibraryName) sb.AppendLine(GetType().Name);
            foreach (var kv in descriptions)
            {
                sb.Append(' ').Append(kv.Key).Append(!excludeDescriptions ? kv.Value : string.Empty).Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds a string representation of the descriptions of each trigger.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetType().Name);
            foreach (var kv in descriptions)
            {
                sb.AppendLine(kv.Value);
            }
            return sb.ToString();
        }

        public static IEnumerable<BaseLibrary> GetAllLibraries()
        {
            if (Assembly.GetEntryAssembly() != null)
            {
                foreach (var asmName in Assembly.GetEntryAssembly().GetReferencedAssemblies())
                {
                    var asm = Assembly.Load(asmName);
                    foreach (var lib in GetLibrariesFromAssembly(asm)) yield return lib;
                }
            }
            else if (Assembly.GetExecutingAssembly() != null)
            {
                foreach (var asmName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    var asm = Assembly.Load(asmName);
                    foreach (var lib in GetLibrariesFromAssembly(asm)) yield return lib;
                }
            }
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                // avoid all the Microsoft and System assemblies.  All assesmblies it is looking for should be in the local path
                if (asm.GlobalAssemblyCache) continue;

                foreach (var lib in GetLibrariesFromAssembly(asm)) yield return lib;
            }
        }

        /// <summary>
        /// Loads trigger handlers from a assembly instance
        /// </summary>
        /// <param name="asm">The assembly instance</param>
        public static IEnumerable<BaseLibrary> GetLibrariesFromAssembly(Assembly asm)
        {
            if (asm == null) yield break;
            foreach (var types in ReflectionHelper.GetAllTypesWithAttributeInMembers<TriggerHandlerAttribute>(asm))
                foreach (MethodInfo method in types.GetMethods().Where(method => method.IsDefined(typeof(TriggerHandlerAttribute), false)))
                {
                    foreach (TriggerHandlerAttribute attribute in ReflectionHelper.GetAllAttributesFromMethod<TriggerHandlerAttribute>(method))
                    {
                        attribute.owner = method;
                        try
                        {
                            var handler = (TriggerHandler)(reader => (bool)attribute.owner.Invoke(null, new object[] { reader }));
                            if (handler != null)
                            {
                                Attributes.Instance.Add(new Trigger(attribute.TriggerCategory, attribute.TriggerID), handler, attribute.Description);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new MonkeyspeakException(String.Format("Failed to load library from assembly '{0}', couldn't bind to method '{1}.{2}'", asm.FullName, method.DeclaringType.Name, method.Name), ex);
                        }
                    }
                }
            yield return Attributes.Instance;
            var subType = typeof(BaseLibrary);
            foreach (var type in asm.GetTypes())
            {
                if (type.IsSubclassOf(subType))
                    yield return (BaseLibrary)Activator.CreateInstance(type);
            }
        }
    }
}