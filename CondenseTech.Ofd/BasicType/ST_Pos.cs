using System;
using System.Drawing;

namespace CondenseTech.Ofd.BasicType
{
    public class ST_Pos
    {
        public ST_Pos()
        {
        }

        public ST_Pos(string raw)
        {
            this.Raw = raw;
        }

        public string Raw
        {
            get => string.Join(" ", PointF.X, PointF.Y);
            set
            {
                try
                {
                    value = value.Trim();
                    string[] contents = value.Split(' ');
                    if (contents.Length == 2)
                    {
                        float x = float.Parse(contents[0]);
                        float y = float.Parse(contents[1]);
                        _PointF.X = x;
                        _PointF.Y = y;
                    }
                    else
                    {
                        throw new ArgumentException("ST_Pos should be an array of 2 decimals or ints.");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Cannot convert \"{value}\" to an ST_Pos, refer to the inner exception for more information.", ex);
                }
            }
        }

        private PointF _PointF = new PointF(0, 0);

        public PointF PointF => _PointF;

        public static implicit operator PointF(ST_Pos stPos)
        {
            return stPos.PointF;
        }

        public static implicit operator string(ST_Pos stPos)
        {
            return stPos.Raw;
        }
    }
}