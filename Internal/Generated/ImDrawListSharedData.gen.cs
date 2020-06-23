using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImDrawListSharedData
    {
        public Vector2 TexUvWhitePixel;
        public ImFont* Font;
        public float FontSize;
        public float CurveTessellationTol;
        public float CircleSegmentMaxError;
        public Vector4 ClipRectFullscreen;
        public ImDrawListFlags InitialFlags;
        public Vector2 CircleVtx12_0;
        public Vector2 CircleVtx12_1;
        public Vector2 CircleVtx12_2;
        public Vector2 CircleVtx12_3;
        public Vector2 CircleVtx12_4;
        public Vector2 CircleVtx12_5;
        public Vector2 CircleVtx12_6;
        public Vector2 CircleVtx12_7;
        public Vector2 CircleVtx12_8;
        public Vector2 CircleVtx12_9;
        public Vector2 CircleVtx12_10;
        public Vector2 CircleVtx12_11;
        public fixed byte CircleSegmentCounts[64];
    }
    public unsafe partial struct ImDrawListSharedDataPtr
    {
        public ImDrawListSharedData* NativePtr { get; }
        public ImDrawListSharedDataPtr(ImDrawListSharedData* nativePtr) => NativePtr = nativePtr;
        public ImDrawListSharedDataPtr(IntPtr nativePtr) => NativePtr = (ImDrawListSharedData*)nativePtr;
        public static implicit operator ImDrawListSharedDataPtr(ImDrawListSharedData* nativePtr) => new ImDrawListSharedDataPtr(nativePtr);
        public static implicit operator ImDrawListSharedData* (ImDrawListSharedDataPtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImDrawListSharedDataPtr(IntPtr nativePtr) => new ImDrawListSharedDataPtr(nativePtr);
        public ref Vector2 TexUvWhitePixel => ref Unsafe.AsRef<Vector2>(&NativePtr->TexUvWhitePixel);
        public ImFontPtr Font => new ImFontPtr(NativePtr->Font);
        public ref float FontSize => ref Unsafe.AsRef<float>(&NativePtr->FontSize);
        public ref float CurveTessellationTol => ref Unsafe.AsRef<float>(&NativePtr->CurveTessellationTol);
        public ref float CircleSegmentMaxError => ref Unsafe.AsRef<float>(&NativePtr->CircleSegmentMaxError);
        public ref Vector4 ClipRectFullscreen => ref Unsafe.AsRef<Vector4>(&NativePtr->ClipRectFullscreen);
        public ref ImDrawListFlags InitialFlags => ref Unsafe.AsRef<ImDrawListFlags>(&NativePtr->InitialFlags);
        public RangeAccessor<Vector2> CircleVtx12 => new RangeAccessor<Vector2>(&NativePtr->CircleVtx12_0, 12);
        public RangeAccessor<byte> CircleSegmentCounts => new RangeAccessor<byte>(NativePtr->CircleSegmentCounts, 64);
    }
}
