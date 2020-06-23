using ImGuiNET.SDL2CS;
using System.Numerics;

namespace ImGuiNET.Extensions {

    public class TestWindow : ImWindowExt {

        public TestWindow() : base(Mode.Layout) {
            BackgroundColor = new Vector4(0.4f, 0.5f, 0.6f, 1.0f);
            mAction = delegate () {
                ImGui.ShowDemoWindow();
                return true;
            };
        }
    }
}