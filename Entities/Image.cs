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
        /// Constructs a opaque <c>Image</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Image</c>.</param>
        /// <param name="path">The path of the texture of the <c>Image</c>. (e.g. a jpg or bmp file)</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Image(RectF area, string path, bool relativeToScreenSize = true)
        : this(area, path, 0xFF, relativeToScreenSize) {

        }

        /// <summary>
        /// Constructs a <c>Image</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Image</c>.</param>
        /// <param name="path">The path of the texture of the <c>Image</c>. (e.g. a jpg or bmp file)</param>
        /// <param name="alpha">The image alpha channel. Set it to 0x0 to make the image transparent.
        /// Set it to 0xFF to make it opaque.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Image(RectF area, string path, byte alpha, bool relativeToScreenSize = true)
        : this(area, path, alpha, 0.0, relativeToScreenSize) {

        }

        /// <summary>
        /// Constructs a <c>Image</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Image</c>.</param>
        /// <param name="path">The path of the texture of the <c>Image</c>. (e.g. a jpg or bmp file)</param>
        /// <param name="alpha">The image alpha channel. Set it to 0x0 to make the image transparent.
        /// Set it to 0xFF to make it opaque.</param>
        /// <param name="angle">The angle in degrees of the rotation applied on the center of the image.
        /// The default is 0.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Image(RectF area, string path, byte alpha, double angle, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.ImagePath = path;
            this.Alpha = alpha;
            this.Angle = angle;
        }


        private static Dictionary<TextureCaracteristics, IntPtr> CachedTextures { get; set; }

        /// <summary>
        /// The image path.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The image alpha channel. Set it to 0x0 to make the image transparent. Set it to 0xFF to make it opaque.
        /// </summary>
        public byte Alpha { get; set; }

        /// <summary>
        /// The angle in degrees of the rotation applied on the center of the image.
        /// </summary>
        public double Angle { get; set; }


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

        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            if (this.ImagePath == null) {
                return;
            }

            var key = new TextureCaracteristics(this.ImagePath);

            if (!CachedTextures.ContainsKey(key)) {
                this.CreateTexture(renderer, key);
            }

            var texture = CachedTextures[key];

            var area = this.GetAbsoluteArea(windowWidth, windowHeight);

            if (SDL.SDL_SetTextureAlphaMod(texture, this.Alpha) < 0) {
                throw new NotSupportedException("The renderer does not support alpha modulation.");
            }

            var center = new SDL.SDL_Point {
                x = area.w / 2,
                y = area.h / 2
            };
            SDL.SDL_RenderCopyEx(renderer, texture, IntPtr.Zero, ref area, this.Angle,
                ref center, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
        }
    }

    struct TextureCaracteristics {
        public string ImagePath;

        public TextureCaracteristics(string path) {
            this.ImagePath = path;
        }
    }
}