using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    internal class MessageService
    {
        public static void ShowInfo(string message) =>
            MessageBox.Show(message, "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ShowWarning(string message) =>
            MessageBox.Show(message, "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void ShowError(string message) =>
            MessageBox.Show(message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static bool Confirm(string message) =>
            MessageBox.Show(message, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
