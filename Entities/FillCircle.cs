using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders a filled circle.
    /// </summary>
    public class FillCircle : Entity, IClickable {
        
        /// <summary>
        /// Constructs a <c>FillCircle</c>.
        /// </summary>
        /// <param name="center">The center of the <c>Circle</c>.</param>
        /// <param name="radius">The radius of the <c>Circle</c>.</param>
        /// <param name="color">The color of the <c>Circle</c>.</param>
        /// <param name="sides">The number of sides the <c>Circle</c> has. 20 by default.</param>
        /// <param name="scale">The scaling behavior of the <c>Entity</c>.</param>
        public FillCircle(PointF center, float radius, Color color, int sides = 20, Scale scale = Scale.RelativeToScreen)
        : base(scale) {
            this.Center = center;
            this.Radius = radius;
            this.Color = color;
            this.Sides = sides;
            this.ClickableEntityTraits = new ClickableEntityTraits(Drag.NotDraggable);
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
        public Color Color { get; set; }

        /// <summary>
        /// The number of sides of the <c>Circle</c>.
        /// </summary>
        public int Sides { get; }

        /// <summary>
        /// This Clickable Entity Traits.
        /// </summary>
        public ClickableEntityTraits ClickableEntityTraits { get; set; }


        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
        }

        public bool Contains((int, int) point, int windowWidth, int windowHeight) {
            var relPt = this.AbsoluteToRelative(point, windowWidth, windowHeight);

            return (relPt.x - this.Center.x) * (relPt.x - this.Center.x)
                + (relPt.y - this.Center.y) * (relPt.y - this.Center.y) <= this.Radius * this.Radius;
        }


        public override void Init() {
            for (int i = 0; i < this.Sides; i++) {
                this.AddChild(("Triangle", i),
                    new FillTriangle(new PointF(), new PointF(), new PointF(), this.Color, Scale.AbsoluteInPixels));
            }
        }

        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            if (!this.EntityTraits.Visible) {
                return;
            }
            
            var center = this.GetAbsolutePoint(this.Center, windowWidth, windowHeight);
            int radius = this.GetAbsolutePoint(new PointF { x = this.Radius, y = this.Radius }, windowWidth, windowHeight).x;

            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            int rd2 = radius * radius;

            for (uint dx = 0; dx < radius * 2; dx++) {
                SDL.SDL_RenderDrawLine(renderer,
                    (int)(center.x - radius + dx),
                    (int)(center.y + Math.Sqrt(rd2 - (dx - radius) * (dx - radius))),
                    (int)(center.x - radius + dx),
                    (int)(center.y - Math.Sqrt(rd2 - (dx - radius) * (dx - radius)))
                );
            }
        }
    }
}