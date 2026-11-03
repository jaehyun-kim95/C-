
namespace WinFormsApp1
{
    public partial class MemberEditForm : Form
    {

        public Member? Result { get; private set; }

        public MemberEditForm(Member? member)
        {
            InitializeComponent();
            Result = member;
            if (member is not null)
            {
                txtName.Text = member.Name;
                txtAge.Text = member.Age.ToString();
                txtEmail.Text = member.Email;
            }
        }

   
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "" || !int.TryParse(txtAge.Text, out int age)) {
                MessageBox.Show("이름이 비었거나 나이가 숫자가 아닙니다.");
                return;
            }

            if (Result is null)
            { // insert
                Result = new Member(txtName.Text.Trim(), age, txtEmail.Text);
            }else
            { // update
                Result = Result with { Name = txtName.Text.Trim(), Age = age, Email = txtEmail.Text };
            }
            DialogResult = DialogResult.OK;
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
