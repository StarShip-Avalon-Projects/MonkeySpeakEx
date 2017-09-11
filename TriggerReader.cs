using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Monkeyspeak
{
    /// <summary>
    /// A Reader that is used to get Variables, Strings, and Numbers from Triggers
    /// </summary>
    [CLSCompliant(true)]
    public sealed class TriggerReader
    {
        #region Private Fields

        private readonly object syncObject = new object();
        private Trigger cloneTrigger, originalTrigger;
        private Page page;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// A Reader that is used to get Variables, Strings, and Numbers
        /// from Triggers
        /// </summary>
        /// <param name="page">
        /// <see cref="Monkeyspeak.Page"/>
        /// </param>
        /// <param name="trigger">
        /// <see cref="Monkeyspeak.Trigger"/>
        /// </param>
        public TriggerReader(Page page, Trigger trigger)
        {
            originalTrigger = trigger;
            cloneTrigger = originalTrigger.Clone();
            this.page = page;
        }

        #endregion Public Constructors

        #region Internal Constructors

        internal TriggerReader(Page page)
        {
            this.page = page;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// </summary>
        public Page Page
        {
            get { return page; }
        }

        public Trigger Trigger
        {
            get { return cloneTrigger; }
            internal set
            {
                originalTrigger = value;
                cloneTrigger = originalTrigger.Clone();
            }
        }

        /// <summary>
        /// </summary>
        public TriggerCategory TriggerCategory
        {
            get { return cloneTrigger.Category; }
        }

        /// <summary>
        /// </summary>
        public int TriggerId
        {
            get { return cloneTrigger.Id; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns>
        /// </returns>
        public bool PeekNumber()
        {
            if (cloneTrigger.contents.Count == 0) return false;
            return cloneTrigger.contents.Peek() is double;
        }

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns>
        /// </returns>
        public bool PeekString()
        {
            if (cloneTrigger.contents.Count == 0) return false;
            return cloneTrigger.contents.Peek() is string;
        }

        /// <summary>
        /// Peeks at the next value
        /// </summary>
        /// <returns>
        /// the current peek <see cref="Monkeyspeak.Variable"/>
        /// </returns>
        public bool PeekVariable()
        {
            if (cloneTrigger.contents.Count == 0) return false;
            string nextContent = cloneTrigger.contents.Peek().ToString();
            return (!String.IsNullOrEmpty(nextContent)) && nextContent[0] == page.Engine.Options.VariableDeclarationSymbol;
        }

        /// <summary>
        /// Reads the next Double available, throws TriggerReaderException
        /// on failure
        /// </summary>
        /// <returns>
        /// <see cref="System.Double"/>
        /// </returns>
        public double ReadNumber()
        {
            if (cloneTrigger.contents.Count == 0) throw new TriggerReaderException("Unexpected end of values.");
            try
            {
                double number = Convert.ToDouble(cloneTrigger.contents.Dequeue());
                return number;
            }
            catch
            {
                throw new TriggerReaderException("No value found.");
            }
        }

        /// <summary>
        /// Reads the next String, throws TriggerReaderException on failure
        /// </summary>
        /// <returns>
        /// <see cref="System.String"/>
        /// </returns>
        public string ReadString(bool processVariables = true)
        {
            if (cloneTrigger.contents.Count == 0) throw new TriggerReaderException("Unexpected end of values.");

            try
            {
                string str = Convert.ToString(cloneTrigger.contents.Dequeue());
                if (processVariables)
                {
                    for (int i = 0; i <= page.Scope.Count - 1; i++)
                    {
                        // replaced string.replace with Regex because
                        //  %ListName would replace %ListName2 leaving the 2 at the end
                        //- Gerolkae
                        string pattern = page.Scope[i].Name + @"\b";
                        string replace = (page.Scope[i].Value != null) ? page.Scope[i].Value.ToString() : "null";
                        str = Regex.Replace(str, pattern, replace);
                    }
                }
                return str;
            }
            catch
            {
                throw new TriggerReaderException("No value found.");
            }
        }

        /// <summary>
        /// Reads the next Variable available, throws TriggerReaderException
        /// on failure
        /// </summary>
        /// <returns>
        /// Variable
        /// </returns>
        public Variable ReadVariable()
        {
            return ReadVariable(false);
        }

        /// <summary>
        /// Reads the next Variable available, throws TriggerReaderException
        /// on failure
        /// </summary>
        /// <param name="addIfNotExist">
        /// Add the Variable if it doesn't exist and return that Variable
        /// with a Value equal to null.
        /// </param>
        /// <returns>
        /// Variable
        /// </returns>
        public Variable ReadVariable(bool addIfNotExist)
        {
            if (cloneTrigger.contents.Count == 0) throw new TriggerReaderException("Unexpected end of values.");

            try
            {
                Variable var;
                string varRef = Convert.ToString(cloneTrigger.contents.Dequeue());
                if (page.HasVariable(varRef, out var) == false)
                    if (addIfNotExist)
                        var = page.SetVariable(varRef, "", false);

                return var;
            }
            catch
            {
                throw new TriggerReaderException("No value found.");
            }
        }

        /// <summary>
        /// Resets the reader's indexes to 0
        /// </summary>
        public void Reset()
        {
            if (originalTrigger != null)
            {
                cloneTrigger.contents = new Queue<object>(originalTrigger.contents);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="number">
        /// <see cref="Double"/>
        /// </param>
        /// <returns>
        /// True on sucess
        /// </returns>
        public bool TryReadNumber(out double number)
        {
            if (PeekNumber() == false)
            {
                number = double.NaN;
                return false;
            }
            return Double.TryParse(cloneTrigger.contents.Dequeue().ToString(), out number);
        }

        /// <summary>
        /// </summary>
        /// <param name="str">
        /// </param>
        /// <returns>
        /// </returns>
        public bool TryReadString(out string str)
        {
            if (PeekString() == false)
            {
                str = String.Empty;
                return false;
            }

            str = Convert.ToString(cloneTrigger.contents.Dequeue());
            return true;
        }

        /// <summary>
        /// Trys to read the next Variable available
        /// </summary>
        /// <param name="var">
        /// Variable is assigned on success
        /// </param>
        /// <returns>
        /// bool on success
        /// </returns>
        public bool TryReadVariable(out Variable var)
        {
            if (PeekVariable() == false)
            {
                var = Variable.NoValue;
                return false;
            }
            string varRef = Convert.ToString(cloneTrigger.contents.Dequeue());
            var = page.GetVariable(varRef);
            return true;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class TriggerReaderException : Exception
    {
        #region Public Constructors

        public TriggerReaderException()
        {
        }

        public TriggerReaderException(string message) : base(message)
        {
        }

        public TriggerReaderException(string message, Exception inner) : base(message, inner)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected TriggerReaderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        #endregion Protected Constructors
    }
}