namespace OdesAppInstaller
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            InstallButton = new Button();
            InstallLogText = new TextBox();
            ProgramListDataGridView = new DataGridView();
            Install = new DataGridViewCheckBoxColumn();
            ProgramNameColumn = new DataGridViewTextBoxColumn();
            Link = new DataGridViewLinkColumn();
            ((System.ComponentModel.ISupportInitialize)ProgramListDataGridView).BeginInit();
            SuspendLayout();
            // 
            // InstallButton
            // 
            InstallButton.Location = new Point(557, 401);
            InstallButton.Name = "InstallButton";
            InstallButton.Size = new Size(75, 23);
            InstallButton.TabIndex = 0;
            InstallButton.Text = "Install";
            InstallButton.UseVisualStyleBackColor = true;
            // 
            // InstallLogText
            // 
            InstallLogText.BorderStyle = BorderStyle.FixedSingle;
            InstallLogText.Location = new Point(399, 12);
            InstallLogText.Multiline = true;
            InstallLogText.Name = "InstallLogText";
            InstallLogText.ReadOnly = true;
            InstallLogText.ScrollBars = ScrollBars.Both;
            InstallLogText.Size = new Size(389, 372);
            InstallLogText.TabIndex = 1;
            InstallLogText.WordWrap = false;
            // 
            // ProgramListDataGridView
            // 
            ProgramListDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ProgramListDataGridView.Columns.AddRange(new DataGridViewColumn[] { Install, ProgramNameColumn, Link });
            ProgramListDataGridView.Location = new Point(28, 12);
            ProgramListDataGridView.Name = "ProgramListDataGridView";
            ProgramListDataGridView.Size = new Size(365, 327);
            ProgramListDataGridView.TabIndex = 2;
            // 
            // Install
            // 
            Install.HeaderText = "Checkbox";
            Install.Name = "Install";
            // 
            // Name
            // 
            ProgramNameColumn.HeaderText = "Name";
            ProgramNameColumn.Name = "Name"; // Note: The internal column Name property can still be "Name"
            // 
            // Link
            // 
            Link.HeaderText = "Link";
            Link.Name = "Link";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ProgramListDataGridView);
            Controls.Add(InstallLogText);
            Controls.Add(InstallButton);
            Name = "Form1";
            Text = "Ode's App Installer";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)ProgramListDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button InstallButton;
        private TextBox InstallLogText;
        private DataGridView ProgramListDataGridView;
        private DataGridViewCheckBoxColumn Install;
        private DataGridViewTextBoxColumn ProgramNameColumn;
        private DataGridViewLinkColumn Link;
    }
}
