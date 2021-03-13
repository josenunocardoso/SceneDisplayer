using System;
using SDL2;
using SceneDisplayer.Utils;

namespace SceneDisplayer.Entities.Utils {
    /// <summary>
    /// An <see cref="Entity"/> that renders a button.
    /// </summary>
    public class Button : RectangularEntity {
        private Color _backgroundColor;

        /// <summary>
        /// Constructs a <c>Button</c>.
        /// </summary>
        /// <param name="area">The area of the <c>Button</c>.</param>
        /// <param name="backgroundColor">The background color of the <c>Button</c>.</param>
        /// <param name="alignment">The text alignment of the <c>Button</c>.</param>
        /// <param name="text">The text of the <c>Button</c>.</param>
        /// <param name="font">The font path of the <c>Button</c>.</param>
        /// <param name="fontSize">The font size of the <c>Button</c>.</param>
        /// <param name="textColor">The text color of the <c>Button</c>.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        public Button(RectF area, Color backgroundColor, TextAlignment alignment,
        string text, string font, int fontSize, Color textColor, bool relativeToScreenSize = true)
        : base(area, relativeToScreenSize) {
            this.BackgroundColor = backgroundColor;
            this.Alignment = alignment;
            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
            this.TextColor = textColor;
        }


        /// <summary>
        /// The background color of the <c>Button</c>.
        /// </summary>
        public Color BackgroundColor {
            get {
                return this._backgroundColor;
            }
            set {
                this._backgroundColor = value;
                this.OnPropertyChanged(nameof(this.BackgroundColor));
            }
        }

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

        /// <summary>
        /// The text color of the <c>Button</c>.
        /// </summary>
        public Color TextColor { get; private set; }


        public override void Init() {
            this.AddChild("Rectangle", new FillRectangle(this.Area, this.BackgroundColor, this.RelativeToScreenSize));
            this.AddChild("Border", new Rectangle(this.Area, new Color(), this.RelativeToScreenSize));
            this.AddChild("Text", new TextEntity(this.Text, this.Font, this.FontSize, this.TextColor, new PointF(), false));
            
            this.PropertyChanged += (_, e) => {
                if (e.PropertyName == nameof(this.Area)) {
                    this.GetChild<FillRectangle>("Rectangle").Area = this.Area;
                    this.GetChild<Rectangle>("Border").Area = this.Area;
                }
                if (e.PropertyName == nameof(this.BackgroundColor)) {
                    this.GetChild<FillRectangle>("Rectangle").Color = this.BackgroundColor;
                }
            };
        }

        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);

            if (!this.Traits.Visible) {
                return;
            }

            var area = this.GetAbsoluteArea(windowWidth, windowHeight);

            var textEntity = this.GetChild<TextEntity>("Text");
            textEntity.Location = TextEntity.GetTextAbsoluteLocation(this.Alignment, area);
        }

        /// <summary>
        /// Updates the text of this <c>Button</c>.
        /// </summary>
        /// <param name="text">The new text.</param>
        public void UpdateText(string text) {
            this.UpdateText(text, this.Font, this.FontSize, this.TextColor);
        }
    
        /// <summary>
        /// Updates the text of this <c>Button</c>.
        /// </summary>
        /// <param name="text">The new text.</param>
        /// <param name="font">The new font path.</param>
        /// <param name="fontSize">The new font size.</param>
        /// <param name="textColor">The new text color.</param>
        public void UpdateText(string text, string font, int fontSize, Color textColor) {
            var textEntity = this.GetChild<TextEntity>("Text");
            textEntity.Text = text;
            textEntity.Font = font;
            textEntity.FontSize = fontSize;
            textEntity.TextColor = textColor;

            this.Text = text;
            this.Font = font;
            this.FontSize = fontSize;
            this.TextColor = textColor;
        }
    }
}