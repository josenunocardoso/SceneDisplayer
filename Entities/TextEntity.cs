using System.Collections.Generic;
using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities {
    public class TextEntity : Entity {
        
        static TextEntity() {
            CachedBitmapFonts = new Dictionary<FontCharacteristics, (IntPtr, int, int)>();
        }

        public TextEntity(string text, string font, int fontSize, PointF location, bool relativeToScreenSize = true)
        : base(relativeToScreenSize) {
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
            this.Location = location;
        }


        private static Dictionary<FontCharacteristics, (IntPtr, int, int)> CachedBitmapFonts { get; set; }

        public string Text { get; }

        public string Font { get; }

        public int FontSize { get; }

        public PointF Location { get; }


        private void CreateBitmapFont(IntPtr renderer, FontCharacteristics key) {
            var bitmapFontTexture = ResourcesManager.GetBitmapTexture(renderer, this.Text, this.FontSize, this.Font,
                new SDL.SDL_Color { r = 0x00, g = 0x00, b = 0x00 });
            
            CachedBitmapFonts.Add(key, bitmapFontTexture);
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            var key = new FontCharacteristics(this.Text, this.Font, this.FontSize);

            if (!CachedBitmapFonts.ContainsKey(key)) {
                this.CreateBitmapFont(renderer, key);
            }

            var (bitmapFont, w, h) = CachedBitmapFonts[key];

            var loc = this.GetAbsolutePoint(this.Location, screenWidth, screenHeight);

            var area = new SDL.SDL_Rect {
                x = loc.x,
                y = loc.y,
                w = w,
                h = h
            };

            SDL.SDL_RenderCopy(renderer, bitmapFont, IntPtr.Zero, ref area);
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
    }
}