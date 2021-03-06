﻿using Monkeyspeak.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monkeyspeak.Libraries
{
    public class Tables : AutoIncrementBaseLibrary
    {
        public override int BaseId => 250;

        public override void Initialize(params object[] args)
        {
            Add(TriggerCategory.Flow, ForEntryInTable,
                "for each entry in table % put it into %,");

            Add(TriggerCategory.Flow, ForKeyValueInTable,
                "for each key/value pair in table % put them into % and %,");

            Add(TriggerCategory.Effect, CreateTable,
                "create a table as %.");

            Add(TriggerCategory.Effect, PutNumIntoTable,
                "with table % put # in it at key {...}.");

            Add(TriggerCategory.Effect, PutStringIntoTable,
                "with table % put {...} in it at key {...}.");

            Add(TriggerCategory.Effect, GetTableKeyIntoVar,
                "with table % get key {...} put it in into variable %.");

            Add(TriggerCategory.Effect, ClearTable,
                "with table % remove all entries in it.");

            Add(TriggerCategory.Condition, VariableIsTable,
                "and variable % is a table,");

            Add(TriggerCategory.Condition, VariableIsNotTable,
                "and variable % is not a table,");
        }

        private bool ClearTable(TriggerReader reader)
        {
            var var = reader.ReadVariableTable();
            var.Clear();
            return true;
        }

        private bool VariableIsNotTable(TriggerReader reader)
        {
            var var = reader.ReadVariable();
            return !(var is VariableTable);
        }

        private bool VariableIsTable(TriggerReader reader)
        {
            var var = reader.ReadVariable();
            return var is VariableTable;
        }

        private bool ForKeyValueInTable(TriggerReader reader)
        {
            var table = reader.ReadVariableTable();
            var key = reader.ReadVariable(true);
            var val = reader.ReadVariable(true);
            if (!table.Next(out object keyVal))
            {
                reader.Page.RemoveVariable(key);
                reader.Page.RemoveVariable(val);
                return false;
            }
            key.Value = table.ActiveIndexer;
            val.Value = keyVal;
            return true;
        }

        private bool ForEntryInTable(TriggerReader reader)
        {
            var table = reader.ReadVariableTable();
            var var = reader.ReadVariable(true);
            if (!table.Next(out object keyVal))
            {
                reader.Page.RemoveVariable(var);
                return false;
            }
            var.Value = keyVal;
            return true;
        }

        private bool GetTableKeyIntoVar(TriggerReader reader)
        {
            var var = reader.ReadVariableTable();
            var key = reader.ReadString();
            var into = reader.ReadVariable(true);
            into.Value = var[key];
            return true;
        }

        private bool PutStringIntoTable(TriggerReader reader)
        {
            var var = reader.ReadVariableTable(true);
            var value = reader.ReadString();
            var key = reader.ReadString();
            var.Add(key, value);
            return true;
        }

        private bool PutNumIntoTable(TriggerReader reader)
        {
            var var = reader.ReadVariableTable(true);
            var value = reader.ReadNumber();
            var key = reader.ReadString();
            var.Add(key, value);
            return true;
        }

        private bool CreateTable(TriggerReader reader)
        {
            reader.ReadVariableTable(true);
            return true;
        }

        public override void Unload(Page page)
        {
        }
    }
}