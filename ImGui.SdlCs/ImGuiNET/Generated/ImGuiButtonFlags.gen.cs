namespace ImGuiNET
{
    [System.Flags]
    public enum ImGuiButtonFlags
    {
        None = 0,
        Repeat = 1 << 0,
        PressedOnClick = 1 << 1,
        PressedOnClickRelease = 1 << 2,
        PressedOnClickReleaseAnywhere = 1 << 3,
        PressedOnRelease = 1 << 4,
        PressedOnDoubleClick = 1 << 5,
        PressedOnDragDropHold = 1 << 6,
        FlattenChildren = 1 << 7,
        AllowItemOverlap = 1 << 8,
        DontClosePopups = 1 << 9,
        Disabled = 1 << 10,
        AlignTextBaseLine = 1 << 11,
        NoKeyModifiers = 1 << 12,
        NoHoldingActiveId = 1 << 13,
        NoNavFocus = 1 << 14,
        NoHoveredOnFocus = 1 << 15,
        MouseButtonLeft = 1 << 16,
        MouseButtonRight = 1 << 17,
        MouseButtonMiddle = 1 << 18,
        MouseButtonMask = MouseButtonLeft | MouseButtonRight | MouseButtonMiddle,
        MouseButtonShift = 16,
        MouseButtonDefault = MouseButtonLeft,
        PressedOnMask = PressedOnClick | PressedOnClickRelease | PressedOnClickReleaseAnywhere | PressedOnRelease | PressedOnDoubleClick | PressedOnDragDropHold,
        PressedOnDefault = PressedOnClickRelease,
    }
}
