using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders a circle.
    /// </summary>
    public class Circle : Entity, IClickable {
        
        /// <summary>
        /// Constructs a <c>Circle</c>.
        /// </summary>
        /// <param name="center">The center of the <c>Circle</c>.</param>
        /// <param name="radius">The radius of the <c>Circle</c>.</param>
        /// <param name="color">The color of the <c>Circle</c>.</param>
        /// <param name="sides">The number of sides the <c>Circle</c> has. 10 by default.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Circle(PointF center, float radius, SDL.SDL_Color color, int sides = 10, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Center = center;
            this.Radius = radius;
            this.Color = color;
            this.Sides = sides;
        }


        /// <summary>
        /// The center of the <c>Circle</c>.
        /// </summary>
        public PointF Center { get; set; }

        /// <summary>
        /// The radius of the <c>Circle</c>.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// The color of the <c>Circle</c>.
        /// </summary>
        public SDL.SDL_Color Color { get; set; }

        /// <summary>
        /// The number of sides of the <c>Circle</c>.
        /// </summary>
        public int Sides { get; set; }


        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
        }

        public bool Contains(SDL.SDL_Point point, int screenWidth, int screenHeight) {
            var relPt = this.AbsoluteToRelative(point, screenWidth, screenHeight);

            return (relPt.x - this.Center.x) * (relPt.x - this.Center.x)
                + (relPt.y - this.Center.y) * (relPt.y - this.Center.y) <= this.Radius * this.Radius;
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