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

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Frasterizer.UI.Windows
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();

            VersionValueLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        protected virtual void MITLicenceLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Frasterizer/blob/main/LICENSE") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }

        protected virtual void WebSiteValueLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Frasterizer") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }
    }
}
