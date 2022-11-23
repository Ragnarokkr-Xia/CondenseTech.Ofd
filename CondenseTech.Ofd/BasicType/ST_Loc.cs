using System;
using CondenseTech.Ofd.Miscellaneous;

namespace CondenseTech.Ofd.BasicType
{
    public class ST_Loc
    {
        public ST_Loc()
        {
        }

        public ST_Loc(string raw)
        {
            this.Raw = raw;
        }

        public string Raw
        {
            get => Location;
            set
            {
                try
                {
                    Location = value;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"Cannot treat \"{value}\" as a location, refer to the inner exception for more information.",
                        ex);
                }
            }
        }

        private string _CurrentDirectory = string.Empty;

        public string CurrentDirectory
        {
            get => _CurrentDirectory;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    value = value.Trim();
                    if (value.Length > 0)
                    {
                        _CurrentDirectory = value;
                    }
                    else
                    {
                        throw new FormatException("Not able to set an empty string as base location.");
                    }
                }
            }
        }

        private string _Location = string.Empty;

        public string Location
        {
            get => _Location;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    value = value.Trim();
                    if (value.Length > 0)
                    {
                        _Location = value;
                    }
                    else
                    {
                        throw new FormatException("Not able to set an empty string as location.");
                    }
                }
            }
        }

        public override string ToString()
        {
            return LongLocation;
        }

        public string LongLocation => UnixPath.Combine(CurrentDirectory, Location).TrimStart('/');

        public static implicit operator string(ST_Loc stLoc)
        {
            return stLoc?.LongLocation;
        }
    }
}