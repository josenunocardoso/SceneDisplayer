using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities.Utils
{
    public class Button : RectangularEntity {

        public Button(RectF area, SDL.SDL_Color backgroundColor, TextAlignment alignment,
        string text, string font, int fontSize, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.BackgroundColor = backgroundColor;
            this.Alignment = alignment;
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
        }


        public SDL.SDL_Color BackgroundColor { get; set; }

        public TextAlignment Alignment { get; }

        public string Text { get; private set; }

        public string Font { get; private set; }

        public int FontSize { get; private set; }


        public override void Init() {
            this.AddChild("Rectangle", new FillRectangle(this.Area, this.BackgroundColor, this.RelativeToScreenSize));
            this.AddChild("Text", new TextEntity(this.Text, this.Font, this.FontSize, new PointF(), false));
            
            this.PropertyChanged += (_, e) => {
                if (e.PropertyName == "Area") {
                    (this.Children["Rectangle"] as FillRectangle).Area = this.Area;
                }
            };
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            var area = this.GetAbsoluteArea(screenWidth, screenHeight);

            var textEntity = this.Children["Text"] as TextEntity;
            textEntity.Location = TextEntity.GetTextAbsoluteLocation(this.Alignment, area);
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