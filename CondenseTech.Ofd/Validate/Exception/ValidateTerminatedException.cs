using System;

namespace CondenseTech.Ofd.Validate.Exception
{
    public class ValidateTerminatedException : ApplicationException
    {
        public ValidateTerminatedException()
        {
        }

        public ValidateTerminatedException(string message) : base(message)
        {
        }

        public ValidateTerminatedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}