using System.Collections.Generic;
using System;
using SDL2;
using SceneDisplayer.Entities.Utils;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    public class TextEntity : Entity {
        
        static TextEntity() {
            CachedBitmapFontsCount = new Dictionary<FontCharacteristics, int>();
            CachedBitmapFonts = new Dictionary<FontCharacteristics, (IntPtr, int, int)>();
        }

        public TextEntity(string text, string font, int fontSize, PointF location, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
            this.Location = location;
            this.CachedKeys = new Queue<FontCharacteristics>();
        }


        private static Dictionary<FontCharacteristics, int> CachedBitmapFontsCount { get; set; }

        private static Dictionary<FontCharacteristics, (IntPtr, int, int)> CachedBitmapFonts { get; set; }

        public string Text { get; set; }

        public string Font { get; set; }

        public int FontSize { get; set; }

        public PointF Location { get; set; }

        private Queue<FontCharacteristics> CachedKeys { get; set; }


        private static void CreateBitmapFont(IntPtr renderer, FontCharacteristics key) {
            var bitmapFontTexture = ResourcesManager.GetBitmapTexture(renderer,
                key.Text, key.FontSize, key.FontPath, new SDL.SDL_Color { r = 0x00, g = 0x00, b = 0x00 });
            
            CachedBitmapFonts.Add(key, bitmapFontTexture);
            if (!CachedBitmapFontsCount.ContainsKey(key)) {
                CachedBitmapFontsCount.Add(key, 1);
            }
            else {
                CachedBitmapFontsCount[key]++;
            }
        }

        public static (int, int) GetTextSize(string text, string font, int fontSize) {
            var key = new FontCharacteristics(text, font, fontSize);

            if (!CachedBitmapFonts.ContainsKey(key)) {
                throw new ArgumentException("No such cached BitmapFont");
            }

            return (CachedBitmapFonts[key].Item2, CachedBitmapFonts[key].Item3);
        }

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

    public struct FontCharacteristics {
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