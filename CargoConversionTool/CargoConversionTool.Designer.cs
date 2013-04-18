namespace CargoConversionTool
{
    partial class frmCargoConversionTool
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
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkForSandbox = new System.Windows.Forms.CheckBox();
            this.chkValidateXSD = new System.Windows.Forms.CheckBox();
            this.txtXMLResults = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCreateXML = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cmbIATAPeriod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabLoadFiles = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkHasHeaders = new System.Windows.Forms.CheckBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTryLoad = new System.Windows.Forms.Button();
            this.btnGetFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chkHasRejectionFileHeaders = new System.Windows.Forms.CheckBox();
            this.txtRejectionFileLoadResults = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnTryLoadRejectionFile = new System.Windows.Forms.Button();
            this.btnOpenRejectionFile = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRejectionFile = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabLoadFiles.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.CausesValidation = false;
            this.btnBack.Location = new System.Drawing.Point(569, 296);
            this.btnBack.Margin = new System.Windows.Forms.Padding(4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 28);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.CausesValidation = false;
            this.btnNext.Location = new System.Drawing.Point(677, 296);
            this.btnNext.Margin = new System.Windows.Forms.Padding(4);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(100, 28);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkForSandbox);
            this.tabPage3.Controls.Add(this.chkValidateXSD);
            this.tabPage3.Controls.Add(this.txtXMLResults);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.btnCreateXML);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage3.Size = new System.Drawing.Size(766, 264);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Create XML";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.Validating += new System.ComponentModel.CancelEventHandler(this.tabPage3_Validating);
            // 
            // chkForSandbox
            // 
            this.chkForSandbox.AutoSize = true;
            this.chkForSandbox.Location = new System.Drawing.Point(181, 8);
            this.chkForSandbox.Name = "chkForSandbox";
            this.chkForSandbox.Size = new System.Drawing.Size(178, 21);
            this.chkForSandbox.TabIndex = 12;
            this.chkForSandbox.Text = "Create File for Sandbox";
            this.chkForSandbox.UseVisualStyleBackColor = true;
            // 
            // chkValidateXSD
            // 
            this.chkValidateXSD.AutoSize = true;
            this.chkValidateXSD.Location = new System.Drawing.Point(12, 8);
            this.chkValidateXSD.Name = "chkValidateXSD";
            this.chkValidateXSD.Size = new System.Drawing.Size(163, 21);
            this.chkValidateXSD.TabIndex = 11;
            this.chkValidateXSD.Text = "Validate against XSD";
            this.chkValidateXSD.UseVisualStyleBackColor = true;
            // 
            // txtXMLResults
            // 
            this.txtXMLResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXMLResults.Location = new System.Drawing.Point(12, 93);
            this.txtXMLResults.Multiline = true;
            this.txtXMLResults.Name = "txtXMLResults";
            this.txtXMLResults.ReadOnly = true;
            this.txtXMLResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtXMLResults.Size = new System.Drawing.Size(748, 163);
            this.txtXMLResults.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "XML Results";
            // 
            // btnCreateXML
            // 
            this.btnCreateXML.Location = new System.Drawing.Point(8, 36);
            this.btnCreateXML.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateXML.Name = "btnCreateXML";
            this.btnCreateXML.Size = new System.Drawing.Size(100, 28);
            this.btnCreateXML.TabIndex = 2;
            this.btnCreateXML.Text = "Create XML";
            this.btnCreateXML.UseVisualStyleBackColor = true;
            this.btnCreateXML.Click += new System.EventHandler(this.btnCreateXML_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cmbIATAPeriod);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(766, 264);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Complete Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Validating += new System.ComponentModel.CancelEventHandler(this.tabPage2_Validating);
            // 
            // cmbIATAPeriod
            // 
            this.cmbIATAPeriod.FormattingEnabled = true;
            this.cmbIATAPeriod.Location = new System.Drawing.Point(98, 8);
            this.cmbIATAPeriod.Name = "cmbIATAPeriod";
            this.cmbIATAPeriod.Size = new System.Drawing.Size(168, 24);
            this.cmbIATAPeriod.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IATA Period";
            // 
            // tabLoadFiles
            // 
            this.tabLoadFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabLoadFiles.Controls.Add(this.tabPage1);
            this.tabLoadFiles.Controls.Add(this.tabPage4);
            this.tabLoadFiles.Controls.Add(this.tabPage2);
            this.tabLoadFiles.Controls.Add(this.tabPage3);
            this.tabLoadFiles.HotTrack = true;
            this.tabLoadFiles.Location = new System.Drawing.Point(0, 0);
            this.tabLoadFiles.Margin = new System.Windows.Forms.Padding(4);
            this.tabLoadFiles.Name = "tabLoadFiles";
            this.tabLoadFiles.SelectedIndex = 0;
            this.tabLoadFiles.Size = new System.Drawing.Size(774, 293);
            this.tabLoadFiles.TabIndex = 0;
            this.tabLoadFiles.SelectedIndexChanged += new System.EventHandler(this.tabLoadFiles_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkHasHeaders);
            this.tabPage1.Controls.Add(this.txtResults);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.btnTryLoad);
            this.tabPage1.Controls.Add(this.btnGetFile);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtFileName);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(766, 264);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Load Prime Billings";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Validating += new System.ComponentModel.CancelEventHandler(this.tabPage1_Validating);
            // 
            // chkHasHeaders
            // 
            this.chkHasHeaders.AutoSize = true;
            this.chkHasHeaders.Location = new System.Drawing.Point(14, 47);
            this.chkHasHeaders.Name = "chkHasHeaders";
            this.chkHasHeaders.Size = new System.Drawing.Size(113, 21);
            this.chkHasHeaders.TabIndex = 9;
            this.chkHasHeaders.Text = "Has Headers";
            this.chkHasHeaders.UseVisualStyleBackColor = true;
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(11, 137);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(752, 123);
            this.txtResults.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Load Results";
            // 
            // btnTryLoad
            // 
            this.btnTryLoad.Location = new System.Drawing.Point(14, 85);
            this.btnTryLoad.Margin = new System.Windows.Forms.Padding(4);
            this.btnTryLoad.Name = "btnTryLoad";
            this.btnTryLoad.Size = new System.Drawing.Size(100, 28);
            this.btnTryLoad.TabIndex = 6;
            this.btnTryLoad.Text = "Try Load";
            this.btnTryLoad.UseVisualStyleBackColor = true;
            this.btnTryLoad.Click += new System.EventHandler(this.btnTryLoad_Click);
            // 
            // btnGetFile
            // 
            this.btnGetFile.Location = new System.Drawing.Point(406, 5);
            this.btnGetFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetFile.Name = "btnGetFile";
            this.btnGetFile.Size = new System.Drawing.Size(100, 28);
            this.btnGetFile.TabIndex = 5;
            this.btnGetFile.Text = "Open File";
            this.btnGetFile.UseVisualStyleBackColor = true;
            this.btnGetFile.Click += new System.EventHandler(this.btnGetFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source File";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(93, 8);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(306, 22);
            this.txtFileName.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chkHasRejectionFileHeaders);
            this.tabPage4.Controls.Add(this.txtRejectionFileLoadResults);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.btnTryLoadRejectionFile);
            this.tabPage4.Controls.Add(this.btnOpenRejectionFile);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.txtRejectionFile);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(766, 264);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Load Rejections";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.Validating += new System.ComponentModel.CancelEventHandler(this.tabPage4_Validating);
            // 
            // chkHasRejectionFileHeaders
            // 
            this.chkHasRejectionFileHeaders.AutoSize = true;
            this.chkHasRejectionFileHeaders.Location = new System.Drawing.Point(13, 48);
            this.chkHasRejectionFileHeaders.Name = "chkHasRejectionFileHeaders";
            this.chkHasRejectionFileHeaders.Size = new System.Drawing.Size(113, 21);
            this.chkHasRejectionFileHeaders.TabIndex = 16;
            this.chkHasRejectionFileHeaders.Text = "Has Headers";
            this.chkHasRejectionFileHeaders.UseVisualStyleBackColor = true;
            // 
            // txtRejectionFileLoadResults
            // 
            this.txtRejectionFileLoadResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRejectionFileLoadResults.Location = new System.Drawing.Point(13, 138);
            this.txtRejectionFileLoadResults.Multiline = true;
            this.txtRejectionFileLoadResults.Name = "txtRejectionFileLoadResults";
            this.txtRejectionFileLoadResults.ReadOnly = true;
            this.txtRejectionFileLoadResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRejectionFileLoadResults.Size = new System.Drawing.Size(748, 120);
            this.txtRejectionFileLoadResults.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "Load Results";
            // 
            // btnTryLoadRejectionFile
            // 
            this.btnTryLoadRejectionFile.Location = new System.Drawing.Point(13, 86);
            this.btnTryLoadRejectionFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnTryLoadRejectionFile.Name = "btnTryLoadRejectionFile";
            this.btnTryLoadRejectionFile.Size = new System.Drawing.Size(100, 28);
            this.btnTryLoadRejectionFile.TabIndex = 13;
            this.btnTryLoadRejectionFile.Text = "Try Load";
            this.btnTryLoadRejectionFile.UseVisualStyleBackColor = true;
            this.btnTryLoadRejectionFile.Click += new System.EventHandler(this.btnTryLoadRejectionFile_Click);
            // 
            // btnOpenRejectionFile
            // 
            this.btnOpenRejectionFile.Location = new System.Drawing.Point(405, 6);
            this.btnOpenRejectionFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenRejectionFile.Name = "btnOpenRejectionFile";
            this.btnOpenRejectionFile.Size = new System.Drawing.Size(100, 28);
            this.btnOpenRejectionFile.TabIndex = 12;
            this.btnOpenRejectionFile.Text = "Open File";
            this.btnOpenRejectionFile.UseVisualStyleBackColor = true;
            this.btnOpenRejectionFile.Click += new System.EventHandler(this.btnOpenRejectionFile_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Source File";
            // 
            // txtRejectionFile
            // 
            this.txtRejectionFile.Location = new System.Drawing.Point(92, 9);
            this.txtRejectionFile.Name = "txtRejectionFile";
            this.txtRejectionFile.Size = new System.Drawing.Size(306, 22);
            this.txtRejectionFile.TabIndex = 10;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.Location = new System.Drawing.Point(4, 296);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmCargoConversionTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(781, 332);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.tabLoadFiles);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBack);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCargoConversionTool";
            this.Text = "Cargo Conversion Wizard";
            this.Activated += new System.EventHandler(this.frmCargoConversionTool_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCargoConversionTool_FormClosing);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabLoadFiles.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabLoadFiles;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox cmbIATAPeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnTryLoad;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtXMLResults;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCreateXML;
        private System.Windows.Forms.CheckBox chkHasHeaders;
        private System.Windows.Forms.CheckBox chkValidateXSD;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox chkForSandbox;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox chkHasRejectionFileHeaders;
        private System.Windows.Forms.TextBox txtRejectionFileLoadResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnTryLoadRejectionFile;
        private System.Windows.Forms.Button btnOpenRejectionFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRejectionFile;
        private System.Windows.Forms.Button btnCancel;
    }
}

