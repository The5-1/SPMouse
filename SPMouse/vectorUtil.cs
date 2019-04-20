using System;
using System.Drawing;
using System.Numerics;

public static class VectorUtil
{
    public static Point toPoint(Vector2 vec)
    {
        return new Point((int)vec.X, (int)vec.Y);
    }

    public static Vector2 toVec(Point p)
    {
        return new Vector2(p.X, p.Y);
    }
    public static Vector2 toVec(int x, int y)
    {
        return new Vector2(x, y);
    }

}