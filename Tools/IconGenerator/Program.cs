using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

class Program
{
    static void Main()
    {
        string outputDir = @"C:\dev\GitProxyManager\Resources\Icons";
        Directory.CreateDirectory(outputDir);

        string inactivePath = Path.Combine(outputDir, "app-icon.ico");
        string activePath = Path.Combine(outputDir, "app-icon-active.ico");

        GenerateIcon(inactivePath, Color.FromArgb(137, 180, 250), "inactive");
        GenerateIcon(activePath, Color.FromArgb(166, 227, 161), "active");

        var inactiveInfo = new FileInfo(inactivePath);
        var activeInfo = new FileInfo(activePath);

        Console.WriteLine($"Generated: {inactivePath} ({inactiveInfo.Length} bytes)");
        Console.WriteLine($"Generated: {activePath} ({activeInfo.Length} bytes)");
    }

    static void GenerateIcon(string path, Color circleColor, string label)
    {
        int[] sizes = [16, 32];
        byte[][] imageData = new byte[sizes.Length][];

        for (int i = 0; i < sizes.Length; i++)
        {
            imageData[i] = RenderIconBitmap(sizes[i], circleColor);
        }

        WriteIcoFile(path, sizes, imageData);
        Console.WriteLine($"  [{label}] Wrote {sizes.Length} sizes: {string.Join(", ", sizes.Select((s, i) => $"{s}x{s}={imageData[i].Length}B"))}");
    }

    static byte[] RenderIconBitmap(int size, Color circleColor)
    {
        using var bmp = new Bitmap(size, size);
        using var g = Graphics.FromImage(bmp);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        int padding = Math.Max(1, size / 8);
        int diameter = size - (padding * 2);
        int cx = size / 2;
        int cy = size / 2;

        using var brush = new SolidBrush(circleColor);
        g.FillEllipse(brush, padding, padding, diameter, diameter);

        using var borderPen = new Pen(Color.FromArgb(80, 0, 0, 0), Math.Max(1, size / 16));
        g.DrawEllipse(borderPen, padding, padding, diameter, diameter);

        return BitmapToBgraBytes(bmp, size);
    }

    static byte[] BitmapToBgraBytes(Bitmap bmp, int size)
    {
        var bits = bmp.LockBits(new Rectangle(0, 0, size, size),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        int byteCount = bits.Stride * bits.Height;
        byte[] pixels = new byte[byteCount];
        System.Runtime.InteropServices.Marshal.Copy(bits.Scan0, pixels, 0, byteCount);
        bmp.UnlockBits(bits);
        return pixels;
    }

    static void WriteIcoFile(string path, int[] sizes, byte[][] imageData)
    {
        using var fs = new FileStream(path, FileMode.Create);
        using var bw = new BinaryWriter(fs);

        // ICO header
        bw.Write((ushort)0);          // reserved
        bw.Write((ushort)1);          // type = 1 (ICO)
        bw.Write((ushort)sizes.Length); // image count

        // Each BMP image entry in ICO:
        // BITMAPINFOHEADER (40 bytes) + AND mask + XOR data
        // For 32bpp with alpha, AND mask is still required but can be all zeros

        int headerSize = 6;                                 // ICO header
        int dirEntrySize = 16 * sizes.Length;               // directory entries
        int bmpHeaderSize = 40;                             // BITMAPINFOHEADER per image

        // Calculate sizes and offsets
        int[] bmpDataSizes = new int[sizes.Length];
        int[] bmpDataOffsets = new int[sizes.Length];

        int currentOffset = headerSize + dirEntrySize;

        for (int i = 0; i < sizes.Length; i++)
        {
            int s = sizes[i];
            int andMaskRowBytes = ((s + 31) / 32) * 4;
            int andMaskSize = andMaskRowBytes * s;
            int xorDataSize = s * s * 4; // 32bpp BGRA
            bmpDataSizes[i] = bmpHeaderSize + andMaskSize + xorDataSize;
            bmpDataOffsets[i] = currentOffset;
            currentOffset += bmpDataSizes[i];
        }

        // Write directory entries
        for (int i = 0; i < sizes.Length; i++)
        {
            int s = sizes[i];
            byte widthByte = (byte)(s >= 256 ? 0 : s);
            byte heightByte = (byte)(s >= 256 ? 0 : s);

            bw.Write(widthByte);              // width
            bw.Write(heightByte);             // height
            bw.Write((byte)0);               // color count (0 for 32bpp)
            bw.Write((byte)0);               // reserved
            bw.Write((ushort)1);             // color planes
            bw.Write((ushort)32);            // bits per pixel
            bw.Write(bmpDataSizes[i]);       // image data size
            bw.Write(bmpDataOffsets[i]);     // offset to image data
        }

        // Write image data
        for (int i = 0; i < sizes.Length; i++)
        {
            int s = sizes[i];
            byte[] pixels = imageData[i];

            // BITMAPINFOHEADER (40 bytes)
            bw.Write((uint)40);                    // biSize
            bw.Write((uint)s);                     // biWidth
            bw.Write((uint)(s * 2));               // biHeight (doubled for ICO: XOR + AND)
            bw.Write((ushort)1);                   // biPlanes
            bw.Write((ushort)32);                  // biBitCount
            bw.Write((uint)0);                     // biCompression (BI_RGB)
            bw.Write((uint)(s * s * 4));           // biSizeImage
            bw.Write((uint)0);                     // biXPelsPerMeter
            bw.Write((uint)0);                     // biYPelsPerMeter
            bw.Write((uint)0);                     // biClrUsed
            bw.Write((uint)0);                     // biClrImportant

            // AND mask (1 bit per pixel, rows padded to 4 bytes)
            int andMaskRowBytesLocal = ((s + 31) / 32) * 4;
            byte[] andMask = new byte[andMaskRowBytesLocal * s];
            // All zeros = fully opaque (alpha channel handles transparency)
            bw.Write(andMask);

            // XOR data: bottom-up BGRA pixels
            // BMP stores rows bottom-up, but our pixel data is top-down
            // We need to flip vertically
            int rowBytes = s * 4;
            for (int y = s - 1; y >= 0; y--)
            {
                int srcRowStart = y * rowBytes;
                bw.Write(pixels, srcRowStart, rowBytes);
            }
        }

        bw.Flush();
    }
}
