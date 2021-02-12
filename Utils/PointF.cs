namespace SceneDisplayer.Utils {
    /// <summary>
    /// Represents a floated point.
    /// </summary>
    public struct PointF {
        public float x;
        public float y;

        /// <summary>
        /// Constructs a PointF.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y Position.</param>
        public PointF(float x, float y) {
            this.x = x;
            this.y = y;
        }


        public override string ToString() {
            return $"({x};{y})";
        }
    }
}