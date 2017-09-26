using System;

namespace Monkeyspeak
{
#pragma warning disable CS1570 // XML comment has badly formed XML -- 'End tag 'summary' does not match the start tag 'para'.'
#pragma warning disable CS1570 // XML comment has badly formed XML -- 'End tag 'para' does not match the start tag 'see'.'
    /// <summary> MonkeySpeak Variable oblect. <para>This obkect acepts <see
    /// cref="String"/> and <see cref="Double"> types</para> </summary>

#pragma warning disable CS1570 // XML comment has badly formed XML -- 'Expected an end tag for element 'summary'.'

    [Serializable]
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'Expected an end tag for element 'summary'.'
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'End tag 'para' does not match the start tag 'see'.'
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'End tag 'summary' does not match the start tag 'para'.'
    [CLSCompliant(true)]
    public class Variable
    {
        #region Public Fields

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.NoValue'
        public static readonly Variable NoValue = new Variable("%none", "", false);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.NoValue'

        #endregion Public Fields

        #region Private Fields

        private bool isConstant;

        private string name;

        private object value;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="value"></param>
        /// <param name="constant"></param>
        public Variable(string Name, object value, bool constant)
        {
            isConstant = constant;
            name = Name;
            this.value = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="value"></param>
        public Variable(string Name, object value)
        {
            isConstant = false;
            name = Name;
            this.value = value;
        }

        /// <summary>
        ///
        /// </summary>
        public Variable()
        {
            this.isConstant = NoValue.isConstant;
            this.name = NoValue.name;
            this.value = NoValue.value;
        }

        #endregion Internal Constructors

        #region Public Properties

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.IsConstant'

        public bool IsConstant
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.IsConstant'
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.Name'

        public string Name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.Name'
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.Value'

        public object Value
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.Value'
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.operator !=(Variable, Variable)'

        public static bool operator !=(Variable varA, Variable varB)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.operator !=(Variable, Variable)'
        {
            return varA.Value != varB.Value;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.operator ==(Variable, Variable)'

        public static bool operator ==(Variable varA, Variable varB)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.operator ==(Variable, Variable)'
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.Equals(object)'

        public override bool Equals(object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.Equals(object)'
        {
            return ((Variable)obj).Name.Equals(Name) && ((Variable)obj).Value.Equals(Value);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.ForceAssignValue(object)'

        public void ForceAssignValue(object _value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.ForceAssignValue(object)'
        {
            if (CheckType(_value) == false) throw new TypeNotSupportedException(_value.GetType().Name +
" is not a supported type. Expecting string or double.");
            value = _value;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Variable.GetHashCode()'

        public override int GetHashCode()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Variable.GetHashCode()'
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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException'
    public class VariableIsConstantException : Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException'
    {
        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException()'

        public VariableIsConstantException()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException()'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException(string)'

        public VariableIsConstantException(string message) : base(message)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException(string)'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException(string, Exception)'

        public VariableIsConstantException(string message, Exception inner) : base(message, inner)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException(string, Exception)'
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException(SerializationInfo, StreamingContext)'

        protected VariableIsConstantException(
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'VariableIsConstantException.VariableIsConstantException(SerializationInfo, StreamingContext)'
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        #endregion Protected Constructors
    }
}