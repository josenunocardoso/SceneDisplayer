using SDL2;
using System;
using System.Collections.Generic;
using SceneDisplayer.Entities;

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
        /// Adds an <see cref="Entity"/> to the scene, and initializes it.
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> to be added.</param>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the <c>entity</c> is null.</exception>
        public void AddEntity(Entity entity) {
            if (entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }

            this.Entities.Add(entity);
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
        /// <param name="mouseButton">Mouse Button pressed.</param>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        public void OnClick(int x, int y, MouseButton mouseButton, int windowWidth, int windowHeight) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    if (child is IClickable) {
                        var clickable = child as IClickable;

                        if (clickable.Contains(new SDL.SDL_Point { x = x, y = y }, windowWidth, windowHeight)) {
                            clickable.OnClick(new ClickArgs { X = x, Y = y, Button = mouseButton });
                            return;
                        }
                    }

                    child.OnMouseDown(new ClickArgs { X = x, Y = y, Button = mouseButton });
                });
            }
        }

        /// <summary>
        /// Triggers a <c>MouseUp</c> event to all the <c>Entities</c>.
        /// </summary>
        /// <param name="x">Mouse Location X.</param>
        /// <param name="y">Mouse Location Y.</param>
        /// <param name="mouseButton">Mouse Button pressed.</param>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        public void OnMouseUp(int x, int y, MouseButton mouseButton, int windowWidth, int windowHeight) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    child.OnMouseUp(new ClickArgs { X = x, Y = y, Button = mouseButton });
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
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        public void OnWindowResized(int windowWidth, int windowHeight) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    child.OnWindowResize(new WindowArgs { Width = windowWidth, Height = windowHeight });
                });
            }
        }

        private void PerformActionOnAllChildren(Entity entity, Action<Entity> action) {
            foreach (var child in entity.GetChildren()) {
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