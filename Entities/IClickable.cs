using System;
using SDL2;

namespace SceneDisplayer.Entities {
    public interface IClickable {
        event EventHandler<ClickArgs> Click;

        void OnClick(ClickArgs args);

        bool Contains(SDL.SDL_Point point, int screenWidth, int screenHeight);
    }
}