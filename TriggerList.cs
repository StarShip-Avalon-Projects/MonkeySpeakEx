using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    [Serializable]
    public class TriggerList : List<Trigger>
    {
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

        /// <summary>
        /// Operates like IndexOf for Triggers
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="id"></param>
        /// <returns>Index of trigger or -1 if not found</returns>
        public int IndexOfTrigger(TriggerCategory cat, int id)
        {
            for (int i = 0; i <= Count - 1; i++)
            {
                Trigger trigger = this[i];
                if (trigger.Category == cat && trigger.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}