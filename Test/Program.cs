using System.Numerics;
using ImGuiExt;
using static SDL2.SDL;

namespace ImGuiNET {
    unsafe class Program {

        public class TestWindow : SDL2_SdlRenderer_Window_Ext {

            ImFontPtr FontCJK;

            public TestWindow() : base(Mode.Layout)
            {
                var io = ImGui.GetIO();
                var fonts = io.Fonts;
                FontCJK = fonts.AddFontFromFileTTF("NotoSansMonoCJKsc-Bold.otf", 20, null, fonts.GetGlyphRangesChineseFull());

                BackgroundColor = new Vector4(0.4f, 0.5f, 0.6f, 1.0f);

                OnWindowStart += (w) => {
                    // Reference: https://github.com/ocornut/imgui/issues/307
                    // To input using Microsoft IME, give ImGui the hwnd of your application
                    var viewport = ImGui.GetMainViewport();
                    var wminfo = new SDL_SysWMinfo();
                    SDL_GetWindowWMInfo(Wnd.Window, ref wminfo);
                    viewport.PlatformHandleRaw = wminfo.info.win.window;
                };
                mAction = delegate () {
                    ImGui.PushFont(FontCJK);
                    ImGui.ShowDemoWindow();

                    // Code from imgui showdemowindow.
                    if(ImGui.Begin("UTF-8 Text")) {
                        ImGui.TextWrapped(
                            "CJK text will only appear if the font was loaded with the appropriate CJK character ranges. " +
                            "Call io.Fonts->AddFontFromFileTTF() manually to load extra character ranges. " +
                            "Read docs/FONTS.md for details.");
                        ImGui.Text("Hiragana: かきくけこ (kakikukeko)");
                        ImGui.Text("Kanjis: 日本語 (nihongo)");
                        var buf = new string("日本語");
                        ImGui.InputText("UTF-8 input", ref buf, 32);
                    }

                    ImGui.PopFont();
                    return true;
                };
            }
        }

        static void Main(string[] args)
        {
            /*ImGuiSDL2CSWindow*/
            new TestWindow().Run();
        }
    }
}