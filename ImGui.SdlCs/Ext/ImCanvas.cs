using System;
using System.Numerics;
using ImGuiNET;

namespace ImGuiNET.Extensions {
    public class ImCanvas {

        public const float ZOOM_MAX = 2.0f;
        public const float ZOOM_MIN = 0.5f;
        public const float GRID_SIZE = 32;

        public ImCanvas(string label) {
            _name = label;
        }

        /**
         * @brief Render the canvas
         * @param new_window If draw canvas as a new window
         */
        public void update(bool child = false) {

            bool _begin = false;
            if (!child) {
                ImGui.SetNextWindowSize(_canvas_size, ImGuiCond.Once);
                _begin = ImGui.Begin(_name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            } else {
                _begin = ImGui.BeginChild(_name, _canvas_size, false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            }

            ImGuiWindowPtr window = ImGui.GetCurrentWindow();
            var start_pos = window.DC.CursorPos;
            _canvas_size = window.Size;

            if (!window.SkipItems && _begin) {

                uint id = ImGui.GetID(_name);
                ImGuiIOPtr io = ImGui.GetIO();
                // ImGuiStylePtr style = ImGui.GetStyle();
                ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

                ImGui.ItemAdd(window.WorkRect, id);
                ImGui.PushID(_name);
                draw_list.PushClipRect(window.WorkRect.Min, window.WorkRect.Max);

                Vector2 offset = _canvas_offset;
                _canvas_center = (window.WorkRect.Min + window.WorkRect.Max) / 2 + offset;

                // Draw grid
                {
                    var grid_color = ImGui.GetColorU32(ImGuiCol.Separator);
                    var axis_color = ImGui.GetColorU32(ImGuiCol.PlotLinesHovered);

                    Vector2 canvas_start = Coordinate_Win2Canvas(window.WorkRect.Min);
                    Vector2 canvas_end = Coordinate_Win2Canvas(window.WorkRect.Max);

                    draw_list.AddLine(
                        Coordinate_Canvas2Win(new Vector2(0, canvas_start.Y)),
                        Coordinate_Canvas2Win(new Vector2(0, canvas_end.Y)), axis_color);
                    for (float x = _canvas_grid_skip; x <= canvas_end.X; x += _canvas_grid_skip) {
                        draw_list.AddLine(
                            Coordinate_Canvas2Win(new Vector2(x, canvas_start.Y)),
                            Coordinate_Canvas2Win(new Vector2(x, canvas_end.Y)), grid_color);
                    }
                    for (float x = -_canvas_grid_skip; x >= canvas_start.X; x -= _canvas_grid_skip) {
                        draw_list.AddLine(
                            Coordinate_Canvas2Win(new Vector2(x, canvas_start.Y)),
                            Coordinate_Canvas2Win(new Vector2(x, canvas_end.Y)), grid_color);
                    }

                    draw_list.AddLine(
                        Coordinate_Canvas2Win(new Vector2(canvas_start.X, 0)),
                        Coordinate_Canvas2Win(new Vector2(canvas_end.X, 0)), axis_color);
                    for (float y = _canvas_grid_skip; y <= canvas_end.Y; y += _canvas_grid_skip) {
                        draw_list.AddLine(
                            Coordinate_Canvas2Win(new Vector2(canvas_start.X, y)),
                            Coordinate_Canvas2Win(new Vector2(canvas_end.X, y)), grid_color);
                    }
                    for (float y = -_canvas_grid_skip; y >= canvas_start.Y; y -= _canvas_grid_skip) {
                        draw_list.AddLine(
                            Coordinate_Canvas2Win(new Vector2(canvas_start.X, y)),
                            Coordinate_Canvas2Win(new Vector2(canvas_end.X, y)), grid_color);
                    }
                }

                // Scale window
                {
                    ImGui.SetWindowFontScale(_canvas_zoom);

                    // Do something ...
                    draw_list.AddLine(
                        Coordinate_Canvas2Win(new Vector2(0, 0)),
                        Coordinate_Canvas2Win(new Vector2(1, 1)), ImGui.GetColorU32(ImGuiCol.PlotLines));

                    ImGui.SetWindowFontScale(1.0f); // Recover font size
                }

                // Normal scale
                {
                    ImGui.SetCursorScreenPos(start_pos);

                    ImGui.SetNextItemWidth(120);
                    ImGui.SliderFloat("##canvas_zoom", ref _canvas_zoom, ZOOM_MIN, ZOOM_MAX, "Zoom = x%.3f");

                    ImGui.SetNextItemWidth(120);
                    ImGui.SliderFloat("##canvas_gridskip", ref _canvas_grid_skip, 0.1f, 10.0f, "px = %0.2f");
                    ImGui.SameLine();
                    if (ImGui.SmallButton("R")) {
                        _canvas_grid_skip = 1;
                    }

                    //TODO: small thumbnail.
                }

                ImGui.PopID(); // canvas
                draw_list.PopClipRect();

                // Mouse operation
                {
                    // Reaction window mouse operation on no node is active.
                    if (!ImGui.IsMouseDown(0) && ImGui.IsWindowHovered()) {
                        if (ImGui.IsMouseDragging(ImGuiMouseButton.Middle))
                            _canvas_offset += io.MouseDelta;
                        if (io.KeyShift && !io.KeyCtrl)
                            _canvas_offset.X += io.MouseWheel * 16.0f;
                        if (!io.KeyShift && io.KeyCtrl)
                            _canvas_offset.Y += io.MouseWheel * 16.0f;
                        if (!io.KeyShift && !io.KeyCtrl) {
                            if (io.MouseWheel != 0) {
                                _canvas_zoom = Math.Clamp(_canvas_zoom + io.MouseWheel * _canvas_zoom / 16.0f, ZOOM_MIN, ZOOM_MAX);
                            }
                        }

                        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Middle)) {
                            _canvas_zoom = 1.0f; // DbClick MMB to reset canvas view
                            _canvas_offset = new Vector2(0, 0);
                        }
                    }
                }
            }

            if (_begin) {
                if (child) {
                    ImGui.EndChild();
                } else {
                    ImGui.End();
                }
            }
        }

        public Vector2 Coordinate_Win2Canvas(Vector2 win) {
            return (win - _canvas_center.Value) / (GRID_SIZE * _canvas_zoom);
        }

        public Vector2 Coordinate_Canvas2Win(Vector2 canvas) {
            return canvas * GRID_SIZE * _canvas_zoom + _canvas_center.Value;
        }

        private string _name;
        private float _canvas_zoom = 1.0f;
        private Vector2 _canvas_size = new Vector2(640, 480);
        private Vector2 _canvas_offset = new Vector2(0, 0);
        private Vector2? _canvas_center = null;
        private float _canvas_grid_skip = 1.0f;
    }
}