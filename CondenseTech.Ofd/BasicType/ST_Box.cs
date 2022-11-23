using System;
using System.Drawing;

namespace CondenseTech.Ofd.BasicType
{
    public class ST_Box
    {
        public ST_Box()
        {
        }

        public ST_Box(RectangleF rectangleF)
        {
            this._RectangleF = rectangleF;
        }

        public ST_Box(string raw)
        {
            this.Raw = raw;
        }

        public string Raw
        {
            get => string.Join(" ", RectangleF.X, RectangleF.Y, RectangleF.Width, RectangleF.Height);
            set
            {
                try
                {
                    value = value.Trim();
                    string[] contents = value.Split(' ');
                    if (contents.Length == 4)
                    {
                        float x = float.Parse(contents[0]), y = float.Parse(contents[1]);
                        float width = float.Parse(contents[2]), height = float.Parse(contents[3]);
                        if (width <= 0 || height <= 0)
                        {
                            // throw new ArgumentException("The width and height of an ST_Box should
                            // be bigger than 0.");
                        }
                        _RectangleF.X = x;
                        _RectangleF.Y = y;
                        _RectangleF.Width = width;
                        _RectangleF.Height = height;
                    }
                    else
                    {
                        throw new ArgumentException("ST_Box should be an array of 4 decimals or integers.");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Cannot convert \"{value}\" to an ST_Box, refer to the inner exception for more information.", ex);
                }
            }
        }

        private RectangleF _RectangleF = new RectangleF(0, 0, 0, 0);

        public RectangleF RectangleF => _RectangleF;

        public static implicit operator RectangleF(ST_Box stBox)
        {
            return stBox.RectangleF;
        }

        public static implicit operator string(ST_Box stBox)
        {
            return stBox.Raw;
        }
    }
}