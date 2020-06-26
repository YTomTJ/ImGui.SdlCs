namespace ImGuiNET
{
    [System.Flags]
    public enum ImGuiSelectableFlagsPrivate
    {
        ImGuiSelectableFlags_NoHoldingActiveID = 1 << 20,
        ImGuiSelectableFlags_SelectOnClick = 1 << 21,
        ImGuiSelectableFlags_SelectOnRelease = 1 << 22,
        ImGuiSelectableFlags_SpanAvailWidth = 1 << 23,
        ImGuiSelectableFlags_DrawHoveredWhenHeld = 1 << 24,
        ImGuiSelectableFlags_SetNavIdOnHover = 1 << 25,
    }
}
