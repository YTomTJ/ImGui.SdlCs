using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiExt.OpenGL;
using ImGuiExt.SDL;
using ImGuiNET;
using static SDL2.SDL;
using SDLCS = SDL2.SDL;

namespace ImGuiExt {

    /// <summary>
    /// Basic window of ImGuiNET with SDL2 + OpenGL.
    /// </summary>
    public class SDL2_SdlRenderer_Window : IWindowBase<SDLCS.SDL_Event> {

        private IntPtr Renderer;

        public SDL2_SdlRenderer_Window(string title = "SDL2_SdlRenderer_Window", int width = 1280, int height = 760)
        {
            var flags = SDLCS.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDLCS.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;
            Wnd = new SDL2Window(title, width, height, flags);

            // Create SDL_Renderer graphics context
            Renderer = SDLCS.SDL_CreateRenderer(Wnd.Window, -1, SDLCS.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC | SDLCS.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if(Renderer == IntPtr.Zero) {
                throw new Exception("Error creating SDL_Renderer!");
            }

            ImGui.CreateContext();
            SDL2Helper.Initialize();

            var fonts = ImGui.GetIO().Fonts;
            unsafe {
                fonts.GetTexDataAsRGBA32(out byte* pixelData, out int texwidth, out int texheight);
                fonts.TexID = LoadTexture((IntPtr)pixelData, texwidth, texheight);
                fonts.ClearTexData();
            }

            base.Initialize();
        }

        private IntPtr LoadTexture(IntPtr pixelData, int width, int height)
        {
            var texture = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_ABGR8888, 0, width, height);
            SDL_Rect rect = new SDL_Rect {
                x = 0,
                y = 0,
                w = width,
                h = height,
            };
            SDL_UpdateTexture(texture, ref rect, pixelData, 4 * width);
            SDL_SetTextureBlendMode(texture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
            return texture;
        }

        internal override unsafe void Create()
        {
            // Setup display size (every frame to accommodate for window resizing)
            SDL_GetWindowSize(Wnd.Window, out int w, out int h);
            if((SDL_GetWindowFlags(Wnd.Window) & (uint)SDLCS.SDL_WindowFlags.SDL_WINDOW_MINIMIZED) > 0) {
                w = h = 0;
            }
            if(Renderer != IntPtr.Zero)
                SDL_GetRendererOutputSize(Renderer, out int display_w, out int display_h);
            else
                SDL_GL_GetDrawableSize(Wnd.Window, out int display_w, out int display_h);
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
                        IntPtr uv_ptr = IntPtr.Add(vtx_buffer, (int)pcmd.VtxOffset + Marshal.SizeOf<Vector2>());
                        IntPtr col_ptr = IntPtr.Add(vtx_buffer, (int)pcmd.VtxOffset + Marshal.SizeOf<Vector2>() * 2);

                        int vtx_size = Marshal.SizeOf<ImDrawVert>();
                        int res = SDLCS.SDL_RenderGeometryRaw(Renderer, pcmd.GetTexID(),
                            (IntPtr)xy_ptr, vtx_size,
                            (IntPtr)col_ptr, vtx_size,
                            (IntPtr)uv_ptr, vtx_size,
                            cmd_list.VtxBuffer.Size - (int)pcmd.VtxOffset,
                            IntPtr.Add(idx_buffer, (int)pcmd.IdxOffset),
                            cmd_list.IdxBuffer.Size - (int)pcmd.IdxOffset,
                            Marshal.SizeOf<ushort>()
                        );

                        if(res != 0) {
                            var ss = SDLCS.SDL_GetError();
                        }
                    }
                }
            }

            // Restore modified SDL_Renderer state
            SDLCS.SDL_RenderSetViewport(Renderer, ref old_Viewport);
            SDLCS.SDL_RenderSetClipRect(Renderer, ref old_ClipRect);
        }

        internal override void OnEventHander(IWindow<SDL2.SDL.SDL_Event> w, SDLCS.SDL_Event e)
        {
            SDL2Helper.EventHandler(e);
        }
    }
}
