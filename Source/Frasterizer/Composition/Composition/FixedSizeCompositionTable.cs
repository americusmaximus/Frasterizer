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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Frasterizer.Composition.Composition
{
    internal class FixedSizeCompositionTable : AbstractCompositionTable
    {
        public FixedSizeCompositionTable(Spacing margin, bool isPowerOfTwo) : base(margin, isPowerOfTwo) { }

        public override void Compose(IEnumerable<RenderResult> items)
        {
            var array = items.ToArray();
            var arrayCount = array.Length;

            var maxHeight = array.Max(i => i.Bounds.MaxY) + Margin.Top + Margin.Bottom;
            var maxWidth = array.Max(i => i.Bounds.MaxX) + Margin.Left + Margin.Right;

            var dimensions = (int)Math.Ceiling(Math.Sqrt(arrayCount)) * Math.Max(maxHeight, maxWidth);

            if (IsPowerOfTwo)
            {
                dimensions = NextPowerOfTwo(dimensions);
            }

            var activeRowIndex = 0;

            // Fill the table
            var rows = new List<CompositionRow>(dimensions / maxHeight);
            rows.Add(new CompositionRow(Margin));

            for (var x = 0; x < arrayCount; x++)
            {
                var item = array[x];

                var row = (rows[activeRowIndex].Count * maxWidth + maxWidth < dimensions)
                            ? rows[activeRowIndex]
                            : default;

                if (row == default)
                {
                    row = new CompositionRow(Margin);
                    rows.Add(row);
                }

                row.Add(item);

                // Select last row as the active one
                activeRowIndex = rows.Count - 1;
            }

            // Allow for height trimming if possible
            var actualHeight = rows.Count * maxHeight;
            Size = new Size(dimensions, IsPowerOfTwo ? NextPowerOfTwo(actualHeight) : actualHeight);

            // Update bounds
            var offsetY = 0;

            foreach (var row in rows)
            {
                var offsetX = 0;

                foreach (var item in row)
                {
                    var width = item.Bounds.MaxX - item.Bounds.MinX;

                    item.Bounds.MinX = Margin.Left + offsetX;
                    item.Bounds.MaxX += item.Bounds.MinX;
                    item.Bounds.MinY = Margin.Top + offsetY;
                    item.Bounds.MaxY += item.Bounds.MinY;

                    offsetX += maxWidth;
                }

                offsetY += maxHeight;
            }

            // 
            Rows = rows;
        }
    }
}
