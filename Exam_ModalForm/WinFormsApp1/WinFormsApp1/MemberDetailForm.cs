
namespace WinFormsApp1
{
    public partial class MemberDetailForm : Form
    {
        private Member _current;
        public event EventHandler<Member> DeleteRequested;


        public MemberDetailForm(Member _mem)
        {
            InitializeComponent();
            _current = _mem;
            txtName.Text = _mem.Name;
            txtAge.Text = _mem.Age.ToString();
            txtEmail.Text = _mem.Email;
        }

        public void SetDetail(Member member)
        {
            _current = member;
            txtName.Text = member.Name;
            txtAge.Text = member.Age.ToString();
            txtEmail.Text = member.Email;
        }

        private void MemberDetailForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void MemberDetailForm_Load(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_current is not null)
            {
                DeleteRequested.Invoke(this, _current);
            }else
            {
                return;
            }
        }
    }
}
