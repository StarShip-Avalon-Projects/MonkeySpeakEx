using Monkeyspeak.lexical;
using Monkeyspeak.lexical.Expressions;
using Shared.Core.Logging;
using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Monkeyspeak.lexical.AbstractParser" />
    public sealed class Parser : AbstractParser
    {
        public TokenVisitorHandler VisitToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public Parser(MonkeyspeakEngine engine) :
            base(engine)
        {
        }

        /// <summary>
        /// Parses the specified lexer's tokens.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns></returns>
        /// <exception cref="MonkeyspeakException">
        /// </exception>
        /// <exception cref="Exception">String length limit exceeded.</exception>
        public override IEnumerable<TriggerList> Parse(AbstractLexer lexer)
        {
            var block = new TriggerList(20);
            Trigger currentTrigger = Trigger.None, prevTrigger = Trigger.None;
            Token token = Token.None, prevToken = Token.None;
            Expression expr = null;
            foreach (var t in lexer.Read())
            {
                token = t;

                if (VisitToken != null)
                    token = VisitToken(ref token);

                var sourcePos = token.Position;

                string value = token.GetValue(lexer);
                if (Engine.Options.Debug) Logger.Debug<Parser>(value);

                switch (token.Type)
                {
                    case TokenType.TRIGGER:
                        if (currentTrigger != Trigger.None)
                        {
                            if (prevTrigger != Trigger.None)
                            {
                                if (prevTrigger.Category == TriggerCategory.Effect && currentTrigger.Category == TriggerCategory.Cause)
                                {
                                    yield return block;
                                    prevTrigger = Trigger.None;
                                    block = new TriggerList();
                                }
                            }
                            block.Add(currentTrigger);
                            prevTrigger = currentTrigger;
                            currentTrigger = Trigger.None;
                        }
                        currentTrigger = new Trigger((TriggerCategory)IntParse(value.Substring(0, value.IndexOf(':'))),
                            IntParse(value.Substring(value.IndexOf(':') + 1)));
                        break;

                    case TokenType.VARIABLE:
                        if (currentTrigger == Trigger.None) throw new MonkeyspeakException($"Trigger was null. \nPrevious trigger = {prevTrigger}\nToken = {token}");
                        expr = new VariableExpression(ref sourcePos, value);
                        break;

                    case TokenType.STRING_LITERAL:
                        if (value.Length > Engine.Options.StringLengthLimit) throw new Exception("String length limit exceeded.");
                        if (currentTrigger == Trigger.None) throw new MonkeyspeakException($"Trigger was null. \nPrevious trigger = {prevTrigger}\nToken = {token}");
                        expr = new StringExpression(ref sourcePos, value);
                        break;

                    case TokenType.NUMBER:
                        double val = double.Parse(value, System.Globalization.NumberStyles.AllowDecimalPoint);
                        if (currentTrigger == Trigger.None) throw new MonkeyspeakException($"Trigger was null. \nPrevious trigger = {prevTrigger}\nToken = {token}");
                        expr = new NumberExpression(ref sourcePos, val);
                        break;

                    case TokenType.COMMENT:
                        // we don't care about comments
                        break;

                    case TokenType.END_OF_FILE:
                        if (currentTrigger != Trigger.None)
                        {
                            if (currentTrigger.Category != TriggerCategory.Undefined)
                            {
                                block.Add(currentTrigger);
                                currentTrigger = Trigger.None;
                                yield return block;
                            }
                        }
                        break;

                    default: break;
                }
                if (currentTrigger != Trigger.None && expr != null)
                {
                    currentTrigger.contents.Add(expr);
                    expr = null;
                }
            }
        }

        /// <summary>
        /// Ints the parse. (I love GhostDoc lol)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int IntParse(string value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                result = 10 * result + (value[i] - 48);
            }
            return result;
        }
    }
}