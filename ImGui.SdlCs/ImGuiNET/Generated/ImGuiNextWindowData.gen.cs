using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiNextWindowData
    {
        public ImGuiNextWindowDataFlags Flags;
        public ImGuiCond PosCond;
        public ImGuiCond SizeCond;
        public ImGuiCond CollapsedCond;
        public Vector2 PosVal;
        public Vector2 PosPivotVal;
        public Vector2 SizeVal;
        public Vector2 ContentSizeVal;
        public byte CollapsedVal;
        public ImRect SizeConstraintRect;
        public IntPtr SizeCallback;
        public void* SizeCallbackUserData;
        public float BgAlphaVal;
        public Vector2 MenuBarOffsetMinVal;
    }
    public unsafe partial struct ImGuiNextWindowDataPtr
    {
        public ImGuiNextWindowData* NativePtr { get; }
        public ImGuiNextWindowDataPtr(ImGuiNextWindowData* nativePtr) => NativePtr = nativePtr;
        public ImGuiNextWindowDataPtr(IntPtr nativePtr) => NativePtr = (ImGuiNextWindowData*)nativePtr;
        public static implicit operator ImGuiNextWindowDataPtr(ImGuiNextWindowData* nativePtr) => new ImGuiNextWindowDataPtr(nativePtr);
        public static implicit operator ImGuiNextWindowData* (ImGuiNextWindowDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiNextWindowDataPtr(IntPtr nativePtr) => new ImGuiNextWindowDataPtr(nativePtr);
        public ref ImGuiNextWindowDataFlags Flags => ref Unsafe.AsRef<ImGuiNextWindowDataFlags>(&NativePtr->Flags);
        public ref ImGuiCond PosCond => ref Unsafe.AsRef<ImGuiCond>(&NativePtr->PosCond);
        public ref ImGuiCond SizeCond => ref Unsafe.AsRef<ImGuiCond>(&NativePtr->SizeCond);
        public ref ImGuiCond CollapsedCond => ref Unsafe.AsRef<ImGuiCond>(&NativePtr->CollapsedCond);
        public ref Vector2 PosVal => ref Unsafe.AsRef<Vector2>(&NativePtr->PosVal);
        public ref Vector2 PosPivotVal => ref Unsafe.AsRef<Vector2>(&NativePtr->PosPivotVal);
        public ref Vector2 SizeVal => ref Unsafe.AsRef<Vector2>(&NativePtr->SizeVal);
        public ref Vector2 ContentSizeVal => ref Unsafe.AsRef<Vector2>(&NativePtr->ContentSizeVal);
        public ref bool CollapsedVal => ref Unsafe.AsRef<bool>(&NativePtr->CollapsedVal);
        public ref ImRect SizeConstraintRect => ref Unsafe.AsRef<ImRect>(&NativePtr->SizeConstraintRect);
        public ref IntPtr SizeCallback => ref Unsafe.AsRef<IntPtr>(&NativePtr->SizeCallback);
        public IntPtr SizeCallbackUserData { get => (IntPtr)NativePtr->SizeCallbackUserData; set => NativePtr->SizeCallbackUserData = (void*)value; }
        public ref float BgAlphaVal => ref Unsafe.AsRef<float>(&NativePtr->BgAlphaVal);
        public ref Vector2 MenuBarOffsetMinVal => ref Unsafe.AsRef<Vector2>(&NativePtr->MenuBarOffsetMinVal);
        public void ClearFlags()
        {
            ImGuiNative.ImGuiNextWindowData_ClearFlags(NativePtr);
        }
        public void Destroy()
        {
            ImGuiNative.ImGuiNextWindowData_destroy(NativePtr);
        }
    }
}
