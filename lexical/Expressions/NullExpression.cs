namespace Monkeyspeak.Lexical.Expressions
{
    public sealed class NullExpression : Expression<object>
    {
        private static NullExpression instance;

        static NullExpression()
        {
            var sourcePos = new SourcePosition();
            instance = new NullExpression(ref sourcePos);
        }

        public static NullExpression Instance => instance;

        protected NullExpression(ref SourcePosition pos) : base(ref pos, null)
        {
        }
    }
}