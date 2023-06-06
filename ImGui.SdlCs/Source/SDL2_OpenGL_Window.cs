using System;
using System.Drawing;
using System.Numerics;
using ImGuiExt.OpenGL;
using ImGuiExt.SDL;
using ImGuiNET;
using SDLCS = SDL2.SDL;

namespace ImGuiExt {

    /// <summary>
    /// Basic window of ImGuiNET with SDL2 + OpenGL.
    /// </summary>
    public class SDL2_OpenGL_Window : IWindowBase<SDLCS.SDL_Event> {

        private static readonly int doubleBuffer = 1;
        private static readonly int depthSize = 24;
        private static readonly int stencilSize = 8;
        private static readonly int majorVersion = 3;
        private static readonly int minorVersion = 0;

        public SDL2_OpenGL_Window(string title = "SDL2_OpenGL_Window", int width = 1280, int height = 760)
        {
            SDLCS.SDL_Init(SDLCS.SDL_INIT_EVERYTHING);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_DOUBLEBUFFER, doubleBuffer);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_DEPTH_SIZE, depthSize);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_STENCIL_SIZE, stencilSize);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, majorVersion);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, minorVersion);

            var flags = SDLCS.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDLCS.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDLCS.SDL_WindowFlags.SDL_WINDOW_SHOWN;
            Wnd = new SDL2Window(title, width, height, flags);

            ImGui.CreateContext();
            SDL2Helper.Initialize();

            Wnd.OnStart += OnStartHander;
            Wnd.OnLoop += OnLoopHandler;
            Wnd.OnEvent += OnEventHander;
            Wnd.OnExit += OnExitHandler;
        }

        internal override unsafe void Create()
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

        internal override void Render()
        {
            SDL2Helper.NewFrame(Wnd.Size, Vector2.One, ref g_Time);
            UpdateLayout();
            ImGui.Render();
            if(ImGui.GetIO()._UnusedPadding == IntPtr.Zero)
                GLHelper.RenderDrawData(ImGui.GetDrawData(), (int)Math.Round(Wnd.Size.X), (int)Math.Round(Wnd.Size.Y));
        }

        internal override void OnLoopHandler(SDL2Window window)
        {
            GL.ClearColor(BackgroundColor.X, BackgroundColor.Y, BackgroundColor.Z, BackgroundColor.W);
            GL.Clear(GL.GlEnum.GL_COLOR_BUFFER_BIT);
            Render();
            Wnd.Swap();
        }

        internal override void OnEventHander(SDL2Window window, SDLCS.SDL_Event e)
        {
            SDL2Helper.EventHandler(e);
        }
    }
}