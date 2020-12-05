namespace Frasterizer.UI.Windows
{
    partial class MapWindow
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
            this.components = new System.ComponentModel.Container();
            this.MainToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.CharacterLabel = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.CharactersSetSearchTextBox = new System.Windows.Forms.TextBox();
            this.CharactersSetGroupBox = new System.Windows.Forms.GroupBox();
            this.CharactersSetFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.OkButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.CharactersGroupBox = new System.Windows.Forms.GroupBox();
            this.CharactersPanel = new System.Windows.Forms.Panel();
            this.CharacterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CharacterValueLabel = new System.Windows.Forms.Label();
            this.ValueValueLabel = new System.Windows.Forms.Label();
            this.CharactersSetGroupBox.SuspendLayout();
            this.CharactersGroupBox.SuspendLayout();
            this.CharactersPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CharacterLabel
            // 
            this.CharacterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CharacterLabel.AutoSize = true;
            this.CharacterLabel.Location = new System.Drawing.Point(312, 472);
            this.CharacterLabel.Name = "CharacterLabel";
            this.CharacterLabel.Size = new System.Drawing.Size(56, 13);
            this.CharacterLabel.TabIndex = 2;
            this.CharacterLabel.Text = "Character:";
            // 
            // ValueLabel
            // 
            this.ValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Location = new System.Drawing.Point(405, 472);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(37, 13);
            this.ValueLabel.TabIndex = 3;
            this.ValueLabel.Text = "Value:";
            // 
            // CharactersSetSearchTextBox
            // 
            this.CharactersSetSearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CharactersSetSearchTextBox.Location = new System.Drawing.Point(6, 19);
            this.CharactersSetSearchTextBox.Name = "CharactersSetSearchTextBox";
            this.CharactersSetSearchTextBox.Size = new System.Drawing.Size(277, 20);
            this.CharactersSetSearchTextBox.TabIndex = 0;
            this.CharactersSetSearchTextBox.TextChanged += new System.EventHandler(this.CharactersSetSearchTextBoxTextChanged);
            // 
            // CharactersSetGroupBox
            // 
            this.CharactersSetGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CharactersSetGroupBox.Controls.Add(this.CharactersSetFlowLayoutPanel);
            this.CharactersSetGroupBox.Controls.Add(this.CharactersSetSearchTextBox);
            this.CharactersSetGroupBox.Location = new System.Drawing.Point(12, 12);
            this.CharactersSetGroupBox.Name = "CharactersSetGroupBox";
            this.CharactersSetGroupBox.Size = new System.Drawing.Size(289, 478);
            this.CharactersSetGroupBox.TabIndex = 7;
            this.CharactersSetGroupBox.TabStop = false;
            this.CharactersSetGroupBox.Text = "Character Sets";
            // 
            // CharactersSetFlowLayoutPanel
            // 
            this.CharactersSetFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CharactersSetFlowLayoutPanel.AutoScroll = true;
            this.CharactersSetFlowLayoutPanel.BackColor = System.Drawing.SystemColors.Window;
            this.CharactersSetFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.CharactersSetFlowLayoutPanel.Location = new System.Drawing.Point(6, 45);
            this.CharactersSetFlowLayoutPanel.Name = "CharactersSetFlowLayoutPanel";
            this.CharactersSetFlowLayoutPanel.Size = new System.Drawing.Size(277, 427);
            this.CharactersSetFlowLayoutPanel.TabIndex = 2;
            this.CharactersSetFlowLayoutPanel.WrapContents = false;
            this.CharactersSetFlowLayoutPanel.ClientSizeChanged += new System.EventHandler(this.CharactersSetFlowLayoutPanel1ClientSizeChanged);
            this.CharactersSetFlowLayoutPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.CharactersSetFlowLayoutPanel1ControlAdded);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.Location = new System.Drawing.Point(771, 467);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 8;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.Location = new System.Drawing.Point(857, 467);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 9;
            this.ExitButton.Text = "Cancel";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // CharactersGroupBox
            // 
            this.CharactersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CharactersGroupBox.Controls.Add(this.CharactersPanel);
            this.CharactersGroupBox.Location = new System.Drawing.Point(307, 12);
            this.CharactersGroupBox.Name = "CharactersGroupBox";
            this.CharactersGroupBox.Size = new System.Drawing.Size(625, 449);
            this.CharactersGroupBox.TabIndex = 10;
            this.CharactersGroupBox.TabStop = false;
            this.CharactersGroupBox.Text = "Characters";
            // 
            // CharactersPanel
            // 
            this.CharactersPanel.AutoScroll = true;
            this.CharactersPanel.Controls.Add(this.CharacterTableLayoutPanel);
            this.CharactersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CharactersPanel.Location = new System.Drawing.Point(3, 16);
            this.CharactersPanel.Name = "CharactersPanel";
            this.CharactersPanel.Size = new System.Drawing.Size(619, 430);
            this.CharactersPanel.TabIndex = 4;
            this.CharactersPanel.SizeChanged += new System.EventHandler(this.CharactersPanelSizeChanged);
            // 
            // CharacterTableLayoutPanel
            // 
            this.CharacterTableLayoutPanel.ColumnCount = 2;
            this.CharacterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CharacterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CharacterTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.CharacterTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.CharacterTableLayoutPanel.Name = "CharacterTableLayoutPanel";
            this.CharacterTableLayoutPanel.RowCount = 2;
            this.CharacterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CharacterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.CharacterTableLayoutPanel.Size = new System.Drawing.Size(200, 100);
            this.CharacterTableLayoutPanel.TabIndex = 0;
            // 
            // CharacterValueLabel
            // 
            this.CharacterValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CharacterValueLabel.AutoSize = true;
            this.CharacterValueLabel.Location = new System.Drawing.Point(368, 472);
            this.CharacterValueLabel.Name = "CharacterValueLabel";
            this.CharacterValueLabel.Size = new System.Drawing.Size(0, 13);
            this.CharacterValueLabel.TabIndex = 11;
            // 
            // ValueValueLabel
            // 
            this.ValueValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ValueValueLabel.AutoSize = true;
            this.ValueValueLabel.Location = new System.Drawing.Point(448, 472);
            this.ValueValueLabel.Name = "ValueValueLabel";
            this.ValueValueLabel.Size = new System.Drawing.Size(0, 13);
            this.ValueValueLabel.TabIndex = 12;
            // 
            // MapWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 502);
            this.Controls.Add(this.ValueValueLabel);
            this.Controls.Add(this.CharacterValueLabel);
            this.Controls.Add(this.CharactersGroupBox);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.CharacterLabel);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.CharactersSetGroupBox);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(960, 540);
            this.Name = "MapWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Character Map";
            this.Load += new System.EventHandler(this.CharacterMapWindowLoad);
            this.CharactersSetGroupBox.ResumeLayout(false);
            this.CharactersSetGroupBox.PerformLayout();
            this.CharactersGroupBox.ResumeLayout(false);
            this.CharactersPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip MainToolTip;
        private System.Windows.Forms.Label CharacterLabel;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.TextBox CharactersSetSearchTextBox;
        private System.Windows.Forms.GroupBox CharactersSetGroupBox;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.GroupBox CharactersGroupBox;
        private System.Windows.Forms.Panel CharactersPanel;
        private System.Windows.Forms.TableLayoutPanel CharacterTableLayoutPanel;
        private System.Windows.Forms.Label CharacterValueLabel;
        private System.Windows.Forms.Label ValueValueLabel;
        private System.Windows.Forms.FlowLayoutPanel CharactersSetFlowLayoutPanel;
    }
}