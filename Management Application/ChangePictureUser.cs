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
    public partial class ChangePictureUser : Form
    {
        String userId;
        public ChangePictureUser()
        {
            InitializeComponent();
        }
        public ChangePictureUser(String UserId)
        {
            userId = UserId;
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                textBoxPath.Text = file;
            }
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.Parameters.AddWithValue("@path", textBoxPath.Text);
            command.CommandText = "UPDATE Zbox.Users SET UserImageLarge=@path WHERE UserId=" + userId;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Picture Changed.", "OK", MessageBoxButtons.OK);
            this.Close();
        }
    }
}
