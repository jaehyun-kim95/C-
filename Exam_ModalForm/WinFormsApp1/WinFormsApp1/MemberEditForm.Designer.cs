namespace WinFormsApp1
{
    partial class MemberEditForm
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtName = new TextBox();
            txtAge = new TextBox();
            txtEmail = new TextBox();
            panel1 = new Panel();
            panel2 = new Panel();
            btnCancel = new Button();
            btnOk = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 13);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 0;
            label1.Text = "이름 :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 44);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 1;
            label2.Text = "나이 :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 75);
            label3.Name = "label3";
            label3.Size = new Size(50, 15);
            label3.TabIndex = 2;
            label3.Text = "이메일 :";
            // 
            // txtName
            // 
            txtName.Location = new Point(63, 10);
            txtName.Name = "txtName";
            txtName.Size = new Size(100, 23);
            txtName.TabIndex = 3;
            // 
            // txtAge
            // 
            txtAge.Location = new Point(63, 41);
            txtAge.Name = "txtAge";
            txtAge.Size = new Size(100, 23);
            txtAge.TabIndex = 4;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(63, 72);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 5;
            // 
            // panel1
            // 
            panel1.Controls.Add(txtEmail);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(txtAge);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(txtName);
            panel1.Controls.Add(label3);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(185, 106);
            panel1.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnCancel);
            panel2.Controls.Add(btnOk);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 106);
            panel2.Name = "panel2";
            panel2.Size = new Size(185, 52);
            panel2.TabIndex = 7;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(98, 17);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(13, 17);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 0;
            btnOk.Text = "확인";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // MemberEditForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(185, 158);
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MemberEditForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "회원정보 수정";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtName;
        private TextBox txtAge;
        private TextBox txtEmail;
        private Panel panel1;
        private Panel panel2;
        private Button btnCancel;
        private Button btnOk;
    }
}