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
using System.Data.OleDb;
using System.Diagnostics.Eventing.Reader;
using GradeMasterPro.Forms;

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


        string connectionString = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={System.Windows.Forms.Application.StartupPath}\GradeMasterProDB.mdb";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtUserNameStudentNumber;
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



        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userIdentifier = txtUserNameStudentNumber.Text;
            string password = txtPassword.Text;
            String role = null;

            if (userIdentifier != "" && password != "")
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {

                    con.Open();

                    string queryString = "SELECT * FROM Students WHERE StudentNumber = @StudentNumber AND [Password]=@Password";

                    using (OleDbCommand cmd = new OleDbCommand(queryString, con))
                    {
                        cmd.Parameters.AddWithValue("@StdNumber", userIdentifier);
                        cmd.Parameters.AddWithValue("Password", password);

                        using (OleDbDataReader reader = cmd.ExecuteReader())

                            if (reader.Read())
                            {
                                role = "Student";

                                StudentForm stdForm = new StudentForm();
                                this.Hide();
                                stdForm.ShowDialog();

                            }

                    }
                    if (role == null)
                    {
                        string queryInstructor = "SELECT * FROM Instructors WHERE UserName=@userName AND [Password]=@password";

                        using (OleDbCommand cmd = new OleDbCommand(queryInstructor, con))
                        {
                            cmd.Parameters.AddWithValue("@userName", userIdentifier);
                            cmd.Parameters.AddWithValue("Password", password);


                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    role = "Instructor";
                                   
                                    InstructorForm instForm = new InstructorForm();
                                    this.Hide();
                                    instForm.ShowDialog();
                                }
                            }

                        }
                    }

                    if (role == null)
                    {
                        MessageBox.Show("User not found, please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
            }
            else
                MessageBox.Show("Please fill in your login information completely.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }
    
