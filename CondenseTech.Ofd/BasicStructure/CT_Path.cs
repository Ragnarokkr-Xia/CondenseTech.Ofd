using System.Collections.Generic;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_Path : CT_GraphicUnit
    {
        public CT_Path()
        {
        }

        public CT_Path(CT_GraphicUnit graphicUnit)
        {
            if (graphicUnit == null) return;
            Id = graphicUnit.Id;
            Boundary = graphicUnit.Boundary;
            Name = graphicUnit.Name;
            Visible = graphicUnit.Visible;
            LineWidth = graphicUnit.LineWidth;
            Alpha = graphicUnit.Alpha;
            CTM = graphicUnit.CTM;
        }

        public bool Stroke { get; set; } = true;
        public bool Fill { get; set; } = false;

        public FillRule Rule { get; set; } = FillRule.NonZero;

        public CT_Color StrokeColor { get; set; } = null;

        public CT_Color FillColor { get; set; } = null;
        public List<PathOperation> Operations { get; set; } = null;
    }

    public enum FillRule
    {
        NonZero, EvenOdd
    }

    public enum PathCommand
    {
        Start, Move, Line, QuadraticBezier, CubicBezier, Arc, Close
    }

    public class PathOperation
    {
        public PathCommand? Command { get; set; } = null;
        public List<float> Operands { get; set; } = null;

        public static PathCommand? GetDeAbbreviatedCommand(string abbreviatedCommand)
        {
            switch (abbreviatedCommand)
            {
                case "S": return PathCommand.Start;
                case "M": return PathCommand.Move;
                case "L": return PathCommand.Line;
                case "Q": return PathCommand.QuadraticBezier;
                case "B": return PathCommand.CubicBezier;
                case "A": return PathCommand.Arc;
                case "C": return PathCommand.Close;
            }

            return null;
        }

        public static string GetAbbreviatedCommand(PathCommand command)
        {
            switch (command)
            {
                case PathCommand.Start: return "S";
                case PathCommand.Move: return "M";
                case PathCommand.Line: return "L";
                case PathCommand.QuadraticBezier: return "Q";
                case PathCommand.CubicBezier: return "B";
                case PathCommand.Arc: return "A";
                case PathCommand.Close: return "C";
            }

            return null;
        }

        public static PathOperation DeAbbreviatePathOperation(string abbreviatedPathOperation)
        {
            if (!string.IsNullOrWhiteSpace(abbreviatedPathOperation))
            {
                abbreviatedPathOperation = abbreviatedPathOperation.Trim();
                string[] operationParts = abbreviatedPathOperation.Split(' ');
                if (operationParts.Length > 0)
                {
                    PathCommand? command = PathOperation.GetDeAbbreviatedCommand(operationParts[0]);
                    if (command != null)
                    {
                        PathOperation pathOperation = new PathOperation()
                        {
                            Command = command,
                            Operands = new List<float>()
                        };
                        for (int count = 1; count < operationParts.Length; count++)
                        {
                            pathOperation.Operands.Add(float.Parse(operationParts[count]));
                        }

                        return pathOperation;
                    }
                }
            }

            return null;
        }
    }
}