namespace GeometryReflectionApp
{
    public class Point : Shape
    {
        public Point(double x, double y)
        {
            CenterX = x;
            CenterY = y;
        }

        public override (double x1, double y1, double x2, double y2) GetBoundingRectangle()
        {
            return (CenterX, CenterY, CenterX, CenterY);
        }

        public override double GetArea() => 0;
    }
}