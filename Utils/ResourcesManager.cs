using System;
using SDL2;

namespace SceneDisplayer.Utils {
    /// <summary>
    /// Class responsible to create and manage cached textures.
    /// </summary>
    public static class ResourcesManager {

        /// <summary>
        /// Returns a BitmapFont of a given text template.
        /// </summary>
        /// <param name="renderer">Pointer of the renderer used.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font path.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="color">The color of the text.</param>
        /// <returns>A tuple with a pointer to the texture, the width and the height of the texture, respectively.</returns>
        public static (IntPtr, int, int) GetTextBitmap(IntPtr renderer,
        string text, string font, int fontSize, SDL.SDL_Color color) {
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