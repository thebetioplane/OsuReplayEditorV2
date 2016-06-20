using System;
using System.Collections.Generic;
using System.Linq;
using ReplayEditor2.Curves;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BMAPI.v1.HitObjects
{
    public class SliderObject : CircleObject
    {
        public SliderObject() { }
        public SliderObject(CircleObject baseInstance) : base(baseInstance) { }

        public new SliderType Type = SliderType.Linear;
        public List<Point2> Points = new List<Point2>();
        public int RepeatCount { get; set; }
        private float _PixelLength = 0f;
        public float PixelLength
        {
            get { return this._PixelLength; }
            set { this._PixelLength = value; }
        }
        private float _TotalLength = -1;
        public float TotalLength
        {
            get
            {
                return this._TotalLength;
            }
        }
        private float _SegmentEndTime = -1;
        public float SegmentEndTime
        {
            get
            {
                if (this._SegmentEndTime < 0)
                {
                    this._SegmentEndTime = this.StartTime + this.TotalLength / this.Velocity;
                }
                return this._SegmentEndTime;
            }
        }
        public float Velocity { get; set; }
        public float MaxPoints { get; set; }
        public List<Curve> Curves { get; set; }

        public void CreateCurves()
        {
            this.Curves = new List<Curve>();
            int n = this.Points.Count;
            if (n == 0)
            {
                return;
            }
            Point2 lastPoint = this.Points[0];
            Curve currentCurve = null;
            for (int i = 0; i < n; i++)
            {
                if (lastPoint.Equals(this.Points[i]))
                {
                    currentCurve = this.CreateCurve();
                    this.Curves.Add(currentCurve);
                }
                currentCurve.AddPoint(this.Points[i].ToVector2());
                lastPoint = this.Points[i];
            }
            this._TotalLength = 0;
            int lastN = this.Curves.Count - 1;
            for (int i = 0; i < lastN; i++)
            {
                this.Curves[i].Init();
                this._TotalLength += this.Curves[i].Length;
            }
            if (lastN >= 0)
            {
                Curve lastCurve = this.Curves[lastN];
                lastCurve.PixelLength = this.PixelLength - this._TotalLength;
                lastCurve.Init();
                this._TotalLength += lastCurve.Length;
            }
        }

        private Curve CreateCurve()
        {
            if (this.Points.Count == 0)
            {
                return null;
            }
            else if (this.Points.Count == 1)
            {
                return new Catmull();
            }
            else if (this.Points.Count == 2)
            {
                return new Line();
            }
            else if (this.Points.Count > 3)
            {
                return new Bezier();
            }
            switch (this.Type)
            {
                case SliderType.Linear:
                    return new Line();
                case SliderType.Bezier:
                    return new Bezier();
                case SliderType.PSpline:
                    return new Circle();
                case SliderType.CSpline:
                    return new Catmull();
                default:
                    return null;
            }
        }

        public Vector2 PositionAtTime(float t)
        {
            return this.PositionAtDistance(this.TotalLength * t);
        }

        public Vector2 PositionAtDistance(float d)
        {
            float sum = 0;
            foreach (Curve curve in this.Curves)
            {
                if (sum + curve.Length >= d)
                {
                    return curve.PositionAtDistance(d - sum);
                }
                sum += curve.Length;
            }
            Curve lastCurve = this.Curves[this.Curves.Count - 1];
            return lastCurve.PositionAtDistance(d - (sum - lastCurve.Length));
        }
    }      
}
