using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders a rectangle.
    /// </summary>
    public class Rectangle : RectangularEntity {
        
        /// <summary>
        /// Constructs a <c>Rectangle</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Rectangle</c>.</param>
        /// <param name="color">The color of the <c>Rectangle</c>.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Rectangle(RectF area, Color color, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.Color = color;
        }


        /// <summary>
        /// The color of the <c>Rectangle</c>.
        /// </summary>
        public Color Color { get; set; }


        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            SDL.SDL_Rect area = this.GetAbsoluteArea(windowWidth, windowHeight);
            
            SDL.SDL_RenderDrawRect(renderer, ref area);
        }
    }
}