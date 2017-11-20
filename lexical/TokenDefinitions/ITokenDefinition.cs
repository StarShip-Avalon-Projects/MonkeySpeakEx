namespace Monkeyspeak.Lexical.TokenDefinitions
{
    public interface ITokenDefinition
    {
        TokenType Type { get; }

        char StartCharacter { get; }

        Token Create(AbstractLexer lexer, SStreamReader reader);
    }
}