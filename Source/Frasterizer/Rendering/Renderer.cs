#region License
/*
MIT License

Copyright (c) 2020 Americus Maximus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using Frasterizer.Settings;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;

namespace Frasterizer.Rendering
{
    public class Renderer : IRenderer
    {
        public const float MinimumFontSize = 0.1f;

        public Renderer(RendererSettings settings)
        {
            BitmapSizeMultiplier = 4;

            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (settings.Fonts == default || !settings.Fonts.Any()) { throw new ArgumentException("There must be at least one font file specified."); }
            if (settings.Size < MinimumFontSize) { throw new ArgumentException(string.Format("The font size has to be minimum of {0}.", MinimumFontSize)); }

            FontCollection = new PrivateFontCollection();
            foreach (var fontPath in settings.Fonts)
            {
                FontCollection.AddFontFile(fontPath);
            }

            Font = new Font(FontCollection.Families[0], settings.Size, (FontStyle)settings.StyleType, settings.SizeType == FontSizeType.Pixel ? GraphicsUnit.Pixel : GraphicsUnit.Point);
        }

        ~Renderer()
        {
            Dispose();
        }

        public virtual Font Font { get; protected set; }
        public virtual bool IsDisposed { get; protected set; }
        public virtual RendererSettings Settings { get; protected set; }
        protected virtual int BitmapSizeMultiplier { get; set; }
        protected virtual PrivateFontCollection FontCollection { get; set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                Font?.Dispose();
                FontCollection?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public virtual RenderResult Render(char character)
        {
            var size = Font.Unit == GraphicsUnit.Pixel
                ? (int)Math.Ceiling(Font.Size * BitmapSizeMultiplier)
                : (int)Math.Ceiling(Font.SizeInPoints / 72f * Settings.DPI * BitmapSizeMultiplier);

            var result = Wrap(Scale(Outline(Draw(character, size, size))));

            return new RenderResult()
            {
                Item = character,
                Image = result.Image,
                Bounds = new RenderBounds()
                {
                    MaxX = result.Size.Width,
                    MaxY = result.Size.Height
                }
            };
        }

        protected virtual RendererIntermediateResult Draw(char character, int width, int height)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(Renderer)); }

            var characterAsString = character.ToString();

            var actualHeight = 0;
            var actualWidth = 0;

            var image = new Bitmap(width, height);

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Settings.BackColor);

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var characterSize = g.MeasureString(characterAsString, Font);

                actualHeight = (int)Math.Ceiling(characterSize.Height);
                actualWidth = (int)Math.Ceiling(characterSize.Width);

                if(actualHeight > height || actualWidth > width)
                {
                    return Draw(character, actualWidth, actualWidth);
                }

                using (var brush = new SolidBrush(Settings.Color))
                {
                    g.DrawString(characterAsString, Font, brush, 0, 0);
                }

                g.Flush();
            }

            return Size(new RendererIntermediateResult()
            {
                Image = (actualWidth <= 0 || actualHeight <= 0)
                    ? default
                    : image.Clone(new Rectangle(0, 0, actualWidth, actualHeight), PixelFormat.Format32bppArgb),
                Size = new Size(actualWidth, actualHeight)
            });
        }

        protected virtual RendererIntermediateResult Outline(RendererIntermediateResult input)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(Renderer)); }

            if (input.Image == default) { return input; }
            if (Settings.Outline.Size <= 0) { return input; }

            var height = input.Size.Height + Settings.Outline.Size * 4;
            var width = input.Size.Width + Settings.Outline.Size * 4;

            var image = new Bitmap(width, height);

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Settings.BackColor);

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(input.Image, Settings.Outline.Size, Settings.Outline.Size);
            }

            var argb = Settings.Color.ToArgb();

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var pixel = image.GetPixel(i, j).ToArgb();

                    if (pixel == argb) { continue; }

                    var bounds = new RenderBounds()
                    {
                        MinX = Math.Max(0, i - 1 - Settings.Outline.Size),
                        MaxX = Math.Min(i + 1 + Settings.Outline.Size, width - 1),
                        MinY = Math.Max(0, j - 1 - Settings.Outline.Size),
                        MaxY = Math.Min(j + 1 + Settings.Outline.Size, height - 1)
                    };

                    var found = false;

                    foreach (var x in Enumerable.Range(bounds.MinX, bounds.MaxX - bounds.MinX))
                    {
                        foreach (var y in Enumerable.Range(bounds.MinY, bounds.MaxY - bounds.MinY))
                        {
                            if (i == x && j == y) { continue; }

                            if (image.GetPixel(x, y).ToArgb() == argb)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found) { break; }
                    }

                    if (found)
                    {
                        image.SetPixel(i, j, Settings.Outline.Color);
                    }
                }
            }

            return Size(new RendererIntermediateResult()
            {
                Image = image,
                Size = new Size(width, height)
            });
        }

        protected virtual RendererIntermediateResult Scale(RendererIntermediateResult input)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(Renderer)); }

            if (input.Image == default) { return input; }
            if (Settings.Scale.Height == 1 && Settings.Scale.Width == 1) { return input; }

            var height = (int)Math.Ceiling(input.Size.Height * Settings.Scale.Height);
            var width = (int)Math.Ceiling(input.Size.Width * Settings.Scale.Width);

            var image = new Bitmap(width, height);

            var sourceRectangle = new Rectangle(0, 0, input.Size.Width, input.Size.Height);
            var destinationRectangle = new Rectangle(0, 0, width, height);

            using (var g = Graphics.FromImage(image))
            {
                using (var brush = new SolidBrush(Settings.BackColor))
                {
                    g.Clear(Settings.BackColor);

                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    g.DrawImage(input.Image, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
                }
            }

            return new RendererIntermediateResult()
            {
                Image = image,
                Size = new Size(width, height)
            };
        }

        protected virtual RendererIntermediateResult Size(RendererIntermediateResult input)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(Renderer)); }

            var height = input.Size.Height;
            var width = input.Size.Width;

            var minX = width;
            var maxX = 0;
            var maxY = 0;

            var argb = Settings.BackColor.ToArgb();

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (input.Image.GetPixel(i, j).ToArgb() != argb)
                    {
                        minX = Math.Min(minX, i);
                        maxX = Math.Max(maxX, i);

                        maxY = Math.Max(maxY, j);
                    }
                }
            }

            var actualMinX = Math.Min(minX, width);

            var actualMaxX = Math.Min(width, Math.Max(minX, maxX));
            var actualMaxY = Math.Min(height, maxY + 1);

            if (actualMaxX - actualMinX == 0 || actualMaxY == 0)
            {
                return Settings.IsEmptyAllowed
                    ? input
                    : new RendererIntermediateResult();
            }

            return new RendererIntermediateResult()
            {
                Image = input.Image.Clone(new Rectangle(actualMinX, 0, actualMaxX - actualMinX, actualMaxY), PixelFormat.Format32bppArgb),
                Size = new Size(actualMaxX - actualMinX, actualMaxY)
            };
        }

        protected virtual RendererIntermediateResult Wrap(RendererIntermediateResult input)
        {
            if (IsDisposed) { throw new ObjectDisposedException(nameof(Renderer)); }

            if (input.Image == default) { return input; }
            if (Settings.Padding.Left == 0 && Settings.Padding.Top == 0 && Settings.Padding.Right == 0 && Settings.Padding.Bottom == 0) { return input; }

            var width = input.Size.Width + Settings.Padding.Left + Settings.Padding.Right;
            var height = input.Size.Height + Settings.Padding.Top + Settings.Padding.Bottom;

            var image = new Bitmap(width, height);

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Settings.BackColor);

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(input.Image, Settings.Padding.Left, Settings.Padding.Top);
            }

            return new RendererIntermediateResult()
            {
                Image = image,
                Size = new Size(width, height)
            };
        }

        protected class RendererIntermediateResult
        {
            public virtual Bitmap Image { get; set; }

            public virtual Size Size { get; set; }
        }
    }
}
