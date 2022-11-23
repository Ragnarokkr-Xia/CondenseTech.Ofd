using System;

namespace CondenseTech.Ofd.Validate.Exception
{
    public class SignatureDistortedException : ApplicationException
    {
        public SignatureDistortedException()
        {
        }

        public SignatureDistortedException(string message) : base(message)
        {
        }

        public SignatureDistortedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}