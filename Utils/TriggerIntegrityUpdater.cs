using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monkeyspeak.Utils
{
    /// <summary>
    /// Reads triggers from a stream and updates those trigger's to reflect the library's trigger description.
    /// This is useful if you have a trigger handler that was changed but it isn't reflected on the script itself
    /// </summary>
    public sealed class TriggerIntegrityUpdater
    {
        private MonkeyspeakEngine engine;
        private Page page;

        public TriggerIntegrityUpdater(Page page)
        {
            engine = page.Engine;
            this.page = page;
        }
    }
}