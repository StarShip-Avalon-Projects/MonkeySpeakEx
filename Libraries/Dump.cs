using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monkeyspeak.Libraries
{
    internal class Dump : AbstractBaseLibrary
    {
        public static readonly Dump Poo = new Dump();

        protected Dump()
        {
        }

        public void AddDescription(Trigger trigger, string description)
        {
            if (string.IsNullOrEmpty(description)) return;
            if (!descriptions.ContainsKey(trigger))
                descriptions.Add(trigger, description);
            handlers.Clear(); // never let handlers be added to this poo.
        }
    }
}