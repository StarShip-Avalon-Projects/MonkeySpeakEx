using System;

namespace Monkeyspeak.Libraries
{
    public class StringOperations : BaseLibrary
    {
        public StringOperations()
        {
            Add(TriggerCategory.Effect, 400, PutWordCountIntoVariable,
                "(5:403) with {...} get word count and set it to variable %.");
            Add(TriggerCategory.Effect, 401, AddStringToVar,
                "(5:404) with {...} set it to variable %.");
            Add(TriggerCategory.Effect, 402, SubStringToVar,
                "(5:402) with {...} get words starting at # to # and set it to variable %.");
            Add(TriggerCategory.Effect, 403, IndexOfStringToVar,
                "(5:400) with {...} get index of {...} and set it to variable %.");
        }

        private bool SubStringToVar(TriggerReader reader)
        {
            var str = reader.ReadString();
            var start = reader.ReadNumber();
            var end = reader.ReadNumber();
            var var = reader.ReadVariable(true);
            var subStr = str.Substring((int)start, (int)end);
            var.Value = subStr;
            return true;
        }

        private bool IndexOfStringToVar(TriggerReader reader)
        {
            var str = reader.ReadString();
            var search = reader.ReadString();
            var var = reader.ReadVariable(true);
            var index = str.IndexOf(search);
            var.Value = index;
            return true;
        }

        private bool AddStringToVar(TriggerReader reader)
        {
            string str = reader.ReadString();
            var var = reader.ReadVariable();
            var.Value = var.Value + str;
            return true;
        }

        private bool PutWordCountIntoVariable(TriggerReader reader)
        {
            string[] words = reader.ReadString().Split(' ');
            var var = reader.ReadVariable(true);
            var.Value = words.Length;
            return true;
        }

        public override void Unload(Page page)
        {
        }
    }
}