using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMasterPro
{
    public partial class AddStudentForm : Form
    {
        public AddStudentForm()
        {
            InitializeComponent();
        }



        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (txtFirst.Text == "" || txtSecond.Text == "" || txtNumber.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Please fill in all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            try
            {
                using (OleDbConnection con = new OleDbConnection($@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={System.Windows.Forms.Application.StartupPath}\GradeMasterProDB.mdb"))
                {
                    con.Open();

                    string checkQuery = "Select Count(*) from Students where StudentNumber=@snum";
                    using(OleDbCommand cmd = new OleDbCommand(checkQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@snum", txtNumber.Text);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("This student number is already registered! Please use a different number.","Dublicate Number",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            return;
                        }
                    }




                    string query = "INSERT INTO Students (FirstName,SecondName,StudentNumber,[Password]) VALUES (@fn,@sn,@snum,@pwd)";

                    using (OleDbCommand cmd = new OleDbCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@fn", txtFirst.Text);
                        cmd.Parameters.AddWithValue("@sn", txtSecond.Text);
                        cmd.Parameters.AddWithValue("@snum", txtNumber.Text);
                        cmd.Parameters.AddWithValue("@pwd", txtPassword.Text);

                        cmd.ExecuteNonQuery();
                    }
                    int newStudentId = 0;
                    using (OleDbCommand cmdId = new OleDbCommand("Select @@Identity", con))
                    {
                        newStudentId = Convert.ToInt32(cmdId.ExecuteScalar());
                    }

                    for (int i = 1; i <= 9; i++)
                    {
                        string queryGrade = "insert into Grades (StudentId,CourseId,InstructorId,Grade1,Grade2,Average) Values (@sid,@cid,@iid,0,0,0)";

                        using (OleDbCommand cmd = new OleDbCommand(queryGrade, con))
                        {
                            cmd.Parameters.AddWithValue("@sid", newStudentId);
                            cmd.Parameters.AddWithValue("@cid", i);
                            cmd.Parameters.AddWithValue("@iid", i);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Student added succesully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
