namespace WinFormsApp1
{
    partial class MainForm
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
            label1 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            btnDetail = new Button();
            btnUpdate = new Button();
            btnAdd = new Button();
            panel3 = new Panel();
            lstMember = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 7);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 0;
            label1.Text = "회원목록";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(386, 28);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnDetail);
            panel2.Controls.Add(btnUpdate);
            panel2.Controls.Add(btnAdd);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 359);
            panel2.Name = "panel2";
            panel2.Size = new Size(386, 58);
            panel2.TabIndex = 2;
            // 
            // btnDetail
            // 
            btnDetail.Location = new Point(268, 17);
            btnDetail.Name = "btnDetail";
            btnDetail.Size = new Size(106, 25);
            btnDetail.TabIndex = 2;
            btnDetail.Text = "상세 창 열기";
            btnDetail.UseVisualStyleBackColor = true;
            btnDetail.Click += btnDetail_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(140, 17);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 25);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "수정";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(12, 17);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 25);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "추가";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(lstMember);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 28);
            panel3.Name = "panel3";
            panel3.Size = new Size(386, 331);
            panel3.TabIndex = 3;
            // 
            // lstMember
            // 
            lstMember.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            lstMember.Dock = DockStyle.Fill;
            lstMember.Location = new Point(0, 0);
            lstMember.Name = "lstMember";
            lstMember.Size = new Size(386, 331);
            lstMember.TabIndex = 0;
            lstMember.UseCompatibleStateImageBehavior = false;
            lstMember.SelectedIndexChanged += lstMember_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "나이";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "이메일";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(386, 417);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "회원목록";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Button btnDetail;
        private Button btnUpdate;
        private Button btnAdd;
        private ListView lstMember;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
    }
}
