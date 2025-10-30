using System;

namespace Domain
{
    public class ValidacaoException : DomainException
    {
        public ValidacaoException(string message) : base(message) { }
    }
}
