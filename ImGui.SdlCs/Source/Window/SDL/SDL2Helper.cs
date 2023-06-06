using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using SDLCS = SDL2.SDL;

namespace ImGuiExt {

    public static class SDL2Helper {

        public static bool IsInitialized {
            get;
            private set;
        } = false;

        /// <summary>
        /// Alternate for ImGui.GetClipboardText
        /// </summary>
        /// <returns></returns>
        public static string GetClipboardText()
        {
            try {
                return SDLCS.SDL_GetClipboardText();
            } catch(Exception) {
                // TODO: deal with exception
                return null;
            }
        }

        /// <summary>
        /// Alternate for ImGui.SetClipboardText
        /// </summary>
        /// <param name="text"></param>
        public static void SetClipboardText(string text)
        {
            try {
                SDLCS.SDL_SetClipboardText(text);
            } catch(Exception) {
                // TODO: deal with exception
            }
        }

        public static void Initialize()
        {
            if(IsInitialized)
                return;
            IsInitialized = true;

            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)SDLCS.SDL_Keycode.SDLK_TAB;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_LEFT;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_RIGHT;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_UP;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_DOWN;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_PAGEUP;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_PAGEDOWN;
            io.KeyMap[(int)ImGuiKey.Home] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_HOME;
            io.KeyMap[(int)ImGuiKey.End] = (int)SDLCS.SDL_Scancode.SDL_SCANCODE_END;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)SDLCS.SDL_Keycode.SDLK_DELETE;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)SDLCS.SDL_Keycode.SDLK_BACKSPACE;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)SDLCS.SDL_Keycode.SDLK_RETURN;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)SDLCS.SDL_Keycode.SDLK_ESCAPE;
            io.KeyMap[(int)ImGuiKey.A] = (int)SDLCS.SDL_Keycode.SDLK_a;
            io.KeyMap[(int)ImGuiKey.C] = (int)SDLCS.SDL_Keycode.SDLK_c;
            io.KeyMap[(int)ImGuiKey.V] = (int)SDLCS.SDL_Keycode.SDLK_v;
            io.KeyMap[(int)ImGuiKey.X] = (int)SDLCS.SDL_Keycode.SDLK_x;
            io.KeyMap[(int)ImGuiKey.Y] = (int)SDLCS.SDL_Keycode.SDLK_y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)SDLCS.SDL_Keycode.SDLK_z;

            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

            //io.SetClipboardTextFn = SDL2Helper.SetClipboardText;
            //io.GetClipboardTextFn = SDL2Helper.GetClipboardText;
            //io.ClipboardUserData = null;
            //io.SetPlatformImeDataFn = SDL2Helper.SetPlatformImeData;

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            // If no font added, add default font.
            if(io.Fonts.Fonts.Size == 0)
                io.Fonts.AddFontDefault();
        }

        public static void NewFrame(Vector2 size, Vector2 scale, ref double g_Time)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = size;
            io.DisplayFramebufferScale = scale;

            // Setup time step
            UInt32 current_time = SDLCS.SDL_GetTicks();
            var dt = (float)((current_time - g_Time) / 1000.0f);
            io.DeltaTime = dt > 0 ? dt : (float)(1.0f / 60.0f);
            g_Time = current_time;

            SDLCS.SDL_ShowCursor(io.MouseDrawCursor ? 0 : 1);

            SDL2Helper.UpdateGamepads();

            ImGui.NewFrame();
        }

        public static bool EventHandler(SDLCS.SDL_Event e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            switch(e.type) {

                #region MouseEvent

                case SDLCS.SDL_EventType.SDL_MOUSEWHEEL:
                    if(e.wheel.y > 0)
                        io.MouseWheel = 1;
                    if(e.wheel.y < 0)
                        io.MouseWheel = -1;
                    return true;

                case SDLCS.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if(e.button.button == SDLCS.SDL_BUTTON_LEFT)
                        io.MouseDown[0] = true;
                    if(e.button.button == SDLCS.SDL_BUTTON_RIGHT)
                        io.MouseDown[1] = true;
                    if(e.button.button == SDLCS.SDL_BUTTON_MIDDLE)
                        io.MouseDown[2] = true;
                    return true;

                case SDLCS.SDL_EventType.SDL_MOUSEBUTTONUP:
                    if(e.button.button == SDLCS.SDL_BUTTON_LEFT)
                        io.MouseDown[0] = false;
                    if(e.button.button == SDLCS.SDL_BUTTON_RIGHT)
                        io.MouseDown[1] = false;
                    if(e.button.button == SDLCS.SDL_BUTTON_MIDDLE)
                        io.MouseDown[2] = false;
                    return true;

                case SDLCS.SDL_EventType.SDL_MOUSEMOTION:
                    io.MousePos = new Vector2(e.motion.x, e.motion.y);
                    return true;

                #endregion MouseEvent

                #region KeyEvent
                case SDLCS.SDL_EventType.SDL_TEXTINPUT:
                    unsafe {
                        // THIS IS THE ONLY UNSAFE THING LEFT!
                        var str = Marshal.PtrToStringUTF8(new IntPtr(e.text.text));
                        io.AddInputCharactersUTF8(str);
                    }
                    return true;

                case SDLCS.SDL_EventType.SDL_KEYDOWN:
                case SDLCS.SDL_EventType.SDL_KEYUP:
                    int key = (int)e.key.keysym.sym & ~SDLCS.SDLK_SCANCODE_MASK;
                    io.KeysDown[key] = e.type == SDLCS.SDL_EventType.SDL_KEYDOWN;
                    SDLCS.SDL_Keymod keyModState = SDLCS.SDL_GetModState();
                    io.KeyShift = (keyModState & SDLCS.SDL_Keymod.KMOD_SHIFT) != 0;
                    io.KeyCtrl = (keyModState & SDLCS.SDL_Keymod.KMOD_CTRL) != 0;
                    io.KeyAlt = (keyModState & SDLCS.SDL_Keymod.KMOD_ALT) != 0;
                    io.KeySuper = (keyModState & SDLCS.SDL_Keymod.KMOD_GUI) != 0;
                    break;

                #endregion KeyEvent

                #region GamepadEvent

                case SDLCS.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    io.BackendFlags |= ImGuiBackendFlags.HasGamepad;
                    IsGamepadEnable = true;
                    break;

                case SDLCS.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
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

        public static void InitGamepads()
        {
            var io = ImGui.GetIO();

            if((io.ConfigFlags | ImGuiConfigFlags.NavEnableGamepad) == 0) {
                CloseGamepads();
                return;
            }

            if(g_Controller == IntPtr.Zero) {
                g_Controller = SDLCS.SDL_GameControllerOpen(0);
            }
            io.NavActive = true;
        }

        public static void CloseGamepads()
        {
            var io = ImGui.GetIO();
            if(g_Controller != IntPtr.Zero)
                SDLCS.SDL_GameControllerClose(g_Controller);
        }

        public static void UpdateGamepads()
        {
            if(IsGamepadEnable) {
                InitGamepads();
                MAP_BUTTON((int)ImGuiNavInput.Activate, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A); // Cross / A
                MAP_BUTTON((int)ImGuiNavInput.Cancel, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B); // Circle / B
                MAP_BUTTON((int)ImGuiNavInput.Menu, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X); // Square / X
                MAP_BUTTON((int)ImGuiNavInput.Input, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y); // Triangle / Y
                MAP_BUTTON((int)ImGuiNavInput.DpadLeft, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT); // D-Pad Left
                MAP_BUTTON((int)ImGuiNavInput.DpadRight, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT); // D-Pad Right
                MAP_BUTTON((int)ImGuiNavInput.DpadUp, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP); // D-Pad Up
                MAP_BUTTON((int)ImGuiNavInput.DpadDown, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN); // D-Pad Down
                MAP_BUTTON((int)ImGuiNavInput.FocusPrev, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER); // L1 / LB
                MAP_BUTTON((int)ImGuiNavInput.FocusNext, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER); // R1 / RB
                MAP_BUTTON((int)ImGuiNavInput.TweakSlow, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER); // L1 / LB
                MAP_BUTTON((int)ImGuiNavInput.TweakFast, SDLCS.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER); // R1 / RB
                MAP_ANALOG((int)ImGuiNavInput.LStickLeft, SDLCS.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, -thumb_dead_zone, -32768);
                MAP_ANALOG((int)ImGuiNavInput.LStickRight, SDLCS.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, +thumb_dead_zone, +32767);
                MAP_ANALOG((int)ImGuiNavInput.LStickUp, SDLCS.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX, -thumb_dead_zone, -32767);
                MAP_ANALOG((int)ImGuiNavInput.LStickDown, SDLCS.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY, +thumb_dead_zone, +32767);
            } else {
                CloseGamepads();
                NavClear();
            }
        }

        private static void NavClear()
        {
            var io = ImGui.GetIO();
            for(int i = 0; i < io.NavInputs.Count; ++i) {
                io.NavInputs[i] = 0;
            }
            io.NavActive = false;
        }

        private static void MAP_BUTTON(int nav, SDLCS.SDL_GameControllerButton button)
        {
            var io = ImGui.GetIO();
            io.NavInputs[nav] = (SDLCS.SDL_GameControllerGetButton(g_Controller, button) != 0) ? 1.0f : 0.0f;
        }

        private static void MAP_ANALOG(int nav, SDLCS.SDL_GameControllerAxis axis, int v0, int v1)
        {
            var io = ImGui.GetIO();
            float vn = (float)(SDLCS.SDL_GameControllerGetAxis(g_Controller, axis) - v0) / (float)(v1 - v0);
            if(vn > 1.0f)
                vn = 1.0f;
            if(vn > 0.0f && io.NavInputs[nav] < vn)
                io.NavInputs[nav] = vn;
        }

        #endregion GamePad
    }
}