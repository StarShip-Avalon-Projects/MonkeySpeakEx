using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Monkeyspeak.Utils
{
    /// <summary>
    /// Condenses and renders scripts very hard, but not impossible, to read.  Use the compiler to make them even harder to read.
    /// </summary>
    public sealed class Obfuscator
    {
        private readonly MonkeyspeakEngine engine;

        public Obfuscator(MonkeyspeakEngine engine)
        {
            this.engine = engine;
        }

        public string Obfuscate(string code)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            using (var reader = new SStreamReader(stream, true))
            {
                reader.Position = 0;
                using (Lexer lexer = new Lexer(engine, reader))
                {
                    Token lastToken = Token.None;
                    foreach (var token in lexer.Read())
                    {
                        lastToken = token;
                        if (lastToken == Token.None) continue;
                        code = code.Remove((int)(lastToken.ValueStartPosition + lastToken.Length) + 1, (int)token.ValueStartPosition - 1);
                    }
                }
            }
            return code;
        }
    }
}