using System.Text.RegularExpressions;

namespace Monkeyspeak
{
    internal class TokenDefinition
    {
        #region Public Constructors

        public TokenDefinition(
            TokenType type,
            Regex regex)
            : this(type, regex, false)
        {
        }

        public TokenDefinition(
            TokenType type,
            Regex regex,
            bool isIgnored)
        {
            Type = type;
            Regex = regex;
            IsIgnored = isIgnored;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool IsIgnored { get; private set; }

        public Regex Regex { get; private set; }

        public TokenType Type { get; private set; }

        #endregion Public Properties
    }
}