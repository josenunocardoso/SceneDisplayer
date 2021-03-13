using System;
using System.Collections.Generic;
using SDL2;
using SceneDisplayer.Entities;
using SceneDisplayer.Utils;

namespace SceneDisplayer {
    /// <summary>
    /// Main class, responsible to manage the active scenes and to render them.
    /// </summary>
    public static class SceneManager {
        private const int DEFAULT_SCREEN_WIDTH = 1000;
        private const int DEFAULT_SCREEN_HEIGHT = 600;


        static SceneManager() {
            Scenes = new Stack<Scene>();
        }

 
        private static IntPtr _window;
        private static IntPtr _renderer;
        private static SDL.SDL_Color _backgroundColor = new SDL.SDL_Color { r = 32, g = 64, b = 128 };
 
        private static Stack<Scene> Scenes { get; }
         
        private static Scene ActiveScene => Scenes.Peek();
        private static uint DELAY_TIME = (uint)(1000f / DEFAULT_FPS);
        private const uint DEFAULT_FPS = 120u;


        /// <summary>
        /// Sets a new backgound color.
        /// </summary>
        /// <param name="color">The new color.</param>
        public static void SetBackgroundColor(Color color) {
            _backgroundColor = color.ToSDLColor();
        }

        /// <summary>
        /// Gets the current desired frames per second of the renderer.
        /// </summary>
        public static uint GetFPS() {
            return DEFAULT_FPS;
        }

        /// <summary>
        /// Sets the desired frames per second of the renderer. By default it is 120.
        /// </summary>
        public static void SetFPS(uint fps) {
            DELAY_TIME = (uint)(1000f / fps);
        }

        /// <summary>
        /// Retrieves the current window size.
        /// </summary>
        /// <returns>A tuple with the width and the height, in pixels, respectively.</returns>
        public static (int, int) GetWindowSize() {
            SDL.SDL_GetWindowSize(_window, out int w, out int h);

            return (w, h);
        }

        /// <summary>
        /// Retrieves the current mouse position.
        /// </summary>
        /// <returns>A tuple with the X location and the Y location, in pixels, respectively.</returns>
        public static (int, int) GetMousePosition() {
            SDL.SDL_GetMouseState(out int w, out int h);

            return (w, h);
        }

        /// <summary>
        /// Initializes the necessary resources to create a window and a renderer.
        /// Must be called before <see cref="Render"/>
        /// </summary>
        /// <param name="defaultScene">The initial <see cref="Scene"/> to be displayed.
        /// It is set as the <see cref="ActiveScene"/></param>
        /// <param name="title">The title of the window.</param>
        public static void Init(Scene defaultScene, string title) {
            SDL_ttf.TTF_Init();
            SDL.SDL_Init(0);

            PushScene(defaultScene);

            _window = SDL.SDL_CreateWindow(
                title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                DEFAULT_SCREEN_WIDTH, DEFAULT_SCREEN_HEIGHT,
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            
            SDL.SDL_ShowWindow(_window);

            _renderer = SDL.SDL_CreateRenderer(
                _window, -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        }

        /// <summary>
        /// Pushes a new <see cref="Scene"/> to the Stack, and initializes it.
        /// The given <see cref="Scene"/> is set as the <see cref="ActiveScene"/>.
        /// </summary>
        /// <param name="scene">The <see cref="Scene"/> to push.</param>
        public static void PushScene(Scene scene) {
            if (scene == null) {
                throw new ArgumentNullException(nameof(scene));
            }

            Scenes.Push(scene);
            ActiveScene.Init();
        }

        /// <summary>
        /// Pops the <see cref="ActiveScene"/> from the Stack, and disposes it.
        /// The previous <see cref="Scene"/> is set as the <see cref="ActiveScene"/>.
        /// </summary>
        /// <returns>The <see cref="Scene"/> that got popped.</returns>
        /// <exception>Throws an <see cref="ArgumentException"/> if there is only one <see cref="Scene"/> in the Stack.</exception>
        public static Scene PopScene() {
            if (Scenes.Count <= 1) {
                throw new ArgumentException("Cannot pop the Scene because it is the only one that exists");
            }

            var popped = Scenes.Pop();
            popped.Dispose();

            return popped;
        }

        /// <summary>
        /// Blocking method, that renders the <see cref="ActiveScene"/> and triggers events.
        /// This method exits when the window is closed.
        /// </summary>
        public static void Render() {
            bool running = true;
            uint frameStartTime = SDL.SDL_GetTicks();
            uint delta = 0;

            while (running) {
                SDL.SDL_SetRenderDrawColor(_renderer,
                    _backgroundColor.r, _backgroundColor.g, _backgroundColor.b, _backgroundColor.a);

                SDL.SDL_RenderClear(_renderer);

                var (w, h) = GetWindowSize();

                if (ActiveScene != null) {
                    foreach (var entity in ActiveScene.Entities) {
                        entity.Draw(_renderer, w, h, delta);
                    }
                    ActiveScene.OnUpdate();
                }

                SDL.SDL_RenderPresent(_renderer);
                
                if (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        running = false;
                        break;
                    }
                    if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN) {
                        var buttonEvent = e.button;

                        if (buttonEvent.button == SDL.SDL_BUTTON_LEFT) {
                            MouseDown(buttonEvent.x, buttonEvent.y, MouseButton.LEFT, w, h);
                        }
                        if (buttonEvent.button == SDL.SDL_BUTTON_RIGHT) {
                            MouseDown(buttonEvent.x, buttonEvent.y, MouseButton.RIGHT, w, h);
                        }
                    }
                    if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP) {
                        var buttonEvent = e.button;

                        if (buttonEvent.button == SDL.SDL_BUTTON_LEFT) {
                            MouseUp(buttonEvent.x, buttonEvent.y, MouseButton.LEFT, w, h);
                        }
                        if (buttonEvent.button == SDL.SDL_BUTTON_RIGHT) {
                            MouseUp(buttonEvent.x, buttonEvent.y, MouseButton.RIGHT, w, h);
                        }
                    }
                    if (e.type == SDL.SDL_EventType.SDL_KEYDOWN) {
                        var keyEvent = e.key;

                        KeyDown(keyEvent.keysym.sym);
                    }
                    if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT) {
                        var windowEvent = e.window;

                        if (windowEvent.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED) {
                            WindowResized(windowEvent.data1, windowEvent.data2);
                        }
                    }
                }
                
                if (DELAY_TIME > delta) {
                    uint delaytime = DELAY_TIME - delta;
                    SDL.SDL_Delay(delaytime);
                }

                delta = SDL.SDL_GetTicks() - frameStartTime;
                
                frameStartTime = SDL.SDL_GetTicks();
            }

            SDL.SDL_Quit();
        }

        /// <summary>
        /// Adds a new <see cref="Entity"/> to the <see cref="ActiveScene"/>.
        /// </summary>
        /// <param name="entity"><see cref="Entity"/> to be added.</param>
        public static void AddEntityToActiveScene(Entity entity) {
            ActiveScene?.Entities.Add(entity);
        }

        /// <summary>
        /// Displays a MessageBox on the active window.
        /// </summary>
        /// <param name="flags"><see cref="MessageBoxFlags"/>.</param>
        /// <param name="title">Title of the MessageBox.</param>
        /// <param name="message">Message of the MessageBox.</param>
        public static void ShowMessageBox(MessageBoxFlags flags, string title, string message) {
            SDL.SDL_ShowSimpleMessageBox((SDL.SDL_MessageBoxFlags)flags, title, message, _window);
        }


        private static void MouseDown(int x, int y, MouseButton mouseButton, int windowWidth, int windowHeight) {
            ActiveScene?.OnClick(x, y, mouseButton, windowWidth, windowHeight);
        }

        private static void MouseUp(int x, int y, MouseButton mouseButton, int windowWidth, int windowHeight) {
            ActiveScene?.OnMouseUp(x, y, mouseButton, windowWidth, windowHeight);
        }

        private static void KeyDown(SDL.SDL_Keycode key) {
            ActiveScene?.OnKeyDown(key);
        }

        private static void WindowResized(int width, int height) {
            ActiveScene?.OnWindowResized(width, height);
        }

        /// <summary>
        /// Disposes all the <c>Scenes</c>.
        /// </summary>
        public static void Dispose() {
            foreach (var scene in Scenes) {
                scene.Dispose();
            }
        }
    }

    [Flags]
    public enum MessageBoxFlags : uint {
        MESSAGEBOX_ERROR = SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
        MESSAGEBOX_WARNING = SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
        MESSAGEBOX_INFORMATION = SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION
    }

    public enum MouseButton {
        LEFT,
        RIGHT
    }
}
