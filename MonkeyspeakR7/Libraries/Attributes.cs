﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monkeyspeak.Libraries
{
    internal class Attributes : AbstractBaseLibrary
    {
        public static readonly Attributes Instance = new Attributes();

        protected Attributes()
        {
        }

        public void AddDescription(Trigger trigger, string description)
        {
            if (string.IsNullOrEmpty(description)) return;
            if (!descriptions.ContainsKey(trigger))
                descriptions.Add(trigger, description);
            handlers.Clear(); // never let handlers be added to this poo.
        }

        public override void Add(Trigger trigger, TriggerHandler handler, string description = null)
        {
            if (description != null && !descriptions.ContainsKey(trigger)) descriptions.Add(trigger, description);
            if (!handlers.ContainsKey(trigger))
                handlers.Add(trigger, handler);
        }

        public override void Add(TriggerCategory cat, int id, TriggerHandler handler, string description = null)
        {
            var trigger = new Trigger(cat, id);
            if (description != null && !descriptions.ContainsKey(trigger)) descriptions.Add(trigger, description);
            if (!handlers.ContainsKey(trigger))
                handlers.Add(trigger, handler);
        }

        public override void OnPageDisposing(Page page)
        {
        }
    }
}