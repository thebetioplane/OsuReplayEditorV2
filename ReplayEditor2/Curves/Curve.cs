using System;
using System.Collections.Generic;
using BMAPI.v1;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ReplayEditor2.Curves
{
    public struct DistanceTime
    {
        public float distance;
        public float t;
        public Vector2 point;
    }
    public abstract class Curve
    {
        public SliderType CurveType { get; set; }
        protected List<Vector2> Points;
        protected List<DistanceTime> CurveSnapshots;
        public float PixelLength { get; set; }

        public Curve(SliderType sliderType)
        {
            this.CurveType = sliderType;
            this.Points = new List<Vector2>();
            this.CurveSnapshots = new List<DistanceTime>();
            this.PixelLength = Single.PositiveInfinity;
        }

        public void AddPoint(Vector2 point)
        {
            this.Points.Add(point);
        }

        private float _Length = -1;
        public float Length
        {
            get
            {
                return this._Length;
            }
        }

        protected abstract Vector2 Interpolate(float t);

        private float CalculateLength(float prec = 0.01f)
        {
            float sum = 0;
            for (float f = 0; f < 1f; f += prec)
            {
                if (f > 1)
                {
                    f = 1;
                }
                float fplus = f + prec;
                if (fplus > 1)
                {
                    fplus = 1;
                }
                Vector2 a = this.Interpolate(f);
                Vector2 b = this.Interpolate(fplus);
                float distance = this.Distance(a, b);
                if (sum == 0 || (this.PixelLength > 0 && distance + sum <= this.PixelLength))
                {
                    sum += distance;
                    this.AddDistanceTime(sum, f, b);
                }
                else
                {
                    break;
                }
            }
            return sum;
        }

        private void AddDistanceTime(float distance, float time, Vector2 point)
        {
            DistanceTime dt = new DistanceTime();
            dt.distance = distance;
            dt.t = time;
            dt.point = point;
            this.CurveSnapshots.Add(dt);
        }

        public void Init()
        {
            this._Length = this.CalculateLength();
        }

        protected Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return (1 - t) * a + t * b;
        }

        protected float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        protected float Atan2(Vector2 a)
        {
            return (float)Math.Atan2(a.Y, a.X);
        }

        public Vector2 PositionAtDistance(float d)
        {
            int high = this.CurveSnapshots.Count - 1;
            int low = 0;
            while (low <= high)
            {
                int mid = (high + low) / 2;
                if (mid == high || mid == low)
                {
                    if (mid + 1 >= this.CurveSnapshots.Count)
                    {
                        return this.CurveSnapshots[mid].point;
                    }
                    else
                    {
                        DistanceTime a = this.CurveSnapshots[mid];
                        DistanceTime b = this.CurveSnapshots[mid + 1];
                        return this.Lerp(a.point, b.point, (d - a.distance) / (b.distance - a.distance));
                    }
                }
                if (this.CurveSnapshots[mid].distance > d)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }
            return Vector2.Zero;
        }
    }
}
