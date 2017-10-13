using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monkeyspeak.lexical.Expressions
{
    public sealed class ReferenceExpression<T> : Expression where T : Expression
    {
        private readonly WeakReference<T> expr;

        public ReferenceExpression(T expr, ref SourcePosition sourcePosition) : base(ref sourcePosition)
        {
            this.expr = new WeakReference<T>(expr);
            Value = this.expr;
        }
    }
}