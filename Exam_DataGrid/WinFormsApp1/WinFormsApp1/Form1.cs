using System.ComponentModel;
using System.Reflection;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        BindingList<Product> products = new();

        public Form1()
        {
            InitializeComponent();
            bindingSource1.DataSource = products;
            products.Add(new Product("test1", "음료", 100, 10));
            products.Add(new Product("test2", "음료", 500, 5));
            products.Add(new Product("test3", "디저트", 11000, 15));
            products.Add(new Product("test4", "디저트", 1000, 3));
            products.Add(new Product("test5", "원두", 20000, 2));

            txtName.DataBindings.Add("Text", bindingSource1, "_name");
            numPrice.DataBindings.Add("Value", bindingSource1, "_price", true, DataSourceUpdateMode.OnPropertyChanged, 0, "N0");
            numStock.DataBindings.Add("Value", bindingSource1, "_stock", true, DataSourceUpdateMode.OnPropertyChanged, 0, "N0");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (bindingSource1.Current is Product p)
            {
                MessageBox.Show(p.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bindingSource1.AddNew();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (bindingSource1.Count < 1) { return; }
            DialogResult result = MessageBox.Show("삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bindingSource1.RemoveCurrent();
            }
            else
            {
                return;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bindingSource1.EndEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bindingSource1.CancelEdit();
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
            dataGridView1.Rows[e.RowIndex].ErrorText = "";

            if (colName == "_price" || colName == "_stock")
            {
                if (!decimal.TryParse(e.FormattedValue.ToString(), out decimal a) || a < 0)
                {
                    e.Cancel = true;
                    dataGridView1.Rows[e.RowIndex].ErrorText = "가격 또는 재고는 0 이상의 숫자여야 합니다.";
                }
            }

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView1.Rows[e.RowIndex].DataBoundItem is Product p)
            {
                if (p._stock <= 5)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void bindingSource1_ListChanged(object sender, ListChangedEventArgs e)
        {
            int totalCount = bindingSource1.Count;
            toolStripStatusLabel1.Text = $"총 {totalCount}종";

            int totalStock = 0;
            if (bindingSource1.DataSource is BindingList<Product> p)
            {
                foreach (var pd in p)
                {
                    totalStock = totalStock + pd._stock;
                }
                toolStripStatusLabel2.Text = $"재고 합계 {totalStock}개";
            }

        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
            var prop = typeof(Product).GetProperty(colName);
            var sorted = products.OrderBy(o => prop.GetValue(o)).ToList();
            products = new BindingList<Product>(sorted);
            bindingSource1.DataSource = products;
        }

        private void txtFindName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (txtFindName.Text.Trim() != "")
                {
                    var filt = products.Where(w => w._name == txtFindName.Text).ToList();
                    bindingSource1.DataSource = new BindingList<Product>(filt);
                } else
                {
                    bindingSource1.DataSource = products;
                }
            }
        }
    }
}
