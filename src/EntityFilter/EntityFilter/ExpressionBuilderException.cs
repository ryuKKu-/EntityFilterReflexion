using System;

namespace EntityFilter
{
    [Serializable]
    class ExpressionBuilderException : Exception
    {
        public ExpressionBuilderException() { }

        public ExpressionBuilderException(string message) : base(message) { }

        public ExpressionBuilderException(string message, Exception inner) : base(message, inner) { }
    }
}
