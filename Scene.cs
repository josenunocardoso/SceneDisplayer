using SDL2;
using SceneDisplayer.Entities;
using System.Collections.Generic;
using System;

namespace SceneDisplayer {
    /// <summary>
    /// A Scene represents a section of the screen to be rendered.
    /// It contains <c>Entities</c> that can be added and removed.
    /// </summary>
    public abstract class Scene : IDisposable {

        protected Scene() {
            this.Entities = new List<Entity>();
        }


        public List<Entity> Entities { get; }


        /// <summary>
        /// Event triggered at each frame the Scene is active.
        /// </summary>
        public event EventHandler Update;


        /// <summary>
        /// Initializes all the <c>Entities</c> and their children, recursively.
        /// </summary>
        public void Init() {
            foreach (var entity in this.Entities) {
                entity.Init();
            }
        }

        /// <summary>
        /// Triggers an <c>Update</c> event to the <c>Scene</c>.
        /// </summary>
        public void OnUpdate() {
            this.Update?.Invoke(this, null);
        }

        /// <summary>
        /// Triggers a <c>Click</c> event to the <c>Entities</c> that are <c>Clickable</c>.
        /// It also triggers a <c>MouseDown</c> event to all the <c>Entities</c>.
        /// </summary>
        /// <param name="x">Mouse Location X.</param>
        /// <param name="y">Mouse Location Y.</param>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        public void OnClick(int x, int y, int screenWidth, int screenHeight) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    if (child is IClickable) {
                        var clickable = child as IClickable;

                        if (clickable.Contains(new SDL.SDL_Point { x = x, y = y }, screenWidth, screenHeight)) {
                            clickable.OnClick(new ClickArgs { X = x, Y = y });
                            return;
                        }
                    }

                    child.OnMouseDown(new ClickArgs { X = x, Y = y });
                });
            }
        }

        /// <summary>
        /// Triggers a <c>KeyDown</c> event to all the <c>Entities</c>.
        /// </summary>
        /// <param name="key">Key pressed.</param>
        public void OnKeyDown(SDL.SDL_Keycode key) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    child.OnKeyDown(new KeyArgs { Key = key });
                });
            }
        }

        /// <summary>
        /// Triggers a <c>WindowResized</c> event to all the <c>Entities</c>.
        /// </summary>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        public void OnWindowResized(int screenWidth, int screenHeight) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    child.OnWindowResize(new WindowArgs { Width = screenWidth, Height = screenHeight });
                });
            }
        }

        private void PerformActionOnAllChildren(Entity entity, Action<Entity> action) {
            foreach (var child in entity.Children.Values) {
                this.PerformActionOnAllChildren(child, action);
            }

            action(entity);
        }

        /// <summary>
        /// Disposes all the <c>Entities</c> and their children, recursively.
        /// </summary>
        public void Dispose() {
            foreach (var entity in this.Entities) {
                entity.Dispose();
            }
        }
    }
}