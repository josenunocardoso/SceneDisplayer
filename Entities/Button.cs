using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities
{
    public class Button : RectangularEntity {
        private (int, int)? textSize;

        public Button(RectF area, SDL.SDL_Color backgroundColor, TextAlignment alignment,
        string text, string font, int fontSize, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.BackgroundColor = backgroundColor;
            this.Alignment = alignment;
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
            this.textSize = null;
        }


        public SDL.SDL_Color BackgroundColor { get; set; }

        public TextAlignment Alignment { get; }

        public string Text { get; private set; }

        public string Font { get; private set; }

        public int FontSize { get; private set; }


        public override void Init() {
            this.AddChild("Rectangle", new FillRectangle(this.Area, this.BackgroundColor, this.RelativeToScreenSize));
            this.AddChild("Text", new TextEntity(this.Text, this.Font, this.FontSize,
                new PointF { x = this.Area.x + this.Area.w / 2, y = this.Area.y + this.Area.h / 2 }, false));
            
            this.PropertyChanged += (_, e) => {
                if (e.PropertyName == "Area") {
                    (this.Children["Rectangle"] as FillRectangle).Area = this.Area;
                }
            };
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            var area = this.GetAbsoluteArea(screenWidth, screenHeight);

            if (this.textSize == null) {
                this.textSize = TextEntity.GetTextSize(renderer, this.Text, this.Font, this.FontSize);
            }

            var (w, h) = this.textSize.Value;

            switch (this.Alignment) {
                case TextAlignment.Center: {
                    var textEntity = this.Children["Text"] as TextEntity;

                    textEntity.Location = new PointF {
                        x = area.x + area.w / 2 - w / 2,
                        y = area.y + area.h / 2 - h / 2
                    };

                    break;
                }
            }
        }
    
        public void UpdateText(string text, string font, int fontSize) {
            var textEntity = this.Children["Text"] as TextEntity;
            textEntity.Text = text;
            textEntity.Font = font;
            textEntity.FontSize = fontSize;

            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
        }
    }

    public enum TextAlignment {
        Center
    }
}