namespace Monkeyspeak.lexical.Expressions
{
    /// <summary>
    /// Expression for the END OF STATEMENT
    /// </summary>
    public sealed class EOSExpression : Expression
    {
        public EOSExpression(ref SourcePosition pos)
            : base(ref pos)
        { }
    }
}