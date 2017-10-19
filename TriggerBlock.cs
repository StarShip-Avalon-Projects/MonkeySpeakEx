using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    [Serializable]
    public class TriggerBlock : List<Trigger>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBlock"/> class.
        /// </summary>
        public TriggerBlock()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBlock"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public TriggerBlock(int initialCapacity) :
            base(initialCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBlock"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public TriggerBlock(IEnumerable<Trigger> collection) : base(collection)
        {
        }

        /// <summary>
        /// Operates like IndexOf for Triggers
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="id"></param>
        /// <param name="startIndex"></param>
        /// <returns>Index of trigger or -1 if not found</returns>
        public int IndexOfTrigger(TriggerCategory cat, int id, int startIndex = 0)
        {
            for (int i = startIndex; i <= Count - 1; i++)
            {
                Trigger trigger = base[i];
                if (trigger.Category == cat && trigger.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines whether the block contains the trigger.
        /// </summary>
        /// <param name="cat">The category.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <c>true</c> if the block contains the trigger; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTrigger(TriggerCategory cat, int id)
        {
            for (int i = 0; i <= Count - 1; i++)
            {
                Trigger trigger = base[i];
                if (trigger.Category == cat && trigger.Id == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}