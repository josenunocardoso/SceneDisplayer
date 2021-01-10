using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    public class Line : Entity {

        public Line(PointF src, PointF dst, SDL.SDL_Color color, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Source = src;
            this.Destination = dst;
            this.Color = color;
        }

        public PointF Source { get; }

        public PointF Destination { get; }

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