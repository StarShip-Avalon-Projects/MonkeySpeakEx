using Monkeyspeak.lexical;
using Monkeyspeak.lexical.Expressions;
using Shared.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Removed by Gerolkae Looking for threads
//using System.Threading;

namespace Monkeyspeak
{
    [Serializable]
    public class TriggerReaderException : Exception
    {
        public TriggerReaderException()
        {
        }

        public TriggerReaderException(string message)
            : base(message)
        {
        }

        public TriggerReaderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TriggerReaderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// A Reader that is used to get Variables, Strings, and Numbers from Triggers
    /// </summary>
    [CLSCompliant(true)]
    public sealed class TriggerReader
    {
        private Trigger originalTrigger;
        private Page page;
        private SourcePosition lastPos;
        private Queue<IExpression> contents;

        private readonly object syncObject = new object();

        /// <summary>
        /// A Reader that is used to get Variables, Strings, and Numbers from Triggers
        /// </summary>
        /// <param name="page"></param>
        /// <param name="trigger"></param>
        public TriggerReader(Page page, Trigger trigger)
        {
            Trigger = trigger;
            this.page = page;
        }

        internal TriggerReader(Page page)
        {
            this.page = page;
        }

        public Trigger Trigger
        {
            get { return originalTrigger; }
            internal set
            {
                originalTrigger = value;
                contents = new Queue<IExpression>(originalTrigger.contents);
            }
        }

        public TriggerCategory TriggerCategory
        {
            get { return Trigger.Category; }
        }

        public int TriggerId
        {
            get { return Trigger.Id; }
        }

        public Page Page
        {
            get { return page; }
        }

        /// <summary>
        /// Resets the reader's indexes to 0
        /// </summary>
        public void Reset()
        {
            if (Trigger != null)
            {
                contents = new Queue<IExpression>(originalTrigger.contents);
            }
        }

        /// <summary>
        /// Reads the next String, throws TriggerReaderException on failure
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TriggerReaderException"></exception>
        public string ReadString(bool processVariables = true)
        {
            if (contents.Count == 0) throw new TriggerReaderException("Unexpected end of values");
            if (!(contents.Peek() is StringExpression)) throw new TriggerReaderException($"Expected string, got {contents.Peek().GetType().Name} at {contents.Peek().Position}");
            try
            {
                var expr = (contents.Dequeue() as StringExpression);
                lastPos = expr.Position;
                var str = expr.Value;

                if (str[0] != '@' || processVariables)
                {
                    for (int i = page.Scope.Count - 1; i >= 0; i--)
                    {
                        // replaced string.replace with Regex because
                        //  %ListName would replace %ListName2 leaving the 2 at the end
                        //- Gerolkae
                        var name = page.Scope[i].Name;
                        string pattern = name + @"\b?";
                        var value = page.Scope[i].Value;
                        string replace = (value != null) ? value.ToString() : "null";
                        str = Regex.Replace(str, pattern, replace, RegexOptions.CultureInvariant);
                    }
                }
                if (str[0] == '@') return str.ToString().Substring(1);
                return str;
            }
            catch (Exception ex)
            {
                Logger.Debug<TriggerReader>(ex);
                throw new TriggerReaderException($"No value found at {lastPos}");
            }
        }

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns></returns>
        public bool PeekString()
        {
            if (contents.Count == 0) return false;
            return contents.Peek() is StringExpression;
        }

        public bool TryReadString(out string str, bool processVariables = true)
        {
            if (!PeekString())
            {
                str = String.Empty;
                return false;
            }

            str = ReadString(processVariables);
            return true;
        }

        /// <summary>
        /// Reads the next Variable available, throws TriggerReaderException on failure
        /// </summary>
        /// <param name="addIfNotExist">Add the Variable if it doesn't exist and return that Variable with a Value equal to null.</param>
        /// <returns>Variable</returns>
        /// <exception cref="TriggerReaderException"></exception>
        public IVariable ReadVariable(bool addIfNotExist = false)
        {
            if (contents.Count == 0) throw new TriggerReaderException("Unexpected end of values");
            if (!(contents.Peek() is VariableExpression)) throw new TriggerReaderException($"Expected variable, got {contents.Peek().GetType().Name} at {contents.Peek().Position}");
            try
            {
                var var = Variable.NoValue;
                var expr = contents.Dequeue() as VariableExpression;
                lastPos = expr.Position;
                var varRef = expr.Value as string;
                if (!page.HasVariable(varRef, out var))
                    if (addIfNotExist)
                        var = page.SetVariable(varRef, null, false);

                return var;
            }
            catch (Exception ex)
            {
                throw new TriggerReaderException($"No value found at {lastPos}");
            }
        }

        /// <summary>
        /// Reads the next Variable list available, throws TriggerReaderException on failure
        /// </summary>
        /// <param name="addIfNotExist">Add the Variable if it doesn't exist and return that Variable with a Value equal to null.</param>
        /// <returns>Variable</returns>
        /// <exception cref="TriggerReaderException"></exception>
        public VariableList ReadVariableList(bool addIfNotExist = false)
        {
            if (contents.Count == 0) throw new TriggerReaderException("Unexpected end of values");
            if (!(contents.Peek() is VariableExpression)) throw new TriggerReaderException($"Expected variable list, got {contents.Peek().GetType().Name} at {contents.Peek().Position}");
            try
            {
                var var = Variable.NoValue;
                var expr = (contents.Dequeue() as VariableExpression);
                lastPos = expr.Position;
                string varRef = expr.Value;
                if (!page.HasVariable(varRef, out var))
                    if (addIfNotExist)
                        var = page.SetVariable(varRef, null, false);

                return var is VariableList ? (VariableList)var : null;
            }
            catch (Exception ex)
            {
                throw new TriggerReaderException($"No value found at {lastPos}");
            }
        }

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns></returns>
        public bool PeekVariable()
        {
            if (contents.Count == 0) return false;
            return contents.Peek() is VariableExpression;
        }

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns></returns>
        public bool PeekVariableList()
        {
            if (contents.Count == 0) return false;
            // TODO VariableListExpression
            return contents.Peek() is VariableExpression;
        }

        /// <summary>
        /// Trys to read the next Variable available
        /// </summary>
        /// <param name="var">Variable is assigned on success</param>
        /// <param name="addIfNotExist"></param>
        /// <returns>bool on success</returns>
        public bool TryReadVariable(out IVariable var, bool addIfNotExist = false)
        {
            if (!PeekVariable())
            {
                var = Variable.NoValue;
                return false;
            }
            var = ReadVariable(addIfNotExist);
            return true;
        }

        /// <summary>
        /// Reads the next Double available, throws TriggerReaderException on failure
        /// </summary>
        /// <returns>Double</returns>
        /// <exception cref="TriggerReaderException"></exception>
        public double ReadNumber()
        {
            if (contents.Count == 0) throw new TriggerReaderException("Unexpected end of values");
            if (!(contents.Peek() is NumberExpression)) throw new TriggerReaderException($"Expected number, got {contents.Peek().GetType().Name} at {contents.Peek().Position}");
            try
            {
                var expr = (contents.Dequeue() as NumberExpression);
                lastPos = expr.Position;
                return expr.Value;
            }
            catch
            {
                throw new TriggerReaderException($"No value found at {lastPos}");
            }
        }

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns></returns>
        public bool PeekNumber()
        {
            if (contents.Count == 0) return false;
            return contents.Peek() is NumberExpression;
        }

        public bool TryReadNumber(out double number)
        {
            if (!PeekNumber())
            {
                number = double.NaN;
                return false;
            }
            number = ReadNumber();
            return true;
        }
    }
}