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
            System.Windows.Forms.Label label2;
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDuplicate = new System.Windows.Forms.Button();
            this.textBoxQuizId = new System.Windows.Forms.TextBox();
            this.textBoxBoxId = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Quiz Id";
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Location = new System.Drawing.Point(16, 118);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(91, 31);
            this.buttonDuplicate.TabIndex = 1;
            this.buttonDuplicate.Text = "Duplicate";
            this.buttonDuplicate.UseVisualStyleBackColor = true;
            this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
            // 
            // textBoxQuizId
            // 
            this.textBoxQuizId.Location = new System.Drawing.Point(66, 13);
            this.textBoxQuizId.Name = "textBoxQuizId";
            this.textBoxQuizId.Size = new System.Drawing.Size(100, 22);
            this.textBoxQuizId.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(13, 61);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(46, 17);
            label2.TabIndex = 3;
            label2.Text = "Box Id";
            // 
            // textBoxBoxId
            // 
            this.textBoxBoxId.Location = new System.Drawing.Point(66, 61);
            this.textBoxBoxId.Name = "textBoxBoxId";
            this.textBoxBoxId.Size = new System.Drawing.Size(100, 22);
            this.textBoxBoxId.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.textBoxBoxId);
            this.Controls.Add(label2);
            this.Controls.Add(this.textBoxQuizId);
            this.Controls.Add(this.buttonDuplicate);
            this.Controls.Add(this.label1);
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
    }
}

