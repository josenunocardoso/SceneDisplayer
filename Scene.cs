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
                this.OnClickRec(entity, x, y);
            }
        }

        private void OnClickRec(Entity entity, int x, int y) {
            foreach (var child in entity.Children.Values) {
                this.OnClickRec(child, x, y);
            }

            if (entity is IClickable) {
                var clickable = entity as IClickable;

                if (clickable.Contains(new SDL.SDL_Point { x = x, y = y })) {
                    clickable.OnClick(new ClickArgs { X = x, Y = y });
                }
            }
        }

        public void Dispose() {
            foreach (var entity in this.Entities) {
                entity.Dispose();
            }
        }
    }
}