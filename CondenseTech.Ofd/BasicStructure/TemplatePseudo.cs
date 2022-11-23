using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class TemplatePseudo
    {
        public ST_ID Id { get; set; } = new ST_ID();
        public ST_Loc BaseLoc { get; set; } = new ST_Loc();
        public string Name { get; set; } = string.Empty;
        public LayerType ZOrder { get; set; } = new LayerType();
    }
}