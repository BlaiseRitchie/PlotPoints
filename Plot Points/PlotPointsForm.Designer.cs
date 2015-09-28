namespace Plot_Points {
    partial class PlotPointsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.fileBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // fileBox
            // 
            this.fileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileBox.Location = new System.Drawing.Point(13, 13);
            this.fileBox.Name = "fileBox";
            this.fileBox.ReadOnly = true;
            this.fileBox.Size = new System.Drawing.Size(173, 22);
            this.fileBox.TabIndex = 0;
            this.fileBox.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(192, 12);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(88, 23);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.loadButton.Location = new System.Drawing.Point(117, 41);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(72, 23);
            this.loadButton.TabIndex = 2;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Kml|*.kml";
            // 
            // PlotPointsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 77);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.fileBox);
            this.MaximumSize = new System.Drawing.Size(400, 122);
            this.MinimumSize = new System.Drawing.Size(200, 122);
            this.Name = "PlotPointsForm";
            this.Text = "Plot Points";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fileBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}