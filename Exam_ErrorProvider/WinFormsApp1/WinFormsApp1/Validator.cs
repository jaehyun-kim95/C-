using System.Text.RegularExpressions;

namespace WinFormsApp1
{
    internal class Validator
    {
        internal class FormValidator
        {
            static readonly Regex PhoneRe = new(@"^\d{3}-\d{4}-\d{4}$", RegexOptions.Compiled);
            public string? ValidatePhone(string phoneText)
            {
                bool isMatch = PhoneRe.IsMatch(phoneText);
                if (isMatch)
                {
                    return null;
                }
                else
                {
                    return "전화번호 양식을 잘못 입력하셨습니다.";
                }
            }
            public string? ValidateDeliveryDate(DateTime date)
            {
                DateTime today = DateTime.Today;
                if (date.Date < today)
                    return "배송 희망일은 오늘 이후여야 합니다.";
                if (date.Date > today.AddDays(30))
                    return "배송 희망일은 오늘부터 30일 이내여야 합니다.";
                return null;
            }

            public string? ValidateDepositor(string payment, string depositor)
            {
                if (payment != "무통장") return null;
                return depositor.Trim() == "" ? "무통장 입금은 입금자명이 필요합니다." : null;
            }
        }
    }
}
