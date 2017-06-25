using System;

namespace Monkeyspeak
{
    /// <summary> MonkeySpeak Variable oblect. <para>This obkect acepts <see
    /// cref="String"/> and <see cref="Double"> types</para> </summary>
    [Serializable]
    [CLSCompliant(true)]
    public class Variable
    {
        #region Public Fields

        public static readonly Variable NoValue = new Variable("%none", "", false);

        #endregion Public Fields

        #region Private Fields

        private bool isConstant;

        private string name;

        private object value;

        #endregion Private Fields

        #region Internal Constructors

        internal Variable(string Name, object value, bool constant = false)
        {
            isConstant = constant;
            name = Name;
            this.value = value;
        }

        protected Variable()
        {
            this.isConstant = NoValue.isConstant;
            this.name = NoValue.name;
            this.value = NoValue.value;
        }

        #endregion Internal Constructors

        #region Public Properties

        public bool IsConstant
        {
            get
            {
                return isConstant;
            }
            set
            {
                isConstant = value;
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    name = "%none";
                return name;
            }
            internal set
            {
                name = value;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                // removed Value = as it interfered with page.setVariable - Gerolkae
                if (CheckType(value) == false) throw new TypeNotSupportedException(value.GetType().Name +
    " is not a supported type. Expecting string or double.");

                if (IsConstant == false)
                    this.value = value;
                else throw new VariableIsConstantException("Attempt to assign a _value to constant \"" + Name + "\"");
            }
        }

        #endregion Public Properties

        // Variable var = new variable(); Preset reader.readvariable with
        // default data Needed for Conditions checking Variables that
        // haven't been defined yet.
        // -Gerolkae
        /* private Variable()
         {
             isConstant = false;
             name = "%none";
             value = null;
         }*/

        #region Public Methods

        public static bool operator !=(Variable varA, Variable varB)
        {
            return varA.Value != varB.Value;
        }

        public static bool operator ==(Variable varA, Variable varB)
        {
            return varA.Value == varB.Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="asConstant">
        /// Clone as Constant
        /// </param>
        /// <returns>
        /// </returns>
        public Variable Clone(bool asConstant = false)
        {
            return new Variable(Name, Value, asConstant);
        }

        public override bool Equals(object obj)
        {
            return ((Variable)obj).Name.Equals(Name) && ((Variable)obj).Value.Equals(Value);
        }

        public void ForceAssignValue(object _value)
        {
            if (CheckType(_value) == false) throw new TypeNotSupportedException(_value.GetType().Name +
" is not a supported type. Expecting string or double.");
            value = _value;
        }

        public override int GetHashCode()
        {
            int n = 0;
            if (value is int)
            {
                n = int.Parse(value.ToString());
                return value.ToString().GetHashCode() ^ name.GetHashCode();
            }
            return n ^ name.GetHashCode();
        }

        /// <summary>
        /// Returns a const identifier if the variable is constant followed
        /// by name,
        /// <para>
        /// otherwise just the name is returned.
        /// </para>
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return ((IsConstant) ? "const " : "") + Name + " = " + ((Value == null) ? "null" : Value.ToString());
        }

        #endregion Public Methods

        #region Private Methods

        private bool CheckType(object _value)
        {
            if (_value == null) return true;

            return _value is string ||
                   _value is double;
        }

        #endregion Private Methods
    }

    [Serializable]
    public class VariableIsConstantException : Exception
    {
        #region Public Constructors

        public VariableIsConstantException()
        {
        }

        public VariableIsConstantException(string message) : base(message)
        {
        }

        public VariableIsConstantException(string message, Exception inner) : base(message, inner)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected VariableIsConstantException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        #endregion Protected Constructors
    }
}