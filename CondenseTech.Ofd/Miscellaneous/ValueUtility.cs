using System;
using System.Globalization;

namespace CondenseTech.Ofd.Miscellaneous
{
    public static class ValueUtility
    {
        public static byte TryConvert2Byte(string value, byte defaultValue)
        {
            byte result = defaultValue;
            byte.TryParse(value, out result);
            return result;
        }

        public static int TryConvert2Int(string value, int defaultValue)
        {
            int result = defaultValue;
            int.TryParse(value, out result);
            return result;
        }

        public static float TryConvert2Float(string value, float defaultValue)
        {
            float result = defaultValue;
            float.TryParse(value, out result);
            return result;
        }

        public static double TryConvert2Double(string value, double defaultValue)
        {
            double result = defaultValue;
            double.TryParse(value, out result);
            return result;
        }

        public static bool TryConvertDateTime(string value, out DateTime dateTime)
        {
            string[] formats =
            {
                "yyyyMMdd",
                "yyyy年MM月dd日",
                "yyyyMMddhhmmssZ",
                "yyyyMMddhhmmss.fffZ" ,
                "yyyy-MM-ddThh:mm:ss",
                // slash separator:
                "d/M/y",        //no leading 0s
                "dd/M/y",       //leading 0 in date
                "d/MM/y",       //leading 0 in month
                "dd/MM/y",      //leading 0 in date & month
                "d/M/yyyy",     //leading 0 in year
                "dd/M/yyyy",    //leading 0 in date & year
                "d/M/yyyy",     //leading 0 in date, month & year
                // dash separator:
                "d-M-y",        //no leading 0s
                "dd-M-y",       //leading 0 in date
                "d-MM-y",       //leading 0 in month
                "dd-MM-y",      //leading 0 in date & month
                "d-M-yyyy",     //leading 0 in year
                "dd-M-yyyy",    //leading 0 in date & year
                "d-M-yyyy",     //leading 0 in date, month & year
                // dot separator:
                "d.M.y",        //no leading 0s
                "dd.M.y",       //leading 0 in date
                "d.MM.y",       //leading 0 in month
                "dd.MM.y",      //leading 0 in date & month
                "d.M.yyyy",     //leading 0 in year
                "dd.M.yyyy",    //leading 0 in date & year
                "d.M.yyyy",     //leading 0 in date, month & year
            };
            return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateTime);
        }
    }
}