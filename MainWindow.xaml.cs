using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateTotpButton_Click(object sender, RoutedEventArgs e)
        {
            // קבלת הקלט מהמשתמש
            string userInput = UserInputTextBox.Text;

            // בדיקה אם הקלט הוא מספר תקין
            if (int.TryParse(userInput, out int secretNumber))
            {
                // יצירת קוד TOTP על בסיס המספר שהוזן
                string totpCode = TotpGenerator.GenerateTotpCode(secretNumber.ToString());
                MessageBox.Show($"הקוד שלך הוא: {totpCode}");
                UserInputTextBox.Clear();
            }
            else
            {
                MessageBox.Show(".נא להזין מספר תקין", "שגיאה");
                UserInputTextBox.Clear();
            }
        }
    }

    public static class TotpGenerator
    {
        public static string GenerateTotpCode(string secretKey, int intervalSeconds = 60)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / intervalSeconds;
            byte[] timeBytes = BitConverter.GetBytes(currentTime);
            if (BitConverter.IsLittleEndian) Array.Reverse(timeBytes);

            using (var hmac = new HMACSHA1(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(timeBytes);
                int offset = hash[hash.Length - 1] & 0x0F;
                int binaryCode = ((hash[offset] & 0x7F) << 24) |
                                 ((hash[offset + 1] & 0xFF) << 16) |
                                 ((hash[offset + 2] & 0xFF) << 8) |
                                 (hash[offset + 3] & 0xFF);
                int totpCode = binaryCode % 1000000;
                return totpCode.ToString("D6");
            }
        }
    }
}
