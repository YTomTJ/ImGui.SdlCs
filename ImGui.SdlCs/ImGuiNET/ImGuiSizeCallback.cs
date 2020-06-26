namespace ImGuiNET
{
    public unsafe delegate void ImGuiSizeCallback(ImGuiSizeCallbackData* data);
    public unsafe delegate int ImGuiInputTextCallback(ImGuiInputTextCallbackData* data);
    public unsafe delegate void ImDrawCallback(ImDrawList* parent_list, ImDrawCmd* cmd);

}
