
namespace Frasterizer.UI.Windows
{
    partial class AboutWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NameLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.VersionValueLabel = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.CopyrightValueLabel = new System.Windows.Forms.Label();
            this.WebSiteLabel = new System.Windows.Forms.Label();
            this.LicenseLabel = new System.Windows.Forms.Label();
            this.WebSiteValueLinkLabel = new System.Windows.Forms.LinkLabel();
            this.MITLicenceLinkLabel = new System.Windows.Forms.LinkLabel();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.BottomPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.BottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(307, 29);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(185, 39);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Frasterizer";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Location = new System.Drawing.Point(311, 133);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(49, 13);
            this.VersionLabel.TabIndex = 1;
            this.VersionLabel.Text = "Version";
            // 
            // VersionValueLabel
            // 
            this.VersionValueLabel.AutoSize = true;
            this.VersionValueLabel.Location = new System.Drawing.Point(401, 133);
            this.VersionValueLabel.Name = "VersionValueLabel";
            this.VersionValueLabel.Size = new System.Drawing.Size(0, 13);
            this.VersionValueLabel.TabIndex = 2;
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.AutoSize = true;
            this.CopyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyrightLabel.Location = new System.Drawing.Point(311, 23);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(60, 13);
            this.CopyrightLabel.TabIndex = 3;
            this.CopyrightLabel.Text = "Copyright";
            // 
            // CopyrightValueLabel
            // 
            this.CopyrightValueLabel.AutoSize = true;
            this.CopyrightValueLabel.Location = new System.Drawing.Point(401, 24);
            this.CopyrightValueLabel.Name = "CopyrightValueLabel";
            this.CopyrightValueLabel.Size = new System.Drawing.Size(133, 13);
            this.CopyrightValueLabel.TabIndex = 4;
            this.CopyrightValueLabel.Text = "© 2020 Americus Maximus";
            // 
            // WebSiteLabel
            // 
            this.WebSiteLabel.AutoSize = true;
            this.WebSiteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WebSiteLabel.Location = new System.Drawing.Point(311, 54);
            this.WebSiteLabel.Name = "WebSiteLabel";
            this.WebSiteLabel.Size = new System.Drawing.Size(59, 13);
            this.WebSiteLabel.TabIndex = 5;
            this.WebSiteLabel.Text = "Web Site";
            // 
            // LicenseLabel
            // 
            this.LicenseLabel.AutoSize = true;
            this.LicenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LicenseLabel.Location = new System.Drawing.Point(311, 83);
            this.LicenseLabel.Name = "LicenseLabel";
            this.LicenseLabel.Size = new System.Drawing.Size(51, 13);
            this.LicenseLabel.TabIndex = 7;
            this.LicenseLabel.Text = "License";
            // 
            // WebSiteValueLinkLabel
            // 
            this.WebSiteValueLinkLabel.AutoSize = true;
            this.WebSiteValueLinkLabel.Location = new System.Drawing.Point(401, 55);
            this.WebSiteValueLinkLabel.Name = "WebSiteValueLinkLabel";
            this.WebSiteValueLinkLabel.Size = new System.Drawing.Size(63, 13);
            this.WebSiteValueLinkLabel.TabIndex = 9;
            this.WebSiteValueLinkLabel.TabStop = true;
            this.WebSiteValueLinkLabel.Text = "GitHub.com";
            this.WebSiteValueLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebSiteValueLinkLabelLinkClicked);
            // 
            // MITLicenceLinkLabel
            // 
            this.MITLicenceLinkLabel.AutoSize = true;
            this.MITLicenceLinkLabel.Location = new System.Drawing.Point(401, 84);
            this.MITLicenceLinkLabel.Name = "MITLicenceLinkLabel";
            this.MITLicenceLinkLabel.Size = new System.Drawing.Size(67, 13);
            this.MITLicenceLinkLabel.TabIndex = 10;
            this.MITLicenceLinkLabel.TabStop = true;
            this.MITLicenceLinkLabel.Text = "MIT Licence";
            this.MITLicenceLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MITLicenceLinkLabelLinkClicked);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(309, 80);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(267, 25);
            this.DescriptionLabel.TabIndex = 11;
            this.DescriptionLabel.Text = "A Font to Image Rasterizer";
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.BackColor = System.Drawing.Color.White;
            this.LogoPictureBox.Image = global::Frasterizer.UI.Properties.Resources.Image;
            this.LogoPictureBox.Location = new System.Drawing.Point(12, 12);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(256, 256);
            this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.LogoPictureBox.TabIndex = 12;
            this.LogoPictureBox.TabStop = false;
            // 
            // BottomPanel
            // 
            this.BottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.BottomPanel.Controls.Add(this.MITLicenceLinkLabel);
            this.BottomPanel.Controls.Add(this.WebSiteValueLinkLabel);
            this.BottomPanel.Controls.Add(this.LicenseLabel);
            this.BottomPanel.Controls.Add(this.WebSiteLabel);
            this.BottomPanel.Controls.Add(this.CopyrightValueLabel);
            this.BottomPanel.Controls.Add(this.CopyrightLabel);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(0, 173);
            this.BottomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(624, 110);
            this.BottomPanel.TabIndex = 13;
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(624, 283);
            this.Controls.Add(this.LogoPictureBox);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.BottomPanel);
            this.Controls.Add(this.VersionValueLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AboutWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.BottomPanel.ResumeLayout(false);
            this.BottomPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label VersionValueLabel;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.Label CopyrightValueLabel;
        private System.Windows.Forms.Label WebSiteLabel;
        private System.Windows.Forms.Label LicenseLabel;
        private System.Windows.Forms.LinkLabel WebSiteValueLinkLabel;
        private System.Windows.Forms.LinkLabel MITLicenceLinkLabel;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.Panel BottomPanel;
    }
}