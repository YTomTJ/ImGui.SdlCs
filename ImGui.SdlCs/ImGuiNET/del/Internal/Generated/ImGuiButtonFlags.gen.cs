namespace ImGuiNET
{
    [System.Flags]
    public enum ImGuiButtonFlags
    {
        None = 0,
        Repeat = 1 << 0,
        PressedOnClickRelease = 1 << 1,
        PressedOnClick = 1 << 2,
        PressedOnRelease = 1 << 3,
        PressedOnDoubleClick = 1 << 4,
        FlattenChildren = 1 << 5,
        AllowItemOverlap = 1 << 6,
        DontClosePopups = 1 << 7,
        Disabled = 1 << 8,
        AlignTextBaseLine = 1 << 9,
        NoKeyModifiers = 1 << 10,
        NoHoldingActiveID = 1 << 11,
        PressedOnDragDropHold = 1 << 12,
        NoNavFocus = 1 << 13,
        NoHoveredOnNav = 1 << 14,
    }
}
