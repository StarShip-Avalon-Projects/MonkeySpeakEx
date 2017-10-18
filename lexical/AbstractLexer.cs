using Monkeyspeak.lexical;
using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    public abstract class AbstractLexer : IDisposable
    {
        protected readonly SStreamReader reader;

        protected AbstractLexer(MonkeyspeakEngine engine, SStreamReader reader)
        {
            this.reader = reader;
            Engine = engine;
        }

        public virtual MonkeyspeakEngine Engine { get; private set; }

        public virtual SourcePosition CurrentSourcePosition
        {
            get;
        }

        /// <summary>
        /// Advances one character in the reader.
        /// </summary>
        protected abstract void Next();

        /// <summary>
        /// Reads the tokens from the reader.  Used by the Parser.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Token> Read();

        /// <summary>
        /// Reads the specified start position in stream.  Used by the Token to read the token's value.
        /// </summary>
        /// <param name="startPosInStream">The start position in stream.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public abstract char[] Read(long startPosInStream, int length);

        protected abstract int LookAhead(int amount);

        protected abstract int LookBack(int amount);

        public abstract void Reset();

        public void Dispose()
        {
            reader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}