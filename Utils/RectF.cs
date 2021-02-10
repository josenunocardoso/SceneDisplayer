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

        public override string ToString() {
            return $"({x};{y};{w};{h})";
        }
    }
}