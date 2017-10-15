using Monkeyspeak.lexical;
using System;
using System.Collections.Generic;

namespace Monkeyspeak
{
    public abstract class AbstractLexer : IDisposable
    {
        protected readonly SStreamReader reader;

        protected AbstractLexer(SStreamReader reader)
        {
            this.reader = reader;
        }

        public virtual SourcePosition CurrentSourcePosition
        {
            get;
        }

        public abstract IEnumerable<Token> Read();

        public abstract void Reset();

        public void Dispose()
        {
            reader.Dispose();
            GC.SuppressFinalize(this);
        }

        public abstract char[] Read(long valueStart, int valueLength);
    }
}