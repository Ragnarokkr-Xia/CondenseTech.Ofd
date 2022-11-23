using System;
using System.Text.RegularExpressions;
using System.Xml;
using CondenseTech.Ofd.Miscellaneous;

namespace CondenseTech.Ofd.BasicType
{
    public class ST_ID
    {
        public ST_ID()
        {
        }

        public ST_ID(string raw)
        {
            this.Raw = raw;
        }

        public string Raw
        {
            get => Id.ToString();
            set
            {
                value = value.Trim();
                Regex sRegex = new Regex("[sS]*([0-9]+)");
                Match sMatch = sRegex.Match(value);
                if (sMatch.Success)
                {
                    value = sMatch.Groups[1].Value;
                }
                try
                {
                    Id = uint.Parse(value);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Cannot treat \"{value}\" as an ST_Id, refer to the inner exception for more information.", ex);
                }
            }
        }

        private uint _Id = 0;

        public uint Id
        {
            get => _Id;
            set
            {
                if (value > 0)
                {
                    _Id = value;
                }
                else
                {
                    throw new ArgumentException("ST_ID should have a value bigger than 0.");
                }
            }
        }

        public bool StIdEquals(ST_ID stId)
        {
            if (stId != null)
            {
                return Id == stId.Id;
            }
            return false;
        }

        public static ST_ID GetIdInAttribute(XmlNode targetNode)
        {
            string idString = XmlUtility.LoadNodeAttributeInnerText(targetNode, "ID");
            if (!string.IsNullOrWhiteSpace(idString))
            {
                return new ST_ID(idString);
            }

            return null;
        }

        public static implicit operator string(ST_ID stId)
        {
            return stId.Raw;
        }

        public static implicit operator uint(ST_ID stId)
        {
            return stId.Id;
        }
    }
}