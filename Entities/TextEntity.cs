using System.Collections.Generic;
using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="Entity"/> that renders text.
    /// </summary>
    public class TextEntity : Entity {
        
        static TextEntity() {
            CachedBitmapFontsCount = new Dictionary<FontCharacteristics, int>();
            CachedBitmapFonts = new Dictionary<FontCharacteristics, (IntPtr, int, int)>();
        }

        /// <summary>
        /// Constructs a TextEntity.
        /// </summary>
        /// <param name="text">The text to be rendered.</param>
        /// <param name="font">The font path (e.g. a ttf file)</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="location">The location of this <c>Entity</c></param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public TextEntity(string text, string font, int fontSize, PointF location, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
            this.Location = location;
            this.CachedKeys = new Queue<FontCharacteristics>();
        }


        /// <summary>
        /// A <see cref="Dictionary{TKey, TValue}"/> that counts the number of <c>TextEntities</c> that are using a cached <c>BitmapFont</c>.
        /// </summary>
        private static Dictionary<FontCharacteristics, int> CachedBitmapFontsCount { get; set; }

        /// <summary>
        /// A <see cref="Dictionary{TKey, TValue}"/> that contains the <c>BitmapFonts</c> that are cached.
        /// </summary>
        private static Dictionary<FontCharacteristics, (IntPtr, int, int)> CachedBitmapFonts { get; set; }

        /// <summary>
        /// The text to be rendered.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The font path.
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// The font size.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// The location of the <c>Entity</c>.
        /// </summary>
        public PointF Location { get; set; }

        private Queue<FontCharacteristics> CachedKeys { get; set; }


        private static void CreateBitmapFont(IntPtr renderer, FontCharacteristics key) {
            var bitmapFontTexture = ResourcesManager.GetTextBitmap(renderer,
                key.Text, key.FontPath, key.FontSize, new SDL.SDL_Color { r = 0x00, g = 0x00, b = 0x00 });
            
            CachedBitmapFonts.Add(key, bitmapFontTexture);
            if (!CachedBitmapFontsCount.ContainsKey(key)) {
                CachedBitmapFontsCount.Add(key, 1);
            }
            else {
                CachedBitmapFontsCount[key]++;
            }
        }

        /// <summary>
        /// Retrieves the size a given TextEntity template would occupy.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="font">Font path.</param>
        /// <param name="fontSize">Font size.</param>
        /// <returns>A tuple with the width and the height of the template, respectively.</returns>
        public static (int, int) GetTextSize(string text, string font, int fontSize) {
            var key = new FontCharacteristics(text, font, fontSize);

            if (!CachedBitmapFonts.ContainsKey(key)) {
                throw new ArgumentException("No such cached BitmapFont");
            }

            return (CachedBitmapFonts[key].Item2, CachedBitmapFonts[key].Item3);
        }

        /// <summary>
        /// Returns the absolute location of this <c>TextEntity</c>, according to the <c>TextAlignment</c> used.
        /// </summary>
        /// <param name="alignment">The text alignment.</param>
        /// <param name="area">The area where the <c>TextEntity</c> would be placed.</param>
        /// <returns>The absolute location.</returns>
        public static PointF GetTextAbsoluteLocation(TextAlignment alignment, SDL.SDL_Rect area) {
            switch (alignment) {
                case TextAlignment.Center: {
                    return new PointF {
                        x = area.x + area.w / 2,
                        y = area.y + area.h / 2
                    };
                }
            }

            throw new NotSupportedException("The given TextAlignment is not supported");
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            if (this.Font == null) return;
            
            var key = new FontCharacteristics(this.Text, this.Font, this.FontSize);

            if (!CachedBitmapFonts.ContainsKey(key)) {
                CreateBitmapFont(renderer, key);
                this.CachedKeys.Enqueue(key);
            }

            if (!this.CachedKeys.Contains(key)) {
                this.CachedKeys.Enqueue(key);
                CachedBitmapFontsCount[key]++;
            }

            var (bitmapFont, w, h) = CachedBitmapFonts[key];

            var loc = this.GetAbsolutePoint(this.Location, screenWidth, screenHeight);

            var area = new SDL.SDL_Rect {
                x = loc.x - w / 2,
                y = loc.y - h / 2,
                w = w,
                h = h
            };

            SDL.SDL_RenderCopy(renderer, bitmapFont, IntPtr.Zero, ref area);
        }

        public override void Dispose() {
            base.Dispose();

            foreach (var key in this.CachedKeys) {
                if (--CachedBitmapFontsCount[key] <= 0) {
                    CachedBitmapFontsCount.Remove(key);
                    CachedBitmapFonts.Remove(key);
                }
            }
        }
    }

    /// <summary>
    /// Represents the <c>TextAlignment</c> of a given text.
    /// </summary>
    public enum TextAlignment {
        Center
    }

    struct FontCharacteristics {
        public string Text;
        public string FontPath;
        public int FontSize;

        public FontCharacteristics(string text, string font, int size) {
            this.Text = text;
            this.FontPath = font;
            this.FontSize = size;
        }


        public override string ToString() {
            return $"{this.Text}; {this.FontPath}; {this.FontSize}";
        }
    }
}