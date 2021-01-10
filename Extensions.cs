using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer {
    public static class Extensions {

        public static bool Contains(this RectF rect, SDL.SDL_Point point) {
            return point.x >= rect.x && point.x <= rect.x + rect.w
                && point.y >= rect.y && point.y <= rect.y + rect.h;
        }
    }
}