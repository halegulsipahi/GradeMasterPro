using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMasterPro.Forms
{
    public partial class StudentForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        string connectionString = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={System.Windows.Forms.Application.StartupPath}\GradeMasterProDB.mdb";

        string currentStudentNo;

        public StudentForm(string studentNo)
        {
            InitializeComponent();
            currentStudentNo = studentNo;

        }
        private void LoadStudentGrades()
        {

            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    string query = @"SELECT 
                    Courses.CourseName AS [Course Name], 
                    Grades.Grade1 AS [Exam 1], 
                    Grades.Grade2 AS [Exam 2], 
                    Grades.Average AS [Average]
                 FROM (Grades 
                 INNER JOIN Courses ON CLng(Grades.CourseID) = Courses.CourseID ) 
                 INNER JOIN Students ON CLng(Grades.StudentID) = Students.StudentID 
                 WHERE Students.StudentNumber = @sNo";

                    using (OleDbDataAdapter dA = new OleDbDataAdapter(query, con))
                    {
                        dA.SelectCommand.Parameters.AddWithValue("@sNo", currentStudentNo);

                        DataTable dt = new DataTable();
                        dA.Fill(dt);

                        dgvGrades.DataSource = dt;

                        object avrg = dt.Compute("AVG([Average])", "");
                        lblGeneralAverage.Text = "Overall GPA: " + Convert.ToDouble(avrg).ToString("0.00");
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Failed to load grades: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentInfo()
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    string query = "Select FirstName , SecondName ,StudentNumber from Students where StudentNumber= @sNo";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@sNo", currentStudentNo);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["FirstName"].ToString();
                                string secondName = reader["SecondName"].ToString();
                                string studentNumber = reader["StudentNumber"].ToString();

                                lblStudentFullName.Text = "Student Name/Surname: " + firstName + " " + secondName;
                                lblStudentNumber.Text = "Student Number: " + studentNumber;

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show("Failed to load student info: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadStudentInfo();
            LoadStudentGrades();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pbMinimize_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
        }

        private void pbBack_Click(object sender, EventArgs e)
        {
            LoginForm lgn = new LoginForm();
            lgn.Show();
            this.Close();
        }

        private void lnkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string message = "GradeMasterPro - Student Support & Academic Help Desk\n\n" +
                     "• Grade Objections / Corrections:\n" +
                     "  If you notice an error in your grades, submit an objection petition to the Student Affairs office within 5 business days following the announcement.\n\n" +
                     "• General Average (GPA) Policy:\n" +
                     "  The overall GPA represents the arithmetic average of all active courses in the current semester.\n\n" +
                     "• Contact Details:\n" +
                     "  - E-mail: support@grademasterpro.com\n" +
                     "  - Student Affairs: ext. 1024\n" +
                     "  - Office Hours: Monday - Friday, 09:00 - 17:00\n\n" +
                     "For urgent system access or login issues, please contact the IT Help Desk.";

            MessageBox.Show(message, "Academic Support & Help Desk", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pbGithub_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/halegulsipahi",
                UseShellExecute = true
            });
        }

        private void StudentForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);

            }
        }
    }
}
