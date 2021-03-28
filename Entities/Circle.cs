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
        /// <param name="sides">The number of sides the <c>Circle</c> has. 20 by default.</param>
        /// <param name="radiusRelativePosition">Whether the radius value should be relative to the X axis, or to the Y axis.
        /// This is only relevant if the Scale is <c>RelativeToScreen</c></param>
        /// <param name="scale">The scaling behavior of the <c>Entity</c>.</param>
        public Circle(PointF center, float radius, Color color, int sides = 20,
            RadiusRelativePosition radiusRelativePosition = RadiusRelativePosition.RelativeToX,
            Scale scale = Scale.RelativeToScreen)
        : base(scale) {
            this.Center = center;
            this.Radius = radius;
            this.Color = color;
            this.Sides = sides;
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
        /// The number of sides of the <c>Circle</c>.
        /// </summary>
        public int Sides { get; }

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


        public override void Init() {
            for (int i = 0; i < this.Sides; i++) {
                this.AddChild(("Side", i), new Line(new PointF(), new PointF(), this.Color, Scale.AbsoluteInPixels));
            }
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

            float sides = this.Sides;
            if (sides == 0) {
                sides = (float)Math.PI * radius;
            }

            float d_a = (float)Math.PI * 2 / this.Sides;
            float angle = d_a;

            PointF start, end;
            end.x = radius + center.x;
            end.y = center.y;

            for (int i = 0; i < sides; i++) {
                start = end;
                end.x = (float)Math.Cos(angle) * radius + center.x;
                end.y = (float)Math.Sin(angle) * radius + center.y;
                
                angle += d_a;

                var line = this.GetChild<Line>(("Side", i));
                line.Source = start;
                line.Destination = end;
                line.Color = this.Color;
            }
        }
    }
}