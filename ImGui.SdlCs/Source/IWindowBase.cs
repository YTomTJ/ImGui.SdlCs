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

        internal abstract void OnEventHander(IWindow<E> window, E e);

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

        internal double g_Time = 0.0f;
        internal int g_FontTexture = 0;

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
            SDL2Helper.NewFrame(Wnd.Size, Vector2.One, ref g_Time);
            UpdateLayout();
            ImGui.Render();

            Render();
        }

        ~IWindowBase()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            if(g_FontTexture != 0) {
                // Texture gets deleted with the context.
                // GL.DeleteTexture(g_FontTexture);
                if((int)io.Fonts.TexID == g_FontTexture)
                    io.Fonts.TexID = IntPtr.Zero;
                g_FontTexture = 0;
            }
            Wnd.Dispose();
        }
    }
}