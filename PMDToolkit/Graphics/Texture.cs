﻿/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PMDToolkit.Graphics
{
    public class Texture : IDisposable
    {
        protected static TextureProgram mTextureProgram;
        private Bitmap mBitmap;

        //VBO IDs
        private int mVBOID;

        private int mIBOID;

        //Current pixels
        public BitmapData ImgData { get; set; }

        public int TextureID { get; private set; }
        public int ImageWidth { get { return mBitmap.Width; } }
        public int ImageHeight { get { return mBitmap.Height; } }
        public System.Drawing.Imaging.PixelFormat PixelFormat { get { return mBitmap.PixelFormat; } }

        public static void SetTextureProgram(TextureProgram program)
        {
            //Set class rendering program
            mTextureProgram = program;
        }

        public Texture()
        {
            //Initialize textureID
            TextureID = 0;

            mBitmap = null;
            ImgData = null;

            //Initialize VBO
            mVBOID = 0;
            mIBOID = 0;
        }

        public virtual void Dispose()
        {
            //Free VBO and IBO if needed
            FreeVBO();
            //Free texture data if needed
            FreeTexture();
        }

        ~Texture()
        {
            Dispose();
        }

        public bool LoadPixelsFromFile32(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadPixelsFromBytes(fileStream);
            }
        }

        public bool LoadPixelsFromBytes(Stream stream)
        {
            //Deallocate texture data
            FreeTexture();

            mBitmap = new Bitmap(stream);

            ImgData = mBitmap.LockBits(new Rectangle(0, 0, mBitmap.Width, mBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return true;
        }

        public void LoadTextureFromPixels32()
        {
            //There is loaded pixels
            if (TextureID == 0 && ImgData != null)
            {
                TextureID = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, TextureID);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ImgData.Width, ImgData.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, ImgData.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                ErrorCode error = GL.GetError();

                if (error != 0)
                {
                    throw new Exception("Error loading texture from " + ImgData.ToString() + "! " + error.ToString());
                }
                else
                {
                    //Release pixels

                    mBitmap.UnlockBits(ImgData);
                    ImgData = null;
                }

                //Generate VBO
                InitVBO();
            }
            else
            {
                //Texture already exists
                if (TextureID != 0)
                {
                    throw new Exception("A texture is already loaded!");
                }
                //No pixel loaded
                else if (ImgData == null)
                {
                    throw new Exception("No pixels to create texture from!");
                }
            }
        }

        public void CreatePixels32(int imgWidth, int imgHeight)
        {
            //Valid dimensions
            if (imgWidth > 0 && imgHeight > 0)
            {
                FreeTexture();
                //Get rid of any current texture data
                mBitmap = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                ImgData = mBitmap.LockBits(new Rectangle(0, 0, mBitmap.Width, mBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
        }

        //copyPixels32

        public void CopyPixels(Image img)
        {
            FreeTexture();
            mBitmap = new Bitmap(img);

            ImgData = mBitmap.LockBits(new Rectangle(0, 0, mBitmap.Width, mBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public void Save(MemoryStream stream, ImageFormat format)
        {
            mBitmap.Save(stream, format);
        }

        //blitPixels32

        public unsafe void SetPixel(int px, int py, Color4 color)
        {
            byte* addr = (byte*)(ImgData.Scan0) + ImgData.Stride * py + px * 4;

            *addr = (byte)(color.B * 256);
            *(addr + 1) = (byte)(color.G * 256);
            *(addr + 2) = (byte)(color.R * 256);
            *(addr + 3) = (byte)(color.A * 256);
        }

        public unsafe Color4 GetPixel(int px, int py)
        {
            byte* addr = (byte*)(ImgData.Scan0) + ImgData.Stride * py + px * 4;

            byte b = *addr;
            byte g = *(addr + 1);
            byte r = *(addr + 2);
            byte a = *(addr + 3);
            return new Color4(r, g, b, a);
        }

        public void BlitOn(System.Drawing.Graphics graphics, int destX, int destY)
        {
            graphics.DrawImage(mBitmap, new Point(destX, destY));
        }

        public void Blit(BitmapData source, int srcPx, int srcPy, int srcW, int srcH, int destX, int destY)
        {
            int bpp = 4;

            int targetStartX, targetEndX;
            int targetStartY, targetEndY;
            int copyW, copyH;

            targetStartX = Math.Max(destX, 0);
            targetEndX = Math.Min(destX + srcW, ImgData.Width);

            targetStartY = Math.Max(destY, 0);
            targetEndY = Math.Min(destY + srcH, ImgData.Height);

            copyW = targetEndX - targetStartX;
            copyH = targetEndY - targetStartY;

            if (copyW < 0)
            {
                return;
            }

            if (copyH < 0)
            {
                return;
            }

            int sourceStartX = srcPx + targetStartX - destX;
            int sourceStartY = srcPy + targetStartY - destY;

            unsafe
            {
                byte* sourcePtr = (byte*)(source.Scan0);
                byte* targetPtr = (byte*)(ImgData.Scan0);

                byte* targetY = targetPtr + targetStartY * ImgData.Stride;
                byte* sourceY = sourcePtr + sourceStartY * source.Stride;
                for (int y = 0; y < copyH; y++, targetY += ImgData.Stride, sourceY += source.Stride)
                {
                    byte* targetOffset = targetY + targetStartX * bpp;
                    byte* sourceOffset = sourceY + sourceStartX * bpp;
                    for (int x = 0; x < copyW * bpp; x++, targetOffset++, sourceOffset++)
                        *(targetOffset) = *(sourceOffset);
                }
            }
        }

        public virtual void FreeTexture()
        {
            //Delete texture
            if (TextureID != 0)
            {
                GL.DeleteTexture(TextureID);
                TextureID = 0;
            }
            //Delete pixels
            if (ImgData != null)
            {
                mBitmap.UnlockBits(ImgData);
            }
            ImgData = null;
            if (mBitmap != null)
            {
                mBitmap.Dispose();
            }
            mBitmap = null;
        }

        public void Render(Rectangle? rect)
        {
            if (TextureID != 0)
            {
                //Texture coordinates
                float texTop = 0;
                float texBottom = 1;
                float texLeft = 0;
                float texRight = 1;
                //Vertex coordinates
                float quadWidth = ImageWidth;
                float quadHeight = ImageHeight;

                //Handle clipping
                if (rect != null)
                {
                    Rectangle clip = rect.Value;
                    //Texture coordinates
                    texLeft = (float)clip.Left / ImageWidth;
                    texRight = (float)clip.Right / ImageWidth;
                    texTop = (float)clip.Top / ImageHeight;
                    texBottom = (float)clip.Bottom / ImageHeight;
                    //Vertex coordinates
                    quadWidth = clip.Width;
                    quadHeight = clip.Height;
                }

                //Set vertex data
                VertexData[] vData = new VertexData[4] {
                    new VertexData(0,0,texLeft,texTop),
                    new VertexData(quadWidth,0,texRight,texTop),
                    new VertexData(quadWidth,quadHeight,texRight,texBottom),
                    new VertexData(0,quadHeight,texLeft,texBottom)
                };

                //Set texture ID
                GL.BindTexture(TextureTarget.Texture2D, TextureID);

                //Enable vertex and texture coordinate arrays
                mTextureProgram.EnableVertexPointer();
                mTextureProgram.EnableTexCoordPointer();

                //Bind vertex buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, mVBOID);

                //Update vertex buffer data
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(4 * VertexData.SizeInBytes), vData);

                //Set texture coordinate data
                mTextureProgram.SetTexCoordPointer(VertexData.SizeInBytes, VertexData.TexCoordOffset);
                //Set vertex data
                mTextureProgram.SetVertexPointer(VertexData.SizeInBytes, VertexData.PositionOffset);

                //bind indexes again
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIBOID);

                //Draw quad using vertex data and index data
                GL.DrawElements(BeginMode.Quads, 4, DrawElementsType.UnsignedInt, 0);

                //Disable vertex and texture coordinate arrays
                mTextureProgram.DisableVertexPointer();
                mTextureProgram.DisableTexCoordPointer();
            }
        }

        private void InitVBO()
        {
            //If texture is loaded and VBO does not already exist
            if (TextureID != 0 && mVBOID == 0)
            {
                //Vertex data
                VertexData[] vData = new VertexData[4];
                //LVertexData2D vData[ 4 ];
                //Set rendering indices
                int[] iData = new int[4] { 0, 1, 2, 3 };

                //Create VBO
                GL.GenBuffers(1, out mVBOID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, mVBOID);
                //send vertex buffer data to GPU; data in this address space can be changed
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(4 * VertexData.SizeInBytes), vData, BufferUsageHint.DynamicDraw);
                //Create IBO
                GL.GenBuffers(1, out mIBOID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIBOID);
                //send index buffer data to GPU; data in this address space can be changed
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(4 * sizeof(int)), iData, BufferUsageHint.DynamicDraw);
                //Unbind buffers
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }

        private void FreeVBO()
        {
            //Free VBO and IBO
            if (mVBOID != 0)
            {
                GL.DeleteBuffers(1, ref mVBOID);
                GL.DeleteBuffers(1, ref mIBOID);
                mVBOID = 0;
                mIBOID = 0;
            }
        }
    }
}