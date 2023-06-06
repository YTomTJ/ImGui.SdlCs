using SDL2;
using System;
using System.Numerics;
using SDLCS = SDL2.SDL;

namespace ImGuiExt.SDL {

    public class SDL2Window : IDisposable, IWindow<SDLCS.SDL_Event> {

        public IntPtr Window {
            get;
            private set;
        }

        public bool IsAlive {
            get;
            private set;
        } = false;

        public bool IsVisible => ((SDLCS.SDL_WindowFlags)SDLCS.SDL_GetWindowFlags(Window) & SDLCS.SDL_WindowFlags.SDL_WINDOW_HIDDEN) == 0;

        public float MaxFPS { get; set; } = 60.0f;

        public string Title {
            get {
                return SDLCS.SDL_GetWindowTitle(Window);
            }
            set {
                SDLCS.SDL_SetWindowTitle(Window, value);
            }
        }

        public Vector2 Position {
            get {
                int x, y;
                SDLCS.SDL_GetWindowPosition(Window, out x, out y);
                return new Vector2(x, y);
            }
            set {
                SDLCS.SDL_SetWindowPosition(Window, (int)Math.Round(value.X), (int)Math.Round(value.Y));
            }
        }

        public Vector2 Size {
            get {
                int x, y;
                SDLCS.SDL_GetWindowSize(Window, out x, out y);
                return new Vector2(x, y);
            }
            set {
                SDLCS.SDL_SetWindowSize(Window, (int)Math.Round(value.X), (int)Math.Round(value.Y));
            }
        }

        public Action<IWindow<SDLCS.SDL_Event>> OnStart { get; set; }

        public Action<IWindow<SDLCS.SDL_Event>> OnLoop { get; set; }

        public Action<IWindow<SDLCS.SDL_Event>> OnExit { get; set; }

        public Action<IWindow<SDLCS.SDL_Event>, SDLCS.SDL_Event> OnEvent { get; set; }

        

        internal SDL2Window(string title, int width, int height, SDLCS.SDL_WindowFlags flags)
        {
            SDLCS.SDL_Init(SDLCS.SDL_INIT_EVERYTHING);
            if(Window != IntPtr.Zero)
                throw new InvalidOperationException("SDL2Window already initialized, Dispose() first before reusing!");
            Window = SDLCS.SDL_CreateWindow(title, SDLCS.SDL_WINDOWPOS_CENTERED, SDLCS.SDL_WINDOWPOS_CENTERED, width, height, flags);
        }

        public void Show()
        {
            SDLCS.SDL_ShowWindow(Window);
        }

        public void Hide()
        {
            SDLCS.SDL_HideWindow(Window);
        }

        public void Run()
        {
            Show();
            IsAlive = true;
            OnStart?.Invoke(this);
            while(IsAlive) {
                //PollEvents
                SDLCS.SDL_Event e;
                while(SDLCS.SDL_PollEvent(out e) != 0) {
                    if(e.type == SDLCS.SDL_EventType.SDL_QUIT)
                        Exit();
                    OnEvent?.Invoke(this, e);
                }

                OnLoop?.Invoke(this);
            }
            OnExit?.Invoke(this);
        }

        public void Swap()
        {
            SDLCS.SDL_GL_SwapWindow(Window);
        }

        public void Exit()
        {
            IsAlive = false;
        }

        ~SDL2Window()
        {
            Dispose();
        }

        public void Dispose()
        {
            if(Window != IntPtr.Zero) {
                SDLCS.SDL_DestroyWindow(Window);
                Window = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }
}