using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities.Utils
{
    /// <summary>
    /// An <see cref="Entity"/> that renders a button.
    /// </summary>
    public class Button : RectangularEntity {

        /// <summary>
        /// Constructs a <c>Button</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Button</c>.</param>
        /// <param name="backgroundColor">The background color of the <c>Button</c>.</param>
        /// <param name="alignment">The text alignment of the <c>Button</c>.</param>
        /// <param name="text">The text of the <c>Button</c>.</param>
        /// <param name="font">The font path of the <c>Button</c>.</param>
        /// <param name="fontSize">The font size of the <c>Button</c>.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Button(RectF area, SDL.SDL_Color backgroundColor, TextAlignment alignment,
        string text, string font, int fontSize, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.BackgroundColor = backgroundColor;
            this.Alignment = alignment;
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
        }


        /// <summary>
        /// The background color of the <c>Button</c>.
        /// </summary>
        public SDL.SDL_Color BackgroundColor { get; set; }

        /// <summary>
        /// The text alignment of the <c>Button</c>.
        /// </summary>
        public TextAlignment Alignment { get; }

        /// <summary>
        /// The text of the <c>Button</c>.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The font path of the <c>Button</c>.
        /// </summary>
        public string Font { get; private set; }

        /// <summary>
        /// The font size of the <c>Button</c>.
        /// </summary>
        public int FontSize { get; private set; }


        public override void Init() {
            this.AddChild("Rectangle", new FillRectangle(this.Area, this.BackgroundColor, this.RelativeToScreenSize));
            this.AddChild("Border", new Rectangle(this.Area, new SDL.SDL_Color(), this.RelativeToScreenSize));
            this.AddChild("Text", new TextEntity(this.Text, this.Font, this.FontSize, new PointF(), false));
            
            this.PropertyChanged += (_, e) => {
                if (e.PropertyName == "Area") {
                    (this.Children["Rectangle"] as FillRectangle).Area = this.Area;
                    (this.Children["Border"] as Rectangle).Area = this.Area;
                }
            };
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            base.Draw(renderer, screenWidth, screenHeight);

            var area = this.GetAbsoluteArea(screenWidth, screenHeight);

            var textEntity = this.Children["Text"] as TextEntity;
            textEntity.Location = TextEntity.GetTextAbsoluteLocation(this.Alignment, area);
        }
    
        /// <summary>
        /// Updates the text of this <c>Button</c>.
        /// </summary>
        /// <param name="text">The new text.</param>
        /// <param name="font">The new font path.</param>
        /// <param name="fontSize">The new font size.</param>
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
}