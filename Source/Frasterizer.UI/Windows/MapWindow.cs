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

using Frasterizer.UI.Characters;
using Frasterizer.UI.Controls;
using Frasterizer.UI.Controls.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Frasterizer.UI.Windows
{
    public partial class MapWindow : Form
    {
        protected static string[] SortOrderExceptions = new string[] { "Basic", "Common", "Enclosed", "High", "Low", "Miscellaneous", "North", "Old", "Small", "South", "Supplemental" };

        public MapWindow()
        {
            Icon = Properties.Resources.Icon;
            InitializeComponent();

            using (var g = CreateGraphics())
            {
                DPIX = g.DpiX;
                DPIY = g.DpiY;
            }

#if !NET40 && !NET45 && !NET47 && !NET48
            CharactersSetSearchTextBox.PlaceholderText = "Search...";
#endif
        }

        [DefaultValue(null)]
        public virtual Font CharacterFont { get; set; }

        [DefaultValue(null)]
        public virtual List<int> Characters { get; set; }

        protected virtual float DPIX { get; set; }
        protected virtual float DPIY { get; set; }

        protected virtual void BuildCharacterTableLayoutPanel(CharacterMapCheckBoxItem item)
        {
            CharactersPanel.Visible = false;
            CharacterTableLayoutPanel.SuspendLayout();

            SizeCharacterTableLayoutPanel(item);

            var range = Enumerable.Range(item.Min, item.Max - item.Min);

            foreach (var i in range)
            {
                var label = new Label()
                {
                    Text = ((char)i).ToString(),
                    Margin = new Padding(0),
                    BorderStyle = BorderStyle.Fixed3D,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = CharacterFont ?? Font,
                    Dock = DockStyle.Fill,
                    BackColor = Characters.Contains(i) ? Color.Silver : Color.White
                };

                label.MouseEnter += (sender, e) => { CharacterValueLabel.Text = ((char)i).ToString(); ValueValueLabel.Text = string.Format("Value: U+{0:X4}", i); };
                label.MouseLeave += (sender, e) => { CharacterValueLabel.Text = string.Empty; ValueValueLabel.Text = string.Empty; };
                label.Click += (sender, e) => {
                    if(label.BackColor == Color.White)
                    {
                        Characters.Add(i);
                        label.BackColor = Color.Silver;
                    }
                    else
                    {
                        Characters.Remove(i);
                        label.BackColor = Color.White;
                    }

                    var checkBox = CharacterTableLayoutPanel.Tag as CharacterMapCheckBoxItem;

                    if(checkBox != default)
                    {
                        checkBox.CheckBox.CheckState = GetCheckBoxState(checkBox);
                    }
                };

                CharacterTableLayoutPanel.Controls.Add(label);
                MainToolTip.SetToolTip(label, string.Format("Value: U+{0:X4}", i));
            }

            CharactersPanel.Visible = true;
            CharacterTableLayoutPanel.ResumeLayout();
        }

        protected virtual void CharacterMapWindowLoad(object sender, EventArgs e)
        {
            if (Characters == default)
            {
                Characters = new List<int>();
            }

            if (CharacterFont != default)
            {
                CharacterValueLabel.Font = CharacterFont;
            }

            var map = MapReader.Get();

            var sortedMap = map
                .Select(i =>
                {
                    var state = GetCheckBoxState(i);

                    var checkBox = new DoubleClickCheckBox()
                    {
                        AutoCheck = false,
                        CheckState = state,
                        Margin = new Padding(0),
                        Padding = new Padding(2, 0, 0, 0),
                        Tag = i,
                        Text = i.Name
                    };

                    var item = new CharacterMapCheckBoxItem(i, checkBox);

                    checkBox.Click += (cs, ce) =>
                    {
                        var items = CharactersSetFlowLayoutPanel.Tag as CharacterMapCheckBoxItem[];

                        if (CharacterTableLayoutPanel.Tag != item)
                        {
                            if (items != default)
                            {
                                foreach (var ii in items)
                                {
                                    if (ii != item)
                                    {
                                        ii.CheckBox.BackColor = SystemColors.Window;
                                    }
                                }
                            }

                            CharactersGroupBox.Text = string.Format("Characters: {0}", item.Name);

                            checkBox.BackColor = SystemColors.ActiveCaption;

                            CharacterTableLayoutPanel.Tag = item;
                            BuildCharacterTableLayoutPanel(item);
                            CharactersPanelSizeChanged(sender, e);
                        }
                    };

                    checkBox.DoubleClick += (cs, ce) =>
                    {
                        var range = Enumerable.Range(item.Min, item.Max - item.Min);

                        if (checkBox.CheckState == CheckState.Checked)
                        {
                            checkBox.CheckState = CheckState.Unchecked;

                            foreach (var r in range)
                            {
                                Characters.Remove(r);
                            }

                            foreach (var c in CharacterTableLayoutPanel.Controls.OfType<Control>())
                            {
                                c.BackColor = Color.White;
                            }
                        }
                        else
                        {
                            checkBox.CheckState = CheckState.Checked;

                            foreach (var r in range)
                            {
                                if (!Characters.Contains(r))
                                {
                                    Characters.Add(r);
                                }
                            }
                            foreach (var c in CharacterTableLayoutPanel.Controls.OfType<Control>())
                            {
                                c.BackColor = Color.Silver;
                            }

                        }
                    };

                    return item;
                })
                .OrderBy(m => string.Join(" ", m.Name.Split(new[] { ' ' }).Where(p => !SortOrderExceptions.Contains(p))))
                .ToArray();

            CharactersSetFlowLayoutPanel.Tag = sortedMap;

            foreach (var c in sortedMap)
            {
                CharactersSetFlowLayoutPanel.Controls.Add(c.CheckBox);

                if (CharacterTableLayoutPanel.Tag == default && (c.CheckBox.CheckState == CheckState.Checked || c.CheckBox.CheckState == CheckState.Indeterminate))
                {
                    CharacterTableLayoutPanel.Tag = c;
                    c.CheckBox.BackColor = SystemColors.GradientActiveCaption;
                    CharactersSetFlowLayoutPanel.ScrollControlIntoView(c.CheckBox);
                }
            }

            if (CharacterTableLayoutPanel.Tag != default)
            {
                var item = CharacterTableLayoutPanel.Tag as CharacterMapCheckBoxItem;

                CharactersGroupBox.Text = string.Format("Characters: {0}", item.Name);

                BuildCharacterTableLayoutPanel(item);
                CharactersPanelSizeChanged(sender, e);
            }
        }

        protected virtual void CharactersPanelSizeChanged(object sender, EventArgs e)
        {
            if (CharacterTableLayoutPanel.Tag == default)
            {
                CharactersPanel.Visible = false;
            }

            if (!CharactersPanel.Visible) { return; }

            CharacterTableLayoutPanel.Location = new Point(
                (CharactersPanel.Width - CharacterTableLayoutPanel.Width) / 2,
                (CharactersPanel.Height - CharacterTableLayoutPanel.Height) / 2);
        }

        protected virtual void CharactersSetFlowLayoutPanel1ClientSizeChanged(object sender, EventArgs e)
        {
            var width = CharactersSetFlowLayoutPanel.ClientSize.Width;

            var control = CharactersSetFlowLayoutPanel.Controls.OfType<Control>().FirstOrDefault();

            if (control != default)
            {
                if (control.Width == width) { return; }

                foreach (var c in CharactersSetFlowLayoutPanel.Controls.OfType<Control>())
                {
                    c.Width = width;
                }
            }
        }

        protected virtual void CharactersSetFlowLayoutPanel1ControlAdded(object sender, ControlEventArgs e)
        {
            var width = CharactersSetFlowLayoutPanel.ClientSize.Width;

            foreach (var c in CharactersSetFlowLayoutPanel.Controls.OfType<Control>())
            {
                c.Width = width;
            }
        }

        protected virtual void CharactersSetSearchTextBoxTextChanged(object sender, EventArgs e)
        {
            var text = CharactersSetSearchTextBox.Text.Trim().ToLower();

            var items = CharactersSetFlowLayoutPanel.Tag as CharacterMapCheckBoxItem[];

            CharactersSetFlowLayoutPanel.Controls.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                CharactersSetFlowLayoutPanel.Controls.AddRange(items.Select(i => i.CheckBox).ToArray());
            }
            else
            {
                var isNumber = int.TryParse(text, out var number);

                var fitered = isNumber
                    ? items.Where(i => i.Min <= number && number <= i.Max).Select(i => i.CheckBox).ToArray()
                    : items.Where(i => i.Name.ToLower().Contains(text)).Select(i => i.CheckBox).ToArray();

                CharactersSetFlowLayoutPanel.Controls.AddRange(fitered);
            }
        }

        protected virtual void ExitButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        protected virtual int GetCellSize()
        {
            var font = CharacterFont ?? Font;

            var points = font.SizeInPoints;
            var pixels = (int)Math.Ceiling(Math.Max(points * DPIX, points * DPIY) / 72);

            return Math.Max(25, pixels * 3);
        }

        protected virtual CheckState GetCheckBoxState(MapItem i)
        {
            var range = Enumerable.Range(i.Min, i.Max - i.Min).ToArray();
            var matches = range.Count(c => Characters.Contains(c));

            var state = (matches == range.Length && range.Length != 0)
                            ? CheckState.Checked
                            : ((matches != 0 && matches < range.Length) ? CheckState.Indeterminate : CheckState.Unchecked);

            return state;
        }

        protected virtual void OkButtonClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void SizeCharacterTableLayoutPanel(CharacterMapCheckBoxItem item)
        {
            var number = item.Max - item.Min;
            var sqrt = (int)Math.Ceiling(Math.Sqrt(number));

            CharacterTableLayoutPanel.Width = sqrt * GetCellSize();
            CharacterTableLayoutPanel.Height = sqrt * GetCellSize();

            CharacterTableLayoutPanel.Controls.Clear();
            CharacterTableLayoutPanel.ColumnStyles.Clear();
            CharacterTableLayoutPanel.RowStyles.Clear();

            CharacterTableLayoutPanel.ColumnCount = sqrt;
            CharacterTableLayoutPanel.RowCount = sqrt;

            for (var x = 0; x < CharacterTableLayoutPanel.ColumnCount; x++)
            {
                CharacterTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, GetCellSize()));
            }

            for (var x = 0; x < CharacterTableLayoutPanel.RowCount; x++)
            {
                CharacterTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, GetCellSize()));
            }
        }
    }
}