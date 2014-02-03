namespace CodeGenerator
{
    partial class Cloudents
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
            System.Windows.Forms.Button buttonGenerate;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Cloudents));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelResult = new System.Windows.Forms.TextBox();
            buttonGenerate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonGenerate
            // 
            buttonGenerate.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            buttonGenerate.AutoSize = true;
            buttonGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(34)))), ((int)(((byte)(98)))));
            buttonGenerate.CausesValidation = false;
            buttonGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            buttonGenerate.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonGenerate.FlatAppearance.BorderSize = 0;
            buttonGenerate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(34)))), ((int)(((byte)(98)))));
            buttonGenerate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(25)))), ((int)(((byte)(73)))));
            buttonGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonGenerate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            buttonGenerate.ForeColor = System.Drawing.Color.White;
            buttonGenerate.Location = new System.Drawing.Point(15, 181);
            buttonGenerate.Margin = new System.Windows.Forms.Padding(0);
            buttonGenerate.Name = "buttonGenerate";
            buttonGenerate.Size = new System.Drawing.Size(185, 34);
            buttonGenerate.TabIndex = 0;
            buttonGenerate.Text = "צור קוד כניסה";
            buttonGenerate.UseVisualStyleBackColor = false;
            buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(211, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(21, 126);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(173, 35);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // labelResult
            // 
            this.labelResult.BackColor = System.Drawing.Color.White;
            this.labelResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelResult.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(116)))), ((int)(((byte)(116)))));
            this.labelResult.Location = new System.Drawing.Point(15, 234);
            this.labelResult.Margin = new System.Windows.Forms.Padding(0);
            this.labelResult.MinimumSize = new System.Drawing.Size(185, 28);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(185, 30);
            this.labelResult.TabIndex = 5;
            this.labelResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Cloudents
            // 
            this.AcceptButton = buttonGenerate;
            this.AccessibleDescription = "";
            this.AccessibleName = "";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CancelButton = buttonGenerate;
            this.ClientSize = new System.Drawing.Size(215, 281);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(buttonGenerate);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.Name = "Cloudents";
            this.Text = "Cloudents";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox labelResult;
    }
}

