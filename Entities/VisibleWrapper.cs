using System;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// An <see cref="SceneDisplayer.Entities.Entity"/> that encapsulates another <c>Entity</c> and manages whether that <c>Entity</c> is visible or not.
    /// </summary>
    public class VisibleWrapper : Entity {
        private Entity entity;

        /// <summary>
        /// Constructs a <c>VisibleWrapper</c>.
        /// </summary>
        /// <param name="entity">The <c>Entity</c> to be encapsulated.</param>
        /// <param name="visible">True to make the child visible, False othwerise.</param>
        public VisibleWrapper(Entity entity, bool visible) {
            this.entity = entity;
        }


        /// <summary>
        /// Whether the child is set to be rendered, or not.
        /// </summary>
        /// <value>True if it is set to be rendered, False othwerise.</value>
        public bool Visible { get; set; }

        /// <summary>
        /// The encapsulated <c>Entity</c>.
        /// </summary>
        public Entity Entity => this.Children[this.Key];


        public override void Init() {
            this.AddChild(this.Key, this.entity);
        }

        public override void Draw(IntPtr renderer, int windowWidth, int windowHeight, uint deltaTime) {
            if (!this.Visible) return;
            
            base.Draw(renderer, windowWidth, windowHeight, deltaTime);
        }
    }
}