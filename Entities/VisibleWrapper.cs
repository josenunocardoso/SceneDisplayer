using System;

namespace SceneDisplayer.Entities {
    public class VisibleWrapper : Entity {
        private Entity entity;

        public VisibleWrapper(Entity entity, bool visible) {
            this.entity = entity;
        }


        public bool Visible { get; set; }

        public Entity Entity => this.Children[this.Key];


        public override void Init() {
            this.AddChild(this.Key, this.entity);
        }

        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            if (!this.Visible) return;
            
            base.Draw(renderer, screenWidth, screenHeight);
        }
    }
}