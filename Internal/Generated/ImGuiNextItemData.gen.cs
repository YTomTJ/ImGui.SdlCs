using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiNextItemData
    {
        public ImGuiNextItemDataFlags Flags;
        public float Width;
        public byte OpenVal;
        public ImGuiCond OpenCond;
    }
    public unsafe partial struct ImGuiNextItemDataPtr
    {
        public ImGuiNextItemData* NativePtr { get; }
        public ImGuiNextItemDataPtr(ImGuiNextItemData* nativePtr) => NativePtr = nativePtr;
        public ImGuiNextItemDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiNextItemData*)nativePtr;
        public static implicit operator ImGuiNextItemDataPtr(ImGuiNextItemData* nativePtr) => new ImGuiNextItemDataPtr(nativePtr);
        public static implicit operator ImGuiNextItemData* (ImGuiNextItemDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiNextItemDataPtr(IntPtr nativePtr) => new ImGuiNextItemDataPtr(nativePtr);
        public ref ImGuiNextItemDataFlags Flags => ref Unsafe.AsRef<ImGuiNextItemDataFlags>(&NativePtr->Flags);
        public ref float Width => ref Unsafe.AsRef<float>(&NativePtr->Width);
        public ref bool OpenVal => ref Unsafe.AsRef<bool>(&NativePtr->OpenVal);
        public ref ImGuiCond OpenCond => ref Unsafe.AsRef<ImGuiCond>(&NativePtr->OpenCond);
    }
}
