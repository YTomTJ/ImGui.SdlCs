using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImGuiNET
{
    public unsafe partial struct ImGuiInputTextState
    {
        public uint ID;
        public int CurLenW;
        public int CurLenA;
        public ImVector TextW;
        public ImVector TextA;
        public ImVector InitialTextA;
        public byte TextAIsValid;
        public int BufCapacityA;
        public float ScrollX;
        public STB_TexteditState Stb;
        public float CursorAnim;
        public byte CursorFollow;
        public byte SelectedAllMouseLock;
        public ImGuiInputTextFlags UserFlags;
        public IntPtr UserCallback;
        public void* UserCallbackData;
    }
    public unsafe partial struct ImGuiInputTextStatePtr
    {
        public ImGuiInputTextState* NativePtr { get; }
        public ImGuiInputTextStatePtr(ImGuiInputTextState* nativePtr) => NativePtr = nativePtr;
        public ImGuiInputTextStatePtr(IntPtr nativePtr) => NativePtr = (ImGuiInputTextState*)nativePtr;
        public static implicit operator ImGuiInputTextStatePtr(ImGuiInputTextState* nativePtr) => new ImGuiInputTextStatePtr(nativePtr);
        public static implicit operator ImGuiInputTextState* (ImGuiInputTextStatePtr wrappedPtr) => wrappedPtr.NativePtr;
        public static implicit operator ImGuiInputTextStatePtr(IntPtr nativePtr) => new ImGuiInputTextStatePtr(nativePtr);
        public ref uint ID => ref Unsafe.AsRef<uint>(&NativePtr->ID);
        public ref int CurLenW => ref Unsafe.AsRef<int>(&NativePtr->CurLenW);
        public ref int CurLenA => ref Unsafe.AsRef<int>(&NativePtr->CurLenA);
        public ImVector<ushort> TextW => new ImVector<ushort>(NativePtr->TextW);
        public ImVector<byte> TextA => new ImVector<byte>(NativePtr->TextA);
        public ImVector<byte> InitialTextA => new ImVector<byte>(NativePtr->InitialTextA);
        public ref bool TextAIsValid => ref Unsafe.AsRef<bool>(&NativePtr->TextAIsValid);
        public ref int BufCapacityA => ref Unsafe.AsRef<int>(&NativePtr->BufCapacityA);
        public ref float ScrollX => ref Unsafe.AsRef<float>(&NativePtr->ScrollX);
        public ref STB_TexteditState Stb => ref Unsafe.AsRef<STB_TexteditState>(&NativePtr->Stb);
        public ref float CursorAnim => ref Unsafe.AsRef<float>(&NativePtr->CursorAnim);
        public ref bool CursorFollow => ref Unsafe.AsRef<bool>(&NativePtr->CursorFollow);
        public ref bool SelectedAllMouseLock => ref Unsafe.AsRef<bool>(&NativePtr->SelectedAllMouseLock);
        public ref ImGuiInputTextFlags UserFlags => ref Unsafe.AsRef<ImGuiInputTextFlags>(&NativePtr->UserFlags);
        public ref IntPtr UserCallback => ref Unsafe.AsRef<IntPtr>(&NativePtr->UserCallback);
        public IntPtr UserCallbackData { get => (IntPtr)NativePtr->UserCallbackData; set => NativePtr->UserCallbackData = (void*)value; }
        public void ClearFreeMemory()
        {
            ImGuiNative.ImGuiInputTextState_ClearFreeMemory(NativePtr);
        }
        public void ClearSelection()
        {
            ImGuiNative.ImGuiInputTextState_ClearSelection(NativePtr);
        }
        public void ClearText()
        {
            ImGuiNative.ImGuiInputTextState_ClearText(NativePtr);
        }
        public void CursorAnimReset()
        {
            ImGuiNative.ImGuiInputTextState_CursorAnimReset(NativePtr);
        }
        public void CursorClamp()
        {
            ImGuiNative.ImGuiInputTextState_CursorClamp(NativePtr);
        }
        public void Destroy()
        {
            ImGuiNative.ImGuiInputTextState_destroy(NativePtr);
        }
        public int GetRedoAvailCount()
        {
            int ret = ImGuiNative.ImGuiInputTextState_GetRedoAvailCount(NativePtr);
            return ret;
        }
        public int GetUndoAvailCount()
        {
            int ret = ImGuiNative.ImGuiInputTextState_GetUndoAvailCount(NativePtr);
            return ret;
        }
        public bool HasSelection()
        {
            byte ret = ImGuiNative.ImGuiInputTextState_HasSelection(NativePtr);
            return ret != 0;
        }
        public void OnKeyPressed(int key)
        {
            ImGuiNative.ImGuiInputTextState_OnKeyPressed(NativePtr, key);
        }
        public void SelectAll()
        {
            ImGuiNative.ImGuiInputTextState_SelectAll(NativePtr);
        }
    }
}
