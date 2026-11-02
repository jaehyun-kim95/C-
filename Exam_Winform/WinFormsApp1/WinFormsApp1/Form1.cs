using System.Drawing.Text;
using System.Reflection;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboMenu.Items.Add(new MenuItem("아메리카노", 4000));
            cboMenu.Items.Add(new MenuItem("카페라떼", 4500));
            cboMenu.Items.Add(new MenuItem("바닐라라테", 5000));
            cboMenu.Items.Add(new MenuItem("녹차라떼", 4800));
            cboMenu.DisplayMember = "Name";
            lstCart.DisplayMember = "Name";

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            decimal price = 0;
            decimal totalPrice = 0;

            MenuItem? item = cboMenu.SelectedItem as MenuItem;
            if (item is null)
            {
                MessageBox.Show("메뉴를 선택하지 않았습니다.");
                return;
            }

            price = CalcPrice(item.Price);
            totalPrice = CartPrice() + price;
            lblTotal.Text = $"합계: {totalPrice}원";
            lstCart.Items.Add(new CartItem(item.Name, price));

            chkShot.Checked = false;
            chkIce.Checked = false;
            numQty.Value = 1;
        }

        private void cboMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            MenuItem? item = cboMenu.SelectedItem as MenuItem;
            if (item is null)
            {
                lblUnitPrice.Text = "단가 : ";
            }
            else
            {
                lblUnitPrice.Text = $"단가 : {item.Price}원";
            }

        }

        private void lstCart_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = lstCart.SelectedIndex >= 0;
        }

        private void lstCart_DoubleClick(object sender, EventArgs e)
        {
            if (lstCart.SelectedIndex < 0) { return; }
            lstCart.Items.RemoveAt(lstCart.SelectedIndex);
            lblTotal.Text = $"합계: {CartPrice()}원";
        }

        decimal CartPrice()
        {
            decimal total = 0;
            foreach (CartItem item in lstCart.Items)
            {
                total = total + item.TotalPrice;
            }
            return total;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstCart.Items.Count < 0) { return; }
            if (lstCart.SelectedIndex < 0) { return; }
            int selectRow = lstCart.SelectedIndex;

            lstCart.Items.RemoveAt(selectRow);
            lblTotal.Text = $"합계: {CartPrice()}원";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstCart.Items.Clear();
            lblTotal.Text = "합계: 0원";
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (lstCart.Items.Count < 1) { MessageBox.Show("장바구니가 비어있습니다."); return; }
            MessageBox.Show($"주문 완료 : {CartPrice():N0}원");
            lstCart.Items.Clear();
            lblTotal.Text = "합계: 0원";
        }

        void UpdatePreview(object? sender, EventArgs e)
        {
            decimal price = 0;
            MenuItem item = cboMenu.SelectedItem as MenuItem;
            if (item is null) { return; }
            label4.Text = $"예상: {CalcPrice(item.Price)}원";
        }

        decimal CalcPrice(decimal itemPrice)
        {
            decimal price = 0;
            price = itemPrice;
            if (chkShot.Checked) { price = price + 500; }
            ;
            if (chkIce.Checked) { price = price + 500; }
            ;

            price = price * numQty.Value;
            return price;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) { return; }
            if (lstCart.Items.Count > 0)
            {
                var result = MessageBox.Show("주문하지 않은 항목이 있습니다. 닫을까요?", "종료 확인", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }


        record MenuItem(string Name, decimal Price);
        record CartItem(string Name, decimal TotalPrice);

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
