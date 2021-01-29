using System;
using System.Collections.Generic;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders an image.
    /// </summary>
    public class Image : RectangularEntity {

        static Image() {
            CachedTextures = new Dictionary<TextureCaracteristics, IntPtr>();
        }

        /// <summary>
        /// Constructs an <c>Image</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Image</c>.</param>
        /// <param name="path">The path of the texture of the <c>Image</c>. (e.g. a jpg or bmp file)</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Image(RectF area, string path, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.ImagePath = path;
        }


        private static Dictionary<TextureCaracteristics, IntPtr> CachedTextures { get; set; }

        /// <summary>
        /// The image path.
        /// </summary>
        public string ImagePath { get; set; }


        private void CreateTexture(IntPtr renderer, TextureCaracteristics key) {
            var temp = SDL_image.IMG_Load(this.ImagePath);
            var texture = SDL.SDL_CreateTextureFromSurface(renderer, temp);

            SDL.SDL_FreeSurface(temp);

            if (texture == IntPtr.Zero) {
                SDL.SDL_DestroyTexture(texture);
                throw new ArgumentException("Texture given was not loaded");
            }

            CachedTextures[key] = texture;
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            var key = new TextureCaracteristics(this.ImagePath);

            if (!CachedTextures.ContainsKey(key)) {
                this.CreateTexture(renderer, key);
            }

            var texture = CachedTextures[key];

            var area = this.GetAbsoluteArea(screenWidth, screenHeight);

            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref area);
        }
    }

    struct TextureCaracteristics {
        public string ImagePath;

        public TextureCaracteristics(string path) {
            this.ImagePath = path;
        }
    }
}