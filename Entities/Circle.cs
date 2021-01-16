using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    public class Circle : Entity, IClickable {
        
        public Circle(PointF center, float radius, SDL.SDL_Color color, int sides = 10, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Center = center;
            this.Radius = radius;
            this.Color = color;
            this.Sides = sides;
        }


        public PointF Center { get; set; }

        public float Radius { get; set; }

        public SDL.SDL_Color Color { get; set; }

        public int Sides { get; set; }


        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
        }

        public bool Contains(SDL.SDL_Point point) {
            return (point.x - this.Center.x) * (point.x - this.Center.x)
                + (point.y - this.Center.y) * (point.y - this.Center.y) <= this.Radius * this.Radius;
        }


        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            var center = this.GetAbsolutePoint(this.Center, screenWidth, screenHeight);
            float radius = this.GetAbsolutePoint(new PointF { x = this.Radius, y = this.Radius }, screenWidth, screenHeight).x;

            float sides = this.Sides;
            if (sides == 0) {
                sides = (float)Math.PI * this.Radius;
            }

            float d_a = (float)Math.PI * 2 / this.Sides;
            float angle = d_a;

            PointF start, end;
            end.x = this.Radius + center.x;
            end.y = center.y;

            for (int i = 0; i != sides; i++) {
                start = end;
                end.x = (float)Math.Cos(angle) * this.Radius + center.x;
                end.y = (float)Math.Sin(angle) * this.Radius + center.y;
                
                angle += d_a;

                using (var line = new Line(start, end, this.Color, false)) {
                    line.Draw(renderer, screenWidth, screenHeight);
                }
            } 
        }
    }
}