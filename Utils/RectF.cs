using SDL2;

namespace SceneDisplayer.Utils {
    public struct RectF {
        public float x;
        public float y;
        public float w;
        public float h;


        public SDL.SDL_Rect ToSDLRect() {
            return new SDL.SDL_Rect {
                x = (int)this.x,
                y = (int)this.y,
                w = (int)this.w,
                h = (int)this.h
            };
        }

        public bool Contains(SDL.SDL_Point point) {
            return this.Contains(new PointF {
                x = point.x, y = point.y
            });
        }

        public bool Contains(PointF point) {
            return point.x >= this.x && point.x <= this.x + this.w
                && point.y >= this.y && point.y <= this.y + this.h;
        }

        public override string ToString() {
            return $"({x};{y};{w};{h})";
        }
    }
}