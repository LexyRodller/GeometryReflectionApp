using System;

namespace GeometryReflectionApp
{
    public class Line : Shape
    {
        public double EndX { get; private set; }
        public double EndY { get; private set; }

        public Line(double startX, double startY, double endX, double endY)
        {
            CenterX = (startX + endX) / 2;
            CenterY = (startY + endY) / 2;
            EndX = endX;
            EndY = endY;
        }

        public override (double x1, double y1, double x2, double y2) GetBoundingRectangle()
        {
            return (Math.Min(CenterX, EndX), Math.Min(CenterY, EndY), 
                   Math.Max(CenterX, EndX), Math.Max(CenterY, EndY));
        }

        public override double GetArea() => 0;
    }
}