using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiItemHoveredDataBackup
    {
        public uint LastItemId;
        public ImGuiItemStatusFlags LastItemStatusFlags;
        public ImRect LastItemRect;
        public ImRect LastItemDisplayRect;
    }
    public unsafe partial struct ImGuiItemHoveredDataBackupPtr
    {
        public ImGuiItemHoveredDataBackup* NativePtr { get; }
        public ImGuiItemHoveredDataBackupPtr(ImGuiItemHoveredDataBackup* nativePtr) => NativePtr = nativePtr;
        public ImGuiItemHoveredDataBackupPtr(IntPtr nativePtr) => NativePtr = (ImGuiItemHoveredDataBackup*)nativePtr;
        public static implicit operator ImGuiItemHoveredDataBackupPtr(ImGuiItemHoveredDataBackup* nativePtr) => new ImGuiItemHoveredDataBackupPtr(nativePtr);
        public static implicit operator ImGuiItemHoveredDataBackup* (ImGuiItemHoveredDataBackupPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiItemHoveredDataBackupPtr(IntPtr nativePtr) => new ImGuiItemHoveredDataBackupPtr(nativePtr);
        public ref uint LastItemId => ref Unsafe.AsRef<uint>(&NativePtr->LastItemId);
        public ref ImGuiItemStatusFlags LastItemStatusFlags => ref Unsafe.AsRef<ImGuiItemStatusFlags>(&NativePtr->LastItemStatusFlags);
        public ref ImRect LastItemRect => ref Unsafe.AsRef<ImRect>(&NativePtr->LastItemRect);
        public ref ImRect LastItemDisplayRect => ref Unsafe.AsRef<ImRect>(&NativePtr->LastItemDisplayRect);
    }
}
