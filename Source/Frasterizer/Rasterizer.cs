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

using Frasterizer.Composition;
using Frasterizer.Rendering;
using Frasterizer.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Frasterizer
{
    public class Rasterizer : IDisposable
    {
        public Rasterizer(IRenderer renderer, IComposer composer = default)
        {
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            Composer = composer;
        }

        ~Rasterizer()
        {
            Dispose();
        }

        public virtual IComposer Composer { get; protected set; }

        public virtual bool IsDisposed { get; set; }

        public virtual IRenderer Renderer { get; protected set; }

        public static RasterizerResult Rasterize(RasterizerSettings settings, IEnumerable<char> characters)
        {
            if (settings == default) { throw new ArgumentNullException(nameof(settings)); }
            if (settings.RendererSettings == default) { throw new ArgumentException("Renderer settings have to have value."); }

            if (characters == default) { throw new ArgumentNullException(nameof(characters)); }
            if (!characters.Any()) { throw new ArgumentException("There has to be at least one character for rasterization."); }

            using (var rasterizer = new Rasterizer(new Renderer(settings.RendererSettings), settings.ComposerSettings == default ? default : new Composer(settings.ComposerSettings)))
            {
                return rasterizer.Rasterize(characters);
            }
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Renderer.Dispose();
                Composer?.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public virtual RasterizerResult Rasterize(IEnumerable<char> characters)
        {
            if (characters == default) { throw new ArgumentNullException(nameof(characters)); }
            if (!characters.Any()) { throw new ArgumentException("There has to be at least one character for rasterization."); }

            if (IsDisposed) { throw new ObjectDisposedException(nameof(Rasterizer)); }

            var results = characters.Distinct().OrderBy(c => c).Select(s => Renderer.Render(s)).ToArray();

            return Composer == default
                ? new RasterizerResult() { Items = results }
                : Composer.Compose(results);
        }
    }
}