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
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Line(PointF src, PointF dst, SDL.SDL_Color color, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Source = src;
            this.Destination = dst;
            this.Color = color;
        }


        /// <summary>
        /// The source point of the Line.
        /// </summary>
        public PointF Source { get; }

        /// <summary>
        /// The destination point of the Line.
        /// </summary>
        public PointF Destination { get; }

        /// <summary>
        /// The color of the Line.
        /// </summary>
        public SDL.SDL_Color Color { get; }


        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);
            
            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            SDL.SDL_Point point1 = this.GetAbsolutePoint(this.Source, screenWidth, screenHeight);
            SDL.SDL_Point point2 = this.GetAbsolutePoint(this.Destination, screenWidth, screenHeight);
            
            SDL.SDL_RenderDrawLine(renderer, point1.x, point1.y, point2.x, point2.y);
        }
    }
}