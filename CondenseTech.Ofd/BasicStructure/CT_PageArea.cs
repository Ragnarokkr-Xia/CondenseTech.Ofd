using System.Drawing;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_PageArea
    {
        private ST_Box _ContentBox = null;
        private ST_Box _ApplicationBox = null;
        private ST_Box _PhysicalBox = null;
        private ST_Box _BleedBox = null;

        public ST_Box PhysicalBox
        {
            get => _PhysicalBox;
            set
            {
                _PhysicalBox = value;
                ForwardAll();
                BackwardAll();
            }
        }

        public ST_Box ApplicationBox
        {
            get => _ApplicationBox;
            set
            {
                _ApplicationBox = value;
                ForwardAll();
                BackwardAll();
            }
        }

        public ST_Box ContentBox
        {
            get => _ContentBox;
            set
            {
                _ContentBox = value;
                ForwardAll();
                BackwardAll();
            }
        }

        public ST_Box BleedBox
        {
            get => _BleedBox;
            set
            {
                _BleedBox = value;
                ForwardAll();
                BackwardAll();
            }
        }

        private void ForwardAll()
        {
            Forward(_ContentBox, ref _ApplicationBox);
            Forward(_ApplicationBox, ref _PhysicalBox);
            Forward(_PhysicalBox, ref _BleedBox);
        }

        private void BackwardAll()
        {
            Backward(ref _PhysicalBox, _BleedBox);
            Backward(ref _ApplicationBox, _PhysicalBox);
            Backward(ref _ContentBox, _ApplicationBox);
        }

        private void Forward(ST_Box inner, ref ST_Box outer)
        {
            if (outer == null && inner != null)
            {
                outer = inner;
            }

            if (outer != null && inner != null)
            {
                outer = new ST_Box(RectangleF.Union(outer, inner));
            }
        }

        private void Backward(ref ST_Box inner, ST_Box outer)
        {
            if (inner == null && outer != null)
            {
                inner = outer;
            }

            if (inner != null && outer != null)
            {
                inner = new ST_Box(RectangleF.Intersect(outer, inner));
            }
        }
    }
}