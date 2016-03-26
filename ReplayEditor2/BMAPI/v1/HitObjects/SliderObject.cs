using System;
using System.Collections.Generic;
using System.Linq;

namespace BMAPI.v1.HitObjects
{
    internal struct DistanceTime
    {
        public float distance;
        public float t;
        public Point2 point;
    }
    public class SliderObject : CircleObject
    {
        public SliderObject() { }
        public SliderObject(CircleObject baseInstance) : base(baseInstance) { }

        public new SliderType Type = SliderType.Linear;
        public List<Point2> Points = new List<Point2>();
        public int RepeatCount { get; set; }
        public float Velocity { get; set; }
        public float MaxPoints { get; set; }
        private List<DistanceTime> distanceTime = new List<DistanceTime>();
        private float? S_Length { get; set; }
        private float? S_SegmentLength { get; set; }
        public float Length
        {
            get
            {
                if (S_Length != null) return (float)S_Length;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_Length = (float)Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2)) * RepeatCount;
                        break;
                    case SliderType.CSpline:
                    case SliderType.PSpline:
                        S_Length = SplineLength(Points) * RepeatCount;
                        break;
                    case SliderType.Bezier:
                        S_Length = BezierLength(Points) * RepeatCount;
                        break;
                    default:
                        S_Length = 0;
                        break;
                }
                return (float)S_Length;
            }
        }
        public float SegmentLength
        {
            get
            {
                if (S_SegmentLength != null) return (float)S_SegmentLength;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_SegmentLength = (float)Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2));
                        break;
                    case SliderType.CSpline:
                    case SliderType.PSpline:
                        S_SegmentLength = SplineLength(Points);
                        break;
                    case SliderType.Bezier:
                        S_SegmentLength = BezierLength(Points);
                        break;
                    default:
                        S_SegmentLength = 0;
                        break;
                }
                return (float)S_SegmentLength;
            }
        }
        public float SegmentEndTime(int SegmentNumber)
        {
            switch (Type)
            {
                case SliderType.Linear:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                case SliderType.CSpline:
                case SliderType.PSpline:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                case SliderType.Bezier:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                default:
                    return 0;
            }
        }
        public Point2 PositionAtTime(float T)
        {
            switch (Type)
            {
                case SliderType.Linear:
                    return Points[0].Lerp(Points[1], T);
                case SliderType.CSpline:
                case SliderType.PSpline:
                    return UniformSpeed(Points, this.Length / this.RepeatCount * T);
                case SliderType.Bezier:
                    return UniformSpeed(Points, this.Length / this.RepeatCount * T);
                default:
                    return new Point2();
            }
        }

        public Point2 SplInterpolate(float t)
        {
            if (Points.Count == 3)
            {
                Point2 center = this.circleCenter(Points[0], Points[1], Points[2]);
                float radius = Points[0].DistanceTo(center);
                float start = (Points[0] - center).Atan2();
                float middle = (Points[1] - center).Atan2();
                float end = (Points[2] - center).Atan2();
                float twopi = (float)(2 * Math.PI);
                Point2 basepoint = (Points[0] + Points[1] + Points[2]) / 2f;
                if (isClockwise(start, middle))
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
                return new Point2((float)(Math.Cos(t) * radius), (float)(Math.Sin(t) * radius)) + center;
            }
            else
            {
                return BezInterpolate(Points, t);
            }
        }

        private bool isClockwise(float a, float b)
        {
            float pi = (float)Math.PI;
            float twopi = 2f * pi;
            return (a - b + twopi) % twopi > pi;
        }

        private Point2 circleCenter(Point2 A, Point2 B, Point2 C)
        {
            float yDelta_a = B.Y - A.Y;
            float xDelta_a = B.X - A.X;
            float yDelta_b = C.Y - B.Y;
            float xDelta_b = C.X - B.X;
            Point2 center = new Point2();
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

        public float SplineLength(List<Point2> Pts, float prec = 0.01f)
        {
            float sum = 0;
            for (float f = 0; f < 1f; f += prec)
            {
                Point2 a = SplInterpolate(f);
                Point2 b = SplInterpolate(f + prec);
                float distance = (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
                DistanceTime dt = new DistanceTime();
                sum += distance;
                dt.distance = sum;
                dt.t = f;
                dt.point = b;
                this.distanceTime.Add(dt);
            }
            return sum;
        }

        // Bezier Interpolation
        public Point2 BezInterpolate(List<Point2> Pts, float t)
        {
            int n = Pts.Count;
            if (n == 2)
            {
                return Points[0].Lerp(Points[1], t);
            }
            Point2[] points = new Point2[Pts.Count];

            for (int i = 0; i < n; i++)
            {
                points[i] = Pts[i] + new Point2();
            }

            for (int k = 1; k < n; k++)
            {
                for (int i = 0; i < n - k; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }
            return points[0];
        }

        public Point2 UniformSpeed(List<Point2> points, float target)
        {
            int high = this.distanceTime.Count - 1;
            int low = 0;
            while (low <= high)
            {
                int mid = (high + low) / 2;
                if (mid == high || mid == low)
                {
                    if (mid + 1 >= this.distanceTime.Count)
                    {
                        return this.distanceTime[mid].point;
                    }
                    else
                    {
                        DistanceTime a = this.distanceTime[mid];
                        DistanceTime b = this.distanceTime[mid + 1];
                        return a.point.Lerp(b.point, (target - a.distance) / (b.distance - a.distance));
                    }
                }
                if (this.distanceTime[mid].distance > target)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }
            return new Point2();
        }
        
        public float BezierLength(List<Point2> Pts, float prec = 0.01f)
        {
            float sum = 0;
            for (float f = 0; f < 1f; f += prec)
            {
                Point2 a = BezInterpolate(Pts, f);
                Point2 b = BezInterpolate(Pts, f + prec);
                float distance = (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
                DistanceTime dt = new DistanceTime();
                sum += distance;
                dt.distance = sum;
                dt.t = f;
                dt.point = b;
                this.distanceTime.Add(dt);
            }
            return sum;
        }

        public override bool ContainsPoint(Point2 Point)
        {
            return ContainsPoint(Point, 0);
        }

        public bool ContainsPoint(Point2 Point, int Time)
        {
            Point2 pAtTime = PositionAtTime(Time);
            return Math.Sqrt(Math.Pow(Point.X - pAtTime.X, 2) + Math.Pow(Point.Y - pAtTime.Y, 2)) <= Radius;            
        }
    }
}
