using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GradeMasterPro.Forms
{
    public partial class InstructorForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public InstructorForm(string instructorId)
        {
            InitializeComponent();
            currentInstructorId = instructorId;
            LoadInstructorAndCourse();
        }
        string connectionString = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={System.Windows.Forms.Application.StartupPath}\GradeMasterProDB.mdb";

        string currentInstructorId;
        string currentCourseId = "";
        string currentCourseName = "";
        int currentGrade1;
        int currentGrade2;

        private void LoadDataToDataGridView()
        {
            pnlUpdateGrade.Visible = false;
            pnlReports.Visible = false;
            pnlViewStudent.Visible = true;
            pnlViewStudent.BringToFront();

            string query = "select StudentID,FirstName,SecondName,StudentNumber from Students";

            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    dgwAllStudents.DataSource = dt;

                    lblInfo.Text = "All Students";
                }
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        private void LoadInstructorAndCourse()
        {
            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();
                string queryInst = "select FirstName , SecondName From Instructors where InstructorID=@id";

                using (OleDbCommand cmdInst = new OleDbCommand(queryInst, con))
                {
                    cmdInst.Parameters.AddWithValue("@id", currentInstructorId);

                    using (OleDbDataReader dr = cmdInst.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            lblInstructor.Text = dr["FirstName"].ToString() + " " + dr["SecondName"].ToString();
                          
                        }

                    }
                }
                string queryCourse = "SELECT CourseID,CourseName From Courses where InstructorID =@id";
                using (OleDbCommand cmdCourse = new OleDbCommand(queryCourse, con))
                {
                    cmdCourse.Parameters.AddWithValue("@id", currentInstructorId);
                    using (OleDbDataReader dr = cmdCourse.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            currentCourseId = dr["CourseID"].ToString();
                            currentCourseName = dr["CourseName"].ToString();
                            lblInstCourse.Text = currentCourseName;
                            lblCourseTitle.Text = "Course: " + currentCourseName;
                           
                        }


                    }
                }
            }
        }
        private void LoadStudentGradeInformation(string studentId)
        {


            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();
                string queryGrade = " select Grade1,Grade2,Average from Grades where StudentID=@sId AND CourseID=@cId";

                using (OleDbCommand cmdG = new OleDbCommand(queryGrade, con))
                {
                    cmdG.Parameters.AddWithValue("@sId", studentId);
                    cmdG.Parameters.AddWithValue("@cId", currentCourseId);

                    using (OleDbDataReader drG = cmdG.ExecuteReader())
                    {
                        lblCourseTitle.Text = "Course: " + currentCourseName;

                        if (drG.Read())
                        {
                            currentGrade1 = Convert.ToInt32(drG["Grade1"]);
                            currentGrade2 = Convert.ToInt32(drG["Grade2"]);


                            lblCurrentGrade1.Text = "Grade 1: " + drG["Grade1"].ToString();
                            lblCurrentGrade2.Text = "Grade 2: " + drG["Grade2"].ToString();
                            lblCurrentAverage.Text = "Average: " + drG["Average"].ToString();

                            double average = Convert.ToDouble(drG["Average"]);
                            if (average >= 50)
                            {
                                lblCurrentAverage.Text = "Average: " + average.ToString("0.##") + " (Passed!)";
                                lblCurrentAverage.ForeColor = ColorTranslator.FromHtml("#8B9A6E");
                            }
                            else
                            {
                                lblCurrentAverage.Text = "Average: " + average.ToString("0.##") + " (Failed!)";
                                lblCurrentAverage.ForeColor = ColorTranslator.FromHtml("#853953");
                            }

                        }
                        else
                        {
                            lblCurrentGrade1.Text = "Grade 1: -";
                            lblCurrentGrade2.Text = "Grade 2: -";
                            lblCurrentAverage.Text = "Average: -";

                            txtNewGrade1.Clear();
                            txtNewGrade2.Clear();
                        }
                    }
                }
            }

        }

        private void UpdateGrades()
        {
            pnlViewStudent.Visible = false;
            pnlReports.Visible = false;
            pnlUpdateGrade.Visible = true;


            string query = "select * from Students";

            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    cmbStudents.DisplayMember = "FirstName";
                    cmbStudents.ValueMember = "StudentID";
                    cmbStudents.DataSource = dt;
                }
            }

            lblInfo.Text = "Update Grade";
        }

        private void LoadReports()
        {
            pnlViewStudent.Visible = false;
            pnlUpdateGrade.Visible = false;
            pnlReports.Visible = true;
            pnlReports.BringToFront();
            lblInfo.Text = "Reports && Analytics";

            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    using (OleDbCommand cmd = new OleDbCommand("Select Count(*) from Students", con))
                    {
                        int totalStudents = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalStudents.Text = "Total Student Number: " + totalStudents.ToString();
                    }

                    using (OleDbCommand cmd = new OleDbCommand("Select Count(*) from Instructors", con))
                    {
                        int totalInstructor = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalInstructors.Text = "Total Instructor Number: " + totalInstructor.ToString();
                    }

                    using (OleDbCommand cmd = new OleDbCommand("Select Count(*) from Courses", con))
                    {
                        int totalCourses = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalCourse.Text = "Total Course Number: " + totalCourses.ToString();
                    }

                    lblReportsCourseName.Text = "Course: " + currentCourseName;

                    string queryPassed = "SELECT COUNT(*) FROM Grades WHERE CourseID=@cId AND Average>=50";
                    using (OleDbCommand cmd = new OleDbCommand(queryPassed, con))
                    {
                        cmd.Parameters.AddWithValue("@cId", currentCourseId);
                        int passedCount = Convert.ToInt32(cmd.ExecuteScalar());
                        lblNumberOfSuccessful.Text = "Number of Successfull Students: " + passedCount.ToString();
                    }

                    string queryFailed = "SELECT COUNT(*) FROM Grades WHERE CourseID=@cId AND Average<50";
                    using (OleDbCommand cmd = new OleDbCommand(queryFailed, con))
                    {
                        cmd.Parameters.AddWithValue("@cId", currentCourseId);
                        int failedCount = Convert.ToInt32(cmd.ExecuteScalar());
                        lblNumberOfFail.Text = "Number of Failing Students:" + failedCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                }
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

        private void pbBack_Click(object sender, EventArgs e)
        {
            LoginForm lgn = new LoginForm();
            lgn.Show();
            this.Close();
        }

        private void btnViewStudent_Click(object sender, EventArgs e)
        {
            LoadDataToDataGridView();

        }

        private void btnUpdateGrade_Click(object sender, EventArgs e)
        {
            UpdateGrades();
        }
        private void txtNewGrade1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtNewGrade2_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }
        private void txtNewGrade2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSaveGrade.PerformClick();
            }

        }

        private void InstructorForm_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);

            }
        }

        private void cmbStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedIndex >= 0)
            {
                DataRowView row = cmbStudents.SelectedItem as DataRowView;
                if (row != null)
                {
                    lblStudentID.Text = "Student ID: " + row["StudentID"].ToString();
                    lblStudentFirstName.Text = "First Name: " + row["FirstName"].ToString();
                    lblStudentSecondName.Text = "Second Name: " + row["SecondName"].ToString();
                    lblStudentNumber.Text = "Student Number: " + row["StudentNumber"].ToString();

                    string selectedId = row["StudentID"].ToString();
                    LoadStudentGradeInformation(selectedId);
                }
            }
        }

        private void btnSaveGrade_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNewGrade1.Text) || string.IsNullOrEmpty(txtNewGrade2.Text))
            {
                MessageBox.Show("Please enter both grades!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int g1 = Convert.ToInt32(txtNewGrade1.Text);
            int g2 = Convert.ToInt32(txtNewGrade2.Text);


            if (g1 < 0 || g1 > 100 || g2 < 0 || g2 > 100)
            {
                MessageBox.Show("Grades must be between 0 and 100!", "Invalid Grade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double average = (g1 + g2) / 2.0;

            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    string query = "UPDATE Grades SET Grade1=@grd1 ,Grade2=@grd2,Average=@avrg WHERE StudentID=@StudentId AND CourseID=@courseId";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@grd1", g1);
                        cmd.Parameters.AddWithValue("@grd2", g2);
                        cmd.Parameters.AddWithValue("@avrg", average);
                        cmd.Parameters.AddWithValue("@StudentId", cmbStudents.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@courseId", currentCourseId);

                        cmd.ExecuteNonQuery();
                    }
                }

                currentGrade1 = Convert.ToInt32(g1);
                currentGrade2 = Convert.ToInt32(g2);
                lblCurrentGrade1.Text = "Grade 1: " + g1.ToString();
                lblCurrentGrade2.Text = "Grade 2: " + g2.ToString();


                if (average >= 50)
                {
                    lblCurrentAverage.Text = "Average: " + average.ToString("0.##") + " (Passed!)";
                    lblCurrentAverage.ForeColor = ColorTranslator.FromHtml("#8B9A6E");
                }
                else
                {
                    lblCurrentAverage.Text = "Average: " + average.ToString("0.##") + " (Failed!)";
                    lblCurrentAverage.ForeColor = ColorTranslator.FromHtml("#853953");
                }
                txtNewGrade1.Clear();
                txtNewGrade2.Clear();
                MessageBox.Show("Grades updated succesfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void lnkSupportInstructorPanel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string helpText = "GradeMasterPro - Instructor User Guide\n\n" +
                              "1. Select a student from the dropdown list.\n" +
                              "2. Current grades will load in the 'Student Grade Information' panel.\n" +
                              "3. Enter both new grades in the relevant boxes under 'Update' (0-100).\n" +
                              "4. Click 'UPDATE' to calculate the new average and save.\n\n" +
                              "Technical Support: support@grademasterpro.com\n" +
                              "Developer: Hale Gül Sipahi\n" +
                              "System Version: v1.0.0";

            MessageBox.Show(helpText, "Instructor Help & Support", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pbGithubInst_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/halegulsipahi",
                UseShellExecute = true
            });
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            this.Hide();

            AddStudentForm frm = new AddStudentForm();
            frm.ShowDialog();

            this.Show();
            LoadDataToDataGridView();
            UpdateGrades();

        }

        private void btnReports_Click(object sender, EventArgs e)
        {

            LoadReports();
        }

      
    }
}
