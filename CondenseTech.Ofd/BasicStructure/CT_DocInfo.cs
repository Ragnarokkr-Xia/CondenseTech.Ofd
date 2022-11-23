using System;
using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_DocInfo
    {
        public string DocId { get; set; } = null;
        public string Title { get; set; } = null;
        public string Author { get; set; } = null;
        public string Subject { get; set; } = null;
        public string Abstract { get; set; } = null;
        public DateTime? CreationDate { get; set; } = null;
        public DateTime? ModDate { get; set; } = null;
        public DocUsage DocUsage { get; set; } = new DocUsage();
        public ST_Loc Cover { get; set; } = null;
        public List<string> KeywordList { get; set; } = null;
        public string Creator { get; set; } = null;
        public string CreatorVersion { get; set; } = null;

        public Dictionary<string, string> CustomDataList = null;
    }

    public class CustomData
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public CustomData()
        {
        }

        public CustomData(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}