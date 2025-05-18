using GeometryLibrary;

namespace GeometryReflectionApp
{
    public abstract class Shape : IGeometricShape
    {
        public double CenterX { get; protected set; }
        public double CenterY { get; protected set; }

        public abstract (double x1, double y1, double x2, double y2) GetBoundingRectangle();
        public abstract double GetArea();
    }

    public interface IHasRadius
    {
        double RadiusX { get; }
        double RadiusY { get; }
    }
}