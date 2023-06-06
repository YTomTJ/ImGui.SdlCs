﻿using System;
using System.Numerics;
using ImGuiExt.OpenGL;
using ImGuiExt.SDL;
using ImGuiNET;
using SDLCS = SDL2.SDL;

namespace ImGuiExt {

    /// <summary>
    /// Basic window of ImGuiNET.
    /// </summary>
    public class ImWindowBase : SDL2Window {
        public static readonly Vector4 defaultClearColor = new Vector4(0.4f, 0.5f, 0.6f, 1.0f);

        private double g_Time = 0.0f;
        private int g_FontTexture = 0;

        public Vector4 BackgroundColor {
            get;
            set;
        } = defaultClearColor;

        public delegate bool LayoutUpdateMethod();

        /// <summary>
        /// User layout update method.
        /// </summary>
        public event LayoutUpdateMethod OnLayoutUpdate;

        public delegate void WindowStartMethod(ImWindowBase window);
        public event WindowStartMethod OnWindowStart;

        public delegate void WindowExitMethod(ImWindowBase window);
        public event WindowExitMethod OnWindowExit;

        public ImWindowBase(string title = "ImGuiNET.SdlCs Basic Window", int width = 1280, int height = 760,
            SDLCS.SDL_WindowFlags flags = SDLCS.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDLCS.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDLCS.SDL_WindowFlags.SDL_WINDOW_SHOWN) : base(title, width, height, flags)
        {
            ImGui.CreateContext();
            ImWindowHelper.Initialize();

            base.OnStart += OnStartHander;
            base.OnLoop += OnLoopHandler;
            base.OnEvent += OnEventHander;
            base.OnExit += OnExitHandler;
        }

        private void OnStartHander(SDL2Window window)
        {
            //if (!File.Exists("imgui.ini"))
            //    File.WriteAllText("imgui.ini", "");
            Create();
            OnWindowStart?.Invoke(this);
        }

        private void OnLoopHandler(SDL2Window window)
        {
            GL.ClearColor(BackgroundColor.X, BackgroundColor.Y, BackgroundColor.Z, BackgroundColor.W);
            GL.Clear(GL.GlEnum.GL_COLOR_BUFFER_BIT);
            Render();
            Swap();
        }

        private void OnEventHander(SDL2Window window, SDLCS.SDL_Event e)
        {
            ImWindowHelper.EventHandler(e);
        }

        private void OnExitHandler(SDL2Window window)
        {
            OnWindowExit?.Invoke(this);
        }

        private void Render()
        {
            ImWindowHelper.NewFrame(Size, Vector2.One, ref g_Time);
            UpdateLayout();
            ImWindowHelper.Render(Size);
        }

        private unsafe void Create()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            // Build texture atlas
            //ImFontTextureData texData = ;
            io.Fonts.GetTexDataAsAlpha8(out byte* pixels, out int width, out int height);

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            int lastTexture;
            GL.GetIntegerv(GL.GlEnum.GL_TEXTURE_BINDING_2D, out lastTexture);

            // Create OpenGL texture
            GL.GenTextures(1, out g_FontTexture);
            GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, g_FontTexture);
            GL.TexParameteri(GL.GlEnum.GL_TEXTURE_2D, GL.GlEnum.GL_TEXTURE_MIN_FILTER, (int)GL.GlEnum.GL_LINEAR);
            GL.TexParameteri(GL.GlEnum.GL_TEXTURE_2D, GL.GlEnum.GL_TEXTURE_MAG_FILTER, (int)GL.GlEnum.GL_LINEAR);
            GL.PixelStorei(GL.GlEnum.GL_UNPACK_ROW_LENGTH, 0);
            GL.TexImage2D(
                GL.GlEnum.GL_TEXTURE_2D,
                0,
                (int)GL.GlEnum.GL_ALPHA,
                width,
                height,
                0,
                GL.GlEnum.GL_ALPHA,
                GL.GlEnum.GL_UNSIGNED_BYTE,
                (IntPtr)pixels
            );

            // Store the texture identifier in the ImFontAtlas substructure.
            io.Fonts.SetTexID((IntPtr)g_FontTexture);
            io.Fonts.ClearTexData(); // Clears CPU side texture data.
            GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, lastTexture);
        }

        private void UpdateLayout()
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

        public new void Dispose()
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
            base.Dispose();
        }

        ~ImWindowBase()
        {
            Dispose();
        }
    }
}