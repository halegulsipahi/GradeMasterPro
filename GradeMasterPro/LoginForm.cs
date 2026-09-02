using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace GradeMasterPro
{
    public partial class LoginForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);

            }
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pbMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lnkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(
        "Need help signing in?\n\n" +
        "• If you forgot your password, click 'Forgot Password?' to reset it.\n" +
        "• For account activation or technical issues, please contact your system administrator.\n\n" +
        "Email: help@grademasterpro.com",
        "GradeMaster Pro - Support & Help",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
    );
        }

        private void pbGithub_Click(object sender, EventArgs e)
        {

            Process.Start("https://github.com/halegulsipahi");

        }

       
        private void pbShowHide_Click_1(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            pbShowHide.Image = txtPassword.UseSystemPasswordChar ? Properties.Resources.show : Properties.Resources.hide;
        }

        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(
        "Please contact your system administrator or IT department to reset your password.\n\n" +
        "Support: help@grademasterpro.com",
        "Password Reset",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
        }
    }
}
