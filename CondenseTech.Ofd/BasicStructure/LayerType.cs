using System;

namespace CondenseTech.Ofd.BasicStructure
{
    public class LayerType
    {
        private const LayerTypeEnum DEFAULT_LAYER_TYPE = LayerTypeEnum.Body;

        public LayerType()
        {
        }

        public LayerType(string raw)
        {
            this.Raw = raw;
        }

        public LayerType(LayerTypeEnum layerTypeEnum)
        {
            this.LayerTypeEnum = layerTypeEnum;
        }

        public string Raw
        {
            get => LayerTypeEnum.ToString();
            set
            {
                value = value.Trim();
                try
                {
                    if (Enum.IsDefined(typeof(LayerTypeEnum), value))
                        LayerTypeEnum = (LayerTypeEnum)Enum.Parse(typeof(LayerTypeEnum), value, false);
                    else
                        throw new ArgumentException(
                            $"\"{value}\" is not a LayerType.");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Cannot treat \"{value}\" as a LayerType, refer to the inner exception for more information.", ex);
                }
            }
        }

        public LayerTypeEnum LayerTypeEnum { get; set; } = DEFAULT_LAYER_TYPE;

        public static implicit operator string(LayerType layerType)
        {
            return layerType.Raw;
        }

        public static implicit operator LayerTypeEnum(LayerType layerType)
        {
            return layerType.LayerTypeEnum;
        }
    }

    public enum LayerTypeEnum
    {
        Body, Foreground, Background
    }
}