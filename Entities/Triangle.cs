using System;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    public class Triangle : Entity, IClickable {
        
        /// <summary>
        /// Constructs a <c>Triangle</c>.
        /// </summary>
        /// <param name="p1">The first point of the <c>Triangle</c>.</param>
        /// <param name="p2">The second point of the <c>Triangle</c>.</param>
        /// <param name="p3">The third point of the <c>Triangle</c>.</param>
        /// <param name="color">The color of the <c>Triangle</c>.</param>
        /// <param name="scale">The scaling behavior of the <c>Entity</c>.</param>
        public Triangle(PointF p1, PointF p2, PointF p3, Color color, Scale scale = Scale.RelativeToScreen)
        : base(scale) {
            this.Point1 = p1;
            this.Point2 = p2;
            this.Point3 = p3;
            this.Color = color;
            this.ClickableEntityTraits = new ClickableEntityTraits(Drag.NotDraggable);
        }


        /// <summary>
        /// The first point of the <c>Triangle</c>.
        /// </summary>
        public PointF Point1 { get; set; }

        /// <summary>
        /// The second point of the <c>Triangle</c>.
        /// </summary>
        public PointF Point2 { get; set; }

        /// <summary>
        /// The third point of the <c>Triangle</c>.
        /// </summary>
        public PointF Point3 { get; set; }

        /// <summary>
        /// The color of the <c>Triangle</c>.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// This Clickable Entity Traits.
        /// </summary>
        public ClickableEntityTraits ClickableEntityTraits { get; set; }


        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
        }

        private float Sign(PointF p1, PointF p2, PointF p3) {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public bool Contains((int, int) point, int windowWidth, int windowHeight) {
            var relPt = this.AbsoluteToRelative(point, windowWidth, windowHeight);

            float d1 = Sign(relPt, this.Point1, this.Point2);
            float d2 = Sign(relPt, this.Point2, this.Point3);
            float d3 = Sign(relPt, this.Point3, this.Point1);

            bool has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !has_neg || !has_pos;
        }


        public override void Init() {
            this.AddChild("Rect1", new Line(this.Point1, this.Point2, this.Color, this.EntityTraits.Scale));
            this.AddChild("Rect2", new Line(this.Point2, this.Point3, this.Color, this.EntityTraits.Scale));
            this.AddChild("Rect3", new Line(this.Point3, this.Point1, this.Color, this.EntityTraits.Scale));
        }

        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            var rect1 = this.GetChild<Line>("Rect1");
            var rect2 = this.GetChild<Line>("Rect2");
            var rect3 = this.GetChild<Line>("Rect3");

            rect1.Source = this.Point1;
            rect1.Destination = this.Point2;

            rect2.Source = this.Point2;
            rect2.Destination = this.Point3;

            rect3.Source = this.Point3;
            rect3.Destination = this.Point1;
        }
    }
}