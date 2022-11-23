using System.Collections.Generic;

namespace CondenseTech.Ofd.BasicStructure
{
    public partial class OfdEntry
    {
        private const string DEFAULT_VERSION = "1.0";

        public string Version { get; set; } = DEFAULT_VERSION;

        public DocType DocType { get; set; } = new DocType();

        public List<DocBody> DocBodyList = new List<DocBody>();
    }
}