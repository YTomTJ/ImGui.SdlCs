using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiNavMoveResult
    {
        public uint ID;
        public uint SelectScopeId;
        public ImGuiWindow* Window;
        public float DistBox;
        public float DistCenter;
        public float DistAxial;
        public ImRect RectRel;
    }
    public unsafe partial struct ImGuiNavMoveResultPtr
    {
        public ImGuiNavMoveResult* NativePtr { get; }
        public ImGuiNavMoveResultPtr(ImGuiNavMoveResult* nativePtr) => NativePtr = nativePtr;
        public ImGuiNavMoveResultPtr(IntPtr nativePtr) => NativePtr = (ImGuiNavMoveResult*)nativePtr;
        public static implicit operator ImGuiNavMoveResultPtr(ImGuiNavMoveResult* nativePtr) => new ImGuiNavMoveResultPtr(nativePtr);
        public static implicit operator ImGuiNavMoveResult* (ImGuiNavMoveResultPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiNavMoveResultPtr(IntPtr nativePtr) => new ImGuiNavMoveResultPtr(nativePtr);
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ref uint SelectScopeId => ref Unsafe.AsRef<uint>(&NativePtr->SelectScopeId);
        public ImGuiWindowPtr Window => new ImGuiWindowPtr(NativePtr->Window);
        public ref float DistBox => ref Unsafe.AsRef<float>(&NativePtr->DistBox);
        public ref float DistCenter => ref Unsafe.AsRef<float>(&NativePtr->DistCenter);
        public ref float DistAxial => ref Unsafe.AsRef<float>(&NativePtr->DistAxial);
        public ref ImRect RectRel => ref Unsafe.AsRef<ImRect>(&NativePtr->RectRel);
    }
}
