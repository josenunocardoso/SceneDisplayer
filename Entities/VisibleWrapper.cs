using System;

namespace SceneDisplayer.Entities {
    public class VisibleWrapper : Entity {
        
        public VisibleWrapper(Entity entity, bool visible) {
            this.AddChild("Child", entity);
        }


        public bool Visible { get; set; }

        public Entity Entity => this.Children["Child"];


        public override void Draw(IntPtr renderer, int screenWidth, int screenHeight) {
            if (!this.Visible) return;
            
            base.Draw(renderer, screenWidth, screenHeight);
        }
    }
}