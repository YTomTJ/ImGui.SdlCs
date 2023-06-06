using SDL2;
using System;
using System.Numerics;

namespace ImGuiExt.SDL {

    public interface IWindow<Event> {

        public IntPtr Window { get; }

        public IntPtr Context { get; }

        public bool IsAlive { get; }

        public bool IsVisible { get; }

        public float MaxFPS { get; set; }

        public string Title { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; set; }

        public Action<SDL2Window> OnStart { get; set; }

        public Action<SDL2Window> OnLoop { get; set; }

        public Action<SDL2Window> OnExit { get; set; }

        public Action<SDL2Window, Event> OnEvent { get; set; }

        public void Show();

        public void Hide();

        public void Run();

        public void Swap();

        public void Exit();

        public void Dispose();
    }
}