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
            cboMenu = new ComboBox();
            lblUnitPrice = new Label();
            numQty = new NumericUpDown();
            chkShot = new CheckBox();
            chkIce = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            btnAdd = new Button();
            btnRemove = new Button();
            btnClear = new Button();
            btnOrder = new Button();
            lstCart = new ListBox();
            lblTotal = new Label();
            label3 = new Label();
            label4 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            panel3 = new Panel();
            ((System.ComponentModel.ISupportInitialize)numQty).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // cboMenu
            // 
            cboMenu.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMenu.FormattingEnabled = true;
            cboMenu.Location = new Point(67, 9);
            cboMenu.Name = "cboMenu";
            cboMenu.Size = new Size(121, 23);
            cboMenu.TabIndex = 0;
            cboMenu.SelectedIndexChanged += UpdatePreview;
            // 
            // lblUnitPrice
            // 
            lblUnitPrice.AutoSize = true;
            lblUnitPrice.ForeColor = Color.Black;
            lblUnitPrice.Location = new Point(211, 14);
            lblUnitPrice.Name = "lblUnitPrice";
            lblUnitPrice.Size = new Size(42, 15);
            lblUnitPrice.TabIndex = 1;
            lblUnitPrice.Text = "단가 : ";
            lblUnitPrice.Click += this.lblUnitPrice_Click;
            // 
            // numQty
            // 
            numQty.Location = new Point(67, 38);
            numQty.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numQty.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQty.Name = "numQty";
            numQty.Size = new Size(57, 23);
            numQty.TabIndex = 2;
            numQty.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numQty.ValueChanged += UpdatePreview;
            // 
            // chkShot
            // 
            chkShot.AutoSize = true;
            chkShot.ForeColor = Color.Black;
            chkShot.Location = new Point(130, 42);
            chkShot.Name = "chkShot";
            chkShot.Size = new Size(103, 19);
            chkShot.TabIndex = 3;
            chkShot.Text = "샷 추가(+500)";
            chkShot.UseVisualStyleBackColor = true;
            chkShot.CheckedChanged += UpdatePreview;
            // 
            // chkIce
            // 
            chkIce.AutoSize = true;
            chkIce.ForeColor = Color.Black;
            chkIce.Location = new Point(239, 42);
            chkIce.Name = "chkIce";
            chkIce.Size = new Size(99, 19);
            chkIce.TabIndex = 4;
            chkIce.Text = "아이스(+500)";
            chkIce.UseVisualStyleBackColor = true;
            chkIce.CheckedChanged += UpdatePreview;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Black;
            label1.Location = new Point(12, 13);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 5;
            label1.Text = "메뉴";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Black;
            label2.Location = new Point(12, 43);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 6;
            label2.Text = "수량";
            label2.Click += label2_Click;
            // 
            // btnAdd
            // 
            btnAdd.ForeColor = Color.Black;
            btnAdd.Location = new Point(211, 67);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(127, 23);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "담기";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemove.Enabled = false;
            btnRemove.ForeColor = Color.Black;
            btnRemove.Location = new Point(12, 19);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(70, 23);
            btnRemove.TabIndex = 8;
            btnRemove.Text = "선택 삭제";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnClear
            // 
            btnClear.ForeColor = Color.Black;
            btnClear.Location = new Point(88, 19);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(70, 23);
            btnClear.TabIndex = 9;
            btnClear.Text = "비우기";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnOrder
            // 
            btnOrder.ForeColor = Color.Black;
            btnOrder.Location = new Point(211, 41);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(127, 23);
            btnOrder.TabIndex = 10;
            btnOrder.Text = "주문하기";
            btnOrder.UseVisualStyleBackColor = true;
            btnOrder.Click += btnOrder_Click;
            // 
            // lstCart
            // 
            lstCart.Dock = DockStyle.Fill;
            lstCart.FormattingEnabled = true;
            lstCart.Location = new Point(0, 0);
            lstCart.Name = "lstCart";
            lstCart.Size = new Size(362, 96);
            lstCart.TabIndex = 11;
            lstCart.SelectedIndexChanged += lstCart_SelectedIndexChanged;
            lstCart.DoubleClick += lstCart_DoubleClick;
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotal.AutoSize = true;
            lblTotal.ForeColor = Color.Black;
            lblTotal.Location = new Point(211, 23);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(57, 15);
            lblTotal.TabIndex = 12;
            lblTotal.Text = "합계: 0원";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 13;
            label3.Text = "label3";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Black;
            label4.Location = new Point(12, 67);
            label4.Name = "label4";
            label4.Size = new Size(0, 15);
            label4.TabIndex = 14;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnAdd);
            panel1.Controls.Add(cboMenu);
            panel1.Controls.Add(lblUnitPrice);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(numQty);
            panel1.Controls.Add(chkShot);
            panel1.Controls.Add(chkIce);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(362, 97);
            panel1.TabIndex = 15;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnRemove);
            panel2.Controls.Add(btnOrder);
            panel2.Controls.Add(btnClear);
            panel2.Controls.Add(lblTotal);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 193);
            panel2.Name = "panel2";
            panel2.Size = new Size(362, 100);
            panel2.TabIndex = 16;
            // 
            // panel3
            // 
            panel3.Controls.Add(lstCart);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 97);
            panel3.Name = "panel3";
            panel3.Size = new Size(362, 96);
            panel3.TabIndex = 17;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(362, 293);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label3);
            ForeColor = SystemColors.ControlLightLight;
            MinimumSize = new Size(378, 277);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "카페 주문";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numQty).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cboMenu;
        private Label lblUnitPrice;
        private NumericUpDown numQty;
        private CheckBox chkShot;
        private CheckBox chkIce;
        private Label label1;
        private Label label2;
        private Button btnAdd;
        private Button btnRemove;
        private Button btnClear;
        private Button btnOrder;
        private ListBox lstCart;
        private Label lblTotal;
        private Label label3;
        private Label label4;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
    }
}
