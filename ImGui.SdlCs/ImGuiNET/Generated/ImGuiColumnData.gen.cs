using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiColumnData
    {
        public float OffsetNorm;
        public float OffsetNormBeforeResize;
        public ImGuiColumnsFlags Flags;
        public ImRect ClipRect;
    }
    public unsafe partial struct ImGuiColumnDataPtr
    {
        public ImGuiColumnData* NativePtr { get; }
        public ImGuiColumnDataPtr(ImGuiColumnData* nativePtr) => NativePtr = nativePtr;
        public ImGuiColumnDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiColumnData*)nativePtr;
        public static implicit operator ImGuiColumnDataPtr(ImGuiColumnData* nativePtr) => new ImGuiColumnDataPtr(nativePtr);
        public static implicit operator ImGuiColumnData* (ImGuiColumnDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiColumnDataPtr(IntPtr nativePtr) => new ImGuiColumnDataPtr(nativePtr);
        public ref float OffsetNorm => ref Unsafe.AsRef<float>(&NativePtr->OffsetNorm);
        public ref float OffsetNormBeforeResize => ref Unsafe.AsRef<float>(&NativePtr->OffsetNormBeforeResize);
        public ref ImGuiColumnsFlags Flags => ref Unsafe.AsRef<ImGuiColumnsFlags>(&NativePtr->Flags);
        public ref ImRect ClipRect => ref Unsafe.AsRef<ImRect>(&NativePtr->ClipRect);
        public void Destroy()
        {
            ImGuiNative.ImGuiColumnData_destroy(NativePtr);
        }
    }
}
