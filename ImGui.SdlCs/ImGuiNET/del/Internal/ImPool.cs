namespace ImGuiNET {

    public unsafe struct ImPool<T> {
        public ImVector Size;
        public ImGuiStorage Map;
        public int FreeIdx;
    }
}