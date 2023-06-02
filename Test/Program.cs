using System.Numerics;
using ImGuiNET.SDL2CS;

namespace ImGuiNET {
    unsafe class Program {

        public class TestWindow : ImWindowExt {

            //ImCanvas canvas = new ImCanvas("CA");

            public TestWindow() : base(Mode.Layout) {
                BackgroundColor = new Vector4(0.4f, 0.5f, 0.6f, 1.0f);
                mAction = delegate() {
                    ImGui.ShowDemoWindow();
                    //canvas.update();
                    return true;
                };
            }
        }

        static void Main(string[] args) {
            /*ImGuiSDL2CSWindow*/
            new TestWindow().Run();
        }
    }
}