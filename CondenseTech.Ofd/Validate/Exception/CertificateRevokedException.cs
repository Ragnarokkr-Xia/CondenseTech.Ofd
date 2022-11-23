using System;

namespace CondenseTech.Ofd.Validate.Exception
{
    public class CertificateRevokedException : ApplicationException
    {
        public CertificateRevokedException()
        {
        }

        public CertificateRevokedException(string message) : base(message)
        {
        }

        public CertificateRevokedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}