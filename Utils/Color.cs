using SDL2;

namespace SceneDisplayer.Utils {
    /// <summary>
    /// Represents a RGBA color.
    /// </summary>
    public struct Color {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Color(byte r, byte g, byte b, byte a = 0xFF) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }


        public SDL.SDL_Color ToSDLColor() {
            return new SDL.SDL_Color {
                r = this.r, g = this.g, b = this.b, a = this.a
            };
        }


        public override string ToString() {
            return $"({this.r};{this.g};{this.b};{this.a})";
        }

        #region Static Colors
        public static readonly Color Cyan = new Color(0x00, 0xFF, 0xFF);
        public static readonly Color Black = new Color(0x00, 0x00, 0x00);
        public static readonly Color Blue = new Color(0x00, 0x00, 0xFF);
        public static readonly Color Magenta = new Color(0xFF, 0x00, 0xFF);
        public static readonly Color Grey = new Color(0x80, 0x80, 0x80);
        public static readonly Color Green = new Color(0x00, 0x80, 0x00);
        public static readonly Color Lime = new Color(0x00, 0xFF, 0x00);
        public static readonly Color Maroon = new Color(0x80, 0x00, 0x00);
        public static readonly Color NavyBlue = new Color(0x00, 0x00, 0x80);
        public static readonly Color Olive = new Color(0x80, 0x80, 0x00);
        public static readonly Color Purple = new Color(0x80, 0x00, 0x80);
        public static readonly Color Red = new Color(0xFF, 0x00, 0x00);
        public static readonly Color Silver = new Color(0xC0, 0xC0, 0xC0);
        public static readonly Color Teal = new Color(0x00, 0x80, 0x80);
        public static readonly Color White = new Color(0xFF, 0xFF, 0xFF);
        public static readonly Color Yellow = new Color(0xFF, 0xFF, 0x00);
        public static readonly Color Orange = new Color(0xFF, 0xA5, 0x00);
        #endregion
    }
}