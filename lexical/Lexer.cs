#region Usings

using Monkeyspeak.lexical;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#endregion Usings

namespace Monkeyspeak
{
    /// <summary>
    ///     Converts a reader containing a Monkeyspeak script into a
    /// </summary>
    public sealed class Lexer : AbstractLexer
    {
        private int columnNo;

        private MonkeyspeakEngine engine;
        private int lineNo = 1, currentChar;

        public Lexer(MonkeyspeakEngine engine, SStreamReader reader)
            : base(reader)
        {
            this.engine = engine;
        }

        public override IEnumerable<Token> Read()
        {
            var tokens = new Queue<Token>();
            int character = 0;
            char c = (char)character;
            Token token = Token.None, lastToken = Token.None;
            while (character != -1 || token.Type != TokenType.END_OF_FILE)
            {
                token = Token.None; // needed to clear Token state
                character = LookAhead(1);
                c = (char)character;
                if (character == -1)
                {
                    token = CreateToken(TokenType.END_OF_FILE);
                    goto CONTINUE;
                }
                if (c == engine.Options.BlockCommentBeginSymbol[0])
                {
                    for (int i = 0; i <= engine.Options.BlockCommentBeginSymbol.Length - 1; i++)
                    {
                        // Ensure it is actually a block comment
                        if (LookAhead(1 + i) != engine.Options.BlockCommentBeginSymbol[i])
                            goto SkipCustoms;
                    }
                    SkipBlockComment();
                    goto CONTINUE;
                }
                else if (c == engine.Options.LineCommentSymbol)
                {
                    SkipLineComment();
                    goto CONTINUE;
                }
                else if (c == engine.Options.StringBeginSymbol)
                {
                    token = MatchString();
                    goto CONTINUE;
                }
                else if (c == engine.Options.VariableDeclarationSymbol)
                {
                    token = MatchVariable();
                    goto CONTINUE;
                }
                SkipCustoms:
                switch (c)
                {
                    case '\r':
                    case '\n':
                        //token = CreateToken(TokenType.END_STATEMENT);
                        Next();
                        break;

                    case '.':
                    case ',':
                        //token = CreateToken(TokenType.END_STATEMENT);
                        Next();
                        break;

                    //case '+':
                    //    token = CreateToken(TokenType.PLUS);
                    //    break;

                    //case '-':
                    //    token = CreateToken(TokenType.MINUS);
                    //    break;

                    //case '^':
                    //    token = CreateToken(TokenType.POWER);
                    //    break;

                    //case '~':
                    //    token = CreateToken(TokenType.CONCAT);
                    //    break;

                    //case ':':
                    //    token = CreateToken(TokenType.COLON);
                    //    break;

                    //case '(':
                    //token = CreateToken(TokenType.LPAREN);
                    //break;

                    //case ')':
                    //token = CreateToken(TokenType.RPAREN);
                    //break;

                    //case '*':
                    //    token = CreateToken(TokenType.MULTIPLY);
                    //    Next();
                    //    break;

                    //case '/':
                    //    token = CreateToken(TokenType.DIVIDE);
                    //    Next();
                    //    break;

                    case '%':
                        token = CreateToken(TokenType.MOD);
                        break;

                    case '0':
                    case '1':
                    case '5':
                        token = MatchTrigger();
                        break;

                    case '2':
                    case '3':
                    case '4':
                    // skipped 5 trigger category
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        token = MatchNumber();
                        break;

                    default: Next(); break;
                }
                CONTINUE:
                if (token.Type != TokenType.NONE)
                {
                    //Logger.Debug<Lexer>(token);
                    lastToken = token;
                    tokens.Enqueue(token);
                }

                if (tokens.Count >= 1000)
                {
                    while (tokens.Count > 1000)
                    {
                        yield return tokens.Dequeue();
                    }
                }
            }
            while (tokens.Count > 0) yield return tokens.Dequeue();
        }

        public override void Reset()
        {
            if (reader.BaseStream.CanSeek)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
            }
        }

        private void CheckMatch(string str)
        {
            foreach (var c in str)
                CheckMatch(c);
        }

        private void CheckMatch(char c)
        {
            if (currentChar != c)
            {
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", c.EscapeForCSharp(), ((char)currentChar).EscapeForCSharp()), CurrentSourcePosition);
            }
        }

        private void CheckMatch(int c)
        {
            int input = currentChar;
            if (input != c)
            {
                string inputChar = (input != -1) ? ((char)input).ToString(CultureInfo.InvariantCulture) : "END_OF_FILE";
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", ((char)c).EscapeForCSharp(), inputChar), CurrentSourcePosition);
            }
        }

        private void CheckIsDigit(char c = '\0')
        {
            if (c == '\0') c = (char)currentChar;
            if (!char.IsDigit(c))
            {
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", c.EscapeForCSharp(), ((char)currentChar).EscapeForCSharp()), CurrentSourcePosition);
            }
        }

        private void CheckEOF()
        {
            int input = currentChar;
            if (input == -1)
            {
                throw new MonkeyspeakException("Unexpected end of file", CurrentSourcePosition);
            }
        }

        private Token CreateToken(TokenType type)
        {
            long startPos = reader.Position;
            int length = 1;
            Next();
            return new Token(type, startPos, length, new SourcePosition(lineNo, columnNo));
        }

        private Token CreateToken(TokenType type, string str)
        {
            long startPos = reader.Position;
            int length = str.Length;
            for (int i = 0; i <= str.Length - 1; i++) Next();
            return new Token(type, startPos, length, new SourcePosition(lineNo, columnNo));
        }

        public override char[] Read(long startPosInStream, int length)
        {
            if (!reader.BaseStream.CanSeek)
            {
                throw new NotSupportedException("Stream does not support forward reading");
            }
            long oldPos = reader.Position;
            reader.Position = startPosInStream;

            var buf = new char[length];
            reader.Read(buf, 0, length);

            reader.Position = oldPos;

            return buf;
        }

        /// <summary>
        ///     Peeks ahead in the reader
        /// </summary>
        /// <param name="steps"></param>
        /// <returns>The character number of steps ahead or -1/returns>
        private int LookAhead(int steps)
        {
            if (!reader.BaseStream.CanSeek)
            {
                throw new NotSupportedException("Stream does not support seeking");
            }
            int ahead = -1;
            if (steps > 1)
            {
                long oldPosition = reader.Position;
                // Subtract 1 from the steps so that the Peek method looks at the right value
                reader.Position = reader.Position + (steps - 1);

                ahead = reader.Peek();

                reader.Position = oldPosition;
            }
            else
            {
                ahead = reader.Peek();
            }
            return ahead;
        }

        private int LookBack(int steps)
        {
            if (!reader.BaseStream.CanSeek)
            {
                throw new NotSupportedException("Stream does not support seeking");
            }
            int aback = -1;
            long oldPosition = reader.Position;
            // Subtract 1 from the steps so that the Peek method looks at the right value
            reader.Position = reader.Position - (steps + 1);

            aback = reader.Peek();

            reader.Position = oldPosition;
            return aback;
        }

        private void Next()
        {
            int c = -1;
            int before = LookBack(1);
            c = reader.Read();
            if (c != -1)
                if (c == '\n' || (before == '\r' && c == '\n'))
                {
                    lineNo++;
                    columnNo = 0;
                }
                else
                {
                    columnNo++;
                }
            currentChar = c;
        }

        private Token MatchNumber()
        {
            var sourcePos = CurrentSourcePosition;
            long startPos = reader.Position;
            Next();
            int length = 0;
            char c = (char)currentChar;
            while (char.IsDigit(c) || currentChar == '.')
            {
                CheckEOF();
                Next();
                length++;
                c = (char)currentChar;
            }
            return new Token(TokenType.NUMBER, startPos, length, sourcePos);
        }

        private Token MatchString()
        {
            Next();
            var sourcePos = CurrentSourcePosition;
            long startPos = reader.Position;
            int length = 0;
            while (true)
            {
                CheckEOF();
                if (length >= engine.Options.StringLengthLimit)
                    throw new MonkeyspeakException("String exceeded limit or was not terminated with a closing bracket", CurrentSourcePosition);
                Next();
                length++;
                if (LookAhead(1) == engine.options.StringEndSymbol)
                    break;
            }
            return new Token(TokenType.STRING_LITERAL, startPos, length, sourcePos);
        }

        private Token MatchTrigger()
        {
            if (LookAhead(2) != ':') // is trigger
            {
                return MatchNumber(); // is not trigger
            }
            var sourcePos = CurrentSourcePosition;
            long startPos = reader.Position;
            int length = 1;
            Next(); // trigger category
            CheckIsDigit();
            Next(); // seperator
            length++;
            CheckMatch(':');
            char c = (char)LookAhead(1);
            CheckIsDigit(c);
            while (char.IsDigit(c))
            {
                CheckEOF();
                Next();
                length++;
                c = (char)currentChar;
                if (!char.IsDigit(c))
                {
                    length--;
                    break;
                }
            }
            CheckIsDigit((char)LookBack(1));
            return new Token(TokenType.TRIGGER, startPos, length, sourcePos);
        }

        private Token MatchVariable(bool isReference = false)
        {
            long startPos = reader.Position;
            int length = 0;
            Next();
            length++;
            var sourcePos = CurrentSourcePosition;

            CheckMatch(engine.Options.VariableDeclarationSymbol);

            Next();
            length++;
            while (true)
            {
                if (!((currentChar >= 'a' && currentChar <= 'z')
                   || (currentChar >= 'A' && currentChar <= 'Z')
                   || (currentChar >= '0' && currentChar <= '9')
                   || currentChar == '_'))
                {
                    length--;
                    break;
                }
                Next();
                length++;
            }

            if (currentChar == -1)
            {
                throw new MonkeyspeakException("Unexpected end of file", sourcePos);
            }

            #region Variable Table Handling

            if (LookAhead(1) == '[')
            {
                while (true)
                {
                    if (currentChar == -1)
                    {
                        throw new MonkeyspeakException("Unexpected end of file", CurrentSourcePosition);
                    }
                    Next();
                    length++;
                    if (currentChar == ']')
                    {
                        break;
                    }
                    if (!((currentChar >= 'a' && currentChar <= 'z')
                        || (currentChar >= 'A' && currentChar <= 'Z')
                        || (currentChar >= '0' && currentChar <= '9')
                        || currentChar == '_'))
                    {
                        throw new MonkeyspeakException("Variable list not terminated", CurrentSourcePosition);
                    }
                }
            }

            #endregion Variable Table Handling

            return new Token(TokenType.VARIABLE, startPos, length, CurrentSourcePosition);
        }

        private void SkipBlockComment()
        {
            CheckMatch(engine.Options.BlockCommentBeginSymbol);

            Next();
            while (true)
            {
                if (currentChar == -1)
                {
                    throw new MonkeyspeakException("Unexpected end of file", CurrentSourcePosition);
                }
                if (LookAhead(1) == '*' && LookAhead(2) == '/')
                {
                    break;
                }
                Next();
            }
            Next();
            CheckMatch(engine.Options.BlockCommentEndSymbol);
        }

        private void SkipLineComment()
        {
            Next();
            CheckMatch(engine.Options.LineCommentSymbol);
            char c = (char)LookAhead(1);
            while (true)
            {
                if (currentChar == -1)
                    break;
                Next();
                c = (char)currentChar;
                if (c == '\n') break;
            }
            if (currentChar != -1)
                CheckMatch('\n');
        }

        public override SourcePosition CurrentSourcePosition => new SourcePosition(lineNo, columnNo);
    }
}