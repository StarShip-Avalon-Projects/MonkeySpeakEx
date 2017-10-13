using Monkeyspeak.Extensions;
using System;

namespace Monkeyspeak.Libraries
{
    public class Math : BaseLibrary
    {
        public Math()
        {
            // (1:150) and variable %Variable is greater than #,
            Add(new Trigger(TriggerCategory.Condition, 150), VariableGreaterThan,
                "(1:150) and variable %Variable is greater than #,");
            // (1:151) and variable %Variable is greater than or equal to #,
            Add(new Trigger(TriggerCategory.Condition, 151), VariableGreaterThanOrEqual,
                "(1:151) and variable %Variable is greater than or equal to #,");

            // (1:152) and variable %Variable is less than #,
            Add(new Trigger(TriggerCategory.Condition, 152), VariableLessThan,
                "(1:152) and variable %Variable is less than #,");

            // (1:153) and variable %Variable is less than or equal to #,
            Add(new Trigger(TriggerCategory.Condition, 153), VariableLessThanOrEqual,
                "(1:153) and variable %Variable is less than or equal to #,");

            // (5:150) take variable %Variable and add # to it.
            Add(new Trigger(TriggerCategory.Effect, 150), AddToVariable,
                "(5:150) take variable %Variable and add # to it.");

            // (5:151) take variable %Variable and substract it by #.
            Add(new Trigger(TriggerCategory.Effect, 151), SubtractFromVariable,
                "(5:151) take variable %Variable and subtract # from it.");

            // (5:152) take variable %Variable and multiply it by #.
            Add(new Trigger(TriggerCategory.Effect, 152), MultiplyByVariable,
                "(5:152) take variable %Variable and multiply it by #.");

            // (5:153) take variable %Variable and divide it by #.
            Add(new Trigger(TriggerCategory.Effect, 153), MultiplyByVariable,
                "(5:153) take variable %Variable and divide it by #.");
        }

        public override void Unload(Page page)
        {
        }

        private bool AddToVariable(TriggerReader reader)
        {
            var toAssign = reader.ReadVariable(true);
            double num = 0;
            if (reader.PeekVariable())
            {
                var valueVariable = reader.ReadVariable();
                if (valueVariable.Value is double)
                    num = (double)valueVariable.Value;
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            toAssign.Value = toAssign.Value.As<double>() + num;
            return true;
        }

        private bool DivideByVariable(TriggerReader reader)
        {
            var toAssign = reader.ReadVariable(true);
            double num = 0;
            if (reader.PeekVariable())
            {
                var valueVariable = reader.ReadVariable();
                if (valueVariable.Value is double)
                    num = (double)valueVariable.Value;
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            toAssign.Value = toAssign.Value.As<double>() / num;
            return true;
        }

        private bool MultiplyByVariable(TriggerReader reader)
        {
            var toAssign = reader.ReadVariable(true);
            double num = 0;
            if (reader.PeekVariable())
            {
                var valueVariable = reader.ReadVariable();
                if (valueVariable.Value is double)
                    num = (double)valueVariable.Value;
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            toAssign.Value = toAssign.Value.As<double>() * num;
            return true;
        }

        private bool SubtractFromVariable(TriggerReader reader)
        {
            var toAssign = reader.ReadVariable(true);
            double num = 0;
            if (reader.PeekVariable())
            {
                var valueVariable = reader.ReadVariable();
                if (valueVariable.Value is double)
                    num = (double)valueVariable.Value;
            }
            else if (reader.PeekNumber())
            {
                num = reader.ReadNumber();
            }

            toAssign.Value = toAssign.Value.As<double>() - num;
            return true;
        }

        private bool VariableGreaterThan(TriggerReader reader)
        {
            var mainVar = reader.ReadVariable();
            double num = 0;
            if (reader.TryReadVariable(out IVariable var))
            {
                return mainVar.Value.As<double>() > var.Value.As<double>();
            }
            else if (reader.TryReadNumber(out num))
            {
                return mainVar.Value.As<double>() > num;
            }
            return false;
        }

        private bool VariableGreaterThanOrEqual(TriggerReader reader)
        {
            var mainVar = reader.ReadVariable();
            double num = 0;
            if (reader.TryReadVariable(out IVariable var))
            {
                return mainVar.Value.As<double>() >= var.Value.As<double>();
            }
            else if (reader.TryReadNumber(out num))
            {
                return mainVar.Value.As<double>() >= num;
            }
            return false;
        }

        private bool VariableLessThan(TriggerReader reader)
        {
            var mainVar = reader.ReadVariable();
            double num = 0;
            if (reader.TryReadVariable(out IVariable var))
            {
                return mainVar.Value.As<double>() < var.Value.As<double>();
            }
            else if (reader.TryReadNumber(out num))
            {
                return mainVar.Value.As<double>() < num;
            }
            return false;
        }

        private bool VariableLessThanOrEqual(TriggerReader reader)
        {
            var mainVar = reader.ReadVariable();
            double num = 0;
            if (reader.TryReadVariable(out IVariable var))
            {
                return mainVar.Value.As<double>() <= var.Value.As<double>();
            }
            else if (reader.TryReadNumber(out num))
            {
                return mainVar.Value.As<double>() <= num;
            }
            return false;
        }
    }
}