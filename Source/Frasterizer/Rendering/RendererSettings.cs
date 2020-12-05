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
using System.Collections.Generic;
using System.Drawing;

namespace Frasterizer.Rendering
{
    public class RendererSettings
    {
        public RendererSettings(string fileName) : this(new[] { fileName }) { }

        public RendererSettings(IEnumerable<string> fileNames) : this()
        {
            Fonts = fileNames;
        }

        public RendererSettings()
        {
            Fonts = new List<string>();
            Outline = new Outline();
            Padding = new Spacing();
            Scale = new Scale();
        }

        public virtual Color BackColor { get; set; }

        public virtual Color Color { get; set; }

        public virtual int DPI { get; set; }

        public virtual IEnumerable<string> Fonts { get; set; }

        public virtual bool IsEmptyAllowed { get; set; }

        public virtual Outline Outline { get; set; }

        public virtual Spacing Padding { get; set; }

        public virtual Scale Scale { get; set; }

        public virtual float Size { get; set; }

        public virtual FontSizeType SizeType { get; set; }

        public virtual FontStyleType StyleType { get; set; }
    }
}
