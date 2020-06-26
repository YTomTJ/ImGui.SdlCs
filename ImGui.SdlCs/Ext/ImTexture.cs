using ImGuiNET.SDL2CS;
using SDL2;
using System;

namespace ImGuiNET.Extensions {

    /// <summary>
    /// A device resource used to store arbitrary image data in a specific format.
    /// See <see cref="TextureDescription"/>.
    /// </summary>
    public class ImTexture {

        /// <summary>
        /// Create texture for RGBA image.
        /// </summary>
        /// <param name="data">Surface data pointer</param>
        /// <param name="size">Data size int byte</param>
        /// <param name="width">Image width in pixels</param>
        /// <param name="height">Image height in pixels</param>
        /// <param name="texture">Exist texture, for not creatinf more texture.</param>
        /// <returns></returns>
        public static IntPtr Convert(IntPtr data, long size, int width, int height, IntPtr texture) {
            if (size != (long)width * height * 4) {
                throw new Exception("Data size not fit image.");
            }

            int tex;
            if (texture == IntPtr.Zero) {
                GL.GenTextures(1, out tex);
            } else {
                tex = (int)texture;
            }
            GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, tex);
            GL.TexParameteri(GL.GlEnum.GL_TEXTURE_2D, GL.GlEnum.GL_TEXTURE_MIN_FILTER, (int)GL.GlEnum.GL_LINEAR);
            GL.TexParameteri(GL.GlEnum.GL_TEXTURE_2D, GL.GlEnum.GL_TEXTURE_MAG_FILTER, (int)GL.GlEnum.GL_LINEAR);
            GL.TexParameteri(GL.GlEnum.GL_TEXTURE_2D, GL.GlEnum.GL_TEXTURE_WRAP_S, (int)GL.GlEnum.GL_REPEAT);
            GL.TexParameteri(GL.GlEnum.GL_TEXTURE_2D, GL.GlEnum.GL_TEXTURE_WRAP_T, (int)GL.GlEnum.GL_REPEAT);
            GL.TexImage2D(GL.GlEnum.GL_TEXTURE_2D, 0, (int)GL.GlEnum.GL_RGBA8, width, height, 0, GL.GlEnum.GL_RGBA, GL.GlEnum.GL_UNSIGNED_BYTE, data);
            GL.BindTexture(GL.GlEnum.GL_TEXTURE_2D, 0);

            return (IntPtr)tex;
        }
    }
}