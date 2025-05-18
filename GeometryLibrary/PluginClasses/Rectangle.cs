using System;

namespace GeometryReflectionApp
{
    public class Rectangle : Shape
    {
        public double Width { get; private set; }
        public double Height { get; private set; }

        public Rectangle(double centerX, double centerY, double width, double height)
        {
            CenterX = centerX;
            CenterY = centerY;
            Width = width;
            Height = height;
        }

        public override (double x1, double y1, double x2, double y2) GetBoundingRectangle()
        {
            double halfWidth = Width / 2;
            double halfHeight = Height / 2;
            return (CenterX - halfWidth, CenterY - halfHeight, 
                    CenterX + halfWidth, CenterY + halfHeight);
        }

        public override double GetArea()
        {
            return Width * Height;
        }

        public void SetSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}