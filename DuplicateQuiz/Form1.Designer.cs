namespace DuplicateQuiz
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDuplicate = new System.Windows.Forms.Button();
            this.textBoxQuizId = new System.Windows.Forms.TextBox();
            this.textBoxBoxId = new System.Windows.Forms.TextBox();
            this.textBoxProgress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Quiz Id";
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Location = new System.Drawing.Point(50, 133);
            this.buttonDuplicate.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(68, 25);
            this.buttonDuplicate.TabIndex = 1;
            this.buttonDuplicate.Text = "Duplicate";
            this.buttonDuplicate.UseVisualStyleBackColor = true;
            this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
            // 
            // textBoxQuizId
            // 
            this.textBoxQuizId.Location = new System.Drawing.Point(50, 11);
            this.textBoxQuizId.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxQuizId.Name = "textBoxQuizId";
            this.textBoxQuizId.Size = new System.Drawing.Size(390, 20);
            this.textBoxQuizId.TabIndex = 2;
            // 
            // textBoxBoxId
            // 
            this.textBoxBoxId.Location = new System.Drawing.Point(50, 81);
            this.textBoxBoxId.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBoxId.Name = "textBoxBoxId";
            this.textBoxBoxId.Size = new System.Drawing.Size(386, 20);
            this.textBoxBoxId.TabIndex = 4;
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.Location = new System.Drawing.Point(13, 174);
            this.textBoxProgress.Multiline = true;
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxProgress.Size = new System.Drawing.Size(427, 372);
            this.textBoxProgress.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "box Ids";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 558);
            this.Controls.Add(this.textBoxProgress);
            this.Controls.Add(this.textBoxBoxId);
            this.Controls.Add(this.textBoxQuizId);
            this.Controls.Add(this.buttonDuplicate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDuplicate;
        private System.Windows.Forms.TextBox textBoxQuizId;
        private System.Windows.Forms.TextBox textBoxBoxId;
        private System.Windows.Forms.TextBox textBoxProgress;
        private System.Windows.Forms.Label label2;
    }
}

