using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SDL2;

namespace ImGuiNET.SDL2CS {

    public static class ImWindowHelper {

        public static bool IsInitialized {
            get;
            private set;
        } = false;

        /// <summary>
        /// Alternate for ImGui.GetClipboardText
        /// </summary>
        /// <returns></returns>
        public static string GetClipboardText() {
            try {
                return SDL.SDL_GetClipboardText();
            } catch (Exception) {
                // TODO: deal with exception
                return null;
            }
        }

        /// <summary>
        /// Alternate for ImGui.SetClipboardText
        /// </summary>
        /// <param name="text"></param>
        public static void SetClipboardText(string text) {
            try {
                SDL.SDL_SetClipboardText(text);
            } catch (Exception) {
                // TODO: deal with exception
            }
        }

        public static void Initialize() {
            if (IsInitialized)
                return;
            IsInitialized = true;

            ImGuiIOPtr io = ImGui.GetIO();

            io.KeyMap[(int) ImGuiKey.Tab] = (int) SDL.SDL_Keycode.SDLK_TAB;
            io.KeyMap[(int) ImGuiKey.LeftArrow] = (int) SDL.SDL_Scancode.SDL_SCANCODE_LEFT;
            io.KeyMap[(int) ImGuiKey.RightArrow] = (int) SDL.SDL_Scancode.SDL_SCANCODE_RIGHT;
            io.KeyMap[(int) ImGuiKey.UpArrow] = (int) SDL.SDL_Scancode.SDL_SCANCODE_UP;
            io.KeyMap[(int) ImGuiKey.DownArrow] = (int) SDL.SDL_Scancode.SDL_SCANCODE_DOWN;
            io.KeyMap[(int) ImGuiKey.PageUp] = (int) SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP;
            io.KeyMap[(int) ImGuiKey.PageDown] = (int) SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN;
            io.KeyMap[(int) ImGuiKey.Home] = (int) SDL.SDL_Scancode.SDL_SCANCODE_HOME;
            io.KeyMap[(int) ImGuiKey.End] = (int) SDL.SDL_Scancode.SDL_SCANCODE_END;
            io.KeyMap[(int) ImGuiKey.Delete] = (int) SDL.SDL_Keycode.SDLK_DELETE;
            io.KeyMap[(int) ImGuiKey.Backspace] = (int) SDL.SDL_Keycode.SDLK_BACKSPACE;
            io.KeyMap[(int) ImGuiKey.Enter] = (int) SDL.SDL_Keycode.SDLK_RETURN;
            io.KeyMap[(int) ImGuiKey.Escape] = (int) SDL.SDL_Keycode.SDLK_ESCAPE;
            io.KeyMap[(int) ImGuiKey.A] = (int) SDL.SDL_Keycode.SDLK_a;
            io.KeyMap[(int) ImGuiKey.C] = (int) SDL.SDL_Keycode.SDLK_c;
            io.KeyMap[(int) ImGuiKey.V] = (int) SDL.SDL_Keycode.SDLK_v;
            io.KeyMap[(int) ImGuiKey.X] = (int) SDL.SDL_Keycode.SDLK_x;
            io.KeyMap[(int) ImGuiKey.Y] = (int) SDL.SDL_Keycode.SDLK_y;
            io.KeyMap[(int) ImGuiKey.Z] = (int) SDL.SDL_Keycode.SDLK_z;

            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

            // If no font added, add default font.
            if (io.Fonts.Fonts.Size == 0)
                io.Fonts.AddFontDefault();
        }

        public static void NewFrame(Vector2 size, Vector2 scale, ref double g_Time) {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = size;
            io.DisplayFramebufferScale = scale;

            // Setup time step
            UInt32 current_time = SDL.SDL_GetTicks();
            var dt = (float) ((current_time - g_Time) / 1000.0f);
            io.DeltaTime = dt > 0 ? dt : (float) (1.0f / 60.0f);
            g_Time = current_time;

            SDL.SDL_ShowCursor(io.MouseDrawCursor ? 0 : 1);

            ImWindowHelper.UpdateGamepads();

            ImGui.NewFrame();
        }

        public static void Render(Vector2 size) {
            ImGui.Render();
            if (ImGui.GetIO()._UnusedPadding == IntPtr.Zero)
                RenderDrawData(ImGui.GetDrawData(), (int) Math.Round(size.X), (int) Math.Round(size.Y));
        }

        public static unsafe void RenderDrawData(ImDrawDataPtr drawData, int displayW, int displayH) {
            // We are using the OpenGL fixed pipeline to make the example code simpler to read!
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers.
            int lastProgram;
            GL.GetIntegerv(GL.GlEnum.GL_CURRENT_PROGRAM, out lastProgram);
            int lastTexture;
            GL.GetIntegerv(GL.GlEnum.GL_TEXTURE_BINDING_2D, out lastTexture);
            Int4 lastViewport;
            GL.GetIntegerv4(GL.GlEnum.GL_VIEWPORT, out lastViewport);
            Int4 lastScissorBox;
            GL.GetIntegerv4(GL.GlEnum.GL_SCISSOR_BOX, out lastScissorBox);

            GL.PushAttrib(GL.GlEnum.GL_ENABLE_BIT | GL.GlEnum.GL_COLOR_BUFFER_BIT | GL.GlEnum.GL_TRANSFORM_BIT);
            GL.Enable(GL.GlEnum.GL_BLEND);
            GL.BlendFunc(GL.GlEnum.GL_SRC_ALPHA, GL.GlEnum.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GlEnum.GL_CULL_FACE);
            GL.Disable(GL.GlEnum.GL_DEPTH_TEST);
            GL.Enable(GL.GlEnum.GL_SCISSOR_TEST);
            GL.EnableClientState(GL.GlEnum.GL_VERTEX_ARRAY);
            GL.EnableClientState(GL.GlEnum.GL_TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.GlEnum.GL_COLOR_ARRAY);
            GL.Enable(GL.GlEnum.GL_TEXTURE_2D);

            GL.UseProgram((uint) lastProgram);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            ImGuiIOPtr io = ImGui.GetIO();
            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.Viewport(0, 0, displayW, displayH);
            GL.MatrixMode(GL.GlEnum.GL_PROJECTION);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f, -1.0f,
                1.0f
            );
            GL.MatrixMode(GL.GlEnum.GL_MODELVIEW);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists
            for (int n = 0; n < drawData.CmdListsCount; n++) {
                ImDrawListPtr cmdList = new ImDrawListPtr(drawData.NativePtr -> CmdLists[n]);
                var vtxBuffer = cmdList.VtxBuffer;
                var idxBuffer = cmdList.IdxBuffer;

                GL.VertexPointer(2, GL.GlEnum.GL_FLOAT, Unsafe.SizeOf<ImDrawVert>(),
                    IntPtr.Add(vtxBuffer.Data, Marshal.OffsetOf<ImDrawVert>("pos").ToInt32()));
                GL.TexCoordPointer(2, GL.GlEnum.GL_FLOAT, Unsafe.SizeOf<ImDrawVert>(),
                    IntPtr.Add(vtxBuffer.Data, Marshal.OffsetOf<ImDrawVert>("uv").ToInt32()));
                GL.ColorPointer(4, GL.GlEnum.GL_UNSIGNED_BYTE, Unsafe.SizeOf<ImDrawVert>(),
                    IntPtr.Add(vtxBuffer.Data, Marshal.OffsetOf<ImDrawVert>("col").ToInt32()));

                long idxBufferOffset = 0;
                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++) {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdi];
                    if (pcmd.UserCallback != IntPtr.Zero) {
                        // TODO: pcmd.UserCallback.Invoke(ref cmdList, ref pcmd);
                        throw new NotImplementedException();
                    } else {
                        GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, (int) pcmd.TextureId);
                        GL.Scissor(
                            (int) pcmd.ClipRect.X,
                            (int) (io.DisplaySize.Y - pcmd.ClipRect.W),
                            (int) (pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (int) (pcmd.ClipRect.W - pcmd.ClipRect.Y)
                        );
                        GL.DrawElements(GL.GlEnum.GL_TRIANGLES, (int) pcmd.ElemCount, GL.GlEnum.GL_UNSIGNED_SHORT, new IntPtr((long) idxBuffer.Data + idxBufferOffset));
                    }
                    idxBufferOffset += pcmd.ElemCount * 2 /*sizeof(ushort)*/ ;
                }
            }

            // Restore modified state
            GL.DisableClientState(GL.GlEnum.GL_COLOR_ARRAY);
            GL.DisableClientState(GL.GlEnum.GL_TEXTURE_COORD_ARRAY);
            GL.DisableClientState(GL.GlEnum.GL_VERTEX_ARRAY);
            GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, lastTexture);
            GL.MatrixMode(GL.GlEnum.GL_MODELVIEW);
            GL.PopMatrix();
            GL.MatrixMode(GL.GlEnum.GL_PROJECTION);
            GL.PopMatrix();
            GL.PopAttrib();
            GL.Viewport(lastViewport.X, lastViewport.Y, lastViewport.Z, lastViewport.W);
            GL.Scissor(lastScissorBox.X, lastScissorBox.Y, lastScissorBox.Z, lastScissorBox.W);
        }

        public static bool EventHandler(SDL.SDL_Event e) {
            ImGuiIOPtr io = ImGui.GetIO();
            switch (e.type) {

                #region MouseEvent

                case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0)
                        io.MouseWheel = 1;
                    if (e.wheel.y < 0)
                        io.MouseWheel = -1;
                    return true;

                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button == SDL.SDL_BUTTON_LEFT)
                        io.MouseDown[0] = true;
                    if (e.button.button == SDL.SDL_BUTTON_RIGHT)
                        io.MouseDown[1] = true;
                    if (e.button.button == SDL.SDL_BUTTON_MIDDLE)
                        io.MouseDown[2] = true;
                    return true;

                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    if (e.button.button == SDL.SDL_BUTTON_LEFT)
                        io.MouseDown[0] = false;
                    if (e.button.button == SDL.SDL_BUTTON_RIGHT)
                        io.MouseDown[1] = false;
                    if (e.button.button == SDL.SDL_BUTTON_MIDDLE)
                        io.MouseDown[2] = false;
                    return true;

                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    io.MousePos = new Vector2(e.motion.x, e.motion.y);
                    return true;

                    #endregion MouseEvent

                    #region KeyEvent
                case SDL.SDL_EventType.SDL_TEXTINPUT:
                    unsafe {
                        // THIS IS THE ONLY UNSAFE THING LEFT!
                        var str = Marshal.PtrToStringUTF8(new IntPtr(e.text.text));
                        io.AddInputCharactersUTF8(str);
                    }
                    return true;

                case SDL.SDL_EventType.SDL_KEYDOWN:
                case SDL.SDL_EventType.SDL_KEYUP:
                    int key = (int) e.key.keysym.sym & ~SDL.SDLK_SCANCODE_MASK;
                    io.KeysDown[key] = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                    SDL.SDL_Keymod keyModState = SDL.SDL_GetModState();
                    io.KeyShift = (keyModState & SDL.SDL_Keymod.KMOD_SHIFT) != 0;
                    io.KeyCtrl = (keyModState & SDL.SDL_Keymod.KMOD_CTRL) != 0;
                    io.KeyAlt = (keyModState & SDL.SDL_Keymod.KMOD_ALT) != 0;
                    io.KeySuper = (keyModState & SDL.SDL_Keymod.KMOD_GUI) != 0;
                    break;

                    #endregion KeyEvent

                    #region GamepadEvent

                case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    io.BackendFlags |= ImGuiBackendFlags.HasGamepad;
                    IsGamepadEnable = true;
                    break;

                case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                    io.BackendFlags &= ~ImGuiBackendFlags.HasGamepad;
                    IsGamepadEnable = false;
                    break;

                    #endregion GamepadEvent
            }
            return true;
        }

        #region GamePad

        /// <summary>
        /// Config if using gamepad
        /// </summary>
        public static bool IsGamepadEnable {
            get;
            set;
        } = false;

        private static IntPtr g_Controller = IntPtr.Zero;

        /// <summary>
        /// SDL_gamecontroller.h suggests using this value.
        /// </summary>
        private const int thumb_dead_zone = 8000;

        public static void InitGamepads() {
            var io = ImGui.GetIO();

            if ((io.ConfigFlags | ImGuiConfigFlags.NavEnableGamepad) == 0) {
                CloseGamepads();
                return;
            }

            if (g_Controller == IntPtr.Zero) {
                g_Controller = SDL.SDL_GameControllerOpen(0);
            }
            io.NavActive = true;
        }

        public static void CloseGamepads() {
            var io = ImGui.GetIO();
            if (g_Controller != IntPtr.Zero)
                SDL.SDL_GameControllerClose(g_Controller);
        }

        public static void UpdateGamepads() {
            if (IsGamepadEnable) {
                InitGamepads();
                MAP_BUTTON((int) ImGuiNavInput.Activate, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A); // Cross / A
                MAP_BUTTON((int) ImGuiNavInput.Cancel, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B); // Circle / B
                MAP_BUTTON((int) ImGuiNavInput.Menu, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X); // Square / X
                MAP_BUTTON((int) ImGuiNavInput.Input, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y); // Triangle / Y
                MAP_BUTTON((int) ImGuiNavInput.DpadLeft, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT); // D-Pad Left
                MAP_BUTTON((int) ImGuiNavInput.DpadRight, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT); // D-Pad Right
                MAP_BUTTON((int) ImGuiNavInput.DpadUp, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP); // D-Pad Up
                MAP_BUTTON((int) ImGuiNavInput.DpadDown, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN); // D-Pad Down
                MAP_BUTTON((int) ImGuiNavInput.FocusPrev, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER); // L1 / LB
                MAP_BUTTON((int) ImGuiNavInput.FocusNext, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER); // R1 / RB
                MAP_BUTTON((int) ImGuiNavInput.TweakSlow, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER); // L1 / LB
                MAP_BUTTON((int) ImGuiNavInput.TweakFast, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER); // R1 / RB
                MAP_ANALOG((int) ImGuiNavInput.LStickLeft, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, -thumb_dead_zone, -32768);
                MAP_ANALOG((int) ImGuiNavInput.LStickRight, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, +thumb_dead_zone, +32767);
                MAP_ANALOG((int) ImGuiNavInput.LStickUp, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, -thumb_dead_zone, -32767);
                MAP_ANALOG((int) ImGuiNavInput.LStickDown, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY, +thumb_dead_zone, +32767);
            } else {
                CloseGamepads();
                NavClear();
            }
        }

        private static void NavClear() {
            var io = ImGui.GetIO();
            for (int i = 0; i < io.NavInputs.Count; ++i) {
                io.NavInputs[i] = 0;
            }
            io.NavActive = false;
        }

        private static void MAP_BUTTON(int nav, SDL.SDL_GameControllerButton button) {
            var io = ImGui.GetIO();
            io.NavInputs[nav] = (SDL.SDL_GameControllerGetButton(g_Controller, button) != 0) ? 1.0f : 0.0f;
        }

        private static void MAP_ANALOG(int nav, SDL.SDL_GameControllerAxis axis, int v0, int v1) {
            var io = ImGui.GetIO();
            float vn = (float) (SDL.SDL_GameControllerGetAxis(g_Controller, axis) - v0) / (float) (v1 - v0);
            if (vn > 1.0f)
                vn = 1.0f;
            if (vn > 0.0f && io.NavInputs[nav] < vn)
                io.NavInputs[nav] = vn;
        }

        #endregion GamePad
    }
}