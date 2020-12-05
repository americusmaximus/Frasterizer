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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Frasterizer.Composition.Composition
{
    internal class CompositionRow : IEnumerable<RenderResult>
    {
        public CompositionRow(Spacing margin)
        {
            Margin = margin ?? new Spacing();

            Items = new List<RenderResult>();
        }

        public virtual int Count { get { return Items.Count; } }

        public virtual int Height { get { return Items.Count == 0 ? 0 : Items.Max(i => Margin.Top + i.Bounds.MaxY - i.Bounds.MinY + Margin.Bottom); } }

        public virtual Spacing Margin { get; protected set; }

        public virtual int Width { get { return Items.Count == 0 ? 0 : Items.Sum(i => Margin.Left + i.Bounds.MaxX - i.Bounds.MinX + Margin.Right); } }

        protected virtual List<RenderResult> Items { get; set; }

        public virtual RenderResult this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public virtual void Add(RenderResult item)
        {
            Items.Add(item);
        }

        public virtual IEnumerator<RenderResult> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public virtual void Remove(RenderResult item)
        {
            Items.Remove(item);
        }
    }
}
