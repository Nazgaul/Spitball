using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Management_Application
{
    public partial class Move_Students : Form
    {
        String uniMoveId;
        String uniDeleteId="";
        String NoOfStudentsToMove;
        String connectionString="";
        public Move_Students()
        {
            InitializeComponent();
        }

        public Move_Students(string uniId, string noOfStudents, string connectionString)
        {
            InitializeComponent();
            uniDeleteId = uniId;
            this.NoOfStudentsToMove = noOfStudents;
            this.connectionString = connectionString;
            label2.Text = "Students to move: " + noOfStudents;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: dependency injection
            string select = "SELECT UniversityName FROM Zbox.University WHERE UniversityName like N'%" + uniNameSearch.Text + "%' and IsDeleted != 1 or IsDeleted IS NULL";

            //TODO: SQL connection is idisposible use the keyword using
            using (SqlConnection c = new SqlConnection(connectionString))
            {
                SqlCommand command = c.CreateCommand();
                command.Parameters.AddWithValue("@name", uniNameSearch.Text);
                command.CommandText = "SELECT UniversityName FROM Zbox.University WHERE UniversityName like @name and IsDeleted != 1 or IsDeleted IS NULL";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(select, connectionString);
                //SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                var ds = new DataSet();
                c.Open();
                dataAdapter.Fill(ds);
                c.Close();
                var dataTable = ds.Tables[0];
                uniListBox.DisplayMember = "UniversityName";
                uniListBox.ValueMember = "UniversityName";
                uniListBox.DataSource = dataTable;
            }
            
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using (var c = new SqlConnection(connectionString))
            {
               await c.OpenAsync();
               // string select = "SELECT Id, NoOfUsers FROM Zbox.University WHERE UniversityName like N'%" + uniListBox.GetItemText(uniListBox.SelectedItem) + "%'";
                if(uniListBox.GetItemText(uniListBox.SelectedItem)=="")
                {
                    MessageBox.Show("No University selected", "Error", MessageBoxButtons.OK);
                    return;
                }
                using (SqlCommand sqcommand = c.CreateCommand())
                {

                    sqcommand.Parameters.AddWithValue("@uniName", uniListBox.GetItemText(uniListBox.SelectedItem));
                    sqcommand.CommandText =
                        "SELECT ID, NoOfUsers FROM Zbox.University WHERE UniversityName like @uniName";
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter())
                    {
                        dataAdapter.SelectCommand = sqcommand;
                        dataAdapter.Fill(dataTable);
                    }
                    uniMoveId = dataTable.Rows[0]["Id"].ToString();
                    String noOfCurrentUsers = dataTable.Rows[0]["NoOfUsers"].ToString();
                    int totalStudents = int.Parse(noOfCurrentUsers) + int.Parse(NoOfStudentsToMove);
                    using (SqlCommand command = c.CreateCommand())
                    {
                        command.CommandText = "UPDATE Zbox.Users SET UniversityId=" + uniMoveId + " WHERE UniversityId=" +
                                              uniDeleteId;
                        command.ExecuteNonQuery();

                        //TODO: is deleted
                        command.CommandText =
                            "UPDATE Zbox.University SET NoOfUsers=0,IsDirty = 1, IsDeleted=1 WHERE Id=" + uniDeleteId;
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE Zbox.University SET IsDirty = 1, NoOfUsers=" + totalStudents +
                                              " WHERE Id=" + uniMoveId;
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("The Students where moved", "Complete", MessageBoxButtons.OK);
                    Close();
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
