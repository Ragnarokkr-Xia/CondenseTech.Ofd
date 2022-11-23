using System;

namespace CondenseTech.Ofd.BasicStructure
{
    public class DocType
    {
        private const DocTypeEnum DEFAULT_DOC_TYPE = DocTypeEnum.Document;

        public DocType()
        {
        }

        public DocType(string raw)
        {
            this.Raw = raw;
        }

        public DocType(DocTypeEnum docTypeEnum)
        {
            this.DocTypeEnum = docTypeEnum;
        }

        public string Raw
        {
            get => DocTypeEnum.ToString();
            set
            {
                try
                {
                    value = value.Trim();
                    switch (value)
                    {
                        case "OFD":
                            DocTypeEnum = DocTypeEnum.Document;
                            break;

                        case "OFD-A":
                            DocTypeEnum = DocTypeEnum.Archive;
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Cannot treat \"{value}\" as a DocType, refer to the inner exception for more information.", ex);
                }
            }
        }

        public DocTypeEnum DocTypeEnum { get; set; } = DEFAULT_DOC_TYPE;

        public static implicit operator string(DocType docType)
        {
            return docType.Raw;
        }

        public static implicit operator DocTypeEnum(DocType docType)
        {
            return docType.DocTypeEnum;
        }
    }

    public enum DocTypeEnum
    {
        Document, Archive
    }
}