namespace CheckSTP
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.rtbSQLUnusedVariable = new System.Windows.Forms.RichTextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnChooseSQLFolder = new System.Windows.Forms.Button();
            this.txtSQLPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CheckSTD = new System.Windows.Forms.Button();
            this.ruleSTDDs = new CheckSTP.RuleSTD();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCodePath = new System.Windows.Forms.TextBox();
            this.btnChooseCodeFolder = new System.Windows.Forms.Button();
            this.btnCheckCode = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ruleSTDDs)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbSQLUnusedVariable
            // 
            this.rtbSQLUnusedVariable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSQLUnusedVariable.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.rtbSQLUnusedVariable.HideSelection = false;
            this.rtbSQLUnusedVariable.Location = new System.Drawing.Point(15, 82);
            this.rtbSQLUnusedVariable.Name = "rtbSQLUnusedVariable";
            this.rtbSQLUnusedVariable.Size = new System.Drawing.Size(692, 431);
            this.rtbSQLUnusedVariable.TabIndex = 0;
            this.rtbSQLUnusedVariable.Text = "";
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheck.Location = new System.Drawing.Point(523, 521);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(184, 27);
            this.btnCheck.TabIndex = 1;
            this.btnCheck.Text = "Check Variable Not Use In STP";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // btnChooseSQLFolder
            // 
            this.btnChooseSQLFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseSQLFolder.Location = new System.Drawing.Point(618, 13);
            this.btnChooseSQLFolder.Name = "btnChooseSQLFolder";
            this.btnChooseSQLFolder.Size = new System.Drawing.Size(87, 27);
            this.btnChooseSQLFolder.TabIndex = 2;
            this.btnChooseSQLFolder.Text = "Browser";
            this.btnChooseSQLFolder.UseVisualStyleBackColor = true;
            this.btnChooseSQLFolder.Click += new System.EventHandler(this.btnChooseSQLFolder_Click);
            // 
            // txtSQLPath
            // 
            this.txtSQLPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSQLPath.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQLPath.Location = new System.Drawing.Point(76, 15);
            this.txtSQLPath.Name = "txtSQLPath";
            this.txtSQLPath.Size = new System.Drawing.Size(534, 22);
            this.txtSQLPath.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "SQL Dir:";
            // 
            // CheckSTD
            // 
            this.CheckSTD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckSTD.Location = new System.Drawing.Point(367, 521);
            this.CheckSTD.Name = "CheckSTD";
            this.CheckSTD.Size = new System.Drawing.Size(141, 27);
            this.CheckSTD.TabIndex = 5;
            this.CheckSTD.Text = "Check STP";
            this.CheckSTD.UseVisualStyleBackColor = true;
            this.CheckSTD.Click += new System.EventHandler(this.CheckSTD_Click);
            // 
            // ruleSTDDs
            // 
            this.ruleSTDDs.DataSetName = "RuleSTD";
            this.ruleSTDDs.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "Code Dir:";
            // 
            // txtCodePath
            // 
            this.txtCodePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCodePath.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodePath.Location = new System.Drawing.Point(76, 43);
            this.txtCodePath.Name = "txtCodePath";
            this.txtCodePath.Size = new System.Drawing.Size(534, 22);
            this.txtCodePath.TabIndex = 10;
            // 
            // btnChooseCodeFolder
            // 
            this.btnChooseCodeFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseCodeFolder.Location = new System.Drawing.Point(618, 40);
            this.btnChooseCodeFolder.Name = "btnChooseCodeFolder";
            this.btnChooseCodeFolder.Size = new System.Drawing.Size(87, 27);
            this.btnChooseCodeFolder.TabIndex = 9;
            this.btnChooseCodeFolder.Text = "Browser";
            this.btnChooseCodeFolder.UseVisualStyleBackColor = true;
            this.btnChooseCodeFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCheckCode
            // 
            this.btnCheckCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckCode.Location = new System.Drawing.Point(211, 521);
            this.btnCheckCode.Name = "btnCheckCode";
            this.btnCheckCode.Size = new System.Drawing.Size(141, 27);
            this.btnCheckCode.TabIndex = 12;
            this.btnCheckCode.Text = "Check Code C#";
            this.btnCheckCode.UseVisualStyleBackColor = true;
            this.btnCheckCode.Click += new System.EventHandler(this.btnCheckCode_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 555);
            this.Controls.Add(this.btnCheckCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCodePath);
            this.Controls.Add(this.btnChooseCodeFolder);
            this.Controls.Add(this.CheckSTD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSQLPath);
            this.Controls.Add(this.btnChooseSQLFolder);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.rtbSQLUnusedVariable);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Check UnUse Variable in SQL Files";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.ruleSTDDs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtbSQLUnusedVariable;
		private System.Windows.Forms.Button btnCheck;
		private System.Windows.Forms.Button btnChooseSQLFolder;
		private System.Windows.Forms.TextBox txtSQLPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button CheckSTD;
		private RuleSTD ruleSTDDs;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtCodePath;
		private System.Windows.Forms.Button btnChooseCodeFolder;
		private System.Windows.Forms.Button btnCheckCode;
	}
}

