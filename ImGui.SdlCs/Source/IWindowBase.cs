using System;
using System.Drawing;
using System.Numerics;
using ImGuiExt.OpenGL;
using ImGuiExt.SDL;
using ImGuiNET;
using SDLCS = SDL2.SDL;

namespace ImGuiExt {

    /// <summary>
    /// Basic window of ImGuiNET.
    /// </summary>
    public abstract class IWindowBase<E> {

        internal abstract unsafe void Create();

        internal abstract void Render();

        internal abstract bool CreateFontsTexture();

        internal abstract void DeleteFontsTexture();

        internal abstract void OnEventHander(IWindow<E> window, E e);

        internal abstract IntPtr Renderer { get; set; }

        public abstract IntPtr FontTexture { get; set; }

        //---------------------------------------------------------------------

        public static readonly Vector4 defaultClearColor = new Vector4(0.4f, 0.5f, 0.6f, 1.0f);

        public IWindow<E> Wnd;

        public Vector4 BackgroundColor {
            get;
            set;
        } = defaultClearColor;

        public void Run()
        {
            Wnd.Run();
        }

        //public double Time { get; private set; } = 0.0f;
        public ulong uTime { get; private set; } = 0;

        //---------------------------------------------------------------------

        internal void Initialize()
        {
            Wnd.OnStart += OnStartHander;
            Wnd.OnLoop += OnLoopHandler;
            Wnd.OnEvent += OnEventHander;
            Wnd.OnExit += OnExitHandler;
        }

        internal void UpdateLayout()
        {
            if(OnLayoutUpdate == null) {
                ImGui.Text($"Create a new class inheriting {GetType().FullName}, overriding {nameof(UpdateLayout)}!");
            } else {
                foreach(LayoutUpdateMethod del in OnLayoutUpdate.GetInvocationList()) {
                    if(!del()) {
                        OnLayoutUpdate -= del;
                    }
                }
            }
        }

        public delegate bool LayoutUpdateMethod();

        /// <summary>
        /// User layout update method.
        /// </summary>
        public event LayoutUpdateMethod OnLayoutUpdate;

        public delegate void WindowStartMethod(IWindowBase<E> window);
        public event WindowStartMethod OnWindowStart;

        public delegate void WindowExitMethod(IWindowBase<E> window);
        public event WindowExitMethod OnWindowExit;

        private void OnStartHander(IWindow<E> window)
        {
            Create();
            OnWindowStart?.Invoke(this);
        }

        private void OnExitHandler(IWindow<E> window)
        {
            OnWindowExit?.Invoke(this);
        }

        private void OnLoopHandler(IWindow<E> window)
        {
            RendererNewFrame();
            ImGui.NewFrame();
            UpdateLayout();
            ImGui.Render();
            Render();
        }

        private void RendererNewFrame()
        {
            if(FontTexture == IntPtr.Zero)
                CreateFontsTexture();

            ImGuiIOPtr io = ImGui.GetIO();
            SDLCS.SDL_GetWindowSize(Wnd.Window, out int w, out int h);
            if((SDLCS.SDL_GetWindowFlags(Wnd.Window) & (uint)SDLCS.SDL_WindowFlags.SDL_WINDOW_MINIMIZED) > 0)
                w = h = 0;
            int display_w, display_h;
            if(Renderer != IntPtr.Zero)
                SDLCS.SDL_GetRendererOutputSize(Renderer, out display_w, out display_h);
            else
                SDLCS.SDL_GL_GetDrawableSize(Wnd.Window, out display_w, out display_h);
            io.DisplaySize = new Vector2((float)w, (float)h);
            if(w > 0 && h > 0)
                io.DisplayFramebufferScale = new Vector2(1.0f * display_w / w, 1.0f * display_h / h);

            // Setup time step
            ulong current_time = SDLCS.SDL_GetPerformanceCounter();
            ulong frequency = SDLCS.SDL_GetPerformanceFrequency();
            if(current_time <= uTime)
                current_time = uTime + 1;
            io.DeltaTime = uTime > 0 ? (float)((double)(current_time - uTime) / frequency) : (float)(1.0f / 60.0f);
            uTime = current_time;

            SDLCS.SDL_ShowCursor(io.MouseDrawCursor ? 0 : 1);

            // TODO:
            SDL2Helper.UpdateGamepads();
        }

        ~IWindowBase()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            if(FontTexture != IntPtr.Zero) {
                // Texture gets deleted with the context.
                DeleteFontsTexture();
                if(io.Fonts.TexID == FontTexture)
                    io.Fonts.TexID = IntPtr.Zero;
                FontTexture = IntPtr.Zero;
            }
            Wnd.Dispose();
        }
    }
}