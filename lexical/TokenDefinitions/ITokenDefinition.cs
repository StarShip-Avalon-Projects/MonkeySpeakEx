namespace Monkeyspeak.lexical.TokenDefinitions
{
    public interface ITokenDefinition
    {
        TokenType Type { get; }

        Token Create(AbstractLexer lexer, SStreamReader reader);
    }
}