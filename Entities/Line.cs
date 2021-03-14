using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders a line.
    /// </summary>
    public class Line : Entity {

        /// <summary>
        /// Constructs a <c>Line</c>.
        /// </summary>
        /// <param name="src">The source point of the <c>Line</c>.</param>
        /// <param name="dst">The destination point of the <c>Line</c>.</param>
        /// <param name="color">The color of the <c>Line</c>.</param>
        /// <param name="scale">The scaling behavior of the <c>Entity</c>.</param>
        public Line(PointF src, PointF dst, Color color, Scale scale = Scale.RelativeToScreen)
        : base(scale) {
            this.Source = src;
            this.Destination = dst;
            this.Color = color;
        }


        /// <summary>
        /// The source point of the Line.
        /// </summary>
        public PointF Source { get; set;}

        /// <summary>
        /// The destination point of the Line.
        /// </summary>
        public PointF Destination { get; set; }

        /// <summary>
        /// The color of the Line.
        /// </summary>
        public Color Color { get; set;}


        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            if (!this.EntityTraits.Visible) {
                return;
            }
            
            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            SDL.SDL_Point point1 = this.GetAbsolutePoint(this.Source, windowWidth, windowHeight);
            SDL.SDL_Point point2 = this.GetAbsolutePoint(this.Destination, windowWidth, windowHeight);
            
            SDL.SDL_RenderDrawLine(renderer, point1.x, point1.y, point2.x, point2.y);
        }
    }
}