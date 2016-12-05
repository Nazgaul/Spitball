using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Management_Application
{
    public partial class ChangePictureUser : Form
    {
        private readonly long m_UserId;
        public ChangePictureUser()
        {
            InitializeComponent();
        }
        public ChangePictureUser(long userId)
        {
            m_UserId = userId;
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
            command.CommandText = "UPDATE Zbox.Users SET UserImageLarge=@path WHERE UserId=" + m_UserId;
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Picture Changed.", "OK", MessageBoxButtons.OK);
            this.Close();
        }
    }
}
