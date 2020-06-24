using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using PerfCopy;

// Ported fom http://www.github.com/ocornut/imgui_club

// - v0.36: minor tweaks, minor refactor.
//
// Todo/Bugs:
// - Arrows are being sent to the InputText() about to disappear which for LeftArrow makes the text cursor appear at position 1 for one frame.
// - Using InputText() is awkward and maybe overkill here, consider implementing something custom.

namespace ImGuiNET.Extensions {

    unsafe public class MemoryEditor {

        enum DataFormat {
            Bin = 0,
            Dec = 1,
            Hex = 2,
            COUNT
        };

        // Settings
        /// set to false when DrawWindow() was closed. ignore if not using DrawWindow().
        public bool Open;                                       // = true
        /// disable any editing.
        public bool ReadOnly;                                   // = false
        /// number of columns to display.
        public int Cols;                                        // = 16
        /// display options button/context menu. when disabled, options will be locked unless you provide your own UI for them.
        public bool OptShowOptions;                             // = true
        /// display a footer previewing the decimal/binary/hex/float representation of the currently selected bytes.
        public bool OptShowDataPreview;                         // = false
        /// display values in HexII representation instead of regular hexadecimal: hide null/zero bytes, ascii values as ".X".
        public bool OptShowHexII;                               // = false
        /// display ASCII representation on the right side.
        public bool OptShowAscii;                               // = true
        /// display null/zero bytes using the TextDisabled color.
        public bool OptGreyOutZeroes;                           // = true
        /// display hexadecimal values as "FF" instead of "ff".
        public bool OptUpperCaseHex;                            // = true
        /// set to 0 to disable extra spacing between every mid-cols.
        public int OptMidColsCount;                             // = 8
        /// number of addr digits to display (default calculated based on maximum displayed addr).
        public int OptAddrDigitsCount;                          // = 0
        /// background color of highlighted bytes.
        public UInt32 HighlightColor;                           //
        /// optional handler to read bytes.
        public delegate byte ReadMethod(byte[] data, int off);       // = 0
        public ReadMethod ReadFn;
        /// optional handler to write bytes.
        public delegate void WriteMethod(byte[] data, int off, byte d);      // = null
        public WriteMethod WriteFn;
        /// optional handler to return Highlight property (to support non-contiguous highlighting).
        public delegate bool HighlightMethod(byte[] data, int off);          // = null
        public HighlightMethod HighlightFn;

        // [Internal State]
        bool ContentsWidthChanged;
        int DataPreviewAddr;
        int DataEditingAddr;
        bool DataEditingTakeFocus;
        byte[] DataInputBuf = new byte[32];
        byte[] AddrInputBuf = new byte[32];
        int GotoAddr;
        int HighlightMin, HighlightMax;
        int PreviewEndianess;
        ImGuiDataType PreviewDataType;

        public MemoryEditor() {
            // Settings
            Open = true;
            ReadOnly = false;
            Cols = 16;
            OptShowOptions = true;
            OptShowDataPreview = false;
            OptShowHexII = false;
            OptShowAscii = true;
            OptGreyOutZeroes = true;
            OptUpperCaseHex = true;
            OptMidColsCount = 8;
            OptAddrDigitsCount = 0;
            HighlightColor = ImGui.GetColorU32(new Vector4(1.0f, 1.0f, 1.0f, 50.0f / 255));
            ReadFn = null;
            WriteFn = null;
            HighlightFn = null;

            // State/Internals
            ContentsWidthChanged = false;
            DataPreviewAddr = DataEditingAddr = (int)-1;
            DataEditingTakeFocus = false;
            for (int i = 0; i < DataInputBuf.Length; ++i) {
                DataInputBuf[i] = 0;
            }
            for (int i = 0; i < AddrInputBuf.Length; ++i) {
                AddrInputBuf[i] = 0;
            }
            GotoAddr = (int)-1;
            HighlightMin = HighlightMax = (int)-1;
            PreviewEndianess = 0;
            PreviewDataType = ImGuiDataType.S32;
        }

        // Standalone Memory Editor window
        public void DrawWindow(string title, byte[] mem_data, int mem_size, int base_display_addr = 0x0000) {
            Sizes s = new Sizes();
            CalcSizes(ref s, mem_size, base_display_addr);
            ImGui.SetNextWindowSizeConstraints(new Vector2(0.0f, 0.0f), new Vector2(s.WindowWidth, float.MaxValue));

            Open = true;
            if (ImGui.Begin(title, ref Open, ImGuiWindowFlags.NoScrollbar)) {
                if (ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows) && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    ImGui.OpenPopup("context");
                DrawContents(mem_data, mem_size, base_display_addr);
                if (ContentsWidthChanged) {
                    CalcSizes(ref s, mem_size, base_display_addr);
                    ImGui.SetWindowSize(new Vector2(s.WindowWidth, ImGui.GetWindowSize().Y));
                }
            }
            ImGui.End();
        }

        // Memory Editor contents only
        public void DrawContents(byte[] mem_data_void, int mem_size, int base_display_addr = 0x0000) {
            if (Cols < 1)
                Cols = 1;

            byte[] mem_data = mem_data_void;
            Sizes s = new Sizes();
            CalcSizes(ref s, mem_size, base_display_addr);
            ImGuiStylePtr style = ImGui.GetStyle();

            // We begin into our scrolling region with the 'ImGuiWindowFlags_NoMove' in order to prevent click from moving the window.
            // This is used as a facility since our main click detection code doesn't assign an ActiveId so the click would normally be caught as a window-move.
            float height_separator = style.ItemSpacing.Y;
            float footer_height = 0;
            if (OptShowOptions)
                footer_height += height_separator + ImGui.GetFrameHeightWithSpacing() * 1;
            if (OptShowDataPreview)
                footer_height += height_separator + ImGui.GetFrameHeightWithSpacing() * 1 + ImGui.GetTextLineHeightWithSpacing() * 3;
            ImGui.BeginChild("##scrolling", new Vector2(0, -footer_height), false, ImGuiWindowFlags.NoMove);
            ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

            int line_total_count = (int)((mem_size + Cols - 1) / Cols);
            ImGuiListClipper _clipper = new ImGuiListClipper();
            ImGuiListClipperPtr clipper = new ImGuiListClipperPtr(&_clipper);
            clipper.Begin(line_total_count, s.LineHeight);
            
            int visible_start_addr = clipper.DisplayStart * Cols;
            int visible_end_addr = clipper.DisplayEnd * Cols;

            bool data_next = false;

            if (ReadOnly || DataEditingAddr >= mem_size)
                DataEditingAddr = (int)-1;
            if (DataPreviewAddr >= mem_size)
                DataPreviewAddr = (int)-1;

            int preview_data_type_size = OptShowDataPreview ? DataTypeGetSize(PreviewDataType) : 0;

            int data_editing_addr_backup = DataEditingAddr;
            int data_editing_addr_next = (int)-1;
            if (DataEditingAddr != (int)-1) {
                // Move cursor but only apply on next frame so scrolling with be synchronized (because currently we can't change the scrolling while the window is being rendered)
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.UpArrow)) && DataEditingAddr >= (int)Cols) {
                    data_editing_addr_next = DataEditingAddr - Cols;
                    DataEditingTakeFocus = true;
                } else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.DownArrow)) && DataEditingAddr < mem_size - Cols) {
                    data_editing_addr_next = DataEditingAddr + Cols;
                    DataEditingTakeFocus = true;
                } else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.LeftArrow)) && DataEditingAddr > 0) {
                    data_editing_addr_next = DataEditingAddr - 1;
                    DataEditingTakeFocus = true;
                } else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.RightArrow)) && DataEditingAddr < mem_size - 1) {
                    data_editing_addr_next = DataEditingAddr + 1;
                    DataEditingTakeFocus = true;
                }
            }
            if (data_editing_addr_next != (int)-1 && (data_editing_addr_next / Cols) != (data_editing_addr_backup / Cols)) {
                // Track cursor movements
                int scroll_offset = ((int)(data_editing_addr_next / Cols) - (int)(data_editing_addr_backup / Cols));
                bool scroll_desired = (scroll_offset < 0 && data_editing_addr_next < visible_start_addr + Cols * 2) || (scroll_offset > 0 && data_editing_addr_next > visible_end_addr - Cols * 2);
                if (scroll_desired)
                    ImGui.SetScrollY(ImGui.GetScrollY() + scroll_offset * s.LineHeight);
            }


            // Draw vertical separator
            Vector2 window_pos = ImGui.GetWindowPos();
            if (OptShowAscii) draw_list.AddLine(new Vector2(window_pos.X + s.PosAsciiStart - s.GlyphWidth, window_pos.Y), new Vector2(window_pos.X + s.PosAsciiStart - s.GlyphWidth, window_pos.Y + 9999), ImGui.GetColorU32(ImGuiCol.Border));

            UInt32 color_text = ImGui.GetColorU32(ImGuiCol.Text);
            UInt32 color_disabled = OptGreyOutZeroes ? ImGui.GetColorU32(ImGuiCol.TextDisabled) : color_text;

            //string format_address = OptUpperCaseHex ? $"{{0:X{s.AddrDigitsCount}}}" : $"{{0:x{s.AddrDigitsCount}}}";
            //string format_data = OptUpperCaseHex ? $"{{0:X{s.AddrDigitsCount}}}" : $"{{0:x{s.AddrDigitsCount}}}";
            //string format_byte = OptUpperCaseHex ? "{0:X2} " : "{0:x2} ";
            //string format_byte_space = OptUpperCaseHex ? "{0:X2} " : "{0:x2} ";

            for (int line_i = clipper.DisplayStart; line_i < clipper.DisplayEnd; line_i++) // display only visible lines
            {
                int addr = (int)(line_i * Cols);
                ImGui.Text(FixedHex(base_display_addr + addr, s.AddrDigitsCount) + ": ");

                // Draw Hexadecimal
                for (int n = 0; n < Cols && addr < mem_size; n++, addr++) {
                    float byte_pos_x = s.PosHexStart + s.HexCellWidth * n;
                    if (OptMidColsCount > 0)
                        byte_pos_x += (float)(n / OptMidColsCount) * s.SpacingBetweenMidCols;
                    ImGui.SameLine(byte_pos_x);

                    // Draw highlight
                    bool is_highlight_from_user_range = (addr >= HighlightMin && addr < HighlightMax);
                    bool is_highlight_from_user_func = (HighlightFn != null && HighlightFn(mem_data, addr));
                    bool is_highlight_from_preview = (addr >= DataPreviewAddr && addr < DataPreviewAddr + preview_data_type_size);
                    if (is_highlight_from_user_range || is_highlight_from_user_func || is_highlight_from_preview) {
                        Vector2 pos = ImGui.GetCursorScreenPos();
                        float highlight_width = s.GlyphWidth * 2;
                        bool is_next_byte_highlighted = (addr + 1 < mem_size) && ((HighlightMax != (int)-1 && addr + 1 < HighlightMax) || (HighlightFn != null && HighlightFn(mem_data, addr + 1)));
                        if (is_next_byte_highlighted || (n + 1 == Cols)) {
                            highlight_width = s.HexCellWidth;
                            if (OptMidColsCount > 0 && n > 0 && (n + 1) < Cols && ((n + 1) % OptMidColsCount) == 0)
                                highlight_width += s.SpacingBetweenMidCols;
                        }
                        draw_list.AddRectFilled(pos, new Vector2(pos.X + highlight_width, pos.Y + s.LineHeight), HighlightColor);
                    }

                    if (DataEditingAddr == addr) {
                        // Display text input on current byte
                        bool data_write = false;
                        ImGui.PushID((IntPtr)addr);
                        if (DataEditingTakeFocus) {
                            ImGui.SetKeyboardFocusHere();
                            ImGui.CaptureKeyboardFromApp(true);

                            //var add = string.Format(format_data, base_display_addr + addr).ToCharArray();
                            //add.DeepCopy(0, AddrInputBuf, 0, add.Length);
                            //var dat = string.Format(format_byte, ).ToCharArray();
                            //dat.DeepCopy(0, DataInputBuf, 0, dat.Length);
                            ReplaceChars(DataInputBuf, FixedHex(ReadFn != null ? ReadFn(mem_data, addr) : mem_data[addr], 2));
                            ReplaceChars(AddrInputBuf, FixedHex(base_display_addr + addr, s.AddrDigitsCount));
                        }
                        ImGui.PushItemWidth(s.GlyphWidth * 2);

                        UserData user_data = new UserData();
                        user_data.CursorPos = -1;

                        // TODO: check it (YTom)
                        var buf = FixedHex(ReadFn != null ? ReadFn(mem_data, addr) : mem_data[addr], 2).ToCharArray();
                        user_data.CurrentBufOverwrite = buf[0];
                        user_data.CurrentBufOverwrite = buf[1];

                        ImGuiInputTextFlags flags = ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.NoHorizontalScroll | ImGuiInputTextFlags.AlwaysInsertMode | ImGuiInputTextFlags.CallbackAlways;
                        if (ImGui.InputText("##data", DataInputBuf, 32, flags, Callback, (IntPtr)(&user_data)))
                            data_write = data_next = true;
                        else if (!DataEditingTakeFocus && !ImGui.IsItemActive())
                            DataEditingAddr = data_editing_addr_next = (int)-1;
                        DataEditingTakeFocus = false;
                        ImGui.PopItemWidth();
                        if (user_data.CursorPos >= 2)
                            data_write = data_next = true;
                        if (data_editing_addr_next != (int)-1)
                            data_write = data_next = false;
                        
                        int data_input_value = 0;
                        if (data_write && TryHexParse(DataInputBuf, out data_input_value)) {
                            if (WriteFn != null)
                                WriteFn(mem_data, addr, (byte)data_input_value);
                            else
                                mem_data[addr] = (byte)data_input_value;
                        }
                        ImGui.PopID();
                    } else {
                        // NB: The trailing space is not visible but ensure there's no gap that the mouse cannot click on.
                        byte b = ReadFn != null ? ReadFn(mem_data, addr) : mem_data[addr];

                        if (OptShowHexII) {
                            if ((b >= 32 && b < 128))
                                ImGui.Text(string.Format("{0} ", (char)b));
                            else if (b == 0xFF && OptGreyOutZeroes)
                                ImGui.TextDisabled("## ");
                            else if (b == 0x00)
                                ImGui.Text("   ");
                            else
                                ImGui.Text(FixedHex(b, 2));
                        } else {
                            if (b == 0 && OptGreyOutZeroes)
                                ImGui.TextDisabled("00 ");
                            else
                                ImGui.Text(FixedHex(b, 2));
                        }
                        if (!ReadOnly && ImGui.IsItemHovered() && ImGui.IsMouseClicked(0)) {
                            DataEditingTakeFocus = true;
                            data_editing_addr_next = addr;
                        }
                    }
                }

                if (OptShowAscii) {
                    // Draw ASCII values
                    ImGui.SameLine(s.PosAsciiStart);
                    Vector2 pos = ImGui.GetCursorScreenPos();
                    addr = line_i * Cols;
                    ImGui.PushID(line_i);
                    if (ImGui.InvisibleButton("ascii", new Vector2(s.PosAsciiEnd - s.PosAsciiStart, s.LineHeight))) {
                        DataEditingAddr = DataPreviewAddr = addr + (int)((ImGui.GetIO().MousePos.X - pos.X) / s.GlyphWidth);
                        DataEditingTakeFocus = true;
                    }
                    ImGui.PopID();
                    for (int n = 0; n < Cols && addr < mem_size; n++, addr++) {
                        if (addr == DataEditingAddr) {
                            draw_list.AddRectFilled(pos, new Vector2(pos.X + s.GlyphWidth, pos.Y + s.LineHeight), ImGui.GetColorU32(ImGuiCol.FrameBg));
                            draw_list.AddRectFilled(pos, new Vector2(pos.X + s.GlyphWidth, pos.Y + s.LineHeight), ImGui.GetColorU32(ImGuiCol.TextSelectedBg));
                        }
                        byte c = ReadFn != null ? ReadFn(mem_data, addr) : mem_data[addr];
                        char display_c = (c < 32 || c >= 128) ? '.' : (char)c;
                        draw_list.AddText(pos, (display_c == '.') ? color_disabled : color_text, new string(display_c, 1));
                        pos.X += s.GlyphWidth;
                    }
                }
            }
            clipper.End();
            ImGui.PopStyleVar(2);
            ImGui.EndChild();

            if (data_next && DataEditingAddr < mem_size) {
                DataEditingAddr = DataPreviewAddr = DataEditingAddr + 1;
                DataEditingTakeFocus = true;
            } else if (data_editing_addr_next != (int)-1) {
                DataEditingAddr = DataPreviewAddr = data_editing_addr_next;
            }

            bool lock_show_data_preview = OptShowDataPreview;
            if (OptShowOptions) {
                ImGui.Separator();
                DrawOptionsLine(ref s, mem_data, mem_size, base_display_addr);
            }

            if (lock_show_data_preview) {
                ImGui.Separator();
                DrawPreviewLine(ref s, mem_data, mem_size, base_display_addr);
            }

            // Notify the main window of our ideal child content size (FIXME: we are missing an API to get the contents size from the child)
            ImGui.SetCursorPosX(s.WindowWidth);
        }

        public void DrawOptionsLine(ref Sizes s, byte[] mem_data, int mem_size, int base_display_addr) {
            //IM_UNUSED(mem_data);
            ImGuiStylePtr style = ImGui.GetStyle();
            string format_range = OptUpperCaseHex ?
                $"Range {{0:X{s.AddrDigitsCount}}}..{{1:X{s.AddrDigitsCount}}}:" :
                $"Range {{0:x{s.AddrDigitsCount}}}..{{1:x{s.AddrDigitsCount}}}:";

            // Options menu
            if (ImGui.Button("Options"))
                ImGui.OpenPopup("context");
            if (ImGui.BeginPopup("context")) {
                ImGui.PushItemWidth(56);
                if (ImGui.DragInt("##cols", ref Cols, 0.2f, 4, 32, "%d cols")) {
                    ContentsWidthChanged = true;
                    if (Cols < 1)
                        Cols = 1;
                }
                ImGui.PopItemWidth();
                ImGui.Checkbox("Show Data Preview", ref OptShowDataPreview);
                ImGui.Checkbox("Show HexII", ref OptShowHexII);
                if (ImGui.Checkbox("Show Ascii", ref OptShowAscii)) {
                    ContentsWidthChanged = true;
                }
                ImGui.Checkbox("Grey out zeroes", ref OptGreyOutZeroes);
                ImGui.Checkbox("Uppercase Hex", ref OptUpperCaseHex);

                ImGui.EndPopup();
            }

            ImGui.SameLine();
            ImGui.Text(string.Format(format_range, base_display_addr, s.AddrDigitsCount, base_display_addr + mem_size - 1));
            ImGui.SameLine();
            ImGui.PushItemWidth((s.AddrDigitsCount + 1) * s.GlyphWidth + style.FramePadding.X * 2.0f);
            if (ImGui.InputText("##addr", (byte[])AddrInputBuf, 32, ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.EnterReturnsTrue)) {
                int goto_addr;
                if (TryHexParse(AddrInputBuf, out goto_addr)) {
                    GotoAddr = goto_addr - base_display_addr;
                    HighlightMin = HighlightMax = (int)-1;
                }
            }
            ImGui.PopItemWidth();

            if (GotoAddr != (int)-1) {
                if (GotoAddr < mem_size) {
                    ImGui.BeginChild("##scrolling");
                    ImGui.SetScrollFromPosY(ImGui.GetCursorStartPos().Y + (GotoAddr / Cols) * ImGui.GetTextLineHeight());
                    ImGui.EndChild();
                    DataEditingAddr = DataPreviewAddr = GotoAddr;
                    DataEditingTakeFocus = true;
                }
                GotoAddr = (int)-1;
            }
        }

        public void DrawPreviewLine(ref Sizes s, byte[] mem_data_void, int mem_size, int base_display_addr) {
            //IM_UNUSED(base_display_addr);
            byte[] mem_data = mem_data_void;
            ImGuiStylePtr style = ImGui.GetStyle();
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Preview as:");
            ImGui.SameLine();
            ImGui.PushItemWidth((s.GlyphWidth * 10.0f) + style.FramePadding.X * 2.0f + style.ItemInnerSpacing.X);
            if (ImGui.BeginCombo("##combo_type", DataTypeGetDesc(PreviewDataType), ImGuiComboFlags.HeightLargest)) {
                for (int n = 0; n < (int)ImGuiDataType.COUNT; n++)
                    if (ImGui.Selectable(DataTypeGetDesc((ImGuiDataType)n), (int)PreviewDataType == n))
                        PreviewDataType = (ImGuiDataType)n;
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            ImGui.SameLine();
            ImGui.PushItemWidth((s.GlyphWidth * 6.0f) + style.FramePadding.X * 2.0f + style.ItemInnerSpacing.X);
            ImGui.Combo("##combo_endianess", ref PreviewEndianess, "LE\0BE\0\0");
            ImGui.PopItemWidth();

            string buf = "";
            float x = s.GlyphWidth * 6.0f;
            bool has_value = DataPreviewAddr != (int)-1;
            if (has_value)
                DrawPreviewData(DataPreviewAddr, mem_data, mem_size, PreviewDataType, DataFormat.Dec, out buf);
            ImGui.Text("Dec");
            ImGui.SameLine(x);
            ImGui.TextUnformatted(has_value ? buf : "N/A");
            if (has_value)
                DrawPreviewData(DataPreviewAddr, mem_data, mem_size, PreviewDataType, DataFormat.Hex, out buf);
            ImGui.Text("Hex");
            ImGui.SameLine(x);
            ImGui.TextUnformatted(has_value ? buf : "N/A");
            if (has_value)
                DrawPreviewData(DataPreviewAddr, mem_data, mem_size, PreviewDataType, DataFormat.Bin, out buf);
            //buf[IM_ARRAYSIZE(buf) - 1] = 0;
            ImGui.Text("Bin");
            ImGui.SameLine(x);
            ImGui.TextUnformatted(has_value ? buf : "N/A");
        }



        #region Utilities for Data Preview
        protected unsafe struct UserData {
            public char CurrentBufOverwrite;                // Input
            public char CurrentBufOverwrite_2;
            public char CurrentBufOverwrite_3;
            public int CursorPos;                            // Output
        };

        public class Sizes {
            public int AddrDigitsCount = 0;
            public float LineHeight = 0;
            public float GlyphWidth = 0;
            public float HexCellWidth = 0;
            public float SpacingBetweenMidCols = 0;
            public float PosHexStart = 0;
            public float PosHexEnd = 0;
            public float PosAsciiStart = 0;
            public float PosAsciiEnd = 0;
            public float WindowWidth = 0;
        };

        void GotoAddrAndHighlight(int addr_min, int addr_max) {
            GotoAddr = addr_min;
            HighlightMin = addr_min;
            HighlightMax = addr_max;
        }

        void CalcSizes(ref Sizes s, int mem_size, int base_display_addr) {
            ImGuiStylePtr style = ImGui.GetStyle();
            s.AddrDigitsCount = OptAddrDigitsCount;
            if (s.AddrDigitsCount == 0)
                for (int n = base_display_addr + mem_size - 1; n > 0; n >>= 4)
                    s.AddrDigitsCount++;
            s.LineHeight = ImGui.GetTextLineHeight();
            s.GlyphWidth = ImGui.CalcTextSize("F").X + 1;                   // We assume the font is mono-space
            s.HexCellWidth = (float)(int)(s.GlyphWidth * 2.5f);             // "FF " we include trailing space in the width to easily catch clicks everywhere
            s.SpacingBetweenMidCols = (float)(int)(s.HexCellWidth * 0.25f); // Every OptMidColsCount columns we add a bit of extra spacing
            s.PosHexStart = (s.AddrDigitsCount + 2) * s.GlyphWidth;
            s.PosHexEnd = s.PosHexStart + (s.HexCellWidth * Cols);
            s.PosAsciiStart = s.PosAsciiEnd = s.PosHexEnd;
            if (OptShowAscii) {
                s.PosAsciiStart = s.PosHexEnd + s.GlyphWidth * 1;
                if (OptMidColsCount > 0)
                    s.PosAsciiStart += (float)((Cols + OptMidColsCount - 1) / OptMidColsCount) * s.SpacingBetweenMidCols;
                s.PosAsciiEnd = s.PosAsciiStart + Cols * s.GlyphWidth;
            }
            s.WindowWidth = s.PosAsciiEnd + style.ScrollbarSize + style.WindowPadding.X * 2 + s.GlyphWidth;
        }

        // FIXME: We should have a way to retrieve the text edit cursor position more easily in the API, this is rather tedious. This is such a ugly mess we may be better off not using InputText() at all here.
        public static int Callback(ImGuiInputTextCallbackData* userdata) {
            var data = new ImGuiInputTextCallbackDataPtr(userdata);
            UserData* user_data = (UserData*)(data.UserData);
            if (!data.HasSelection())
                user_data->CursorPos = data.CursorPos;
            if (data.SelectionStart == 0 && data.SelectionEnd == data.BufTextLen) {
                /// When not editing a byte, always rewrite its content (this is a bit tricky, since InputText technically "owns" the master copy of the buffer we edit it in there)
                data.DeleteChars(0, data.BufTextLen);
                data.InsertChars(0, new string(&user_data->CurrentBufOverwrite));
                data.SelectionStart = 0;
                data.SelectionEnd = data.CursorPos = 2;
            }
            return 0;
        }

        static readonly string[] datatypedescs = { "Int8", "Uint8", "Int16", "Uint16", "Int32", "Uint32", "Int64", "Uint64", "Float", "Double" };
        string DataTypeGetDesc(ImGuiDataType data_type) {
            Debug.Assert(data_type >= 0 && data_type < ImGuiDataType.COUNT);
            return datatypedescs[(int)data_type];
        }

        static readonly int[] sizes = new int[] { 1, 1, 2, 2, 4, 4, 8, 8, sizeof(float), sizeof(double) };
        int DataTypeGetSize(ImGuiDataType data_type) {
            Debug.Assert(data_type >= 0 && data_type < ImGuiDataType.COUNT);
            return sizes[(int)data_type];
        }

        static readonly string[] dataformatdescs = { "Bin", "Dec", "Hex" };
        string DataFormatGetDesc(DataFormat data_format) {
            Debug.Assert(data_format >= 0 && data_format < DataFormat.COUNT);
            return dataformatdescs[(int)data_format];
        }

        bool IsBigEndian() {
            byte[] c = BitConverter.GetBytes((UInt16)1);
            return c[0] != 0;
        }

        static byte[] EndianessCopyBigEndian(byte[] _dst, byte[] _src, int s, int is_little_endian) {
            if (is_little_endian > 0) {
                for (int i = 0, n = (int)s; i < n; ++i, --s) {
                    _dst[i] = _src[s - 1];
                }
            } else {
                _src.DeepCopy(0, _dst, 0, s);
            }
            return _dst;
        }

        static byte[] EndianessCopyLittleEndian(byte[] _dst, byte[] _src, int s, int is_little_endian) {
            if (is_little_endian > 0) {
                _src.DeepCopy(0, _dst, 0, s);
            } else {
                for (int i = 0, n = (int)s; i < n; ++i, --s) {
                    _dst[i] = _src[s - 1];
                }
            }
            return _dst;
        }

        static Func<byte[], byte[], int, int, byte[]> endianesscopyfp = null;
        byte[] EndianessCopy(byte[] dst, byte[] src, int size) {
            if (endianesscopyfp == null)
                if (IsBigEndian())
                    endianesscopyfp = EndianessCopyBigEndian;
                else
                    endianesscopyfp = EndianessCopyLittleEndian;
            return endianesscopyfp(dst, src, size, PreviewEndianess);
        }

        char[] FormatBinary(byte[] buf, int width) {
            Debug.Assert(width <= 64);
            int out_n = 0;
            char[] out_buf = new char[64 + 8 + 1];
            int n = width / 8;
            for (int j = n - 1; j >= 0; --j) {
                for (int i = 0; i < 8; ++i)
                    out_buf[out_n++] = (buf[j] & (1 << (7 - i))) != 0 ? '1' : '0';
                out_buf[out_n++] = ' ';
            }
            Debug.Assert(out_n < out_buf.Length);
            out_buf[out_n] = '\0';
            return out_buf;
        }

        // [Internal]
        void DrawPreviewData(int addr, byte[] mem_data, int mem_size, ImGuiDataType data_type, DataFormat data_format, out string out_buf) {
            byte[] buf = new byte[8];
            int elem_size = DataTypeGetSize(data_type);
            int size = addr + elem_size > mem_size ? mem_size - addr : elem_size;
            if (ReadFn != null)
                for (int i = 0, n = (int)size; i < n; ++i)
                    buf[i] = ReadFn(mem_data, addr + i);
            else
                mem_data.DeepCopy(addr, buf, 0, size);

            if (data_format == DataFormat.Bin) {
                byte[] binbuf = new byte[8];
                EndianessCopy(binbuf, buf, size);
                out_buf = new string(FormatBinary(binbuf, (int)size * 8));
                return;
            }

            out_buf = "\0";
            byte[] _b = new byte[size];
            EndianessCopy(_b, buf, size);
            switch (data_type) {
                case ImGuiDataType.S8: {
                        sbyte int8 = (sbyte)_b[0];
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", int8);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", int8 & 0xFF);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.U8: {
                        byte uint8 = _b[0];
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", uint8);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", uint8 & 0XFF);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.S16: {
                        Int16 int16 = BitConverter.ToInt16(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", int16);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", int16 & 0xFFFF);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.U16: {
                        UInt16 uint16 = BitConverter.ToUInt16(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", uint16);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", uint16 & 0xFFFF);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.S32: {
                        Int32 int32 = BitConverter.ToInt32(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", int32);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", int32);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.U32: {
                        UInt32 uint32 = BitConverter.ToUInt32(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", uint32);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", uint32);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.S64: {
                        Int64 int64 = BitConverter.ToInt64(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", (long)int64);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", (long)int64);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.U64: {
                        UInt64 uint64 = BitConverter.ToUInt64(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", (long)uint64);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = string.Format("{0:X}", (long)uint64);
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.Float: {
                        float float32 = BitConverter.ToSingle(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", float32);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = BitConverter.SingleToInt32Bits(float32).ToString("X");
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.Double: {
                        double float64 = BitConverter.ToDouble(_b);
                        if (data_format == DataFormat.Dec) {
                            out_buf = string.Format("{0}", float64);
                            return;
                        }
                        if (data_format == DataFormat.Hex) {
                            out_buf = BitConverter.DoubleToInt64Bits(float64).ToString("X");
                            return;
                        }
                        break;
                    }
                case ImGuiDataType.COUNT:
                    break;
            } // Switch
            Debug.Assert(false); // Shouldn't reach
        }

        private string FixedHex(int v, int count) {
            return OptUpperCaseHex ? v.ToString("X").PadLeft(count, '0') : v.ToString("x").PadLeft(count, '0');
        }

        private bool TryHexParse(byte[] bytes, out int result) {
            string input = System.Text.Encoding.UTF8.GetString(bytes).ToString();
            return int.TryParse(input, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out result);
        }

        private void ReplaceChars(byte[] bytes, string input) {
            var address = System.Text.Encoding.ASCII.GetBytes(input);
            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] = (i < address.Length) ? address[i] : (byte)0;
            }
        }

        #endregion
    }
}