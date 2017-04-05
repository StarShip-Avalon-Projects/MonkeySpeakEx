using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    [Serializable]
    internal class TriggerList : List<Trigger>
    {
        #region Public Constructors

        public TriggerList()
        {
        }

        public TriggerList(int initialCapacity) :
            base(initialCapacity)
        {
        }

        public TriggerList(params Trigger[] triggers) :
            base(triggers)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public bool HasTrigger(TriggerCategory cat, int id)
        {
            for (int i = 0; i <= Count - 1; i++)
            {
                Trigger trigger = this[i];
                if (trigger.Category == cat && trigger.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Public Methods
    }
}