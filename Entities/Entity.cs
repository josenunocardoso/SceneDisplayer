using System;
using System.ComponentModel;
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
            this.Traits = new Traits(true);
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
        private Dictionary<object, Entity> Children { get; }

        /// <summary>
        /// This Entity Traits.
        /// </summary>
        public Traits Traits { get; set; }


        /// <summary>
        /// Adds an <see cref="Entity"/> as a new child, and initializes it.
        /// </summary>
        /// <param name="key"><see cref="Key"/> of the child.</param>
        /// <param name="child"><see cref="Entity"/> to be added.</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the <c>child</c> is null.</exception>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the <c>key</c> already exists.</exception>
        public void AddChild(object key, Entity child) {
            if (child == null) {
                throw new ArgumentNullException(nameof(child));
            }

            if (this.Children.ContainsKey(key)) {
                throw new ArgumentException("Key already exists");
            }

            child.Key = key;
            this.Children.Add(key, child);
            child.Init();
        }

        /// <summary>
        /// Returns a child of this <see cref="Entity"/>.
        /// </summary>
        /// <param name="key"><see cref="Key"/> of the child.</param>
        /// <returns>The child.</returns>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the child does not exist.</exception>
        public Entity GetChild(object key) {
            if (!this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child does not exist");
            }

            return this.Children[key];
        }

        /// <summary>
        /// Returns a child of this <see cref="Entity"/>.
        /// </summary>
        /// <param name="key"><see cref="Key"/> of the child.</param>
        /// <typeparam name="T">Type of the child.</typeparam>
        /// <returns>The child.</returns>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the child does not exist,
        /// or if the child is not of the given type.</exception>
        public T GetChild<T>(object key) where T : Entity {
            if (!this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child does not exist");
            }

            if (!(this.Children[key] is T)) {
                throw new ArgumentException("Child is not of the given type");
            }

            return (T)this.Children[key];
        }

        /// <summary>
        /// Returns all this <see cref="Entity"/> children.
        /// </summary>
        /// <returns>The children.</returns>
        public Dictionary<object, Entity>.ValueCollection GetChildren() {
            return this.Children.Values;
        }

        /// <summary>
        /// Replaces an existing <see cref="Entity"/>, initializes it and disposes the original <see cref="Entity"/>.
        /// </summary>
        /// <param name="key"><see cref="Key"/> of the child.</param>
        /// <param name="newChild">The new <see cref="Entity"/> to be added.</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the <c>newChild</c> is null.</exception>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the <c>newChild</c> does not exist.</exception>
        public void EditChild(object key, Entity newChild) {
            if (newChild == null) {
                throw new ArgumentNullException(nameof(newChild));
            }

            if (!this.HasChild(key)) {
                throw new ArgumentException("Child does not exist");
            }

            this.Children[key].Dispose();

            newChild.Key = key;
            this.Children[key] = newChild;
            newChild.Init();
        }

        /// <summary>
        /// Returns whether this <see cref="Entity"/> has a child or not.
        /// </summary>
        /// <param name="key">The <c>key</c> of the child.</param>
        /// <returns>True if this <see cref="Entity"/> has the given child, False otherwise.</returns>
        public bool HasChild(object key) {
            return this.Children.ContainsKey(key);
        }

        /// <summary>
        /// Removes a <c>child</c> and disposes it.
        /// </summary>
        /// <param name="key">The <c>key</c> of the child to be removed.</param>
        /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the <c>child</c> does not exist.</exception>
        public void RemoveChild(object key) {
            if (!this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child does not exist");
            }

            this.Children[key].Dispose();

            this.Children.Remove(key);
        }

        /// <summary>
        /// Performs an action on all the children.
        /// This action is performed recursively (e.g. on the children of the children).
        /// </summary>
        /// <param name="ac">The action to perform.</param>
        public void PerformActionOnChildrenRecursively(Action<Entity> ac) {
            this.PerformActionOnChildrenRecursively(ac, _ => true);
        }

        /// <summary>
        /// Performs an action on all the children on a given predicate.
        /// This action is performed recursively (e.g. on the children of the children).
        /// </summary>
        /// <param name="ac">The action to perform.</param>
        /// <param name="pred">The predicate to test.</param>
        public void PerformActionOnChildrenRecursively(Action<Entity> ac, Predicate<Entity> pred) {
            foreach (var child in this.Children.Values) {
                if (pred(child)) {
                    ac(child);
                }

                foreach (var subchild in child.Children.Values) {
                    subchild.PerformActionOnChildrenRecursively(ac, pred);
                }
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
        /// Should be called to update the state of the <c>Entity</c>.
        /// Updates the <c>Entity</c>. It is called by the <see cref="SceneManager.Render"/> method.
        /// The method is called once, for every frame rendered, before the <see cref="Draw"/> method.
        /// </summary>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <param name="deltaTime">Time elapsed since the last draw call, in milliseconds.</param>
        public virtual void Update(int windowWidth, int windowHeight, uint deltaTime) {
            foreach (var child in this.Children.Values) {
                child.Update(windowWidth, windowHeight, deltaTime);
            }
        }

        /// <summary>
        /// Should be called to do rendering logic.
        /// Draws the <c>Entity</c>. It is called by the <see cref="SceneManager.Render"/> method.
        /// The method is called once, for every frame rendered, after the <see cref="Update"/> method.
        /// </summary>
        /// <param name="renderer">Pointer to the renderer used.</param>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <param name="deltaTime">Time elapsed since the last draw call, in milliseconds.</param>
        public virtual void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            if (!this.Traits.Visible) {
                return;
            }

            foreach (var child in this.Children.Values) {
                child.Draw(renderer, windowWidth, windowHeight, deltaTime);
            }
        }

        /// <summary>
        /// Converts a point to an absolute point, in pixels.
        /// If <see cref="RelativeToScreenSize"/> is set to false, it returns an equivalent point.
        /// </summary>
        /// <param name="point">Point to convert</param>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <returns>Absolute point.</returns>
        protected SDL.SDL_Point GetAbsolutePoint(PointF point, int windowWidth, int windowHeight) {
            return this.RelativeToScreenSize
                ? new SDL.SDL_Point {
                    x = (int)(point.x * windowWidth),
                    y = (int)(point.y * windowHeight)
                }
                : new SDL.SDL_Point {
                    x = (int)point.x, y = (int)point.y
                };
        }

        /// <summary>
        /// Converts a point to a point, relative to the screen.
        /// </summary>
        /// <param name="point">Point to convert</param>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <returns>Relative point.</returns>
        protected PointF AbsoluteToRelative(SDL.SDL_Point point, int windowWidth, int windowHeight) {
            return new PointF {
                x = (float)point.x / windowWidth,
                y = (float)point.y / windowHeight
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
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <returns>Absolute area.</returns>
        protected SDL.SDL_Rect GetAbsoluteArea(int windowWidth, int windowHeight) {
            return this.RelativeToScreenSize
                ? new SDL.SDL_Rect {
                    x = (int)((this.Area.x - this.Area.w / 2) * windowWidth),
                    y = (int)((this.Area.y - this.Area.h / 2) * windowHeight),
                    w = (int)(this.Area.w * windowWidth),
                    h = (int)(this.Area.h * windowHeight)
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
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <returns>Relative area.</returns>
        protected RectF GetRelativeArea(int windowWidth, int windowHeight) {
            return this.RelativeToScreenSize
                ? new RectF {
                    x = this.Area.x - this.Area.w / 2,
                    y = this.Area.y - this.Area.h / 2,
                    w = this.Area.w,
                    h = this.Area.h
                }
                : new RectF {
                    x = this.Area.x / windowWidth,
                    y = this.Area.y / windowHeight,
                    w = this.Area.w / windowWidth,
                    h = this.Area.h / windowHeight
                };
        }

        public bool Contains(SDL.SDL_Point point, int windowWidth, int windowHeight) {
            var relPt = this.AbsoluteToRelative(point, windowWidth, windowHeight);
            var relArea = this.GetRelativeArea(windowWidth, windowHeight);

            return relArea.Contains(relPt);
        }
    }

    public sealed class Traits {

        public Traits(bool visible) {
            this.Visible = visible;
        }


        /// <summary>
        /// Whether this Entity and its children will be drawn.
        /// </summary>
        /// <value>True to display this Entity and its children, False otherwise.</value>
        public bool Visible { get; set; }
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

        /// <summary>
        /// Mouse Button pressed.
        /// </summary>
        public MouseButton Button { get; set; }
    }

    public class WindowArgs : EventArgs {
        /// <summary>
        /// Window width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Window height.
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