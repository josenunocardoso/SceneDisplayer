using SDL2;

namespace SceneDisplayer.Utils {
    /// <summary>
    /// Represents a floated rectangle.
    /// </summary>
    public struct RectF {
        public float x;
        public float y;
        public float w;
        public float h;

        /// <summary>
        /// Constructs a RectF.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y Position.</param>
        /// <param name="w">Width.</param>
        /// <param name="h">Height.</param>
        public RectF(float x, float y, float w, float h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }


        /// <summary>
        /// Returns an equivalent <see cref="SDL.SDL_Rect"/>, with the values casted to <see cref="int"/>.
        /// </summary>
        /// <returns>The SDL_Rect.</returns>
        public SDL.SDL_Rect ToSDLRect() {
            return new SDL.SDL_Rect {
                x = (int)this.x,
                y = (int)this.y,
                w = (int)this.w,
                h = (int)this.h
            };
        }

        /// <summary>
        /// Returns whether this RectF contains a given point.
        /// </summary>
        /// <param name="point">The point to compare.</param>
        /// <returns>True if it contains, False otherwise.</returns>
        public bool Contains(SDL.SDL_Point point) {
            return this.Contains(new PointF {
                x = point.x, y = point.y
            });
        }

        /// <summary>
        /// Returns whether this RectF contains a given point.
        /// </summary>
        /// <param name="point">The point to compare.</param>
        /// <returns>True if it contains, False otherwise.</returns>
        public bool Contains(PointF point) {
            return point.x >= this.x && point.x <= this.x + this.w
                && point.y >= this.y && point.y <= this.y + this.h;
        }


        public static RectF operator +(RectF r1, RectF r2) {
            return new RectF(r1.x + r2.x, r1.y + r2.y, r1.w + r2.w, r1.h + r2.h);
        }

        public static RectF operator +(RectF r, PointF p) {
            return new RectF(r.x + p.x, r.y + p.y, r.w, r.h);
        }

        public static RectF operator -(RectF r1, RectF r2) {
            return new RectF(r1.x - r2.x, r1.y - r2.y, r1.w - r2.w, r1.h - r2.h);
        }

        public static RectF operator -(RectF r, PointF p) {
            return new RectF(r.x - p.x, r.y - p.y, r.w, r.h);
        }

        public static RectF operator *(RectF r1, RectF r2) {
            return new RectF(r1.x * r2.x, r1.y * r2.y, r1.w * r2.w, r1.h * r2.h);
        }


        public override string ToString() {
            return $"({x};{y};{w};{h})";
        }
    }
}