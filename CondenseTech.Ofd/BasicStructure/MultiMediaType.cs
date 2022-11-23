using System;

namespace CondenseTech.Ofd.BasicStructure
{
    public class MultiMediaType
    {
        private const MultiMediaTypeEnum DEFAULT_MULTIMEDIA_TYPE = MultiMediaTypeEnum.Image;

        public MultiMediaType()
        {
        }

        public MultiMediaType(string raw)
        {
            this.Raw = raw;
        }

        public MultiMediaType(MultiMediaTypeEnum multiMediaTypeEnum)
        {
            this.MultiMediaTypeEnum = multiMediaTypeEnum;
        }

        public string Raw
        {
            get => MultiMediaTypeEnum.ToString();
            set
            {
                value = value.Trim();
                try
                {
                    if (Enum.IsDefined(typeof(MultiMediaTypeEnum), value))
                        MultiMediaTypeEnum = (MultiMediaTypeEnum)Enum.Parse(typeof(MultiMediaTypeEnum), value, false);
                    else
                        throw new ArgumentException(
                            $"\"{value}\" is not a MultiMediaType.");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Cannot treat \"{value}\" as a MultiMediaType, refer to the inner exception for more information.", ex);
                }
            }
        }

        public MultiMediaTypeEnum MultiMediaTypeEnum { get; set; } = DEFAULT_MULTIMEDIA_TYPE;

        public static implicit operator string(MultiMediaType multiMediaType)
        {
            return multiMediaType.Raw;
        }

        public static implicit operator MultiMediaTypeEnum(MultiMediaType multiMediaType)
        {
            return multiMediaType.MultiMediaTypeEnum;
        }
    }

    public enum MultiMediaTypeEnum
    {
        Image, Audio, Video
    }
}