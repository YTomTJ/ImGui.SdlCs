using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImBoolVector
    {
        public ImVector Storage;
    }
    public unsafe partial struct ImBoolVectorPtr
    {
        public ImBoolVector* NativePtr { get; }
        public ImBoolVectorPtr(ImBoolVector* nativePtr) => NativePtr = nativePtr;
        public ImBoolVectorPtr(IntPtr nativePtr) => NativePtr = (ImBoolVector*)nativePtr;
        public static implicit operator ImBoolVectorPtr(ImBoolVector* nativePtr) => new ImBoolVectorPtr(nativePtr);
        public static implicit operator ImBoolVector* (ImBoolVectorPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImBoolVectorPtr(IntPtr nativePtr) => new ImBoolVectorPtr(nativePtr);
        public ImVector<int> Storage => new ImVector<int>(NativePtr->Storage);
    }
}
