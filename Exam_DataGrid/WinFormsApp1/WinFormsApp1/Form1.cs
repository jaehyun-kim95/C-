using System.ComponentModel;

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
    }
}
