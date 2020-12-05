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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Frasterizer.UI.Controls
{
    public partial class SplitButton : Button
    {
        public SplitButton()
        {
            SplitWidth = 20;
        }

        [DefaultValue(null), Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContextMenuStrip Menu { get; set; }

        [DefaultValue(20), Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SplitWidth { get; set; }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            var splitRect = new Rectangle(Width - SplitWidth, 0, SplitWidth, Height);

            // Figure out if the button click was on the button itself or the menu split
            if (Menu != default &&  mevent.Button == MouseButtons.Left && splitRect.Contains(mevent.Location))
            {
                // Shows menu under button
                Menu.Show(this, 0, Height);
                return;
            }

            base.OnMouseDown(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (Menu != null && SplitWidth > 0)
            {
                // Draw the arrow glyph on the right side of the button
                int arrowX = ClientRectangle.Width - 14;
                int arrowY = ClientRectangle.Height / 2 - 1;

                var arrowBrush = Enabled ? SystemBrushes.ControlText : SystemBrushes.ButtonShadow;
                var arrows = new[] { new Point(arrowX, arrowY), new Point(arrowX + 7, arrowY), new Point(arrowX + 3, arrowY + 4) };
                pevent.Graphics.FillPolygon(arrowBrush, arrows);

                // Draw a dashed separator on the left of the arrow
                int lineX = ClientRectangle.Width - SplitWidth;
                int lineYFrom = arrowY - 4;
                int lineYTo = arrowY + 8;
                using (var separatorPen = new Pen(Brushes.DarkGray) { DashStyle = DashStyle.Dot })
                {
                    pevent.Graphics.DrawLine(separatorPen, lineX, lineYFrom, lineX, lineYTo);
                }
            }
        }
    }
}
