// Graphics library for SDL2-CS
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace HHSAdvWin
{
    public class Canvas
    {
        private class Point
        {
            public int x { get; set; }
            public int y { get; set; }
            public Point(int x, int y) { this.x = x; this.y = y; }
        }

        public WriteableBitmap Bitmap { get; private set; }
        private Int32Rect viewport;
        private readonly UInt32[] palette = {
            0xff000000, // black
            0xff0000ff, // blue
            0xffff0000, // red
            0xffff00ff, // magenta
            0xff00ff00, // green
            0xff00ffff, // cyan
            0xffffff00, // yellow
            0xffffffff, // white
        };

        private static readonly float[] blueFilter = {
            0.0F, 0.0F, 0.1F,
            0.0F, 0.0F, 0.2F,
            0.0F, 0.0F, 0.7F,
        };

        private readonly float[] redFilter = {
            0.7F, 0.0F, 0.0F,
            0.2F, 0.0F, 0.0F,
            0.1F, 0.0F, 0.0F,
        };

        private readonly float[] sepiaFilter = {
            0.269021F, 0.527950F, 0.103030F,
            0.209238F, 0.410628F, 0.080135F,
            0.119565F, 0.234644F, 0.045791F,
        };


        public Canvas(Image parent)
        {
            Bitmap = new WriteableBitmap(256, 152, 96, 96, PixelFormats.Pbgra32, null);
            viewport = new Int32Rect { X = 0, Y = 0, Width = 256, Height = 152 };
            parent.Source = Bitmap;
        }
        public void Cls(UInt32 c)
        {
            Bitmap.Lock();
            IntPtr vbuf = Bitmap.BackBuffer;
            unsafe
            {
                for (int y = 0; y < viewport.Height; y++)
                {
                    for (int x = 0; x < viewport.Width; x++)
                    {
                        IntPtr p = vbuf + (y * Bitmap.BackBufferStride) + x * sizeof(UInt32);
                        *((UInt32*)p) = c;
                    }
                }
            }
            Bitmap.AddDirtyRect(viewport);
            Bitmap.Unlock();
        }

        private int GetColorIndex(UInt32 c) =>
            (((c & 0x000000ff) == 0) ? 0 : 1) + (((c & 0x00ff0000) == 0) ? 0 : 2) + (((c & 0x0000ff00) == 0) ? 0 : 4);
        public void Draw()
        {
            Bitmap.Lock();
            Bitmap.AddDirtyRect(viewport);
            Bitmap.Unlock();
        }
        public void Cls()
        {
            Cls(palette[0]);
        }
        public void Invalidate() => Draw();

        public void pset(int x, int y, UInt32 c)
        {
            if (x >= viewport.Width || y >= viewport.Height || x < 0 || y < 0) return;
            Bitmap.Lock();
            IntPtr p = Bitmap.BackBuffer;
            p += y * Bitmap.BackBufferStride + x * sizeof(UInt32);
            unsafe
            {
                *((UInt32*)p) = c;
            }
            Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            Bitmap.Unlock();
        }
        public void pset(int x, int y, int col) => pset(x, y, palette[col]);

        public UInt32 pget(int x, int y)
        {
            if (x >= viewport.Width || y >= viewport.Height || x < 0 || y < 0) return palette[0];
            Bitmap.Lock();
            UInt32 c = palette[0];
            unsafe
            {
                UInt32 *p = (UInt32*)(Bitmap.BackBuffer + (y * Bitmap.BackBufferStride) + x * sizeof(UInt32));
                c = *p;
            }
            Bitmap.Unlock();
            return c;
        }
        public void line(int x1, int y1, int x2, int y2, UInt32 col)
        {
            int dx, ddx, dy, ddy;
            int wx, wy;
            int x, y;
            dy = y2 - y1;
            ddy = 1;
            if (dy < 0)
            {
                dy = -dy;
                ddy = -1;
            }
            wy = dy / 2;
            dx = x2 - x1;
            ddx = 1;
            if (dx < 0)
            {
                dx = -dx;
                ddx = -1;
            }
            wx = dx / 2;
            if (dx == 0 && dy == 0)
            {
                pset(x1, y1, col);
                return;
            }
            if (dy == 0)
            {
                for (x = x1; x != x2; x += ddx) pset(x, y1, col);
                pset(x2, y1, col);
                return;
            }
            if (dx == 0)
            {
                for (y = y1; y != y2; y += ddy) pset(x1, y, col);
                pset(x1, y2, col);
                return;
            }
            pset(x1, y1, col);
            if (dx > dy)
            {
                y = y1;
                for (x = x1; x != x2; x += ddx)
                {
                    pset(x, y, col);
                    wx -= dy;
                    if (wx < 0)
                    {
                        wx += dx;
                        y += ddy;
                    }
                }
            }
            else
            {
                x = x1;
                for (y = y1; y != y2; y += ddy)
                {
                    pset(x, y, col);
                    wy -= dx;
                    if (wy < 0)
                    {
                        wy += dy;
                        x += ddx;
                    }
                }
            }
            pset(x2, y2, col);
        }

        public void line(int x1, int y1, int x2, int y2, int col) => line(x1, y1, x2, y2, palette[col]);

        public void paint(int x, int y, UInt32 f, UInt32 b)
        {
            int l, r;
            int wx;
            Queue<Point> q = new Queue<Point>();
            UInt32 c = pget(x, y);
            if (c.Equals(f) || c.Equals(b))
            {
                return;
            }
            q.Enqueue(new Point(x, y));
            while (q.Count > 0)
            {
                Point p = q.Dequeue();
                c = pget(p.x, p.y);
                if (c.Equals(f) || c.Equals(b)) continue;
                for (l = p.x - 1; l >= 0; l--)
                {
                    c = pget(l, p.y);
                    if (c.Equals(f) || c.Equals(b)) break;
                }
                ++l;
                for (r = p.x + 1; r < viewport.Width; r++)
                {
                    c = pget(r, p.y);
                    if (c.Equals(f) || c.Equals(b)) break;
                }
                --r;
                line(l, p.y, r, p.y, f);
                for (wx = l; wx <= r; wx++)
                {
                    int uy = p.y - 1;
                    if (uy >= 0)
                    {
                        c = pget(wx, uy);
                        if (!c.Equals(f) && !c.Equals(b))
                        {
                            if (wx == r)
                            {
                                q.Enqueue(new Point(wx, uy));
                            }
                            else
                            {
                                c = pget(wx + 1, uy);
                                if (c.Equals(f) || c.Equals(b)) q.Enqueue(new Point(wx, uy));
                            }
                        }
                    }
                    int ly = p.y + 1;
                    if (ly < viewport.Height)
                    {
                        c = pget(wx, ly);
                        if (!c.Equals(f) && !c.Equals(b))
                        {
                            if (wx == r)
                            {
                                q.Enqueue(new Point(wx, ly));
                            }
                            else
                            {
                                c = pget(wx + 1, ly);
                                if (c.Equals(f) || c.Equals(b)) q.Enqueue(new Point(wx, ly));
                            }
                        }
                    }
                }
            }
        }
        public void paint(int x, int y, int fc, int bc)
        {
            paint(x, y, palette[fc], palette[bc]);
        }
        public void tonePaint(byte[] tone, bool tiling = false)
        {
            UInt32[] pat = {
                0xff000000,      // 0 black
                0xff0000ff,      // 1 blue
                0xffff0000,      // 2 red
                0xffff00ff,      // 3 magenta
                0xff00ff00,      // 4 green
                0xff00ffff,      // 5 cyan
                0xffffff00,      // 6 yellow
                0xffffffff,      // 7 white
            };
            UInt32[] col = new UInt32[pat.Length];
            Array.Copy(pat, col, pat.Length);
            int p = 0;
            int n = (int)tone[p++];
            for (int i = 1; i <= n; i++)
            {
                UInt32 pb = (UInt32)tone[p++];
                UInt32 pr = (UInt32)tone[p++];
                UInt32 pg = (UInt32)tone[p++];
                pat[i] = (UInt32)0xff000000 | (pr << 16) | (pg << 8) | pb;

                UInt32 r = 0, g = 0, b = 0;
                for (int bit = 0; bit < 8; bit++)
                {
                    byte mask = (byte)(1 << bit);
                    if ((pr & mask) != 0) r++;
                    if ((pg & mask) != 0) g++;
                    if ((pb & mask) != 0) b++;
                }
                if (r > 0) r = r * 32 - 1;
                if (g > 0) g = g * 32 - 1;
                if (b > 0) b = b * 32 - 1;
                col[i] = 0xff000000 | (r << 16) | (g << 8) | b;
            }
            Bitmap.Lock();
            for (int wy = 0; wy < viewport.Height; wy++)
            {
                for (int wx = 0; wx < viewport.Width; wx++)
                {
                    unsafe
                    {
                        UInt32* q = (UInt32*)(Bitmap.BackBuffer + wy * Bitmap.BackBufferStride + wx * sizeof(UInt32));
                        UInt32 c = *q;
                        int ci = GetColorIndex(c);
                        if (ci > 0 && ci <= n)
                        {
                            *q = col[ci];
                        }
                    }
                }
            }
            Bitmap.AddDirtyRect(viewport);
            Bitmap.Unlock();
        }
        public enum FilterType
        {
            None,
            Blue,
            Red,
            Sepia
        };

        public FilterType ColorFilterType { get; set; } = FilterType.None;
        public void colorFilter()
        {
            float[] f;
            switch (ColorFilterType)
            {
                case FilterType.Blue:
                    f = blueFilter;
                    break;
                case FilterType.Red:
                    f = redFilter;
                    break;
                case FilterType.Sepia:
                    f = sepiaFilter;
                    break;
                default:
                    return;
            }
            Bitmap.Lock();

            unsafe
            {
                for (int y = 0; y < viewport.Height; y++)
                {
                    for (int x = 0; x < viewport.Width; x++)
                    {
                        UInt32* p = (UInt32*)(Bitmap.BackBuffer + y * Bitmap.BackBufferStride + x * sizeof(UInt32));
                        *p = applyFilter(*p , f);
                    }
                }
            }
            Bitmap.AddDirtyRect(viewport);
            Bitmap.Unlock();
        }
        private UInt32 applyFilter(UInt32 c, float[] f)
        {
            UInt32 b = (c >> 16) & 0xff;
            UInt32 r = (c >> 8) & 0xff;
            UInt32 g = c & 0xff;
            UInt32 nr = (UInt32)(r * f[0] + g * f[1] + b * f[2]);
            UInt32 ng = (UInt32)(r * f[3] + g * f[4] + b * f[5]);
            UInt32 nb = (UInt32)(r * f[6] + g * f[7] + b * f[8]);
            if (nr > 255) nr = 255;
            if (ng > 255) ng = 255;
            if (nb > 255) nb = 255;
            return 0xff000000 | (nr << 16) | (ng << 8) | nb;
        }
        public void drawRect(int x0, int y0, int x1, int y1, UInt32 c)
        {
            line(x0, y0, x1, y0, c);
            line(x1, y0, x1, y1, c);
            line(x0, y1, x1, y1, c);
            line(x0, y1, x0, y0, c);
        }

        public void fillRect(int x0, int y0, int x1, int y1, UInt32 c)
        {
            if (y0 > y1)
            {
                int y = y0;
                y0 = y1;
                y1 = y;
            }
            for (int y = y0; y <= y1; y++)
            {
                line(x0, y, x1, y, c);
            }
        }
        
        public UInt32 GetPaletteColor(int i) => palette[i];
    }
}
