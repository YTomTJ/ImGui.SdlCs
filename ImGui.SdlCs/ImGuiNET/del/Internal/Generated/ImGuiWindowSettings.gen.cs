using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiWindowSettings
    {
        public uint ID;
        public ImVec2ih Pos;
        public ImVec2ih Size;
        public byte Collapsed;
    }
    public unsafe partial struct ImGuiWindowSettingsPtr
    {
        public ImGuiWindowSettings* NativePtr { get; }
        public ImGuiWindowSettingsPtr(ImGuiWindowSettings* nativePtr) => NativePtr = nativePtr;
        public ImGuiWindowSettingsPtr(IntPtr nativePtr) => NativePtr = (ImGuiWindowSettings*)nativePtr;
        public static implicit operator ImGuiWindowSettingsPtr(ImGuiWindowSettings* nativePtr) => new ImGuiWindowSettingsPtr(nativePtr);
        public static implicit operator ImGuiWindowSettings* (ImGuiWindowSettingsPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiWindowSettingsPtr(IntPtr nativePtr) => new ImGuiWindowSettingsPtr(nativePtr);
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ref ImVec2ih Pos => ref Unsafe.AsRef<ImVec2ih>(&NativePtr->Pos);
        public ref ImVec2ih Size => ref Unsafe.AsRef<ImVec2ih>(&NativePtr->Size);
        public ref bool Collapsed => ref Unsafe.AsRef<bool>(&NativePtr->Collapsed);
    }
}
