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

using Frasterizer.Composition.Composition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

namespace Frasterizer.Composition
{
    public class Composer : IComposer
    {
        public Composer(ComposerSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        ~Composer()
        {
            if (!IsDisposed)
            {
                Dispose();
            }
        }

        public virtual ComposerSettings Settings { get; protected set; }

        protected virtual bool IsDisposed { get; set; }

        public virtual RasterizerResult Compose(IEnumerable<RenderResult> items)
        {
            if (items == default) { throw new ArgumentNullException(nameof(items)); }

            var array = items.Where(i => i.Image != default).ToArray();

            if (array.Length == 0) { return default; }

            var grid = Settings.IsMonospace
                ? (AbstractCompositionTable)new FixedSizeCompositionTable(Settings.Margin, Settings.IsPowerOfTwo)
                : (AbstractCompositionTable)new VariableSizeCompositionTable(Settings.Margin, Settings.IsPowerOfTwo);

            grid.Compose(array);

            var result = new Bitmap(grid.Size.Width, grid.Size.Height);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Settings.Color);

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                foreach (var row in grid.Rows)
                {
                    foreach (var item in row)
                    {
                        g.DrawImage(item.Image, item.Bounds.MinX, item.Bounds.MinY);
                    }
                }
            }

            return new RasterizerResult()
            {
                Items = array,
                Image = result
            };
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
