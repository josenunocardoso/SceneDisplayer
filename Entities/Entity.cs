using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// A component that is displayed on a <see cref="Scene"/> It may contain other <c>Entities</c> as children.
    /// </summary>
    public abstract class Entity : IDisposable {

        /// <summary>
        /// Constructs an <c>Entity</c>.
        /// </summary>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        protected Entity(bool relativeToScreenSize = true) {
            this.RelativeToScreenSize = relativeToScreenSize;
            this.Children = new Dictionary<object, Entity>();
        }


        /// <summary>
        /// The <c>Key</c> that was given with <see cref="AddChild"/>.
        /// </summary>
        /// <value>The <c>Key</c>, or null if <see cref="AddChild"/> was not used.</value>
        public object Key { get; private set; }

        /// <summary>
        /// Flag indicating if the positions in this Entity are assumed to be relative to the screen, or not.
        /// </summary>
        /// <value>True if it is relative to the screen, False otherwise.</value>
        protected bool RelativeToScreenSize { get; }

        /// <summary>
        /// The children of this Entity.
        /// </summary>
        /// <value>A <see cref="Dictionary{TKey, TValue}"/>, where <c>TKey</c> is the <see cref="Key"/> of the child, and <c>TValue</c> is the child itself.</value>
        public Dictionary<object, Entity> Children { get; }


        /// <summary>
        /// Adds an <see cref="Entity"/> as a new child, and initializes it.
        /// </summary>
        /// <param name="key"><see cref="Key"/> of the child.</param>
        /// <param name="child"><see cref="Entity"/> to be added.</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the <c>child</c> is null.</exception>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the <c>child</c> already exists.</exception>
        public void AddChild(object key, Entity child) {
            if (child == null) {
                throw new ArgumentNullException(nameof(child));
            }

            if (this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child already exists");
            }

            child.Key = key;
            this.Children.Add(key, child);
            child.Init();
        }

        /// <summary>
        /// Removes a <c>child</c> and disposes it.
        /// </summary>
        /// <param name="key">The <c>key</c> of the child to be removed.</param>
        /// <returns>The <see cref="Entity"/> removed.</returns>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the <c>child</c> does not exist.</exception>
        public bool RemoveChild(object key) {
            if (!this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child does not exist");
            }

            this.Children[key].Dispose();

            return this.Children.Remove(key);
        }

        /// <summary>
        /// Removes children on a given predicate.
        /// </summary>
        /// <param name="pred">The predicate to test.</param>
        public void RemoveChildren(Predicate<Entity> pred) {
            foreach (var child in this.Children.Values.Where(c => pred(c))) {
                child.Dispose();
                this.RemoveChild(child);
            }
        }

        /// <summary>
        /// Clears all the children, and disposes them.
        /// </summary>
        public void ClearChildren() {
            foreach (var child in this.Children.Values) {
                child.Dispose();
            }

            this.Children.Clear();
        }

        /// <summary>
        /// Initializes the <c>Entity</c>. It is called when an <c>Entity</c> is added.
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// Draws the <c>Entity</c>. It is called by the <see cref="SceneManager.Render"/> method.
        /// The method is called once, for every frame rendered.
        /// </summary>
        /// <param name="renderer">Pointer to the renderer used.</param>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        public virtual void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            foreach (var child in this.Children.Values) {
                child.Draw(renderer, screenWidth, screenHeight);
            }
        }

        /// <summary>
        /// Converts a point to an absolute point, in pixels.
        /// If <see cref="RelativeToScreenSize"/> is set to false, it returns an equivalent point.
        /// </summary>
        /// <param name="point">Point to convert</param>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        /// <returns>Absolute point.</returns>
        protected SDL.SDL_Point GetAbsolutePoint(PointF point, int screenWidth, int screenHeight) {
            return this.RelativeToScreenSize
                ? new SDL.SDL_Point {
                    x = (int)(point.x * screenWidth),
                    y = (int)(point.y * screenHeight)
                }
                : new SDL.SDL_Point {
                    x = (int)point.x, y = (int)point.y
                };
        }

        /// <summary>
        /// Converts a point to a point, relative to the screen.
        /// </summary>
        /// <param name="point">Point to convert</param>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        /// <returns>Relative point.</returns>
        protected PointF AbsoluteToRelative(SDL.SDL_Point point, int screenWidth, int screenHeight) {
            return new PointF {
                x = (float)point.x / screenWidth,
                y = (float)point.y / screenHeight
            };
        }


        public event EventHandler<ClickArgs> MouseDown;

        public event EventHandler<ClickArgs> MouseUp;

        public event EventHandler<KeyArgs> KeyDown;

        public event EventHandler<WindowArgs> WindowResize;


        public void OnMouseDown(ClickArgs args) {
            this.MouseDown?.Invoke(this, args);
        }

        public void OnMouseUp(ClickArgs args) {
            this.MouseUp?.Invoke(this, args);
        }

        public void OnKeyDown(KeyArgs args) {
            this.KeyDown?.Invoke(this, args);
        }

        public void OnWindowResize(WindowArgs args) {
            this.WindowResize?.Invoke(this, args);
        }


        /// <summary>
        /// Disposes the <c>Entity</c>.
        /// </summary>
        public virtual void Dispose() {
            foreach (var child in this.Children.Values) {
                child.Dispose();
            }
        }
    }

    /// <summary>
    /// An <see cref="Entity"/> that has a rectangular shape.
    /// </summary>
    public abstract class RectangularEntity : Entity, IClickable {
        private RectF area;

        /// <summary>
        /// Constructs a <c>RectangularEntity</c>.
        /// </summary>
        /// <param name="area">The area of the rectangular shape.</param>
        /// <param name="relativeToScreenSize">True, to consider positions relative to the screen size.
        /// False, to consider absolute positions, in pixels.</param>
        protected RectangularEntity(RectF area, bool relativeToScreenSize) : base(relativeToScreenSize) {
            this.Area = area;
        }


        /// <summary>
        /// The area of the rectangular shape.
        /// </summary>
        public RectF Area {
            get {
                return this.area;
            } set {
                this.area = value;
                this.OnPropertyChanged(nameof(this.Area));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
        }

        /// <summary>
        /// Should be called to trigger the <see cref="PropertyChanged" event./>
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// Returns the <see cref="Area"/>, with absolute values, in pixels.
        /// If <see cref="Entity.RelativeToScreenSize"/> is set to false, it returns an equivalent area.
        /// </summary>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        /// <returns>Absolute area.</returns>
        protected SDL.SDL_Rect GetAbsoluteArea(int screenWidth, int screenHeight) {
            return this.RelativeToScreenSize
                ? new SDL.SDL_Rect {
                    x = (int)((this.Area.x - this.Area.w / 2) * screenWidth),
                    y = (int)((this.Area.y - this.Area.h / 2) * screenHeight),
                    w = (int)(this.Area.w * screenWidth),
                    h = (int)(this.Area.h * screenHeight)
                }
                : new SDL.SDL_Rect {
                    x = (int)this.Area.x, y = (int)this.Area.y,
                    w = (int)this.Area.w, h = (int)this.Area.h
                };
        }

        /// <summary>
        /// Returns the <see cref="Area"/>, with relative values.
        /// If <see cref="Entity.RelativeToScreenSize"/> is set to true, it returns an equivalent area.
        /// </summary>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        /// <returns>Relative area.</returns>
        protected RectF GetRelativeArea(int screenWidth, int screenHeight) {
            return this.RelativeToScreenSize
                ? new RectF {
                    x = this.Area.x - this.Area.w / 2,
                    y = this.Area.y - this.Area.h / 2,
                    w = this.Area.w,
                    h = this.Area.h
                }
                : new RectF {
                    x = this.Area.x / screenWidth,
                    y = this.Area.y / screenHeight,
                    w = this.Area.w / screenWidth,
                    h = this.Area.h / screenHeight
                };
        }

        public bool Contains(SDL.SDL_Point point, int screenWidth, int screenHeight) {
            var relPt = this.AbsoluteToRelative(point, screenWidth, screenHeight);
            var relArea = this.GetRelativeArea(screenWidth, screenHeight);

            return relArea.Contains(relPt);
        }
    }

    public class ClickArgs : EventArgs {
        /// <summary>
        /// Mouse position X.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Mouse position Y.
        /// </summary>
        public int Y { get; set; }
    }

    public class WindowArgs : EventArgs {
        /// <summary>
        /// Screen width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Screen height.
        /// </summary>
        public int Height { get; set; }
    }

    public class KeyArgs : EventArgs {
        /// <summary>
        /// Key pressed.
        /// </summary>
        public SDL.SDL_Keycode Key { get; set; }
    }
}