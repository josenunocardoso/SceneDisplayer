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
        /// <param name="center">The center of the <c>FillCircle</c>.</param>
        /// <param name="radius">The radius of the <c>FillCircle</c>.</param>
        /// <param name="color">The color of the <c>FillCircle</c>.</param>
        /// <param name="radiusRelativePosition">Whether the radius value should be relative to the X axis, or to the Y axis.
        /// This is only relevant if the Scale is <c>RelativeToScreen</c></param>
        /// <param name="scale">The scaling behavior of the <c>Entity</c>.</param>
        public FillCircle(PointF center, float radius, Color color,
            RadiusRelativePosition radiusRelativePosition = RadiusRelativePosition.RelativeToX,
            Scale scale = Scale.RelativeToScreen)
        : base(scale) {
            this.Center = center;
            this.Radius = radius;
            this.Color = color;
            this.RadiusRelativePosition = radiusRelativePosition;
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
        /// Whether the radius value should be relative to the X axis, or to the Y axis.
        /// </summary>
        public RadiusRelativePosition RadiusRelativePosition { get; set; }

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


        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            if (!this.EntityTraits.Visible) {
                return;
            }
            
            var center = this.GetAbsolutePoint(this.Center, windowWidth, windowHeight);
            int radius = 0;
            if (this.EntityTraits.Scale == Scale.AbsoluteInPixels) {
                radius = (int)this.Radius;
            }
            else if (this.EntityTraits.Scale == Scale.RelativeToScreen) {
                var radiusPt = this.GetAbsolutePoint(new PointF(this.Radius, this.Radius), windowWidth, windowHeight);
                radius = this.RadiusRelativePosition == RadiusRelativePosition.RelativeToX ? radiusPt.x : radiusPt.y;
            }

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

    public enum RadiusRelativePosition {
        RelativeToX,
        RelativeToY
    }
}