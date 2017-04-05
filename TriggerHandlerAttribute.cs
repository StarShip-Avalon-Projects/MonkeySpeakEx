using System;
using System.Reflection;

namespace Monkeyspeak
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class TriggerHandlerAttribute : Attribute
    {
        #region Internal Fields

        internal MethodInfo owner;

        #endregion Internal Fields

        #region Private Fields

        private string description;
        private TriggerCategory triggerCategory;
        private int triggerID;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="category">
        /// Trigger Category
        /// </param>
        /// <param name="id">
        /// Trigger ID
        /// </param>
        /// <param name="description">
        /// Trigger Description
        /// </param>
        public TriggerHandlerAttribute(TriggerCategory category, int id, string description)
        {
            this.triggerCategory = category;
            this.triggerID = id;
            this.description = description;
        }

        #endregion Public Constructors

        #region Internal Methods

        internal void Register(Page page)
        {
            TriggerHandler handler = (TriggerHandler)Delegate.CreateDelegate(typeof(TriggerHandler), owner, false);
            if (handler != null)
                page.SetTriggerHandler(triggerCategory, triggerID, (TriggerHandler)Delegate.CreateDelegate(typeof(TriggerHandler), owner, false), description);
        }

        #endregion Internal Methods
    }
}