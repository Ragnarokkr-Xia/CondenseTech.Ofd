using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Miscellaneous
{
    public static class ASN1Utility
    {
        public static DateTime GetDateTime(object AnyDerTime)
        {
            string dateTimeString = string.Empty;
            if (AnyDerTime is DerGeneralizedTime DerGeneralizedTime)
            {
                dateTimeString = DerGeneralizedTime.TimeString;
            }

            if (AnyDerTime is DerUtcTime DerUtcTime)
            {
                dateTimeString = DerUtcTime.ToString();
            }

            if (!string.IsNullOrWhiteSpace(dateTimeString))
            {
                bool converted = ValueUtility.TryConvertDateTime(dateTimeString, out DateTime dateTime);
                if (converted)
                {
                    return dateTime;
                }
                try
                {
                    Regex timeRegEx = new Regex("[0-9]+");
                    Match timeMatch = timeRegEx.Match(dateTimeString);
                    if (timeMatch.Success)
                    {
                        string universalDateString = timeMatch.Value.Substring(0, 8);
                        return DateTime.ParseExact(universalDateString, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            return DateTime.MinValue;
        }

        public static Asn1Object ExtractInnerSingleObject(Asn1Object asn1Object)
        {
            if (asn1Object is Asn1Sequence asn1Sequence)
            {
                return (Asn1Object)asn1Sequence[0];
            }
            else
            {
                return asn1Object;
            }
        }
    }
}