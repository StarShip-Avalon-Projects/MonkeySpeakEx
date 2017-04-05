using System.Collections.Generic;

namespace Monkeyspeak.lexical
{
    internal abstract class AbstractParser
    {
        #region Protected Fields

        protected MonkeyspeakEngine Engine;
        protected ILexer Lexer;

        #endregion Protected Fields

        #region Protected Constructors

        protected AbstractParser(MonkeyspeakEngine engine, ILexer lexer)
        {
            Engine = engine;
            Lexer = lexer;
        }

        #endregion Protected Constructors

        #region Public Methods

        public abstract List<TriggerList> Parse(string source);

        #endregion Public Methods
    }
}