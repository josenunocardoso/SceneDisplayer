using System;
using SDL2;
using SceneDisplayer.Utils;

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
        /// <param name="scale">The scaling behavior of the <c>Entity</c>.</param>
        public FillRectangle(RectF area, Color color, Scale scale = Scale.RelativeToScreen)
        : base(area, scale) {
            this.Color = color;
        }


        /// <summary>
        /// The color of the rectangle.
        /// </summary>
        public Color Color { get; set; }


        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            if (!this.Traits.Visible) {
                return;
            }

            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            SDL.SDL_Rect area = this.GetAbsoluteArea(windowWidth, windowHeight);
            
            SDL.SDL_RenderFillRect(renderer, ref area);
        }
    }
}