﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Libs.NpcFinder
{
    public sealed class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        private Int32[] bits;
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int TopOffset { get; private set; } = 100;
        public int BottomOffset { get; private set; } = 200;

        private GCHandle BitsHandle { get; set; }

        public Point ToScreenCoordinates(int x, int y)
        {
            return new Point(x, y + this.TopOffset);
        }

        public DirectBitmap(int width, int height, int topOffset, int bottomOffset)
        {
            this.TopOffset = topOffset;
            this.BottomOffset = bottomOffset;
            this.Width = width;
            this.Height = height - TopOffset - BottomOffset;
            this.bits = new Int32[width * height];
            this.BitsHandle = GCHandle.Alloc(bits, GCHandleType.Pinned);
            this.Bitmap = new Bitmap(width, Height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public DirectBitmap(int width, int height)
        {
            this.Width = width;
            this.Height = height - TopOffset - BottomOffset;
            this.bits = new Int32[width * height];
            this.BitsHandle = GCHandle.Alloc(bits, GCHandleType.Pinned);
            this.Bitmap = new Bitmap(width, Height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public string ToBase64(int size)
        {
            int width, height;
            if (Bitmap.Width > Bitmap.Height)
            {
                width = size;
                height = Convert.ToInt32(Bitmap.Height * size / (double)Bitmap.Width);
            }
            else
            {
                width = Convert.ToInt32(Bitmap.Width * size / (double)Bitmap.Height);
                height = size;
            }
            var resized = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(Bitmap, 0, 0, width, height);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                resized.Save(ms, ImageFormat.Jpeg);
                resized.Dispose();
                byte[] byteImage = ms.ToArray();
                return Convert.ToBase64String(byteImage); // Get Base64
            }
        }

        public void CaptureScreen()
        {
            using (var graphics = Graphics.FromImage(Bitmap))
            {
                graphics.CopyFromScreen(0, TopOffset, 0, 0, Bitmap.Size);
            }
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();
            bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}