using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET {

    public unsafe struct ImPool {
        public ImVector Buf;
        public ImGuiStorage Map;
        public int FreeIdx;
    }

    public unsafe struct ImPool<T> {
        public ImVector<T> Buf;
        public ImGuiStorage Map;
        public int FreeIdx;
    }
}