using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiExt.OpenGL;
using ImGuiNET;

namespace ImGuiExt {

    public static class GLHelper {

        public static unsafe void RenderDrawData(ImDrawDataPtr drawData, int displayW, int displayH) {
            // We are using the OpenGL fixed pipeline to make the example code simpler to read!
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers.
            int lastProgram;
            GL.GetIntegerv(GL.GlEnum.GL_CURRENT_PROGRAM, out lastProgram);
            int lastTexture;
            GL.GetIntegerv(GL.GlEnum.GL_TEXTURE_BINDING_2D, out lastTexture);
            Int4 lastViewport;
            GL.GetIntegerv4(GL.GlEnum.GL_VIEWPORT, out lastViewport);
            Int4 lastScissorBox;
            GL.GetIntegerv4(GL.GlEnum.GL_SCISSOR_BOX, out lastScissorBox);

            GL.PushAttrib(GL.GlEnum.GL_ENABLE_BIT | GL.GlEnum.GL_COLOR_BUFFER_BIT | GL.GlEnum.GL_TRANSFORM_BIT);
            GL.Enable(GL.GlEnum.GL_BLEND);
            GL.BlendFunc(GL.GlEnum.GL_SRC_ALPHA, GL.GlEnum.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GlEnum.GL_CULL_FACE);
            GL.Disable(GL.GlEnum.GL_DEPTH_TEST);
            GL.Enable(GL.GlEnum.GL_SCISSOR_TEST);
            GL.EnableClientState(GL.GlEnum.GL_VERTEX_ARRAY);
            GL.EnableClientState(GL.GlEnum.GL_TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.GlEnum.GL_COLOR_ARRAY);
            GL.Enable(GL.GlEnum.GL_TEXTURE_2D);

            GL.UseProgram((uint) lastProgram);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            ImGuiIOPtr io = ImGui.GetIO();
            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.Viewport(0, 0, displayW, displayH);
            GL.MatrixMode(GL.GlEnum.GL_PROJECTION);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f, -1.0f,
                1.0f
            );
            GL.MatrixMode(GL.GlEnum.GL_MODELVIEW);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists
            for (int n = 0; n < drawData.CmdListsCount; n++) {
                ImDrawListPtr cmdList = new ImDrawListPtr(drawData.NativePtr -> CmdLists[n]);
                var vtxBuffer = cmdList.VtxBuffer;
                var idxBuffer = cmdList.IdxBuffer;

                GL.VertexPointer(2, GL.GlEnum.GL_FLOAT, Unsafe.SizeOf<ImDrawVert>(),
                    IntPtr.Add(vtxBuffer.Data, Marshal.OffsetOf<ImDrawVert>("pos").ToInt32()));
                GL.TexCoordPointer(2, GL.GlEnum.GL_FLOAT, Unsafe.SizeOf<ImDrawVert>(),
                    IntPtr.Add(vtxBuffer.Data, Marshal.OffsetOf<ImDrawVert>("uv").ToInt32()));
                GL.ColorPointer(4, GL.GlEnum.GL_UNSIGNED_BYTE, Unsafe.SizeOf<ImDrawVert>(),
                    IntPtr.Add(vtxBuffer.Data, Marshal.OffsetOf<ImDrawVert>("col").ToInt32()));

                long idxBufferOffset = 0;
                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++) {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdi];
                    if (pcmd.UserCallback != IntPtr.Zero) {
                        // TODO: pcmd.UserCallback.Invoke(ref cmdList, ref pcmd);
                        throw new NotImplementedException();
                    } else {
                        GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, (int) pcmd.TextureId);
                        GL.Scissor(
                            (int) pcmd.ClipRect.X,
                            (int) (io.DisplaySize.Y - pcmd.ClipRect.W),
                            (int) (pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (int) (pcmd.ClipRect.W - pcmd.ClipRect.Y)
                        );
                        GL.DrawElements(GL.GlEnum.GL_TRIANGLES, (int) pcmd.ElemCount, GL.GlEnum.GL_UNSIGNED_SHORT, new IntPtr((long) idxBuffer.Data + idxBufferOffset));
                    }
                    idxBufferOffset += pcmd.ElemCount * 2 /*sizeof(ushort)*/ ;
                }
            }

            // Restore modified state
            GL.DisableClientState(GL.GlEnum.GL_COLOR_ARRAY);
            GL.DisableClientState(GL.GlEnum.GL_TEXTURE_COORD_ARRAY);
            GL.DisableClientState(GL.GlEnum.GL_VERTEX_ARRAY);
            GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, lastTexture);
            GL.MatrixMode(GL.GlEnum.GL_MODELVIEW);
            GL.PopMatrix();
            GL.MatrixMode(GL.GlEnum.GL_PROJECTION);
            GL.PopMatrix();
            GL.PopAttrib();
            GL.Viewport(lastViewport.X, lastViewport.Y, lastViewport.Z, lastViewport.W);
            GL.Scissor(lastScissorBox.X, lastScissorBox.Y, lastScissorBox.Z, lastScissorBox.W);
        }
    }
}