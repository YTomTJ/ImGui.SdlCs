using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiWindowTempData
    {
        public Vector2 CursorPos;
        public Vector2 CursorPosPrevLine;
        public Vector2 CursorStartPos;
        public Vector2 CursorMaxPos;
        public Vector2 CurrLineSize;
        public Vector2 PrevLineSize;
        public float CurrLineTextBaseOffset;
        public float PrevLineTextBaseOffset;
        public ImVec1 Indent;
        public ImVec1 ColumnsOffset;
        public ImVec1 GroupOffset;
        public uint LastItemId;
        public ImGuiItemStatusFlags LastItemStatusFlags;
        public ImRect LastItemRect;
        public ImRect LastItemDisplayRect;
        public ImGuiNavLayer NavLayerCurrent;
        public int NavLayerCurrentMask;
        public int NavLayerActiveMask;
        public int NavLayerActiveMaskNext;
        public uint NavFocusScopeIdCurrent;
        public byte NavHideHighlightOneFrame;
        public byte NavHasScroll;
        public byte MenuBarAppending;
        public Vector2 MenuBarOffset;
        public ImGuiMenuColumns MenuColumns;
        public int TreeDepth;
        public uint TreeJumpToParentOnPopMask;
        public ImVector ChildWindows;
        public ImGuiStorage* StateStorage;
        public ImGuiColumns* CurrentColumns;
        public ImGuiLayoutType LayoutType;
        public ImGuiLayoutType ParentLayoutType;
        public int FocusCounterRegular;
        public int FocusCounterTabStop;
        public ImGuiItemFlags ItemFlags;
        public float ItemWidth;
        public float TextWrapPos;
        public ImVector ItemFlagsStack;
        public ImVector ItemWidthStack;
        public ImVector TextWrapPosStack;
        public ImVector GroupStack;
        public fixed short StackSizesBackup[6];
    }
    public unsafe partial struct ImGuiWindowTempDataPtr
    {
        public ImGuiWindowTempData* NativePtr { get; }
        public ImGuiWindowTempDataPtr(ImGuiWindowTempData* nativePtr) => NativePtr = nativePtr;
        public ImGuiWindowTempDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiWindowTempData*)nativePtr;
        public static implicit operator ImGuiWindowTempDataPtr(ImGuiWindowTempData* nativePtr) => new ImGuiWindowTempDataPtr(nativePtr);
        public static implicit operator ImGuiWindowTempData* (ImGuiWindowTempDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiWindowTempDataPtr(IntPtr nativePtr) => new ImGuiWindowTempDataPtr(nativePtr);
        public ref Vector2 CursorPos => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorPos);
        public ref Vector2 CursorPosPrevLine => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorPosPrevLine);
        public ref Vector2 CursorStartPos => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorStartPos);
        public ref Vector2 CursorMaxPos => ref Unsafe.AsRef<Vector2>(&NativePtr->CursorMaxPos);
        public ref Vector2 CurrLineSize => ref Unsafe.AsRef<Vector2>(&NativePtr->CurrLineSize);
        public ref Vector2 PrevLineSize => ref Unsafe.AsRef<Vector2>(&NativePtr->PrevLineSize);
        public ref float CurrLineTextBaseOffset => ref Unsafe.AsRef<float>(&NativePtr->CurrLineTextBaseOffset);
        public ref float PrevLineTextBaseOffset => ref Unsafe.AsRef<float>(&NativePtr->PrevLineTextBaseOffset);
        public ref ImVec1 Indent => ref Unsafe.AsRef<ImVec1>(&NativePtr->Indent);
        public ref ImVec1 ColumnsOffset => ref Unsafe.AsRef<ImVec1>(&NativePtr->ColumnsOffset);
        public ref ImVec1 GroupOffset => ref Unsafe.AsRef<ImVec1>(&NativePtr->GroupOffset);
        public ref uint LastItemId => ref Unsafe.AsRef<uint>(&NativePtr->LastItemId);
        public ref ImGuiItemStatusFlags LastItemStatusFlags => ref Unsafe.AsRef<ImGuiItemStatusFlags>(&NativePtr->LastItemStatusFlags);
        public ref ImRect LastItemRect => ref Unsafe.AsRef<ImRect>(&NativePtr->LastItemRect);
        public ref ImRect LastItemDisplayRect => ref Unsafe.AsRef<ImRect>(&NativePtr->LastItemDisplayRect);
        public ref ImGuiNavLayer NavLayerCurrent => ref Unsafe.AsRef<ImGuiNavLayer>(&NativePtr->NavLayerCurrent);
        public ref int NavLayerCurrentMask => ref Unsafe.AsRef<int>(&NativePtr->NavLayerCurrentMask);
        public ref int NavLayerActiveMask => ref Unsafe.AsRef<int>(&NativePtr->NavLayerActiveMask);
        public ref int NavLayerActiveMaskNext => ref Unsafe.AsRef<int>(&NativePtr->NavLayerActiveMaskNext);
        public ref uint NavFocusScopeIdCurrent => ref Unsafe.AsRef<uint>(&NativePtr->NavFocusScopeIdCurrent);
        public ref bool NavHideHighlightOneFrame => ref Unsafe.AsRef<bool>(&NativePtr->NavHideHighlightOneFrame);
        public ref bool NavHasScroll => ref Unsafe.AsRef<bool>(&NativePtr->NavHasScroll);
        public ref bool MenuBarAppending => ref Unsafe.AsRef<bool>(&NativePtr->MenuBarAppending);
        public ref Vector2 MenuBarOffset => ref Unsafe.AsRef<Vector2>(&NativePtr->MenuBarOffset);
        public ref ImGuiMenuColumns MenuColumns => ref Unsafe.AsRef<ImGuiMenuColumns>(&NativePtr->MenuColumns);
        public ref int TreeDepth => ref Unsafe.AsRef<int>(&NativePtr->TreeDepth);
        public ref uint TreeJumpToParentOnPopMask => ref Unsafe.AsRef<uint>(&NativePtr->TreeJumpToParentOnPopMask);
        public ImVector<ImGuiWindowPtr> ChildWindows => new ImVector<ImGuiWindowPtr>(NativePtr->ChildWindows);
        public ImGuiStoragePtr StateStorage => new ImGuiStoragePtr(NativePtr->StateStorage);
        public ImGuiColumnsPtr CurrentColumns => new ImGuiColumnsPtr(NativePtr->CurrentColumns);
        public ref ImGuiLayoutType LayoutType => ref Unsafe.AsRef<ImGuiLayoutType>(&NativePtr->LayoutType);
        public ref ImGuiLayoutType ParentLayoutType => ref Unsafe.AsRef<ImGuiLayoutType>(&NativePtr->ParentLayoutType);
        public ref int FocusCounterRegular => ref Unsafe.AsRef<int>(&NativePtr->FocusCounterRegular);
        public ref int FocusCounterTabStop => ref Unsafe.AsRef<int>(&NativePtr->FocusCounterTabStop);
        public ref ImGuiItemFlags ItemFlags => ref Unsafe.AsRef<ImGuiItemFlags>(&NativePtr->ItemFlags);
        public ref float ItemWidth => ref Unsafe.AsRef<float>(&NativePtr->ItemWidth);
        public ref float TextWrapPos => ref Unsafe.AsRef<float>(&NativePtr->TextWrapPos);
        public ImVector<ImGuiItemFlags> ItemFlagsStack => new ImVector<ImGuiItemFlags>(NativePtr->ItemFlagsStack);
        public ImVector<float> ItemWidthStack => new ImVector<float>(NativePtr->ItemWidthStack);
        public ImVector<float> TextWrapPosStack => new ImVector<float>(NativePtr->TextWrapPosStack);
        public ImPtrVector<ImGuiGroupDataPtr> GroupStack => new ImPtrVector<ImGuiGroupDataPtr>(NativePtr->GroupStack, Unsafe.SizeOf<ImGuiGroupData>());
        public RangeAccessor<short> StackSizesBackup => new RangeAccessor<short>(NativePtr->StackSizesBackup, 6);
        public void Destroy()
        {
            ImGuiNative.ImGuiWindowTempData_destroy(NativePtr);
        }
    }
}
