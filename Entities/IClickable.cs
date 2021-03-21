using System;

namespace SceneDisplayer.Entities {
    /// <summary>
    /// Represents an interface of an <c>Entity</c> that allows for <c>Click</c> events.
    /// </summary>
    public interface IClickable {
        event EventHandler<ClickArgs> Click;

        /// <summary>
        /// Method called when the <c>Entity</c> is clicked.
        /// </summary>
        /// <param name="args">Arguments relative to the click.</param>
        void OnClick(ClickArgs args);

        /// <summary>
        /// Returns if an absolute point is contained in this <c>Entity</c>.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="windowWidth">Window width in pixels.</param>
        /// <param name="windowHeight">Window height in pixels.</param>
        /// <returns>True if it is contained, False otherwise.</returns>
        bool Contains((int, int) point, int windowWidth, int windowHeight);

        /// <summary>
        /// This Clickable Entity Traits.
        /// </summary>
        ClickableEntityTraits ClickableEntityTraits { get; set; }
    }
}