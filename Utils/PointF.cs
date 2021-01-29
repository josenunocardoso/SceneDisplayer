namespace SceneDisplayer.Utils {
    /// <summary>
    /// Represents a floated point.
    /// </summary>
    public struct PointF {
        public float x;
        public float y;


        public override string ToString() {
            return $"({x};{y})";
        }
    }
}