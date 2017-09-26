using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    /// <summary>
    /// Monkey Speak Trigger Catagories
    /// </summary>
    [Serializable]
    public enum TriggerCategory : int
    {
        /// <summary>
        /// A trigger defined with a 0
        /// <para>
        /// Example: (0:1) when someone says something,
        /// </para>
        /// </summary>
        Cause = 0,

        /// <summary>
        /// A trigger defined with a 1
        /// <para>
        /// Example: (1:2) and they moved # units left,
        /// </para>
        /// </summary>
        Condition = 1,

        /// <summary>
        /// A trigger defined with a 5
        /// <para>
        /// Example: (5:1) print {Hello World} to the console.
        /// </para>
        /// </summary>
        Effect = 5,

        /// <summary>
        /// A trigger that was not defined. You should never encounter this
        /// if you do then something isn't quite right.
        /// </summary>
        Undefined = -1
    }

    /// <summary>
    /// Monkey Speak Trigger
    /// <para>
    /// ( <see cref="TriggerCategory"/>, <see cref="int">Id</see>
    /// </para>
    /// </summary>
    [Serializable]
    public sealed class Trigger : IEquatable<Trigger>
    {
        #region Internal Fields

        internal Queue<object> contents = new Queue<object>(16);

        #endregion Internal Fields

        #region Private Fields

        private TriggerCategory category;
        private string description = "";
        private int id;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Establish a new trigger
        /// </summary>
        /// <param name="cat">
        /// <see cref="TriggerCategory"/>
        /// </param>
        /// <param name="id">
        /// Line ID
        /// </param>
        public Trigger(TriggerCategory cat, int id)
        {
            category = cat;
            this.id = id;
        }

        #endregion Public Constructors

        #region Internal Constructors

        internal Trigger()
        {
            category = TriggerCategory.Undefined;
            id = -1;
        }

        #endregion Internal Constructors

        #region Public Properties

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Category'

        public TriggerCategory Category
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Category'
        {
            get { return category; }
            internal set { category = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Description'

        public string Description
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Description'
        {
            get { return description; }
            set { description = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Id'

        public int Id
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Id'
        {
            get { return id; }
            internal set { id = value; }
        }

        #endregion Public Properties

        #region Internal Properties

        internal Queue<object> Contents
        {
            get { return contents; }
            set { contents = value; }
        }

        #endregion Internal Properties

        #region Public Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Equals(Trigger)'

        public bool Equals(Trigger other)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Equals(Trigger)'
        {
            return other.category == this.category && other.id == this.id;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Equals(object)'

        public override bool Equals(object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Trigger.Equals(object)'
        {
            if (obj is Trigger other)
            {
                return other.category == category && other.id == id;
            }
            return false;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Trigger.GetHashCode()'

        public override int GetHashCode()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Trigger.GetHashCode()'
        {
            return ((int)this.category ^ this.id);
        }

        /// <summary>
        /// Display the line id format (#:#)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0}:{1})", (int)category, id);
        }

        #endregion Public Methods

        #region Internal Methods

        internal Trigger Clone()
        {
            Trigger clone = new Trigger(category, id);
            clone.contents = new Queue<object>(this.contents);
            return clone;
        }

        #endregion Internal Methods
    }
}