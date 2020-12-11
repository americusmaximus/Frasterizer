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
using Frasterizer.UI.Controls.Items;
using Frasterizer.UI.Windows.Main;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Frasterizer.UI.Windows
{
    public partial class MainWindow : Form
    {
        protected const string DefaultCharacters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        protected static string DefaultFontDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

        public MainWindow()
        {
            Icon = Properties.Resources.Icon;
            InitializeComponent();

            CharactersTextBox.Text = DefaultCharacters;
            ImagePreviewZoomValue = 100;
            MainColorDialog.CustomColors = new int[] { Color.Transparent.ToArgb() };
        }

        protected virtual double ImagePreviewZoomValue { get; set; }

        protected virtual bool IsAutomaticUserInterfaceChange { get; set; }

        protected virtual void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutWindow().ShowDialog(this);
        }

        protected virtual void AllowEmptyCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                if (rasterizer != default)
                {
                    rasterizer.Renderer.Settings.IsEmptyAllowed = AllowEmptyCheckBox.Checked;

                    if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                    RasterizeFont(new RasterizeRequest()
                    {
                        Rasterizer = rasterizer,
                        Text = CharactersTextBox.Text
                    });
                }
            }
        }

        protected virtual void BackColorPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = BackColorPictureBox.BackColor;

            if (MainColorDialog.ShowDialog(this) == DialogResult.OK)
            {
                BackColorPictureBox.BackColor = MainColorDialog.Color;

                var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                if (rasterizer != default)
                {
                    rasterizer.Renderer.Settings.BackColor = MainColorDialog.Color;
                    rasterizer.Composer.Settings.Color = MainColorDialog.Color;

                    if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                    RasterizeFont(new RasterizeRequest()
                    {
                        Rasterizer = rasterizer,
                        Text = CharactersTextBox.Text
                    });
                }
            }
        }

        protected virtual void BoldFontStyleButtonCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            Focus();

            var isChangeAllowed = RegularFontStyleButton.Enabled || ItalicFontStyleButton.Enabled || UnderlineFontStyleButton.Enabled || StrikeoutFontStyleButton.Enabled;

            IsAutomaticUserInterfaceChange = true;

            if (!isChangeAllowed)
            {
                BoldFontStyleButton.Checked = !BoldFontStyleButton.Checked;
                IsAutomaticUserInterfaceChange = false;
                return;
            }

            if (RegularFontStyleButton.Enabled)
            {
                RegularFontStyleButton.Checked = !BoldFontStyleButton.Checked && !ItalicFontStyleButton.Checked && !UnderlineFontStyleButton.Checked && !StrikeoutFontStyleButton.Checked;
            }

            IsAutomaticUserInterfaceChange = false;

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                LoadFont(CreateLoadFontRequest());
            }
        }

        protected virtual void BoxCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.IsMonospace = MonospaceCheckBox.Checked;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }
        }
        protected virtual void CharactersDefaultToolStripButtonClick(object sender, EventArgs e)
        {
            CharactersTextBox.Text = DefaultCharacters;
        }

        protected virtual void CharactersExportToolStripButtonClick(object sender, EventArgs e)
        {
            SaveTextFileDialog.FileName = string.Empty;

            if (SaveTextFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(SaveTextFileDialog.FileName, CharactersTextBox.Text, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Unable to saving the file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void CharactersImportToolStripButtonClick(object sender, EventArgs e)
        {
            OpenTextFileDialog.FileName = string.Empty;

            if (OpenTextFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    CharactersTextBox.Text = File.ReadAllText(OpenTextFileDialog.FileName, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Unable to open the file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void CharactersMapToolStripButtonClick(object sender, EventArgs e)
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;
            var font = rasterizer == default ? Font : rasterizer.Renderer.Font;

            var window = new MapWindow()
            {
                CharacterFont = new Font(font.FontFamily, Math.Min(12, font.SizeInPoints), font.Style),
                Characters = CharactersTextBox.Text.Distinct().Select(c => (int)c).ToList()
            };

            if (window.ShowDialog(this) == DialogResult.OK)
            {
                CharactersTextBox.Text = string.Join(string.Empty, window.Characters.Distinct().OrderBy(c => c).Where(c => c > 0).Select(c => (char)c).ToArray());
            }
        }

        protected virtual void CharactersRefreshToolStripButtonClick(object sender, EventArgs e)
        {
            CharactersTextBox.Text = string.Join("", CharactersTextBox.Text.Distinct().OrderBy(c => c).ToArray());
        }

        protected virtual void CharactersTextBoxTextChanged(object sender, EventArgs e)
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer == default) { return; }

            RenderPreviewImage();
        }

        protected virtual LoadFontRequest CreateLoadFontRequest()
        {
            return new LoadFontRequest()
            {
                BackColor = BackColorPictureBox.BackColor,
                Characters = CharactersTextBox.Text.ToCharArray(),
                DPI = (int)DpiNumericUpDown.Value,
                Files = (FontFamilyComboBox.SelectedItem as FontFamilyNameComboBoxItem).Files,
                FontColor = FontColorPictureBox.BackColor,
                IsBoldRequested = BoldFontStyleButton.Checked,
                IsItalicRequested = ItalicFontStyleButton.Checked,
                IsRegularRequested = RegularFontStyleButton.Checked,
                IsStrikeoutRequested = StrikeoutFontStyleButton.Checked,
                IsUnderlineRequested = UnderlineFontStyleButton.Checked,
                Size = (float)FontSizeNumericUpDown.Value,
                SizeType = (FontSizeTypeComboBox.SelectedItem as FontSizeTypeComboBoxItem).SizeType
            };
        }

        protected virtual void DefaultFontDirectoryToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (FontDirectoryTextBox.Text == DefaultFontDirectoryPath) { return; }

            FontDirectoryTextBox.Text = DefaultFontDirectoryPath;

            LoadFonts(DefaultFontDirectoryPath);
        }

        protected virtual void DpiNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                if (rasterizer != default)
                {
                    rasterizer.Renderer.Settings.DPI = (int)DpiNumericUpDown.Value;

                    if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                    RasterizeFont(new RasterizeRequest()
                    {
                        Rasterizer = rasterizer,
                        Text = CharactersTextBox.Text
                    });
                }
            }

            DpiNumericUpDown.Focus();
        }

        protected virtual void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected virtual void ExportConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;
            if (rasterizer == default) { return; }

            var data = ImagePreviewPictureBox.Tag as RasterizerResult;
            if (data == default) { return; }

            ConfigurationSaveFileDialog.FileName = GetFileNameTemplate();

            if (ConfigurationSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var style = FontStyleType.Regular;

                if (BoldFontStyleButton.Checked)
                {
                    style |= FontStyleType.Bold;
                }

                if (ItalicFontStyleButton.Checked)
                {
                    style |= FontStyleType.Italic;
                }

                if (UnderlineFontStyleButton.Checked)
                {
                    style |= FontStyleType.Underline;
                }

                if (StrikeoutFontStyleButton.Checked)
                {
                    style |= FontStyleType.Strikeout;
                }

                if (!RegularFontStyleButton.Checked)
                {
                    style ^= FontStyleType.Regular;
                }

                var settings = new RasterizerSettings(
                    new RendererSettings(FontFilesListBox.Items.OfType<FontFileNameComboBoxItem>().Select(i => i.FullName))
                    {
                        Color = FontColorPictureBox.BackColor,
                        BackColor = BackColorPictureBox.BackColor,
                        Size = (float)FontSizeNumericUpDown.Value,
                        SizeType = (FontSizeTypeComboBox.SelectedItem as FontSizeTypeComboBoxItem).SizeType,
                        DPI = (int)DpiNumericUpDown.Value,
                        IsEmptyAllowed = AllowEmptyCheckBox.Checked,
                        Padding = new Spacing()
                        {
                            Left = (int)PaddingLeftNumericUpDown.Value,
                            Top = (int)PaddingTopNumericUpDown.Value,
                            Right = (int)PaddingRightNumericUpDown.Value,
                            Bottom = (int)PaddingBottomNumericUpDown.Value
                        },
                        Outline = new Outline()
                        {
                            Color = OutlineColorPictureBox.BackColor,
                            Size = (int)OutlineThicknessNumericUpDown.Value
                        },
                        Scale = new Scale()
                        {
                            Height = (int)ScaleHeightNumericUpDown.Value,
                            Width = (int)ScaleWidthNumericUpDown.Value
                        },
                        StyleType = style

                    },
                    new ComposerSettings()
                    {
                        Color = BackColorPictureBox.BackColor,
                        IsMonospace = MonospaceCheckBox.Checked,
                        IsPowerOfTwo = PowerOfTwoCheckBox.Checked,
                        Margin = new Spacing()
                        {
                            Left = (int)MarginLeftNumericUpDown.Value,
                            Top = (int)MarginTopNumericUpDown.Value,
                            Right = (int)MarginRightNumericUpDown.Value,
                            Bottom = (int)MarginBottomNumericUpDown.Value
                        }
                    });

                var output = JsonConvert.SerializeObject(settings);

                try
                {
                    File.WriteAllText(ConfigurationSaveFileDialog.FileName, output, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void FontColorPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = FontColorPictureBox.BackColor;

            if (MainColorDialog.ShowDialog(this) == DialogResult.OK)
            {
                FontColorPictureBox.BackColor = MainColorDialog.Color;

                if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
                {
                    var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                    if (rasterizer != default)
                    {
                        rasterizer.Renderer.Settings.Color = MainColorDialog.Color;

                        if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                        RasterizeFont(new RasterizeRequest()
                        {
                            Rasterizer = rasterizer,
                            Text = CharactersTextBox.Text
                        });
                    }
                }
            }
        }

        protected virtual void FontDirectorySelectButtonClick(object sender, EventArgs e)
        {
            FontDirectoryBrowserDialog.SelectedPath = string.Empty;

            if (FontDirectoryBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = FontDirectoryBrowserDialog.SelectedPath;

                if (FontDirectoryTextBox.Text == path) { return; }

                FontDirectoryTextBox.Text = path;

                LoadFonts(path);
            }
        }

        protected virtual void FontFamilyComboBoxSelectedValueChanged(object sender, EventArgs e)
        {
            FontFilesListBox.Items.Clear();

            var item = FontFamilyComboBox.SelectedItem as FontFamilyNameComboBoxItem;

            if (item == default) { return; }

            // Populate the list of files in this font family
            FontFilesListBox.Items.AddRange(item.Files.Select(i => new FontFileNameComboBoxItem() { FullName = i, Name = Path.GetFileName(i) }).ToArray());

            // Load the font
            var style = FontStyle.Regular;

            if (BoldFontStyleButton.Checked)
            {
                style |= FontStyle.Bold;
            }

            if (ItalicFontStyleButton.Checked)
            {
                style |= FontStyle.Italic;
            }

            if (UnderlineFontStyleButton.Checked)
            {
                style |= FontStyle.Underline;
            }

            if (StrikeoutFontStyleButton.Checked)
            {
                style |= FontStyle.Strikeout;
            }

            LoadFont(CreateLoadFontRequest());
        }

        protected virtual void FontFilesListBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                for (var x = 0; x < FontFilesListBox.Items.Count; x++)
                {
                    FontFilesListBox.SetSelected(x, true);
                }

                return;
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                var sb = new StringBuilder();

                for (var x = 0; x < FontFilesListBox.SelectedIndices.Count; x++)
                {
                    sb.AppendLine((FontFilesListBox.Items[x] as FontFileNameComboBoxItem).FullName);
                }

                var result = sb.ToString().Trim();

                if (!string.IsNullOrWhiteSpace(result))
                {
                    Clipboard.SetText(result);
                }
            }
        }

        protected virtual void FontFilesListBoxMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = FontFilesListBox.IndexFromPoint(e.Location);

            if (index == ListBox.NoMatches) { return; }

            var path = (FontFilesListBox.Items[index] as FontFileNameComboBoxItem).FullName;

            Clipboard.SetText(path);
        }

        protected virtual void FontSizeNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                LoadFont(CreateLoadFontRequest());
            }

            FontSizeNumericUpDown.Focus();
        }

        protected virtual void FontSizeTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                LoadFont(CreateLoadFontRequest());
            }
        }

        protected virtual string GetFileNameTemplate()
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;
            if (rasterizer == default) { return string.Empty; }

            return string.Format("{0}_{1}_{2}_{3}",
                                    (FontFamilyComboBox.SelectedItem as FontFamilyNameComboBoxItem).Name.Replace(" ", string.Empty),
                                    rasterizer.Renderer.Settings.Size,
                                    rasterizer.Renderer.Settings.SizeType,
                                    rasterizer.Renderer.Settings.StyleType.ToString().Replace(" ", string.Empty).Replace(",", string.Empty));
        }

        protected string GetFontFamilyName(string fontFileName)
        {
            if (!File.Exists(fontFileName)) { return string.Empty; }

            using (var fonts = new PrivateFontCollection())
            {
                try
                {
                    fonts.AddFontFile(fontFileName);
                }
                catch (Exception) { }

                return fonts.Families.Length != 0 ? fonts.Families[0].Name : string.Empty;
            }
        }

        protected virtual void ImagePreviewDrawGridToolStripButtonCheckedChanged(object sender, EventArgs e)
        {
            var data = ImagePreviewPictureBox.Tag as RasterizerResult;

            if (data == default) { return; }

            var result = ImagePreviewDrawGridToolStripButton.Checked ? new Bitmap(data.Image) : data.Image;

            if (ImagePreviewDrawGridToolStripButton.Checked)
            {
                using (var g = Graphics.FromImage(result))
                {
                    using (var pen = new Pen(Color.Magenta, 1))
                    {
                        foreach (var item in data.Items)
                        {
                            var bounds = item.Bounds;
                            g.DrawRectangle(pen, bounds.MinX, bounds.MinY, bounds.MaxX - bounds.MinX, bounds.MaxY - bounds.MinY);
                        }
                    }
                }
            }

            ImagePreviewPictureBox.Image = result;

            ImagePreviewZoomValueChanged(sender, EventArgs.Empty);
        }

        protected virtual void ImagePreviewPanelHorizontalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            ImagePreviewPictureBox.Left = -e.NewValue;
        }

        protected virtual void ImagePreviewPanelResize(object sender, EventArgs e)
        {
            var point = new Point(
                Math.Max(0, (ImagePreviewPanel.Width - ImagePreviewPictureBox.Width) / 2),
                Math.Max(0, (ImagePreviewPanel.Height - ImagePreviewPictureBox.Height) / 2));

            var extraWidth = ImagePreviewPictureBox.Width - ImagePreviewPanel.Width;
            var extraHeight = ImagePreviewPictureBox.Height - ImagePreviewPanel.Height;

            if (extraWidth > 0)
            {
                ImagePreviewPanelHorizontalScrollBar.Maximum = ImagePreviewPictureBox.Width - ImagePreviewPanel.Width;
            }

            ImagePreviewPanelHorizontalScrollBar.Visible = extraWidth > 0;

            if (extraHeight > 0)
            {
                ImagePreviewPanelVerticalScrollBar.Maximum = ImagePreviewPictureBox.Height - ImagePreviewPanel.Height;
            }

            ImagePreviewPanelVerticalScrollBar.Visible = extraHeight > 0;

            ImagePreviewPictureBox.Location = new Point(point.X - (ImagePreviewPanelHorizontalScrollBar.Visible ? ImagePreviewPanelHorizontalScrollBar.Value : 0),
                                                                point.Y - (ImagePreviewPanelVerticalScrollBar.Visible ? ImagePreviewPanelVerticalScrollBar.Value : 0));
        }

        protected virtual void ImagePreviewPanelVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            ImagePreviewPictureBox.Top = -e.NewValue;
        }

        protected virtual void ImagePreviewSaveDescriptionToolStripButtonClick(object sender, EventArgs e)
        {
            SavePreviewImageDescription();
        }

        protected virtual void ImagePreviewSaveToolStripButtonClick(object sender, EventArgs e)
        {
            SavePreviewImage();
        }

        protected virtual void ImagePreviewZoom100ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 100;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom100ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoom150ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 150;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom150ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoom200ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 200;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom200ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoom300ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 300;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom300ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoom400ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 400;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom400ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoom500ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 500;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom500ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoom50ToolStripMenuItemClick(object sender, EventArgs e)
        {
            ImagePreviewZoomValue = 50;
            ImagePreviewZoomToolStripDropDownButton.Text = ImagePreviewZoom50ToolStripMenuItem.Text;

            ImagePreviewZoomValueChanged(sender, e);
        }

        protected virtual void ImagePreviewZoomValueChanged(object sender, EventArgs e)
        {
            ImagePreviewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (ImagePreviewPictureBox.Image != default)
            {
                ImagePreviewPictureBox.Width = (int)Math.Ceiling(ImagePreviewPictureBox.Image.Width * ImagePreviewZoomValue / 100);
                ImagePreviewPictureBox.Height = (int)Math.Ceiling(ImagePreviewPictureBox.Image.Height * ImagePreviewZoomValue / 100);

                ImagePreviewPanelResize(sender, e);
            }
        }

        protected virtual void ImageTestAlignmentLCenterRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RasterizeTestText();
        }

        protected virtual void ImageTestAlignmentLeftRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RasterizeTestText();
        }

        protected virtual void ImageTestAlignmentRightRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RasterizeTestText();
        }

        protected virtual void ImageTestPanelHorizontalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            ImageTestPictureBox.Left = -e.NewValue;
        }

        protected virtual void ImageTestPanelResize(object sender, EventArgs e)
        {
            if (ImageTestPictureBox.Image != default)
            {
                ImageTestPictureBox.Width = ImageTestPictureBox.Image.Width;
                ImageTestPictureBox.Height = ImageTestPictureBox.Image.Height;
            }

            var point = new Point(
                Math.Max(0, (ImageTestPanel.Width - ImageTestPictureBox.Width) / 2),
                Math.Max(0, (ImageTestPanel.Height - ImageTestPictureBox.Height) / 2));

            var extraWidth = ImageTestPictureBox.Width - ImageTestPanel.Width;
            var extraHeight = ImageTestPictureBox.Height - ImageTestPanel.Height;

            if (extraWidth > 0)
            {
                ImageTestPanelHorizontalScrollBar.Maximum = ImageTestPictureBox.Width - ImageTestPanel.Width;
            }

            ImageTestPanelHorizontalScrollBar.Visible = extraWidth > 0;

            if (extraHeight > 0)
            {
                ImageTestPanelVerticalScrollBar.Maximum = ImageTestPictureBox.Height - ImageTestPanel.Height;
            }

            ImageTestPanelVerticalScrollBar.Visible = extraHeight > 0;

            ImageTestPictureBox.Location = new Point(point.X - (ImageTestPanelHorizontalScrollBar.Visible ? ImageTestPanelHorizontalScrollBar.Value : 0),
                                                                point.Y - (ImageTestPanelVerticalScrollBar.Visible ? ImageTestPanelVerticalScrollBar.Value : 0));
        }

        protected virtual void ImageTestPanelVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            ImageTestPictureBox.Top = -e.NewValue;
        }

        protected virtual void ImageTestTextBoxTextChanged(object sender, EventArgs e)
        {
            RasterizeTestText();
        }

        protected virtual void ImportConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            ConfigurationOpenFileDialog.FileName = string.Empty;

            if (ConfigurationOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settingsString = File.ReadAllText(ConfigurationOpenFileDialog.FileName, Encoding.UTF8);

                    var settings = JsonConvert.DeserializeObject<RasterizerSettings>(settingsString);

                    if (settings == default || settings.RendererSettings == default || settings.ComposerSettings == default)
                    {
                        MessageBox.Show(this, "Unable to load the configuration file.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    IsAutomaticUserInterfaceChange = true;

                    FontSizeNumericUpDown.Value = (decimal)settings.RendererSettings.Size;
                    FontSizeTypeComboBox.SelectedIndex = settings.RendererSettings.SizeType == FontSizeType.Pixel ? 0 : 1;

                    RegularFontStyleButton.Checked = settings.RendererSettings.StyleType.HasFlag(FontStyleType.Regular);
                    BoldFontStyleButton.Checked = settings.RendererSettings.StyleType.HasFlag(FontStyleType.Bold);
                    ItalicFontStyleButton.Checked = settings.RendererSettings.StyleType.HasFlag(FontStyleType.Italic);
                    UnderlineFontStyleButton.Checked = settings.RendererSettings.StyleType.HasFlag(FontStyleType.Underline);
                    StrikeoutFontStyleButton.Checked = settings.RendererSettings.StyleType.HasFlag(FontStyleType.Strikeout);

                    FontColorPictureBox.BackColor = settings.RendererSettings.Color;
                    BackColorPictureBox.BackColor = settings.RendererSettings.BackColor;
                    DpiNumericUpDown.Value = settings.RendererSettings.DPI;
                    AllowEmptyCheckBox.Checked = settings.RendererSettings.IsEmptyAllowed;

                    PaddingLeftNumericUpDown.Value = settings.RendererSettings.Padding == default ? 0 : settings.RendererSettings.Padding.Left;
                    PaddingTopNumericUpDown.Value = settings.RendererSettings.Padding == default ? 0 : settings.RendererSettings.Padding.Top;
                    PaddingRightNumericUpDown.Value = settings.RendererSettings.Padding == default ? 0 : settings.RendererSettings.Padding.Right;
                    PaddingBottomNumericUpDown.Value = settings.RendererSettings.Padding == default ? 0 : settings.RendererSettings.Padding.Bottom;

                    OutlineColorPictureBox.BackColor = settings.RendererSettings.Outline == default ? Color.Maroon : settings.RendererSettings.Outline.Color;
                    OutlineThicknessNumericUpDown.Value = settings.RendererSettings.Outline == default ? 1 : settings.RendererSettings.Outline.Size;

                    ScaleHeightNumericUpDown.Value = (decimal)settings.RendererSettings.Scale.Height * 200;
                    ScaleWidthNumericUpDown.Value = (decimal)settings.RendererSettings.Scale.Width * 100;

                    PowerOfTwoCheckBox.Checked = settings.ComposerSettings.IsPowerOfTwo;
                    MonospaceCheckBox.Checked = settings.ComposerSettings.IsMonospace;

                    MarginLeftNumericUpDown.Value = settings.ComposerSettings.Margin == default ? 0 : settings.ComposerSettings.Margin.Left;
                    MarginTopNumericUpDown.Value = settings.ComposerSettings.Margin == default ? 0 : settings.ComposerSettings.Margin.Left;
                    MarginRightNumericUpDown.Value = settings.ComposerSettings.Margin == default ? 0 : settings.ComposerSettings.Margin.Left;
                    MarginBottomNumericUpDown.Value = settings.ComposerSettings.Margin == default ? 0 : settings.ComposerSettings.Margin.Left;

                    CharactersTextBox.Text = DefaultCharacters;

                    IsAutomaticUserInterfaceChange = false;

                    FontDirectoryTextBox.Text = Path.GetDirectoryName(settings.RendererSettings.Fonts.FirstOrDefault() ?? DefaultFontDirectoryPath);
                    LoadFonts(settings.RendererSettings.Fonts);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void ItalicFontStyleButtonCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            Focus();

            var isChangeAllowed = RegularFontStyleButton.Enabled || BoldFontStyleButton.Enabled || UnderlineFontStyleButton.Enabled || StrikeoutFontStyleButton.Enabled;

            IsAutomaticUserInterfaceChange = true;

            if (!isChangeAllowed)
            {
                ItalicFontStyleButton.Checked = !ItalicFontStyleButton.Checked;
                IsAutomaticUserInterfaceChange = false;
                return;
            }

            if (RegularFontStyleButton.Enabled)
            {
                RegularFontStyleButton.Checked = !BoldFontStyleButton.Checked && !ItalicFontStyleButton.Checked && !UnderlineFontStyleButton.Checked && !StrikeoutFontStyleButton.Checked;
            }

            IsAutomaticUserInterfaceChange = false;

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                LoadFont(CreateLoadFontRequest());
            }
        }

        protected virtual void LoadFont(LoadFontRequest request)
        {
            var result = default(LoadFontResult);

            using (var fonts = new PrivateFontCollection())
            {
                foreach (var fontPath in request.Files)
                {
                    if (File.Exists(fontPath))
                    {
                        fonts.AddFontFile(fontPath);
                    }
                }

                if (fonts.Families.Length != 0)
                {

                    var fontFamily = fonts.Families[0];

                    result = new LoadFontResult()
                    {
                        IsRegularAvailable = fontFamily.IsStyleAvailable(FontStyle.Regular),
                        IsBoldAvailable = fontFamily.IsStyleAvailable(FontStyle.Bold),
                        IsItalicAvailable = fontFamily.IsStyleAvailable(FontStyle.Italic),
                        IsUnderlineAvailable = fontFamily.IsStyleAvailable(FontStyle.Underline),
                        IsStrikeoutAvailable = fontFamily.IsStyleAvailable(FontStyle.Strikeout)
                    };

                    result.IsBoldSelected = result.IsBoldAvailable && (request.IsBoldRequested || !result.IsRegularAvailable);
                    result.IsItalicSelected = (result.IsItalicAvailable && (request.IsItalicRequested || (!result.IsRegularAvailable && !result.IsBoldAvailable)));
                    result.IsUnderlineSelected = result.IsUnderlineAvailable && (request.IsUnderlineRequested || (!result.IsRegularAvailable && !result.IsBoldAvailable && !result.IsItalicAvailable));
                    result.IsStrikeoutSelected = result.IsStrikeoutAvailable && (request.IsStrikeoutRequested || (!result.IsRegularAvailable && !result.IsBoldAvailable && !result.IsItalicAvailable && !result.IsStrikeoutAvailable));

                    result.Style = FontStyle.Regular;

                    if (result.IsBoldSelected)
                    {
                        result.Style |= FontStyle.Bold;
                    }

                    if (result.IsItalicSelected)
                    {
                        result.Style |= FontStyle.Italic;
                    }

                    if (result.IsUnderlineSelected)
                    {
                        result.Style |= FontStyle.Underline;
                    }

                    if (result.IsStrikeoutSelected)
                    {
                        result.Style |= FontStyle.Strikeout;
                    }

                    if (!result.IsRegularAvailable)
                    {
                        result.Style ^= FontStyle.Regular;
                    }

                    var renderer = new Renderer(new RendererSettings(request.Files)
                    {
                        IsEmptyAllowed = AllowEmptyCheckBox.Checked,
                        BackColor = request.BackColor,
                        DPI = request.DPI,
                        Color = request.FontColor,
                        Padding = new Spacing
                        {
                            Bottom = (int)PaddingBottomNumericUpDown.Value,
                            Left = (int)PaddingLeftNumericUpDown.Value,
                            Right = (int)PaddingRightNumericUpDown.Value,
                            Top = (int)PaddingTopNumericUpDown.Value
                        },
                        Outline = new Outline()
                        {
                            Color = OutlineColorPictureBox.BackColor,
                            Size = (int)OutlineThicknessNumericUpDown.Value
                        },
                        Scale = new Scale()
                        {
                            Height = (float)ScaleHeightNumericUpDown.Value / 100,
                            Width = (float)ScaleWidthNumericUpDown.Value / 100
                        },
                        Size = request.Size,
                        SizeType = request.SizeType,
                        StyleType = (FontStyleType)result.Style,
                    });

                    var composer = new Composer(new ComposerSettings()
                    {
                        Color = request.BackColor,
                        IsPowerOfTwo = PowerOfTwoCheckBox.Checked,
                        IsMonospace = MonospaceCheckBox.Checked,
                        Margin = new Spacing()
                        {
                            Bottom = (int)MarginBottomNumericUpDown.Value,
                            Left = (int)MarginLeftNumericUpDown.Value,
                            Right = (int)MarginTopNumericUpDown.Value,
                            Top = (int)MarginTopNumericUpDown.Value
                        }
                    });

                    result.Rasterizer = new RasterizerWrapper()
                    {
                        Composer = composer,
                        Rasterizer = new Rasterizer(renderer, composer),
                        Renderer = renderer
                    };
                }
            }

            if (result == default)
            {
                ImagePreviewPanel.Tag = default;
                ImagePreviewPictureBox.Tag = default;
                ImagePreviewPictureBox.Image = default;
            }

            IsAutomaticUserInterfaceChange = true;

            RegularFontStyleButton.Enabled = result.IsRegularAvailable;
            RegularFontStyleButton.Checked = !result.IsBoldSelected && !result.IsItalicSelected && !result.IsUnderlineSelected && !result.IsStrikeoutSelected;
            BoldFontStyleButton.Enabled = result.IsBoldAvailable;
            BoldFontStyleButton.Checked = result.IsBoldSelected;
            ItalicFontStyleButton.Enabled = result.IsItalicAvailable;
            ItalicFontStyleButton.Checked = result.IsItalicSelected;
            UnderlineFontStyleButton.Enabled = result.IsUnderlineAvailable;
            UnderlineFontStyleButton.Checked = result.IsUnderlineSelected;
            StrikeoutFontStyleButton.Enabled = result.IsStrikeoutAvailable;
            StrikeoutFontStyleButton.Checked = result.IsStrikeoutSelected;

            IsAutomaticUserInterfaceChange = false;

            ImagePreviewPanel.Tag = result.Rasterizer;

            RenderPreviewImage();
            RenderExampleImage();
        }

        protected virtual void LoadFonts(IEnumerable<string> fontPaths)
        {
            using (var window = new WorkerWindow() { Action = () => { return LoadFontsAction(fontPaths); } })
            {
                var result = window.ShowDialog(this);

                FontFamilyComboBox.Items.Clear();
                FontFilesListBox.Items.Clear();

                SetUIControlsEnabledProperty(result == DialogResult.OK);

                if (result == DialogResult.OK)
                {
                    LoadFontsCompleted(window.Result as FontFamilyNameComboBoxItem[]);
                    return;
                }
            }
        }

        protected virtual void LoadFonts(string fontDirectory)
        {
            using (var window = new WorkerWindow() { Action = () => { return LoadFontsAction(fontDirectory); } })
            {
                var result = window.ShowDialog(this);

                FontFamilyComboBox.Items.Clear();
                FontFilesListBox.Items.Clear();

                SetUIControlsEnabledProperty(result == DialogResult.OK);

                if (result == DialogResult.OK)
                {
                    LoadFontsCompleted(window.Result as FontFamilyNameComboBoxItem[]);
                    return;
                }
            }
        }

        protected virtual object LoadFontsAction(string fontDirectory)
        {
            if (string.IsNullOrEmpty(fontDirectory)) { return default; }

            var files = Directory.GetFiles(fontDirectory, "*.ttf", SearchOption.AllDirectories);

            return files
                .Select(fileName => new KeyValuePair<string, string>(GetFontFamilyName(fileName), fileName))
                .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                .GroupBy(kvp => kvp.Key)
                .Select(g => new FontFamilyNameComboBoxItem()
                {
                    Name = g.Key,
                    Files = g.Select(i => i.Value).OrderBy(i => i).ToArray()
                })
                .OrderBy(i => i.Name)
                .ToArray();
        }

        protected virtual object LoadFontsAction(IEnumerable<string> fontPaths)
        {
            if (fontPaths == default || !fontPaths.Any()) { return default; }

            return fontPaths
                .Select(fileName => new KeyValuePair<string, string>(GetFontFamilyName(fileName), fileName))
                .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                .GroupBy(kvp => kvp.Key)
                .Select(g => new FontFamilyNameComboBoxItem()
                {
                    Name = g.Key,
                    Files = g.Select(i => i.Value).OrderBy(i => i).ToArray()
                })
                .OrderBy(i => i.Name)
                .ToArray();
        }

        protected virtual void LoadFontsCompleted(FontFamilyNameComboBoxItem[] items)
        {
            FontFamilyComboBox.Items.AddRange(items);
            FontSizeNumericUpDown.Enabled = true;
            FontSizeTypeComboBox.Enabled = true;
            FontFamilyComboBox.SelectedItem = items.Length != 0 ? items[0] : default;
        }

        protected virtual void MainWindowFormClosed(object sender, FormClosedEventArgs e)
        {
            var existingRasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (existingRasterizer != default)
            {
                existingRasterizer.Dispose();
            }
        }

        protected virtual void MainWindowLoad(object sender, EventArgs e)
        {
            FontDirectoryTextBox.Text = DefaultFontDirectoryPath;

            FontSizeTypeComboBox.Items.AddRange(new[] {
                new FontSizeTypeComboBoxItem() { SizeType = FontSizeType.Pixel },
                new FontSizeTypeComboBoxItem() { SizeType = FontSizeType.Point }
            });

            FontSizeTypeComboBox.SelectedIndex = 0;
        }

        protected virtual void MainWindowShown(object sender, EventArgs e)
        {
            LoadFonts(DefaultFontDirectoryPath);
        }

        protected virtual void MarginBottomNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.Margin.Bottom = (int)MarginBottomNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            MarginBottomNumericUpDown.Focus();
        }

        protected virtual void MarginLeftNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.Margin.Left = (int)MarginLeftNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            MarginLeftNumericUpDown.Focus();
        }

        protected virtual void MarginRightNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.Margin.Right = (int)MarginRightNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            MarginRightNumericUpDown.Focus();
        }

        protected virtual void MarginTopNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.Margin.Top = (int)MarginTopNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            MarginTopNumericUpDown.Focus();
        }

        protected virtual void MonospaceCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.IsMonospace = MonospaceCheckBox.Checked;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }
        }

        protected virtual void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            IsAutomaticUserInterfaceChange = true;

            FontSizeNumericUpDown.Value = 24;
            FontSizeTypeComboBox.SelectedIndex = 0;

            FontColorPictureBox.BackColor = Color.White;
            BackColorPictureBox.BackColor = Color.Black;
            DpiNumericUpDown.Value = 96;
            AllowEmptyCheckBox.Checked = true;

            PaddingLeftNumericUpDown.Value = 0;
            PaddingTopNumericUpDown.Value = 0;
            PaddingRightNumericUpDown.Value = 0;
            PaddingBottomNumericUpDown.Value = 0;

            OutlineColorPictureBox.BackColor = Color.Maroon;
            OutlineThicknessNumericUpDown.Value = 0;

            ScaleHeightNumericUpDown.Value = 100;
            ScaleWidthNumericUpDown.Value = 100;

            PowerOfTwoCheckBox.Checked = true;
            MonospaceCheckBox.Checked = false;

            MarginLeftNumericUpDown.Value = 0;
            MarginTopNumericUpDown.Value = 0;
            MarginRightNumericUpDown.Value = 0;
            MarginBottomNumericUpDown.Value = 0;

            CharactersTextBox.Text = DefaultCharacters;

            IsAutomaticUserInterfaceChange = false;

            FontDirectoryTextBox.Text = DefaultFontDirectoryPath;
            LoadFonts(DefaultFontDirectoryPath);
        }

        protected virtual void OutlineColorPictureBoxClick(object sender, EventArgs e)
        {
            MainColorDialog.Color = OutlineColorPictureBox.BackColor;

            if (MainColorDialog.ShowDialog(this) == DialogResult.OK)
            {
                OutlineColorPictureBox.BackColor = MainColorDialog.Color;

                var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                if (rasterizer != default)
                {
                    rasterizer.Renderer.Settings.Outline.Color = MainColorDialog.Color;

                    if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                    RasterizeFont(new RasterizeRequest()
                    {
                        Rasterizer = rasterizer,
                        Text = CharactersTextBox.Text
                    });
                }
            }
        }

        protected virtual void OutlineThicknessNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Renderer.Settings.Outline.Size = (int)OutlineThicknessNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            OutlineThicknessNumericUpDown.Focus();
        }

        protected virtual void PaddingBottomNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Renderer.Settings.Padding.Bottom = (int)PaddingBottomNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            PaddingBottomNumericUpDown.Focus();
        }

        protected virtual void PaddingLeftNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Renderer.Settings.Padding.Left = (int)PaddingLeftNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            PaddingLeftNumericUpDown.Focus();
        }

        protected virtual void PaddingRightNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Renderer.Settings.Padding.Right = (int)PaddingRightNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            PaddingRightNumericUpDown.Focus();
        }

        protected virtual void PaddingTopNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Renderer.Settings.Padding.Top = (int)PaddingTopNumericUpDown.Value;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }

            PaddingTopNumericUpDown.Focus();
        }

        protected virtual void PowerOfTwoCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer != default)
            {
                rasterizer.Composer.Settings.IsPowerOfTwo = PowerOfTwoCheckBox.Checked;

                if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                RasterizeFont(new RasterizeRequest()
                {
                    Rasterizer = rasterizer,
                    Text = CharactersTextBox.Text
                });
            }
        }

        protected virtual void PreviewTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ImageTabControl.SelectedIndex == 0) { return; }

            RasterizeTestText();
        }

        protected virtual void RasterizeFont(RasterizeRequest request)
        {
            if (request == default) { return; }

            using (var window = new WorkerWindow() { Action = () => { return RasterizeFontAction(request); } })
            {
                if (window.ShowDialog(this) == DialogResult.OK)
                {
                    RasterizeFontCompleted(window.Result as RasterizerResult);
                }
            }
        }

        protected virtual RasterizerResult RasterizeFontAction(RasterizeRequest request)
        {
            return request.Rasterizer.Rasterizer.Rasterize(request.Text.ToCharArray());
        }

        protected virtual void RasterizeFontCompleted(RasterizerResult result)
        {
            if (result == default) { return; }
            if (result.Image == default) { return; }

            ImagePreviewPictureBox.Tag = result;
            ImagePreviewPictureBox.Image = result.Image;

            ImagePreviewZoomValueChanged(this, EventArgs.Empty);
            ImagePreviewDrawGridToolStripButtonCheckedChanged(this, EventArgs.Empty);

            ImagePreviewPictureBox.Visible = true;

            if (ImageTabControl.SelectedIndex == 1)
            {
                RasterizeTestText();
            }
        }

        protected void RasterizeTestText()
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;
            if (rasterizer == default) { return; }

            var data = ImagePreviewPictureBox.Tag as RasterizerResult;
            if (data == default) { return; }

            if (string.IsNullOrEmpty(ImageTestTextBox.Text)) { return; }

            var lines = ImageTestTextBox.Lines;

            var dictionary = data.Items.ToDictionary(i => i.Item);

            var height = 0;
            var width = 0;

            var lineCharacters = new List<List<RenderResult>>(lines.Length);

            for (var x = 0; x < lines.Length; x++)
            {
                var currentHeight = 0;
                var currentWidth = 0;

                lineCharacters.Add(new List<RenderResult>(lines[x].Length));

                foreach (var character in lines[x])
                {
                    if (dictionary.TryGetValue(character, out var item))
                    {
                        lineCharacters[x].Add(item);

                        currentHeight = Math.Max(currentHeight, item.Bounds.MaxY - item.Bounds.MinY);
                        currentWidth += item.Bounds.MaxX - item.Bounds.MinX;
                    }
                }

                height += currentHeight;
                width = Math.Max(width, currentWidth);
            }

            var alignment = ImageTestAlignmentLeftRadioButton.Checked
                                ? ContentAlignment.MiddleLeft
                                : (ImageTestAlignmentRightRadioButton.Checked ? ContentAlignment.MiddleRight : ContentAlignment.MiddleCenter);

            var result = new Bitmap(width, height);

            using (var g = Graphics.FromImage(result))
            {
                g.Clear(BackColorPictureBox.BackColor);

                var offsetY = 0;
                foreach (var line in lineCharacters)
                {
                    var currentOffsetX = 0;
                    var currentOffsetY = 0;

                    var lineWidth = line.Sum(c => c.Bounds.MaxX - c.Bounds.MinX);

                    switch (alignment)
                    {
                        case ContentAlignment.MiddleCenter:
                            {
                                currentOffsetX = (width - lineWidth) / 2;
                                break;
                            }
                        case ContentAlignment.MiddleRight:
                            {
                                currentOffsetX = width - lineWidth;
                                break;
                            }
                    }

                    foreach (var character in line)
                    {
                        g.DrawImage(character.Image, currentOffsetX, offsetY);

                        currentOffsetX += character.Bounds.MaxX - character.Bounds.MinX;
                        currentOffsetY = Math.Max(currentOffsetY, character.Bounds.MaxY - character.Bounds.MinY);
                    }

                    offsetY += currentOffsetY;
                }
            }

            ImageTestPictureBox.Image = result;

            ImageTestPanelResize(this, EventArgs.Empty);
        }

        protected virtual void RegularFontStyleButtonCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            Focus();

            var isChangeAllowed = BoldFontStyleButton.Enabled || ItalicFontStyleButton.Enabled || UnderlineFontStyleButton.Enabled || StrikeoutFontStyleButton.Enabled;

            IsAutomaticUserInterfaceChange = true;

            if (!isChangeAllowed)
            {
                RegularFontStyleButton.Checked = !RegularFontStyleButton.Checked;
                IsAutomaticUserInterfaceChange = false;
                return;
            }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                // Revert back to Regullar
                if (BoldFontStyleButton.Enabled)
                {
                    BoldFontStyleButton.Checked = false;
                }

                if (ItalicFontStyleButton.Enabled)
                {
                    ItalicFontStyleButton.Checked = false;
                }

                if (UnderlineFontStyleButton.Enabled)
                {
                    UnderlineFontStyleButton.Checked = false;
                }

                if (StrikeoutFontStyleButton.Enabled)
                {
                    StrikeoutFontStyleButton.Checked = false;
                }

                IsAutomaticUserInterfaceChange = false;

                LoadFont(CreateLoadFontRequest());
            }
        }

        protected virtual void RenderExampleImage()
        {
            if (ImageTabControl.SelectedIndex == 0) { return; }
        }

        protected virtual void RenderPreviewImage()
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

            if (rasterizer == default) { return; }

            if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

            RasterizeFont(new RasterizeRequest()
            {
                Rasterizer = rasterizer,
                Text = CharactersTextBox.Text
            });
        }

        protected virtual void ReportAnIssueToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Frasterizer/issues") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }

        protected virtual void SaveImageDescriptionToolStripMenuItemClick(object sender, EventArgs e)
        {
            SavePreviewImageDescription();
        }

        protected virtual void SaveImageToolStripMenuItemClick(object sender, EventArgs e)
        {
            SavePreviewImage();
        }

        protected virtual void SavePreviewImage()
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;
            if (rasterizer == default) { return; }

            var data = ImagePreviewPictureBox.Tag as RasterizerResult;
            if (data == default) { return; }

            ImageSaveFileDialog.FileName = GetFileNameTemplate();


            if (ImageSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var extension = Path.GetExtension(ImageSaveFileDialog.FileName).ToLowerInvariant()
                                        .Replace(".", string.Empty).Replace("ico", "icon").Replace("jpg", "jpeg").Replace("tif", "tiff"); ;

                var imageFormatProperty = typeof(ImageFormat).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                                                                .FirstOrDefault(p => p.Name.ToLowerInvariant() == extension);
                if (imageFormatProperty == default)
                {
                    MessageBox.Show(this, string.Format("Unable to save the image in {0} format.", extension.ToUpperInvariant()), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var imageFormat = (ImageFormat)imageFormatProperty.GetValue(default, default);

                try
                {
                    data.Image.Save(ImageSaveFileDialog.FileName, imageFormat);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void SavePreviewImageDescription()
        {
            var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;
            if (rasterizer == default) { return; }

            var data = ImagePreviewPictureBox.Tag as RasterizerResult;
            if (data == default) { return; }

            ImageDescriptionSaveFileDialog.FileName = GetFileNameTemplate();


            if (ImageDescriptionSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var items = data.Items.Select(i => new { Item = i.Item, Bounds = i.Bounds }).ToArray();

                var output = JsonConvert.SerializeObject(items);

                try
                {
                    File.WriteAllText(ImageDescriptionSaveFileDialog.FileName, output, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void ScaleHeightNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                if (rasterizer != default)
                {
                    rasterizer.Renderer.Settings.Scale.Height = (float)ScaleHeightNumericUpDown.Value / 100;

                    if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                    RasterizeFont(new RasterizeRequest()
                    {
                        Rasterizer = rasterizer,
                        Text = CharactersTextBox.Text
                    });
                }
            }

            ScaleHeightNumericUpDown.Focus();
        }

        protected virtual void ScaleWidthNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                var rasterizer = ImagePreviewPanel.Tag as RasterizerWrapper;

                if (rasterizer != default)
                {
                    rasterizer.Renderer.Settings.Scale.Width = (float)ScaleWidthNumericUpDown.Value / 100;

                    if (string.IsNullOrEmpty(CharactersTextBox.Text)) { return; }

                    RasterizeFont(new RasterizeRequest()
                    {
                        Rasterizer = rasterizer,
                        Text = CharactersTextBox.Text
                    });
                }
            }

            ScaleWidthNumericUpDown.Focus();
        }

        protected virtual void SetUIControlsEnabledProperty(bool isEnabled)
        {
            IsAutomaticUserInterfaceChange = true;

            FontFamilyComboBox.Enabled = isEnabled;
            FontFilesListBox.Enabled = isEnabled;

            RegularFontStyleButton.Enabled = isEnabled;
            BoldFontStyleButton.Enabled = isEnabled;
            ItalicFontStyleButton.Enabled = isEnabled;
            UnderlineFontStyleButton.Enabled = isEnabled;
            StrikeoutFontStyleButton.Enabled = isEnabled;

            FontSizeNumericUpDown.Enabled = isEnabled;
            FontSizeTypeComboBox.Enabled = isEnabled;

            OptionsGroupBox.Enabled = isEnabled;
            CompositionGroupBox.Enabled = isEnabled;

            ImagePreviewToolStrip.Enabled = isEnabled;

            ImagePreviewPanel.Tag = default;
            ImagePreviewPictureBox.Tag = default;
            ImagePreviewPictureBox.Visible = isEnabled;

            ImageTestPictureBox.Image = default;
            ImageTestAlignmentGroupBox.Enabled = isEnabled;
            ImageTestTextBox.Enabled = isEnabled;

            SaveImageToolStripMenuItem.Enabled = isEnabled;
            SaveImageDescriptionToolStripMenuItem.Enabled = isEnabled;
            ExportConfigurationToolStripMenuItem.Enabled = isEnabled;

            IsAutomaticUserInterfaceChange = false;
        }

        protected virtual void StrikeoutFontStyleButtonCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            Focus();

            var isChangeAllowed = RegularFontStyleButton.Enabled || BoldFontStyleButton.Enabled || ItalicFontStyleButton.Enabled || UnderlineFontStyleButton.Enabled;

            IsAutomaticUserInterfaceChange = true;

            if (!isChangeAllowed)
            {
                StrikeoutFontStyleButton.Checked = !StrikeoutFontStyleButton.Checked;
                IsAutomaticUserInterfaceChange = false;
                return;
            }

            if (RegularFontStyleButton.Enabled)
            {
                RegularFontStyleButton.Checked = !BoldFontStyleButton.Checked && !ItalicFontStyleButton.Checked && !UnderlineFontStyleButton.Checked && !StrikeoutFontStyleButton.Checked;
            }

            IsAutomaticUserInterfaceChange = false;

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                LoadFont(CreateLoadFontRequest());
            }
        }

        protected virtual void UnderlineFontStyleButtonCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticUserInterfaceChange) { return; }

            Focus();

            var isChangeAllowed = RegularFontStyleButton.Enabled || BoldFontStyleButton.Enabled || ItalicFontStyleButton.Enabled || StrikeoutFontStyleButton.Enabled;

            IsAutomaticUserInterfaceChange = true;

            if (!isChangeAllowed)
            {
                UnderlineFontStyleButton.Checked = !UnderlineFontStyleButton.Checked;
                IsAutomaticUserInterfaceChange = false;
                return;
            }

            if (RegularFontStyleButton.Enabled)
            {
                RegularFontStyleButton.Checked = !BoldFontStyleButton.Checked && !ItalicFontStyleButton.Checked && !UnderlineFontStyleButton.Checked && !StrikeoutFontStyleButton.Checked;
            }

            IsAutomaticUserInterfaceChange = false;

            if (FontFamilyComboBox.SelectedItem is FontFamilyNameComboBoxItem)
            {
                LoadFont(CreateLoadFontRequest());
            }
        }

        protected virtual void VisitWebsiteToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Frasterizer") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }
    }
}
