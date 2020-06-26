using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiTabBar
    {
        public ImVector Tabs;
        public uint ID;
        public uint SelectedTabId;
        public uint NextSelectedTabId;
        public uint VisibleTabId;
        public int CurrFrameVisible;
        public int PrevFrameVisible;
        public ImRect BarRect;
        public float LastTabContentHeight;
        public float OffsetMax;
        public float OffsetMaxIdeal;
        public float OffsetNextTab;
        public float ScrollingAnim;
        public float ScrollingTarget;
        public float ScrollingTargetDistToVisibility;
        public float ScrollingSpeed;
        public ImGuiTabBarFlags Flags;
        public uint ReorderRequestTabId;
        public sbyte ReorderRequestDir;
        public byte WantLayout;
        public byte VisibleTabWasSubmitted;
        public short LastTabItemIdx;
        public Vector2 FramePadding;
        public ImGuiTextBuffer TabsNames;
    }
    public unsafe partial struct ImGuiTabBarPtr
    {
        public ImGuiTabBar* NativePtr { get; }
        public ImGuiTabBarPtr(ImGuiTabBar* nativePtr) => NativePtr = nativePtr;
        public ImGuiTabBarPtr(IntPtr nativePtr) => NativePtr = (ImGuiTabBar*)nativePtr;
        public static implicit operator ImGuiTabBarPtr(ImGuiTabBar* nativePtr) => new ImGuiTabBarPtr(nativePtr);
        public static implicit operator ImGuiTabBar* (ImGuiTabBarPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiTabBarPtr(IntPtr nativePtr) => new ImGuiTabBarPtr(nativePtr);
        public ImPtrVector<ImGuiTabItemPtr> Tabs => new ImPtrVector<ImGuiTabItemPtr>(NativePtr->Tabs, Unsafe.SizeOf<ImGuiTabItem>());
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ref uint SelectedTabId => ref Unsafe.AsRef<uint>(&NativePtr->SelectedTabId);
        public ref uint NextSelectedTabId => ref Unsafe.AsRef<uint>(&NativePtr->NextSelectedTabId);
        public ref uint VisibleTabId => ref Unsafe.AsRef<uint>(&NativePtr->VisibleTabId);
        public ref int CurrFrameVisible => ref Unsafe.AsRef<int>(&NativePtr->CurrFrameVisible);
        public ref int PrevFrameVisible => ref Unsafe.AsRef<int>(&NativePtr->PrevFrameVisible);
        public ref ImRect BarRect => ref Unsafe.AsRef<ImRect>(&NativePtr->BarRect);
        public ref float LastTabContentHeight => ref Unsafe.AsRef<float>(&NativePtr->LastTabContentHeight);
        public ref float OffsetMax => ref Unsafe.AsRef<float>(&NativePtr->OffsetMax);
        public ref float OffsetMaxIdeal => ref Unsafe.AsRef<float>(&NativePtr->OffsetMaxIdeal);
        public ref float OffsetNextTab => ref Unsafe.AsRef<float>(&NativePtr->OffsetNextTab);
        public ref float ScrollingAnim => ref Unsafe.AsRef<float>(&NativePtr->ScrollingAnim);
        public ref float ScrollingTarget => ref Unsafe.AsRef<float>(&NativePtr->ScrollingTarget);
        public ref float ScrollingTargetDistToVisibility => ref Unsafe.AsRef<float>(&NativePtr->ScrollingTargetDistToVisibility);
        public ref float ScrollingSpeed => ref Unsafe.AsRef<float>(&NativePtr->ScrollingSpeed);
        public ref ImGuiTabBarFlags Flags => ref Unsafe.AsRef<ImGuiTabBarFlags>(&NativePtr->Flags);
        public ref uint ReorderRequestTabId => ref Unsafe.AsRef<uint>(&NativePtr->ReorderRequestTabId);
        public ref sbyte ReorderRequestDir => ref Unsafe.AsRef<sbyte>(&NativePtr->ReorderRequestDir);
        public ref bool WantLayout => ref Unsafe.AsRef<bool>(&NativePtr->WantLayout);
        public ref bool VisibleTabWasSubmitted => ref Unsafe.AsRef<bool>(&NativePtr->VisibleTabWasSubmitted);
        public ref short LastTabItemIdx => ref Unsafe.AsRef<short>(&NativePtr->LastTabItemIdx);
        public ref Vector2 FramePadding => ref Unsafe.AsRef<Vector2>(&NativePtr->FramePadding);
        public ref ImGuiTextBuffer TabsNames => ref Unsafe.AsRef<ImGuiTextBuffer>(&NativePtr->TabsNames);
        public void Destroy()
        {
            ImGuiNative.ImGuiTabBar_destroy(NativePtr);
        }
        public string GetTabName(ImGuiTabItemPtr tab)
        {
            ImGuiTabItem* native_tab = tab.NativePtr;
            byte* ret = ImGuiNative.ImGuiTabBar_GetTabName(NativePtr, native_tab);
            return Util.StringFromPtr(ret);
        }
        public int GetTabOrder(ImGuiTabItemPtr tab)
        {
            ImGuiTabItem* native_tab = tab.NativePtr;
            int ret = ImGuiNative.ImGuiTabBar_GetTabOrder(NativePtr, native_tab);
            return ret;
        }
    }
}
