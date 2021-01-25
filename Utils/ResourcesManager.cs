using System;
using System.Collections.Generic;
using SDL2;

namespace SceneDisplayer.Utils {
    public static class ResourcesManager {

        static ResourcesManager() {
            CachedTextures = new Dictionary<string, IntPtr>();
        }


        public static Dictionary<string, IntPtr> CachedTextures { get; }


        public static (IntPtr, int, int) GetBitmapTexture(IntPtr renderer,
        string text, int fontSize, string font, SDL.SDL_Color color) {
            var bitmapFont = SDL_ttf.TTF_OpenFont(font, fontSize);

            if (bitmapFont == IntPtr.Zero) {
                throw new ArgumentException("Invalid Font path given");
            }

            var surface = SDL_ttf.TTF_RenderText_Blended(bitmapFont, text, color);

            int w, h;
            SDL_ttf.TTF_SizeText(bitmapFont, text, out w, out h);

            return (SDL.SDL_CreateTextureFromSurface(renderer, surface), w, h);
        }
    }
}