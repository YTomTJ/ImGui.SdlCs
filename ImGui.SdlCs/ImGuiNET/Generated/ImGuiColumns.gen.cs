using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiColumns
    {
        public uint ID;
        public ImGuiColumnsFlags Flags;
        public byte IsFirstFrame;
        public byte IsBeingResized;
        public int Current;
        public int Count;
        public float OffMinX;
        public float OffMaxX;
        public float LineMinY;
        public float LineMaxY;
        public float HostCursorPosY;
        public float HostCursorMaxPosX;
        public ImRect HostClipRect;
        public ImRect HostWorkRect;
        public ImVector Columns;
        public ImDrawListSplitter Splitter;
    }
    public unsafe partial struct ImGuiColumnsPtr
    {
        public ImGuiColumns* NativePtr { get; }
        public ImGuiColumnsPtr(ImGuiColumns* nativePtr) => NativePtr = nativePtr;
        public ImGuiColumnsPtr(IntPtr nativePtr) => NativePtr = (ImGuiColumns*)nativePtr;
        public static implicit operator ImGuiColumnsPtr(ImGuiColumns* nativePtr) => new ImGuiColumnsPtr(nativePtr);
        public static implicit operator ImGuiColumns* (ImGuiColumnsPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiColumnsPtr(IntPtr nativePtr) => new ImGuiColumnsPtr(nativePtr);
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ref ImGuiColumnsFlags Flags => ref Unsafe.AsRef<ImGuiColumnsFlags>(&NativePtr->Flags);
        public ref bool IsFirstFrame => ref Unsafe.AsRef<bool>(&NativePtr->IsFirstFrame);
        public ref bool IsBeingResized => ref Unsafe.AsRef<bool>(&NativePtr->IsBeingResized);
        public ref int Current => ref Unsafe.AsRef<int>(&NativePtr->Current);
        public ref int Count => ref Unsafe.AsRef<int>(&NativePtr->Count);
        public ref float OffMinX => ref Unsafe.AsRef<float>(&NativePtr->OffMinX);
        public ref float OffMaxX => ref Unsafe.AsRef<float>(&NativePtr->OffMaxX);
        public ref float LineMinY => ref Unsafe.AsRef<float>(&NativePtr->LineMinY);
        public ref float LineMaxY => ref Unsafe.AsRef<float>(&NativePtr->LineMaxY);
        public ref float HostCursorPosY => ref Unsafe.AsRef<float>(&NativePtr->HostCursorPosY);
        public ref float HostCursorMaxPosX => ref Unsafe.AsRef<float>(&NativePtr->HostCursorMaxPosX);
        public ref ImRect HostClipRect => ref Unsafe.AsRef<ImRect>(&NativePtr->HostClipRect);
        public ref ImRect HostWorkRect => ref Unsafe.AsRef<ImRect>(&NativePtr->HostWorkRect);
        public ImPtrVector<ImGuiColumnDataPtr> Columns => new ImPtrVector<ImGuiColumnDataPtr>(NativePtr->Columns, Unsafe.SizeOf<ImGuiColumnData>());
        public ref ImDrawListSplitter Splitter => ref Unsafe.AsRef<ImDrawListSplitter>(&NativePtr->Splitter);
        public void Clear()
        {
            ImGuiNative.ImGuiColumns_Clear(NativePtr);
        }
        public void Destroy()
        {
            ImGuiNative.ImGuiColumns_destroy(NativePtr);
        }
    }
}
