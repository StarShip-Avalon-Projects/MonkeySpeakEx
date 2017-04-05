using System.Collections.Generic;

namespace Monkeyspeak
{
    internal interface ILexer
    {
        #region Public Methods

        void AddDefinition(TokenDefinition tokenDefinition);

        IEnumerable<Token> Tokenize(string source);

        #endregion Public Methods
    }
}