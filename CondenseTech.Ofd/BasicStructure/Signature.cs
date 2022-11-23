using System;
using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class Signature
    {
        public ST_Loc FileLocation { get; set; }
        public SignatureType Type { get; set; }
        public SignedInfo SignedInfo { get; set; }

        public ST_Loc SignedValue { get; set; }
    }

    public class SignedInfo
    {
        public Provider Provider { get; set; }

        public string SignatureMethod { get; set; }

        public DateTime SignatureDateTime { get; set; }

        public References References { get; set; }

        public List<StampAnnotation> StampAnnotations { get; set; }
    }

    public class Provider
    {
        public string ProviderName { get; set; }

        public string Company { get; set; }

        public string Version { get; set; }
    }

    public class References
    {
        public List<Reference> Reference { get; set; }

        public string CheckMethod { get; set; }
    }

    public class Reference
    {
        public byte[] CheckValue { get; set; }

        public ST_Loc FileRef { get; set; }
    }

    public class StampAnnotation
    {
        public ST_ID PageRef { get; set; }

        public ST_ID Id { get; set; }

        public ST_Box Boundary { get; set; }
        public ST_Box Clip { get; set; }
    }
}