using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders a filled rectangle.
    /// </summary>
    public class FillRectangle : RectangularEntity {
        
        /// <summary>
        /// Constructs a <c>FillRectangle</c>.
        /// </summary>
        /// <param name="area">The area of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public FillRectangle(RectF area, SDL.SDL_Color color, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.Color = color;
        }


        /// <summary>
        /// The color of the rectangle.
        /// </summary>
        public SDL.SDL_Color Color { get; set; }


        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            SDL.SDL_Rect area = this.GetAbsoluteArea(screenWidth, screenHeight);
            
            SDL.SDL_RenderFillRect(renderer, ref area);
        }
    }
}