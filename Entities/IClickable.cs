using System;
using SDL2;

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
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        /// <returns>True if it is contained, False otherwise.</returns>
        bool Contains(SDL.SDL_Point point, int screenWidth, int screenHeight);
    }
}