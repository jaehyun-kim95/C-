
namespace WinFormsApp1
{
    public partial class MainForm : Form
    {
        private readonly List<Member> members = new();
        public MemberDetailForm detailForm;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var memberEditfrm = new MemberEditForm(null);
            if (memberEditfrm.ShowDialog(this) == DialogResult.OK)
            {
                if (memberEditfrm.Result is null) { return; }
                members.Add(memberEditfrm.Result);
                ReFreshView();
            }
        }

        void ReFreshView()
        {
            lstMember.Items.Clear();
            foreach (var mem in members)
            {
                var lvi = new ListViewItem(mem.Name) { Tag = mem };
                lvi.SubItems.Add(mem.Age.ToString());
                lvi.SubItems.Add(mem.Email);
                lstMember.Items.Add(lvi);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lstMember.SelectedItems.Count == 0) { return; }
            Member? member = lstMember.SelectedItems[0].Tag as Member;

            using var memEditfrm = new MemberEditForm(member);
            if (memEditfrm.ShowDialog(this) == DialogResult.OK && memEditfrm.Result is not null)
            {
                var lst = lstMember.SelectedItems[0];
                lst.Tag = memEditfrm.Result;
                lst.Text = memEditfrm.Result.Name;
                lst.SubItems[1].Text = memEditfrm.Result.Age.ToString();
                lst.SubItems[2].Text = memEditfrm.Result.Email;
                if (detailForm is not null) {
                    detailForm.SetDetail(lstMember.SelectedItems[0].Tag as Member);
                }
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            if (detailForm is null || detailForm.IsDisposed)
            {
                if (lstMember.SelectedItems.Count != 0)
                {
                    Member? member = lstMember.SelectedItems[0].Tag as Member;
                    detailForm = new MemberDetailForm(member);
                    detailForm.DeleteRequested += OnDeleteRequested;
                    detailForm.FormClosed += OnFormClosed;

                    detailForm.Show(this);
                }
            }
            else
            {
                detailForm.Activate();
            }
        }

        private void lstMember_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (detailForm is not null && lstMember.SelectedItems.Count != 0) {
                detailForm.SetDetail(lstMember.SelectedItems[0].Tag as Member);
            }
        }
        private void OnDeleteRequested(object? sender, Member member)
        {
            members.Remove(member);
            ReFreshView();
            detailForm.Close();
        }

        private void OnFormClosed(object? sender, FormClosedEventArgs e)
        {
            detailForm.DeleteRequested -= OnDeleteRequested;
        }

    }

    public record Member(string Name, int Age, string Email)
    {
        public override string ToString()
        {
            return $"이름: {this.Name} 나이: {this.Age} 이메일: {this.Email}";
        }
    };
}
