using System.Runtime.InteropServices;

namespace Monkeyspeak.lexical
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SourcePosition
    {
        private int line;

        private int col;

        public SourcePosition(int line, int column)
        {
            this.line = line;
            col = column;
        }

        public int Line
        {
            get { return line; }
        }

        public int Column
        {
            get { return col; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SourcePosition))
            {
                return false;
            }
            SourcePosition pos = (SourcePosition)obj;
            return line == pos.line && this.col == pos.col;
        }

        public override string ToString()
        {
            return $"Line {line}, Column {col}";
        }
    }
}