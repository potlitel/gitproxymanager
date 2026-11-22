using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string outputDir = @"C:\dev\GitProxyManager\Resources\Icons";
        Directory.CreateDirectory(outputDir);

        string inactivePath = Path.Combine(outputDir, "app-icon.ico");
        string activePath = Path.Combine(outputDir, "app-icon-active.ico");

        GenerateIcon(inactivePath, inactive: true);
        GenerateIcon(activePath, inactive: false);

        var inactiveInfo = new FileInfo(inactivePath);
        var activeInfo = new FileInfo(activePath);

        Console.WriteLine($"Generated: {inactivePath} ({inactiveInfo.Length} bytes)");
        Console.WriteLine($"Generated: {activePath} ({activeInfo.Length} bytes)");
    }

    static void GenerateIcon(string path, bool inactive)
    {
        int[] sizes = [16, 32, 48, 64, 128, 256];
        byte[][] pngData = new byte[sizes.Length][];

        for (int i = 0; i < sizes.Length; i++)
        {
            using var bmp = RenderIconBitmap(sizes[i], inactive);
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            pngData[i] = ms.ToArray();
        }

        WriteIcoPng(path, sizes, pngData);
        Console.WriteLine($"  [{(inactive ? "inactive" : "active")}] Wrote {sizes.Length} sizes: {string.Join(", ", sizes.Select((s, i) => $"{s}x{s}={pngData[i].Length}B"))}");
    }

    static Bitmap RenderIconBitmap(int size, bool inactive)
    {
        var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.Clear(Color.Transparent);

        float scale = size / 256f;
        g.ScaleTransform(scale, scale);

        Color bgColor = inactive
            ? Color.FromArgb(69, 71, 90)
            : Color.FromArgb(69, 71, 90);
        Color bgColorLight = inactive
            ? Color.FromArgb(88, 91, 122)
            : Color.FromArgb(88, 91, 122);
        Color nodeColor = inactive
            ? Color.FromArgb(137, 180, 250)
            : Color.FromArgb(166, 227, 161);
        Color lineColor = inactive
            ? Color.FromArgb(180, 190, 230)
            : Color.FromArgb(180, 210, 170);

        var bgRect = new RectangleF(8, 8, 240, 240);
        float cornerRadius = 48f;

        // Shadow
        using (var shadowPath = CreateRoundedRectPath(new RectangleF(12, 14, 240, 240), cornerRadius))
        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            g.FillPath(shadowBrush, shadowPath);

        // Background gradient
        using (var bgPath = CreateRoundedRectPath(bgRect, cornerRadius))
        using (var bgBrush = new LinearGradientBrush(bgRect, bgColorLight, bgColor, LinearGradientMode.Vertical))
            g.FillPath(bgBrush, bgPath);

        // Border
        using (var bgPath = CreateRoundedRectPath(bgRect, cornerRadius))
        using (var borderPen = new Pen(Color.FromArgb(60, 0, 0, 0), 2f))
            g.DrawPath(borderPen, bgPath);

        // Network nodes
        float centerX = 128f, centerY = 128f;
        float nodeRadius = 24f, lineThickness = 6f;

        var nodeTop = new PointF(centerX, centerY - 52f);
        var nodeLeft = new PointF(centerX - 48f, centerY + 32f);
        var nodeRight = new PointF(centerX + 48f, centerY + 32f);

        using (var linePen = new Pen(lineColor, lineThickness) { StartCap = LineCap.Round, EndCap = LineCap.Round })
        {
            g.DrawLine(linePen, nodeTop, nodeLeft);
            g.DrawLine(linePen, nodeTop, nodeRight);
            g.DrawLine(linePen, nodeLeft, nodeRight);
        }

        DrawNode(g, nodeLeft, nodeRadius, nodeColor);
        DrawNode(g, nodeRight, nodeRadius, nodeColor);
        DrawNode(g, nodeTop, nodeRadius * 1.15f, nodeColor);

        DrawInnerDot(g, nodeLeft, nodeRadius * 0.35f, bgColor);
        DrawInnerDot(g, nodeRight, nodeRadius * 0.35f, bgColor);
        DrawInnerDot(g, nodeTop, nodeRadius * 0.35f * 1.15f, bgColor);

        return bmp;
    }

    static void DrawNode(Graphics g, PointF center, float radius, Color fillColor)
    {
        float x = center.X - radius, y = center.Y - radius, d = radius * 2f;

        using var glowBrush = new SolidBrush(Color.FromArgb(30, fillColor));
        g.FillEllipse(glowBrush, x - 4f, y - 4f, d + 8f, d + 8f);

        using var brush = new SolidBrush(fillColor);
        g.FillEllipse(brush, x, y, d, d);

        using var pen = new Pen(Color.FromArgb(120, 255, 255, 255), 2f);
        g.DrawEllipse(pen, x, y, d, d);
    }

    static void DrawInnerDot(Graphics g, PointF center, float radius, Color color)
    {
        float x = center.X - radius, y = center.Y - radius, d = radius * 2f;
        using var brush = new SolidBrush(color);
        g.FillEllipse(brush, x, y, d, d);
    }

    static GraphicsPath CreateRoundedRectPath(RectangleF rect, float radius)
    {
        var path = new GraphicsPath();
        float d = radius * 2f;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    static void WriteIcoPng(string path, int[] sizes, byte[][] pngData)
    {
        using var fs = new FileStream(path, FileMode.Create);
        using var bw = new BinaryWriter(fs);

        // ICO header
        bw.Write((ushort)0);           // reserved
        bw.Write((ushort)1);           // type = ICO
        bw.Write((ushort)sizes.Length); // image count

        int headerSize = 6;
        int dirEntrySize = 16 * sizes.Length;
        int offset = headerSize + dirEntrySize;

        // Directory entries
        for (int i = 0; i < sizes.Length; i++)
        {
            int s = sizes[i];
            bw.Write((byte)(s >= 256 ? 0 : (byte)s)); // width
            bw.Write((byte)(s >= 256 ? 0 : (byte)s)); // height
            bw.Write((byte)0);  // color palette
            bw.Write((byte)0);  // reserved
            bw.Write((ushort)1); // color planes
            bw.Write((ushort)32); // bits per pixel
            bw.Write((uint)pngData[i].Length); // image data size
            bw.Write((uint)offset); // offset to image data
            offset += pngData[i].Length;
        }

        // PNG image data
        for (int i = 0; i < sizes.Length; i++)
        {
            bw.Write(pngData[i]);
        }

        bw.Flush();
    }
}
