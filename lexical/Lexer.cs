﻿#region Usings

using Monkeyspeak.Extensions;
using Monkeyspeak.lexical;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#endregion Usings

namespace Monkeyspeak
{
    /// <summary>
    ///     Converts a reader containing a Monkeyspeak script into a
    /// </summary>
    public sealed class Lexer : AbstractLexer
    {
        private int lineNo = 1, columnNo, rawPos, currentChar;
        private char varDeclSym, stringBeginSym, stringEndSym, lineCommentSym;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lexer"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="reader">The reader.</param>
        public Lexer(MonkeyspeakEngine engine, SStreamReader reader)
            : base(engine, reader)
        {
            varDeclSym = engine.Options.VariableDeclarationSymbol;
            stringBeginSym = engine.Options.StringBeginSymbol;
            stringEndSym = engine.Options.StringEndSymbol;
            lineCommentSym = engine.Options.LineCommentSymbol;
        }

        /// <summary>
        /// Reads the tokens from the reader.  Used by the Parser.
        /// </summary>
        /// <returns></returns>
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
                }
                else if (c == Engine.Options.BlockCommentBeginSymbol[0])
                {
                    bool isBlockComment = true;
                    for (int i = 0; i <= Engine.Options.BlockCommentBeginSymbol.Length - 1; i++)
                    {
                        // Ensure it is actually a block comment
                        if (LookAhead(2 + i) != Engine.Options.BlockCommentBeginSymbol[i])
                        {
                            isBlockComment = false;
                            break;
                        }
                    }
                    if (isBlockComment)
                        SkipBlockComment();
                }
                else if (c == lineCommentSym)
                {
                    SkipLineComment();
                }
                else if (c == stringBeginSym)
                {
                    token = MatchString();
                }
                else if (c == varDeclSym)
                {
                    token = MatchVariable();
                }
                else
                {
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
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            token = MatchTrigger();
                            break;

                        default: Next(); break;
                    }
                }
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

        public bool IsMatch(string str)
        {
            return str.All(c => IsMatch(c));
        }

        public bool IsMatch(char c)
        {
            return currentChar == c;
        }

        public bool IsMatch(int c)
        {
            return currentChar == c;
        }

        public override void CheckMatch(string str)
        {
            for (int i = 0; i <= str.Length - 1; i++)
            {
                var c = LookAhead(i);
                CheckMatch(str[i], c);
            }
        }

        public override void CheckMatch(char c)
        {
            if (currentChar != c)
            {
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", c.EscapeForCSharp(), ((char)currentChar).EscapeForCSharp()), CurrentSourcePosition);
            }
        }

        public override void CheckMatch(int c)
        {
            int input = currentChar;
            if (input != c)
            {
                string inputChar = (input != -1) ? ((char)input).ToString(CultureInfo.InvariantCulture) : "END_OF_FILE";
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", ((char)c).EscapeForCSharp(), inputChar), CurrentSourcePosition);
            }
        }

        public void CheckMatch(char a, char b)
        {
            if (a != b)
            {
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", b.EscapeForCSharp(), a.EscapeForCSharp()), CurrentSourcePosition);
            }
        }

        public void CheckMatch(int a, int b)
        {
            if (a != b)
            {
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", ((char)b).EscapeForCSharp(), ((char)a).EscapeForCSharp()), CurrentSourcePosition);
            }
        }

        public override void CheckIsDigit(char c = '\0')
        {
            if (c == '\0') c = (char)currentChar;
            if (!char.IsDigit(c))
            {
                throw new MonkeyspeakException(String.Format("Expected '{0}' but got '{1}'", c.EscapeForCSharp(), ((char)currentChar).EscapeForCSharp()), CurrentSourcePosition);
            }
        }

        public override void CheckEOF(int c)
        {
            if (c == -1)
            {
                throw new MonkeyspeakException("Unexpected end of file", CurrentSourcePosition);
            }
        }

        private Token CreateToken(TokenType type)
        {
            var sourcePos = CurrentSourcePosition;
            long startPos = reader.Position;
            int length = 1;
            Next();
            return new Token(type, startPos, length, sourcePos);
        }

        private Token CreateToken(TokenType type, string str)
        {
            var sourcePos = CurrentSourcePosition;
            long startPos = reader.Position;
            int length = str.Length;
            for (int i = 0; i <= str.Length - 1; i++) Next();
            return new Token(type, startPos, length, sourcePos);
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
        public override int LookAhead(int steps)
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

        public override int LookBack(int steps)
        {
            if (!reader.BaseStream.CanSeek)
            {
                throw new NotSupportedException("Stream does not support seeking");
            }
            int aback = -1;
            long oldPosition = reader.Position;
            // Subtract 1 from the steps so that the Peek method looks at the right value
            if (reader.Position - (steps + 1) > 0)
                reader.Position -= (steps + 1);
            else reader.Position = 0;
            aback = reader.Peek();

            reader.Position = oldPosition;
            return aback;
        }

        public override int Next()
        {
            int before = LookBack(1);
            int c = reader.Read();
            if (c != -1)
            {
                if (c == '\n') //|| (before == '\r' && c == '\n'))
                {
                    lineNo++;
                    columnNo = 0;
                }
                else
                {
                    columnNo++;
                }
                rawPos++;
            }
            currentChar = c;
            return currentChar;
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
                CheckEOF(currentChar);
                Next();
                length++;
                c = (char)currentChar;
            }
            return new Token(TokenType.NUMBER, startPos, length, sourcePos);
        }

        private Token MatchString()
        {
            Next();
            CheckMatch(stringBeginSym);
            var stringLenLimit = Engine.Options.StringLengthLimit;
            var sourcePos = CurrentSourcePosition;
            long startPos = reader.Position;
            int length = 0;
            while (true)
            {
                CheckEOF(currentChar);
                if (length >= stringLenLimit)
                    throw new MonkeyspeakException($"String exceeded limit or was not terminated with a '{stringEndSym}'", CurrentSourcePosition);
                Next();
                length++;
                if (LookAhead(1) == stringEndSym)
                    break;
            }
            Next(); // hit string end sym
            CheckMatch(stringEndSym);
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
                CheckEOF(currentChar);
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

        private Token MatchVariable()
        {
            long startPos = reader.Position;
            int length = 0;
            Next();
            length++;
            var sourcePos = CurrentSourcePosition;

            CheckMatch(varDeclSym);

            Next();
            length++;
            while ((currentChar >= 'a' && currentChar <= 'z')
                   || (currentChar >= 'A' && currentChar <= 'Z')
                   || (currentChar >= '0' && currentChar <= '9')
                   || currentChar == '_' || currentChar == '@'
                   || currentChar == '$' || currentChar == '#'
                   || currentChar == '&')
            {
                CheckEOF(currentChar);

                Next();
                length++;
            }
            length--;

            if (IsMatch('['))
            {
                Next();
                length++;
                while ((currentChar >= 'a' && currentChar <= 'z')
                       || (currentChar >= 'A' && currentChar <= 'Z')
                       || (currentChar >= '0' && currentChar <= '9')
                       || currentChar == '_' || currentChar == '@'
                       || currentChar == '$' || currentChar == '#'
                       || currentChar == '&')
                {
                    CheckEOF(currentChar);

                    Next();
                    length++;
                    if (currentChar == ']')
                    {
                        break;
                    }
                }
                CheckMatch(']');
                length++;
                return new Token(TokenType.TABLE, startPos, length, CurrentSourcePosition);
            }

            if (currentChar == -1)
            {
                throw new MonkeyspeakException("Unexpected end of file", sourcePos);
            }

            return new Token(TokenType.VARIABLE, startPos, length, CurrentSourcePosition);
        }

        private void SkipBlockComment()
        {
            CheckMatch(Engine.Options.BlockCommentBeginSymbol);

            Next();
            while (LookAhead(1) != '*' && LookAhead(2) != '/')
            {
                if (currentChar == -1)
                {
                    throw new MonkeyspeakException("Unexpected end of file", CurrentSourcePosition);
                }
                Next();
            }
            CheckMatch(Engine.Options.BlockCommentEndSymbol);
        }

        private void SkipLineComment()
        {
            Next();
            CheckMatch(lineCommentSym);
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

        public override SourcePosition CurrentSourcePosition => new SourcePosition(lineNo, columnNo, rawPos);

        public int CurrentCharacter { get => currentChar; set => currentChar = value; }
    }
}