using System;

namespace lab05
{
    public class InvalidArmyOperationException : Exception
    {
        public InvalidArmyOperationException(string message) : base(message) { }
    }

    public class TransformerOverloadException : Exception
    {
        public TransformerOverloadException(string message) : base(message) { }
    }

    public class HumanNameEmptyException : Exception
    {
        public HumanNameEmptyException(string message) : base(message) { }
    }
}
