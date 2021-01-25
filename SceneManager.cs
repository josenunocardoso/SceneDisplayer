using System;
using System.Collections.Generic;
using SDL2;
using SceneDisplayer.Entities;

namespace SceneDisplayer {
    public class SceneManager : IDisposable {
        private const int FPS = 120;
        private readonly int DEFAULT_SCREEN_WIDTH = 1000;
        private readonly int DEFAULT_SCREEN_HEIGHT = 600;
        private readonly SDL.SDL_Color BACKGROUND_COLOR = new SDL.SDL_Color { r = 32, g = 64, b = 128 };
        private const uint DELAY_TIME = (uint)(1000f / FPS);


        public SceneManager() {
            this.Scenes = new Stack<Scene>();
        }


        private static IntPtr _window;
        private IntPtr _renderer;

        private Stack<Scene> Scenes { get; }
        
        private Scene ActiveScene => this.Scenes.Peek();


        public static (int, int) GetWindowSize() {
            SDL.SDL_GetWindowSize(_window, out int w, out int h);

            return (w, h);
        }

        public void Init(Scene defaultScene, string title) {
            SDL_ttf.TTF_Init();
            SDL.SDL_Init(0);

            this.PushScene(defaultScene);

            _window = SDL.SDL_CreateWindow(
                title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                DEFAULT_SCREEN_WIDTH, DEFAULT_SCREEN_HEIGHT,
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            
            SDL.SDL_ShowWindow(_window);

            this._renderer = SDL.SDL_CreateRenderer(
                _window, -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        }

        public void PushScene(Scene scene) {
            if (scene == null) {
                throw new ArgumentNullException("scene");
            }

            this.Scenes.Push(scene);
            this.ActiveScene.Init();
        }

        public void Render() {
            bool running = true;
            uint frameStartTime = SDL.SDL_GetTicks();

            while (running) {
                SDL.SDL_SetRenderDrawColor(this._renderer,
                    BACKGROUND_COLOR.r, BACKGROUND_COLOR.g, BACKGROUND_COLOR.b, BACKGROUND_COLOR.a);

                SDL.SDL_RenderClear(this._renderer);

                var (w, h) = GetWindowSize();

                if (this.ActiveScene != null) {
                    foreach (var entity in this.ActiveScene.Entities) {
                        entity.Draw(this._renderer, w, h);
                    }
                }

                SDL.SDL_RenderPresent(this._renderer);
                
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        running = false;
                        break;
                    }
                    if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN) {
                        var buttonEvent = e.button;

                        if (buttonEvent.button == SDL.SDL_BUTTON_LEFT) {
                            this.MouseLeftDown(buttonEvent.x, buttonEvent.y);
                        }
                        
                        break;
                    }
                    if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT) {
                        var windowEvent = e.window;

                        if (windowEvent.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED) {
                            this.WindowResized(windowEvent.data1, windowEvent.data2);
                        }
                    }
                }

                int delaytime = (int)(DELAY_TIME - (SDL.SDL_GetTicks() - frameStartTime));

                if (delaytime > 0) {
                    SDL.SDL_Delay((uint)delaytime);
                }
                
                frameStartTime = SDL.SDL_GetTicks();
            }

            SDL.SDL_Quit();
        }

        public void AddEntityToActiveScene(Entity entity) {
            this.ActiveScene?.Entities.Add(entity);
        }

        private void MouseLeftDown(int x, int y) {
            this.ActiveScene?.OnClick(x, y);
        }

        private void WindowResized(int width, int height) {
            this.ActiveScene?.OnWindowResized(width, height);
        }

        public void Dispose() {
            foreach (var scene in this.Scenes) {
                scene.Dispose();
            }
        }
    }
}
