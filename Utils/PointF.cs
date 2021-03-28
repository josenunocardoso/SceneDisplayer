using System;

namespace SceneDisplayer.Utils {
    /// <summary>
    /// Represents a floated point.
    /// </summary>
    public struct PointF : IEquatable<PointF> {
        public float x;
        public float y;

        /// <summary>
        /// Constructs a PointF.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y Position.</param>
        public PointF(float x, float y) {
            this.x = x;
            this.y = y;
        }


        public static PointF operator +(PointF p1, PointF p2) {
            return new PointF(p1.x + p2.x, p1.y + p2.y);
        }
        
        public static PointF operator -(PointF p1, PointF p2) {
            return new PointF(p1.x - p2.x, p1.y - p2.y);
        }

        public static PointF operator *(PointF p1, PointF p2) {
            return new PointF(p1.x * p2.x, p1.y * p2.y);
        }

        public static PointF operator *(PointF p, float multiplier) {
            return new PointF(p.x * multiplier, p.y * multiplier);
        }

        public static PointF operator /(PointF p, float divider) {
            return new PointF(p.x / divider, p.y / divider);
        }

        /// <summary>
        /// Returns a PointF with the position at (0, 0).
        /// </summary>
        public static PointF Zero => new PointF(0, 0);


        public bool Equals(PointF obj) {
            return this.x == obj.x && this.y == obj.y;
        }

        public override bool Equals(object obj) {
            return obj is PointF && this.Equals((PointF)obj);
        }

        public override int GetHashCode() {
            return (int)(this.x * 31 + this.y * 47);
        }

        public override string ToString() {
            return $"({x};{y})";
        }
    }
}