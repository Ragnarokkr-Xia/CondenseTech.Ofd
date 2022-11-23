using System;

namespace CondenseTech.Ofd.BasicStructure
{
    public class DocUsage
    {
        private const DocUsageEnum DEFAULT_DOC_USAGE = DocUsageEnum.Normal;

        public DocUsage()
        {
        }

        public DocUsage(string raw)
        {
            this.Raw = raw;
        }

        public DocUsage(DocUsageEnum docUsageEnum)
        {
            this.DocUsageEnum = docUsageEnum;
        }

        public string Raw
        {
            get => DocUsageEnum.ToString();
            set
            {
                value = value.Trim();
                try
                {
                    if (Enum.IsDefined(typeof(DocUsageEnum), value))
                        DocUsageEnum = (DocUsageEnum)Enum.Parse(typeof(DocUsageEnum), value, false);
                    else
                        throw new ArgumentException(
                            $"\"{value}\" is not a DocUsage.");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Cannot treat \"{value}\" as a DocUsage, refer to the inner exception for more information.", ex);
                }
            }
        }

        public DocUsageEnum DocUsageEnum { get; set; } = DEFAULT_DOC_USAGE;

        public static implicit operator string(DocUsage docUsage)
        {
            return docUsage.Raw;
        }

        public static implicit operator DocUsageEnum(DocUsage docUsage)
        {
            return docUsage.DocUsageEnum;
        }
    }

    public enum DocUsageEnum
    {
        Normal, EBook, ENewsPaper, EMagzine
    }
}