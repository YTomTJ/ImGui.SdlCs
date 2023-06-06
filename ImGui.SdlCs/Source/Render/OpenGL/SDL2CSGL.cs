using System;
using System.Runtime.InteropServices;
using SDLCS = SDL2.SDL;

namespace ImGuiExt.OpenGL {

    [StructLayout(LayoutKind.Sequential)]
    public struct Int4 {
        public readonly int X, Y, Z, W;
    }

    // Even smaller than MiniTK, only offering the bare minimum required for ImGuiSDL2CS.
    public unsafe static partial class GL {

        private static T _<T>() where T : class
        {
            string name = typeof(T).Name;
            int indexOfSplit = name.IndexOf("__");
            if(indexOfSplit != -1)
                name = name.Substring(0, indexOfSplit);
            IntPtr ptr = SDLCS.SDL_GL_GetProcAddress(name);
            if(ptr == IntPtr.Zero)
                return null;
            return Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
        }

        public delegate IntPtr glGetString(GlEnum pname);

        private static glGetString _GetString = _<glGetString>();

        public static string GetString(GlEnum pname) => new string((sbyte*)_GetString(pname));

        public delegate void glGetIntegerv(GlEnum pname, out int param);

        public static glGetIntegerv GetIntegerv = _<glGetIntegerv>();

        public delegate void glGetIntegerv__4(GlEnum pname, out Int4 param);

        public static glGetIntegerv__4 GetIntegerv4 = _<glGetIntegerv__4>();

        public delegate void glEnable(GlEnum cap);

        public static glEnable Enable = _<glEnable>();

        public delegate void glDisable(GlEnum cap);

        public static glDisable Disable = _<glDisable>();

        public delegate void glViewport(int x, int y, int width, int height);

        public static glViewport Viewport = _<glViewport>();

        public delegate void glPushAttrib(GlEnum mask);

        public static glPushAttrib PushAttrib = _<glPushAttrib>();

        public delegate void glPopAttrib();

        public static glPopAttrib PopAttrib = _<glPopAttrib>();

        public delegate void glBlendFunc(GlEnum src, GlEnum dst);

        public static glBlendFunc BlendFunc = _<glBlendFunc>();

        public delegate void glEnableClientState(GlEnum array);

        public static glEnableClientState EnableClientState = _<glEnableClientState>();

        public delegate void glDisableClientState(GlEnum array);

        public static glDisableClientState DisableClientState = _<glDisableClientState>();

        public delegate void glUseProgram(uint program);

        public static glUseProgram UseProgram = _<glUseProgram>();

        public delegate void glMatrixMode(GlEnum mode);

        public static glMatrixMode MatrixMode = _<glMatrixMode>();

        public delegate void glPushMatrix();

        public static glPushMatrix PushMatrix = _<glPushMatrix>();

        public delegate void glPopMatrix();

        public static glPopMatrix PopMatrix = _<glPopMatrix>();

        public delegate void glLoadIdentity();

        public static glLoadIdentity LoadIdentity = _<glLoadIdentity>();

        public delegate void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);

        public static glOrtho Ortho = _<glOrtho>();

        public delegate void glVertexPointer(int size, GlEnum type, int stride, IntPtr pointer);

        public static glVertexPointer VertexPointer = _<glVertexPointer>();

        public delegate void glTexCoordPointer(int size, GlEnum type, int stride, IntPtr pointer);

        public static glTexCoordPointer TexCoordPointer = _<glTexCoordPointer>();

        public delegate void glColorPointer(int size, GlEnum type, int stride, IntPtr pointer);

        public static glColorPointer ColorPointer = _<glColorPointer>();

        public delegate void glBindTexture(GlEnum target, int texture);

        public static glBindTexture BindTexture = _<glBindTexture>();

        public delegate void glDeleteTexture(int n, IntPtr texture);

        public static glDeleteTexture DeleteTexture = _<glDeleteTexture>();

        public delegate void glScissor(int x, int y, int width, int height);

        public static glScissor Scissor = _<glScissor>();

        public delegate void glDrawElements(GlEnum mode, int count, GlEnum type, IntPtr indices);

        public static glDrawElements DrawElements = _<glDrawElements>();

        public delegate void glClearColor(float r, float g, float b, float a);

        public static glClearColor ClearColor = _<glClearColor>();

        public delegate void glClear(GlEnum mask);

        public static glClear Clear = _<glClear>();

        public delegate void glGenTextures(int n, out int textures);

        public static glGenTextures GenTextures = _<glGenTextures>();

        public delegate void glTexParameteri(GlEnum target, GlEnum pname, int param);

        public static glTexParameteri TexParameteri = _<glTexParameteri>();

        public delegate void glPixelStorei(GlEnum pname, int param);

        public static glPixelStorei PixelStorei = _<glPixelStorei>();

        public delegate void glTexImage2D(GlEnum target, int level, int internalFormat, int width, int height, int border, GlEnum format, GlEnum type, IntPtr pixels);

        public static glTexImage2D TexImage2D = _<glTexImage2D>();

        //public delegate void gl();
        //public static gl  = _<gl>();
    }
}