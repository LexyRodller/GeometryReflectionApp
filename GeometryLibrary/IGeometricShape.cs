namespace GeometryLibrary
{
    public interface IGeometricShape
    {
        double CenterX { get; }
        double CenterY { get; }
        (double x1, double y1, double x2, double y2) GetBoundingRectangle();
        double GetArea();
    }
}