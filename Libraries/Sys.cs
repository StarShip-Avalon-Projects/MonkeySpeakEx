using Monkeyspeak.Extensions;
using Shared.Core.Logging;
using System;

namespace Monkeyspeak.Libraries
{
    public class Sys : BaseLibrary
    {
        public Sys()
        {
            // (1:100) and variable %Variable is defined,
            Add(new Trigger(TriggerCategory.Condition, 100), IsVariableDefined,
                "(1:100) and variable % is defined,");

            // (1:101) and variable %Variable is not defined,
            Add(new Trigger(TriggerCategory.Condition, 101), IsVariableNotDefined,
                "(1:101) and variable % is not defined,");

            // (1:102) and variable %Variable equals #,
            Add(new Trigger(TriggerCategory.Condition, 102), IsVariableEqualToNumberOrVar,
                "(1:102) and variable % equals #,");

            // (1:103) and variable %Variable does not equal #,
            Add(new Trigger(TriggerCategory.Condition, 103), IsVariableNotEqualToNumberOrVar,
                "(1:103) and variable % does not equal #,");

            // (1:104) and variable %Variable equals {...},
            Add(new Trigger(TriggerCategory.Condition, 104), IsVariableEqualToString,
                "(1:104) and variable % equals {...},");

            // (1:105) and variable %Variable does not equal {...},
            Add(new Trigger(TriggerCategory.Condition, 105), IsVariableNotEqualToString,
                "(1:105) and variable % does not equal {...},");

            // (1:106) and variable %Variable is constant,
            Add(new Trigger(TriggerCategory.Condition, 106), VariableIsConstant,
                "(1:106) and variable % is constant,");

            // (1:107) and variable %Variable is not constant,
            Add(new Trigger(TriggerCategory.Condition, 107), VariableIsNotConstant,
                "(1:107) and variable % is not constant,");

            // (5:100) set variable %Variable to {...}.
            Add(new Trigger(TriggerCategory.Effect, 100), SetVariableToString,
                "(5:100) set variable % to {...}.");

            // (5:101) set variable %Variable to #.
            Add(new Trigger(TriggerCategory.Effect, 101), SetVariableToNumberOrVariable,
                "(5:101) set variable % to #.");

            // (5:102) print {...} to the console.
            Add(new Trigger(TriggerCategory.Effect, 102), PrintToConsole,
                "(5:102) print {...} to the console.");

            // (5:103) get the environment variable named {...} and put it into #,
            Add(new Trigger(TriggerCategory.Effect, 103), GetEnvVariable,
                "(5:103) get the environment variable named {...} and put it into %, (ex: PATH)");

            Add(TriggerCategory.Effect, 104, RandomValueToVar,
                "(5:104) create random number and put it into variable %.");
            // (5:105) raise an error.
            Add(new Trigger(TriggerCategory.Effect, 105), RaiseAnError,
                "(5:105) raise an error.");

            // (5:110) load library from file {...}.
            Add(new Trigger(TriggerCategory.Effect, 110), LoadLibraryFromFile,
                "(5:110) load library from file {...}. (example Monkeyspeak.dll)");
        }

        public override void Unload(Page page)
        {
        }

        private bool GetEnvVariable(TriggerReader reader)
        {
            string envVar = Environment.GetEnvironmentVariable(reader.ReadString());
            var var = reader.ReadVariable(true);
            var.Value = envVar;
            return true;
        }

        private bool IsVariableDefined(TriggerReader reader)
        {
            var var = reader.ReadVariable();
            return reader.Page.HasVariable(var.Name) && var.Value != null;
        }

        private bool IsVariableEqualToNumberOrVar(TriggerReader reader)
        {
            var var = reader.ReadVariable();
            double num = 0;
            if (reader.PeekVariable())
            {
                var optVar = reader.ReadVariable();
                num = optVar.Value.As<double>();
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            return num == var.Value.As<double>();
        }

        private bool IsVariableEqualToString(TriggerReader reader)
        {
            var var = reader.ReadVariable();
            string str = null;
            if (reader.PeekString())
            {
                str = reader.ReadString();
                return var.Value.As<string>().Equals(str, StringComparison.InvariantCulture);
            }
            return false;
        }

        private bool IsVariableNotDefined(TriggerReader reader)
        {
            return !IsVariableDefined(reader);
        }

        private bool IsVariableNotEqualToNumberOrVar(TriggerReader reader)
        {
            return !IsVariableEqualToNumberOrVar(reader);
        }

        private bool IsVariableNotEqualToString(TriggerReader reader)
        {
            return !IsVariableEqualToString(reader);
        }

        private bool LoadLibraryFromFile(TriggerReader reader)
        {
            if (!reader.PeekString()) return false;
            reader.Page.LoadLibraryFromAssembly(reader.ReadString());
            return true;
        }

        private bool PrintToConsole(TriggerReader reader)
        {
            string output = reader.ReadString();
            Console.WriteLine(output);
            return true;
        }

        private bool RaiseAnError(TriggerReader reader)
        {
            string errorMsg = "";
            if (reader.PeekString()) errorMsg = reader.ReadString();
            RaiseError(errorMsg);
            return false;
        }

        private bool RandomValueToVar(TriggerReader reader)
        {
            var var = reader.ReadVariable(true);
            var.Value = (double)new Random().Next();
            return true;
        }

        private bool SetVariableToNumberOrVariable(TriggerReader reader)
        {
            var var = reader.ReadVariable(true);
            if (reader.PeekVariable())
            {
                var otherVar = reader.ReadVariable();
                var.Value = otherVar.Value;
            }
            else if (reader.PeekNumber())
            {
                var.Value = reader.ReadNumber();
            }

            return true;
        }

        private bool SetVariableToString(TriggerReader reader)
        {
            if (reader.PeekVariable())
            {
                var var = reader.ReadVariable(true);
                if (reader.PeekString())
                    var.Value = reader.ReadString();
                else var.Value = string.Empty;
                return true;
            }
            return false;
        }

        private bool VariableIsConstant(TriggerReader reader)
        {
            return reader.ReadVariable().IsConstant;
        }

        private bool VariableIsNotConstant(TriggerReader reader)
        {
            return !VariableIsConstant(reader);
        }
    }
}