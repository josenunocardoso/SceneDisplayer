using System.Collections.Generic;
using System;
using SceneDisplayer.Utils;
using SDL2;

namespace SceneDisplayer.Entities {
    public abstract class Entity : IDisposable {

        protected Entity(bool relativeToScreenSize = true) {
            this.RelativeToScreenSize = relativeToScreenSize;
            this.Children = new List<Entity>();
        }


        protected bool RelativeToScreenSize { get; }

        public List<Entity> Children { get; }


        public void AddChild(Entity child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }

            this.Children.Add(child);
        }

        public virtual void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            foreach (var child in this.Children) {
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

        public virtual void Dispose() { }
    }

    public abstract class RectanglarEntity : Entity, IClickable {

        protected RectanglarEntity(RectF area, bool relativeToScreenSize) : base(relativeToScreenSize) {
            this.Area = area;
        }

        public RectF Area { get; }


        public event EventHandler<ClickArgs> Click;

        public void OnClick(ClickArgs args) {
            this.Click?.Invoke(this, args);
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
}