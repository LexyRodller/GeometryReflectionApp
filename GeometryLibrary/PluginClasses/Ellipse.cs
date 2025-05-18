using System;

namespace GeometryReflectionApp
{
    public class Ellipse : Shape, IHasRadius
    {
        public double RadiusX { get; private set; }
        public double RadiusY { get; private set; }

        public Ellipse(double centerX, double centerY, double radiusX, double radiusY)
        {
            CenterX = centerX;
            CenterY = centerY;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public override (double x1, double y1, double x2, double y2) GetBoundingRectangle()
        {
            return (CenterX - RadiusX, CenterY - RadiusY, CenterX + RadiusX, CenterY + RadiusY);
        }

        public override double GetArea()
        {
            return Math.PI * RadiusX * RadiusY;
        }
    }
}