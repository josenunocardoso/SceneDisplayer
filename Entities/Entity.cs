using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    public abstract class Entity : IDisposable {

        protected Entity(bool relativeToScreenSize = true) {
            this.RelativeToScreenSize = relativeToScreenSize;
            this.Children = new Dictionary<object, Entity>();
        }


        public object Key { get; private set; }

        protected bool RelativeToScreenSize { get; }

        public Dictionary<object, Entity> Children { get; }


        public void AddChild(object key, Entity child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }

            if (this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child already exists");
            }

            child.Key = key;
            this.Children.Add(key, child);
            child.Init();
        }

        public bool RemoveChild(object key) {
            if (!this.Children.ContainsKey(key)) {
                throw new ArgumentException("Child does not exist");
            }

            this.Children[key].Dispose();

            return this.Children.Remove(key);
        }

        public void RemoveChildren(Predicate<Entity> pred) {
            foreach (var child in this.Children.Values.Where(c => pred(c))) {
                child.Dispose();
                this.RemoveChild(child);
            }
        }

        public void ClearChildren() {
            foreach (var child in this.Children.Values) {
                child.Dispose();
            }

            this.Children.Clear();
        }

        public virtual void Init() { }

        public virtual void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            foreach (var child in this.Children.Values) {
                child.Draw(renderer, screenWidth, screenHeight);
            }
        }

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

        protected PointF GetRelativePoint(SDL.SDL_Point point, int screenWidth, int screenHeight) {
            return new PointF {
                x = (float)point.x / screenWidth,
                y = (float)point.y / screenHeight
            };
        }


        public event EventHandler<WindowArgs> WindowResize;

        public void OnWindowResize(WindowArgs args) {
            this.WindowResize?.Invoke(this, args);
        }


        public virtual void Dispose() { }
    }

    public abstract class RectangularEntity : Entity, IClickable {
        private RectF area;

        protected RectangularEntity(RectF area, bool relativeToScreenSize) : base(relativeToScreenSize) {
            this.Area = area;
        }


        public RectF Area {
            get {
                return this.area;
            } set {
                this.area = value;
                this.OnPropertyChanged("Area");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
        }

        private void OnPropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        public bool Contains(SDL.SDL_Point point) {
            return this.Area.Contains(point);
        }
    }

    public class ClickArgs : EventArgs {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public class WindowArgs : EventArgs {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}