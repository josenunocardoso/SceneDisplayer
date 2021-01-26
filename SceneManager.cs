using System;
using System.Collections.Generic;
using SDL2;
using SceneDisplayer.Entities;

namespace SceneDisplayer {
    public static class SceneManager {
        private const int FPS = 120;
        private const int DEFAULT_SCREEN_WIDTH = 1000;
        private const int DEFAULT_SCREEN_HEIGHT = 600;
        private static readonly SDL.SDL_Color BACKGROUND_COLOR = new SDL.SDL_Color { r = 32, g = 64, b = 128 };
        private const uint DELAY_TIME = (uint)(1000f / FPS);


        static SceneManager() {
            Scenes = new Stack<Scene>();
        }

 
        private static IntPtr _window;
        private static IntPtr _renderer;
 
        private static Stack<Scene> Scenes { get; }
         
        private static Scene ActiveScene => Scenes.Peek();


        public static (int, int) GetWindowSize() {
            SDL.SDL_GetWindowSize(_window, out int w, out int h);

            return (w, h);
        }

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

        public static void PushScene(Scene scene) {
            if (scene == null) {
                throw new ArgumentNullException("scene");
            }

            Scenes.Push(scene);
            ActiveScene.Init();
        }

        public static void Render() {
            bool running = true;
            uint frameStartTime = SDL.SDL_GetTicks();

            while (running) {
                SDL.SDL_SetRenderDrawColor(_renderer,
                    BACKGROUND_COLOR.r, BACKGROUND_COLOR.g, BACKGROUND_COLOR.b, BACKGROUND_COLOR.a);

                SDL.SDL_RenderClear(_renderer);

                var (w, h) = GetWindowSize();

                if (ActiveScene != null) {
                    foreach (var entity in ActiveScene.Entities) {
                        entity.Draw(_renderer, w, h);
                    }
                }

                SDL.SDL_RenderPresent(_renderer);
                
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        running = false;
                        break;
                    }
                    if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN) {
                        var buttonEvent = e.button;

                        if (buttonEvent.button == SDL.SDL_BUTTON_LEFT) {
                            MouseLeftDown(buttonEvent.x, buttonEvent.y, w, h);
                        }
                        
                        break;
                    }
                    if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT) {
                        var windowEvent = e.window;

                        if (windowEvent.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED) {
                            WindowResized(windowEvent.data1, windowEvent.data2);
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

        public static void AddEntityToActiveScene(Entity entity) {
            ActiveScene?.Entities.Add(entity);
        }

        private static void MouseLeftDown(int x, int y, int screenWidth, int screenHeight) {
            ActiveScene?.OnClick(x, y, screenWidth, screenHeight);
        }

        private static void WindowResized(int width, int height) {
            ActiveScene?.OnWindowResized(width, height);
        }

        public static void Dispose() {
            foreach (var scene in Scenes) {
                scene.Dispose();
            }
        }
    }
}
