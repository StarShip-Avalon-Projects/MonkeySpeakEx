namespace Monkeyspeak
{
    internal class TokenPosition
    {
        #region Public Constructors

        public TokenPosition(int index, int line, int column)
        {
            Index = index;
            Line = line;
            Column = column;
        }

        #endregion Public Constructors

        #region Public Properties

        public int Column { get; private set; }

        public int Index { get; private set; }

        public int Line { get; private set; }

        #endregion Public Properties
    }
}