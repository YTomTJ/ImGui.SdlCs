using SDLCS = SDL2.SDL;

namespace ImGuiExt.SDL {

    public static class SDL2Helper {
        private static bool _Initialized = false;
        public static bool IsInitialized => _Initialized;

        public static void Initialize()
        {
            if(_Initialized)
                return;
            _Initialized = true;

            SDLCS.SDL_Init(SDLCS.SDL_INIT_EVERYTHING);

            SetGLAttributes();
        }

        public static void SetGLAttributes(
            int doubleBuffer = 1,
            int depthSize = 24,
            int stencilSize = 8,
            int majorVersion = 3,
            int minorVersion = 0
        )
        {
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_DOUBLEBUFFER, doubleBuffer);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_DEPTH_SIZE, depthSize);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_STENCIL_SIZE, stencilSize);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, majorVersion);
            SDLCS.SDL_GL_SetAttribute(SDLCS.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, minorVersion);
        }
    }
}