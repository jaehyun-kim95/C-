namespace WinFormsApp1
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            txtReceiver = new TextBox();
            txtPhone = new TextBox();
            txtZip = new TextBox();
            txtAddress = new TextBox();
            txtDepositor = new TextBox();
            dtpDelivery = new DateTimePicker();
            cboPayment = new ComboBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnCancel = new Button();
            btnSubmit = new Button();
            errorProvider1 = new ErrorProvider(components);
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 18);
            label1.Name = "label1";
            label1.Size = new Size(59, 15);
            label1.TabIndex = 0;
            label1.Text = "받는 사람";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 58);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 1;
            label2.Text = "휴대폰";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 98);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 2;
            label3.Text = "우편번호";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 137);
            label4.Name = "label4";
            label4.Size = new Size(31, 15);
            label4.TabIndex = 3;
            label4.Text = "주소";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 176);
            label5.Name = "label5";
            label5.Size = new Size(71, 15);
            label5.TabIndex = 4;
            label5.Text = "희망 배송일";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 218);
            label6.Name = "label6";
            label6.Size = new Size(59, 15);
            label6.TabIndex = 5;
            label6.Text = "결제 수단";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 257);
            label7.Name = "label7";
            label7.Size = new Size(55, 15);
            label7.TabIndex = 6;
            label7.Text = "입금자명";
            // 
            // txtReceiver
            // 
            txtReceiver.Location = new Point(89, 15);
            txtReceiver.MaxLength = 20;
            txtReceiver.Name = "txtReceiver";
            txtReceiver.Size = new Size(172, 23);
            txtReceiver.TabIndex = 7;
            txtReceiver.Validating += txtReceiver_Validating;
            // 
            // txtPhone
            // 
            txtPhone.Location = new Point(89, 54);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(172, 23);
            txtPhone.TabIndex = 8;
            txtPhone.Validating += txtPhone_Validating;
            // 
            // txtZip
            // 
            txtZip.Location = new Point(89, 95);
            txtZip.Name = "txtZip";
            txtZip.Size = new Size(51, 23);
            txtZip.TabIndex = 9;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(89, 135);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(172, 23);
            txtAddress.TabIndex = 10;
            // 
            // txtDepositor
            // 
            txtDepositor.Location = new Point(89, 250);
            txtDepositor.Name = "txtDepositor";
            txtDepositor.Size = new Size(172, 23);
            txtDepositor.TabIndex = 11;
            txtDepositor.Validating += txtDepositor_Validating;
            // 
            // dtpDelivery
            // 
            dtpDelivery.Location = new Point(89, 176);
            dtpDelivery.Name = "dtpDelivery";
            dtpDelivery.Size = new Size(172, 23);
            dtpDelivery.TabIndex = 12;
            dtpDelivery.Validating += dtpDelivery_Validating;
            // 
            // cboPayment
            // 
            cboPayment.FormattingEnabled = true;
            cboPayment.Location = new Point(89, 215);
            cboPayment.Name = "cboPayment";
            cboPayment.Size = new Size(51, 23);
            cboPayment.TabIndex = 13;
            cboPayment.SelectedIndexChanged += cboPayment_SelectedIndexChanged;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnCancel);
            flowLayoutPanel1.Controls.Add(btnSubmit);
            flowLayoutPanel1.Dock = DockStyle.Bottom;
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(0, 283);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(6);
            flowLayoutPanel1.Size = new Size(300, 41);
            flowLayoutPanel1.TabIndex = 14;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(210, 9);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(129, 9);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(75, 23);
            btnSubmit.TabIndex = 1;
            btnSubmit.Text = "신청";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider1.ContainerControl = this;
            // 
            // Form1
            // 
            AcceptButton = btnSubmit;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(300, 324);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(cboPayment);
            Controls.Add(dtpDelivery);
            Controls.Add(txtDepositor);
            Controls.Add(txtAddress);
            Controls.Add(txtZip);
            Controls.Add(txtPhone);
            Controls.Add(txtReceiver);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private TextBox txtReceiver;
        private TextBox txtPhone;
        private TextBox txtZip;
        private TextBox txtAddress;
        private TextBox txtDepositor;
        private DateTimePicker dtpDelivery;
        private ComboBox cboPayment;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnCancel;
        private Button btnSubmit;
        private ErrorProvider errorProvider1;
    }
}
