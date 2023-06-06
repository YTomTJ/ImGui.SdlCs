using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using ImGuiExt.SDL;
using ImGuiNET;
using SDLCS = SDL2.SDL;

namespace ImGuiExt {

    /// <summary>
    /// Basic window of ImGuiNET with SDL2 + OpenGL.
    /// </summary>
    public class SDL2_SdlRenderer_Window : IWindowBase<SDLCS.SDL_Event> {

        internal override IntPtr Renderer { get; set; }
        public override IntPtr FontTexture { get; set; }

        public SDL2_SdlRenderer_Window(string title = "SDL2_SdlRenderer_Window", int width = 1280, int height = 760)
        {
            //set the gl version to be loaded
            var flags = SDLCS.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDLCS.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI | SDLCS.SDL_WindowFlags.SDL_WINDOW_OPENGL;
            Wnd = new SDL2Window(title, width, height, flags);

            // Create SDL_Renderer graphics context
            Renderer = SDLCS.SDL_CreateRenderer(Wnd.Window, -1, SDLCS.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC | SDLCS.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if(Renderer == IntPtr.Zero) {
                throw new Exception("Error creating SDL_Renderer!");
            }
            SDLCS.SDL_GetRendererInfo(Renderer, out SDLCS.SDL_RendererInfo info);
            Debug.WriteLine(string.Format("Current SDL_Renderer: {0}", Marshal.PtrToStringAnsi(info.name)));

            ImGui.CreateContext();
            SDL2Helper.Initialize(Wnd.Window);
            base.Initialize();
        }

        internal override unsafe void Create()
        {
        }

        internal override void Render()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            SDLCS.SDL_RenderSetScale(Renderer, io.DisplayFramebufferScale.X, io.DisplayFramebufferScale.Y);

            SDLCS.SDL_SetRenderDrawColor(Renderer, (byte)(BackgroundColor.X * 255), (byte)(BackgroundColor.Y * 255),
                 (byte)(BackgroundColor.Z * 255), (byte)(BackgroundColor.W * 255));
            SDLCS.SDL_RenderClear(Renderer);
            RenderDrawData(ImGui.GetDrawData());
            SDLCS.SDL_RenderPresent(Renderer);
        }

        internal override bool CreateFontsTexture()
        {
            var io = ImGui.GetIO();
            var fonts = io.Fonts;

            // Build texture atlas
            // Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more
            // likely to be compatible with user's existing shaders. If your ImTextureId represent a higher-level
            // concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory.
            fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);

            // Upload texture to graphics system
            // (Bilinear sampling is required by default. Set 'io.Fonts->Flags |= ImFontAtlasFlags_NoBakedLines'
            // or 'style.AntiAliasedLinesUseTex = false' to allow point/nearest sampling)
            FontTexture = SDLCS.SDL_CreateTexture(Renderer, SDLCS.SDL_PIXELFORMAT_ABGR8888,
                (int)SDLCS.SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, width, height);
            if(FontTexture == IntPtr.Zero) {
                throw new Exception("Error creating texture.");
            }
            SDLCS.SDL_UpdateTexture(FontTexture, IntPtr.Zero, pixels, 4 * width);
            SDLCS.SDL_SetTextureBlendMode(FontTexture, SDLCS.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDLCS.SDL_SetTextureScaleMode(FontTexture, SDLCS.SDL_ScaleMode.SDL_ScaleModeLinear);
            // Store our identifier
            io.Fonts.SetTexID(FontTexture);
            return true;
        }

        internal override void DeleteFontsTexture()
        {
            SDLCS.SDL_DestroyTexture(FontTexture);
        }

        internal override void OnEventHander(IWindow<SDL2.SDL.SDL_Event> w, SDLCS.SDL_Event e)
        {
            SDL2Helper.EventHandler(e);
        }

        private void RenderDrawData(ImDrawDataPtr draw_data)
        {
            SDLCS.SDL_RenderGetScale(Renderer, out float rsx, out float rsy);

            Vector2 render_scale;
            render_scale.X = (rsx == 1.0f) ? draw_data.FramebufferScale.X : 1.0f;
            render_scale.Y = (rsy == 1.0f) ? draw_data.FramebufferScale.Y : 1.0f;

            int fb_width = (int)(draw_data.DisplaySize.X * render_scale.X);
            int fb_height = (int)(draw_data.DisplaySize.Y * render_scale.Y);
            if(fb_width == 0 || fb_height == 0)
                return;

            SDLCS.SDL_Rect old_Viewport;
            SDLCS.SDL_Rect old_ClipRect;
            SDLCS.SDL_RenderGetViewport(Renderer, out old_Viewport);
            SDLCS.SDL_RenderGetClipRect(Renderer, out old_ClipRect);

            Vector2 clip_off = draw_data.DisplayPos;         // (0,0) unless using multi-viewports
            Vector2 clip_scale = render_scale;

            for(int n = 0; n < draw_data.CmdListsCount; n++) {
                ImDrawListPtr cmd_list = draw_data.CmdListsRange[n];
                IntPtr vtx_buffer = cmd_list.VtxBuffer.Data;
                IntPtr idx_buffer = cmd_list.IdxBuffer.Data;

                for(var cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++) {
                    var pcmd = cmd_list.CmdBuffer[cmd_i];
                    if(pcmd.UserCallback != IntPtr.Zero) {
                        Console.WriteLine("UserCallback not implemented");
                    } else {
                        // Project scissor/clipping rectangles into framebuffer space
                        Vector2 clip_min = new Vector2((pcmd.ClipRect.X - clip_off.X) * clip_scale.X, (pcmd.ClipRect.Y - clip_off.Y) * clip_scale.Y);
                        Vector2 clip_max = new Vector2((pcmd.ClipRect.Z - clip_off.X) * clip_scale.X, (pcmd.ClipRect.W - clip_off.Y) * clip_scale.Y);

                        if(clip_min.X < 0.0f) { clip_min.X = 0.0f; }
                        if(clip_min.Y < 0.0f) { clip_min.Y = 0.0f; }
                        if(clip_max.X > (float)fb_width) { clip_max.X = (float)fb_width; }
                        if(clip_max.Y > (float)fb_height) { clip_max.Y = (float)fb_height; }
                        if(clip_max.X <= clip_min.X || clip_max.Y <= clip_min.Y)
                            continue;

                        var r = new SDLCS.SDL_Rect {
                            x = (int)(clip_min.X),
                            y = (int)(clip_min.Y),
                            w = (int)(clip_max.X - clip_min.X),
                            h = (int)(clip_max.Y - clip_min.Y)
                        };
                        SDLCS.SDL_RenderSetClipRect(Renderer, ref r);

                        IntPtr xy_ptr = IntPtr.Add(vtx_buffer, (int)pcmd.VtxOffset);
                        IntPtr uv_ptr = IntPtr.Add(vtx_buffer, (int)pcmd.VtxOffset + Marshal.OffsetOf<ImDrawVert>("uv").ToInt32());
                        IntPtr col_ptr = IntPtr.Add(vtx_buffer, (int)pcmd.VtxOffset + Marshal.OffsetOf<ImDrawVert>("col").ToInt32());

                        int vtx_size = Marshal.SizeOf<ImDrawVert>();
                        int res = SDLCS.SDL_RenderGeometryRaw(Renderer, pcmd.GetTexID(),
                            (IntPtr)xy_ptr, vtx_size,
                            (IntPtr)col_ptr, vtx_size,
                            (IntPtr)uv_ptr, vtx_size,
                            cmd_list.VtxBuffer.Size - (int)pcmd.VtxOffset,
                            IntPtr.Add(idx_buffer, (int)pcmd.IdxOffset),
                            (int)pcmd.ElemCount,
                            Marshal.SizeOf<ushort>()
                        );

                    }
                }
            }

            // Restore modified SDL_Renderer state
            SDLCS.SDL_RenderSetViewport(Renderer, ref old_Viewport);
            SDLCS.SDL_RenderSetClipRect(Renderer, ref old_ClipRect);
        }

    }
}
