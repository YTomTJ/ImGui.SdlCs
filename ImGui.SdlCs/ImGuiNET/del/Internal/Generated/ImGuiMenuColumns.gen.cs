using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiMenuColumns
    {
        public float Spacing;
        public float Width;
        public float NextWidth;
        public fixed float Pos[3];
        public fixed float NextWidths[3];
    }
    public unsafe partial struct ImGuiMenuColumnsPtr
    {
        public ImGuiMenuColumns* NativePtr { get; }
        public ImGuiMenuColumnsPtr(ImGuiMenuColumns* nativePtr) => NativePtr = nativePtr;
        public ImGuiMenuColumnsPtr(IntPtr nativePtr) => NativePtr = (ImGuiMenuColumns*)nativePtr;
        public static implicit operator ImGuiMenuColumnsPtr(ImGuiMenuColumns* nativePtr) => new ImGuiMenuColumnsPtr(nativePtr);
        public static implicit operator ImGuiMenuColumns* (ImGuiMenuColumnsPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiMenuColumnsPtr(IntPtr nativePtr) => new ImGuiMenuColumnsPtr(nativePtr);
        public ref float Spacing => ref Unsafe.AsRef<float>(&NativePtr->Spacing);
        public ref float Width => ref Unsafe.AsRef<float>(&NativePtr->Width);
        public ref float NextWidth => ref Unsafe.AsRef<float>(&NativePtr->NextWidth);
        public RangeAccessor<float> Pos => new RangeAccessor<float>(NativePtr->Pos, 3);
        public RangeAccessor<float> NextWidths => new RangeAccessor<float>(NativePtr->NextWidths, 3);
    }
}
