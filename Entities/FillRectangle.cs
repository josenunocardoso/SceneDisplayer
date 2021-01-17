using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    public class FillRectangle : RectangularEntity {
        
        public FillRectangle(RectF area, SDL.SDL_Color color, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.Color = color;
        }


        public SDL.SDL_Color Color { get; set; }


        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            SDL.SDL_SetRenderDrawColor(renderer, this.Color.r, this.Color.g, this.Color.b, this.Color.a);

            SDL.SDL_Rect area = this.GetAbsoluteArea(screenWidth, screenHeight);
            
            SDL.SDL_RenderFillRect(renderer, ref area);
        }
    }
}