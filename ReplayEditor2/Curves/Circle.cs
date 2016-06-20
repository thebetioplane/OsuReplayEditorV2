using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ReplayEditor2.Curves
{
    public class Circle : Curve
    {
        public Circle() : base(BMAPI.v1.SliderType.PSpline)
        {
        }

        protected override Vector2 Interpolate(float t)
        {
            if (this.Points.Count == 3)
            {
                Vector2 center = this.CircleCenter(this.Points[0], this.Points[1], this.Points[2]);
                float radius = this.Distance(this.Points[0], center);
                float start = this.Atan2(this.Points[0] - center);
                float end = this.Atan2(this.Points[2] - center);
                float twopi = (float)(2 * Math.PI);
                if (this.IsClockwise(this.Points[0], this.Points[1], this.Points[2]))
                {
                    while (end < start)
                    {
                        end += twopi;
                    }
                }
                else
                {
                    while (start < end)
                    {
                        start += twopi;
                    }
                }
                t = start + (end - start) * t;
                return new Vector2((float)(Math.Cos(t) * radius), (float)(Math.Sin(t) * radius)) + center;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        private Vector2 CircleCenter(Vector2 A, Vector2 B, Vector2 C)
        {
            float yDelta_a = B.Y - A.Y;
            float xDelta_a = B.X - A.X;
            float yDelta_b = C.Y - B.Y;
            float xDelta_b = C.X - B.X;
            Vector2 center = new Vector2();
            if (xDelta_a == 0)
            {
                xDelta_a = 0.00001f;
            }
            if (xDelta_b == 0)
            {
                xDelta_b = 0.00001f;
            }
            float aSlope = yDelta_a / xDelta_a;
            float bSlope = yDelta_b / xDelta_b;
            center.X = (aSlope * bSlope * (A.Y - C.Y) + bSlope * (A.X + B.X) - aSlope * (B.X + C.X)) / (2 * (bSlope - aSlope));
            center.Y = -1 * (center.X - (A.X + B.X) / 2) / aSlope + (A.Y + B.Y) / 2;
            return center;
        }

        private bool IsClockwise(Vector2 a, Vector2 b, Vector2 c)
        {
            return a.X * b.Y - b.X * a.Y + b.X * c.Y - c.X * b.Y + c.X * a.Y - a.X * c.Y > 0;
        }
    }
}
