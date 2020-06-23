namespace ImGuiNET
{
    [System.Flags]
    public enum ImGuiItemStatusFlags
    {
        None = 0,
        HoveredRect = 1 << 0,
        HasDisplayRect = 1 << 1,
        Edited = 1 << 2,
        ToggledSelection = 1 << 3,
        ToggledOpen = 1 << 4,
        HasDeactivated = 1 << 5,
        Deactivated = 1 << 6,
        Openable = 1 << 10,
        Opened = 1 << 11,
        Checkable = 1 << 12,
        Checked = 1 << 13,
    }
}
