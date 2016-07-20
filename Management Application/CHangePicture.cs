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
    public partial class ChangePicture : Form
    {
        String Id;
        public ChangePicture(String Id)
        {
            InitializeComponent();
            this.Id = Id;
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
            command.CommandText = "UPDATE Zbox.University SET LargeImage=@path WHERE Id=" + Id;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Picture Changed.", "OK", MessageBoxButtons.OK);
            this.Close();
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChangePicture_Load(object sender, EventArgs e)
        {

        }
    }
}
