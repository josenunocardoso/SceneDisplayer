using SDL2;
using SceneDisplayer.Entities;
using System.Collections.Generic;
using System;

namespace SceneDisplayer {
    public abstract class Scene : IDisposable {

        protected Scene() {
            this.Entities = new List<Entity>();
        }

        public List<Entity> Entities { get; }


        public void Init() {
            foreach (var entity in this.Entities) {
                entity.Init();
            }
        }

        public void OnClick(int x, int y) {
            foreach (var entity in this.Entities) {
                this.PerformActionOnAllChildren(entity, child => {
                    if (child is IClickable) {
                        var clickable = child as IClickable;

                        if (clickable.Contains(new SDL.SDL_Point { x = x, y = y })) {
                            clickable.OnClick(new ClickArgs { X = x, Y = y });
                        }
                    }
                });
            }
        }

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

        public void Dispose() {
            foreach (var entity in this.Entities) {
                entity.Dispose();
            }
        }
    }
}