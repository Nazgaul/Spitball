﻿using System.Reflection;
using System.Text;
using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Net.Mail;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using SendGrid;
using System.IO;
using Microsoft.Win32;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Management_Application
{


    public partial class Form1 : Form
    {
        DataTable m_GlobalUserTable;
        DataTable m_GlobalTable;
        string m_Id;
        string m_NoOfStudents;
        readonly string m_ConnectionString;
        string m_UserId;
        UserrequestEntity m_GlobalFlaggedItem;
        UserrequestEntity m_GlobalFlaggedPost;

        public Form1()
        {
            InitializeComponent();
            m_ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString;
            CultureInfo[] c = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var a in c)
            {
                try
                {
                    var r = new RegionInfo(a.LCID);
                    //Console.WriteLine(r.TwoLetterISORegionName);
                    comboBoxCountry.Items.Add(r.TwoLetterISORegionName);
                }
                catch
                {
                }
            }
            InitializeFlaggedItems();
            InitializeFlaggedPosts();
        }

        private bool IsEmailValid(string emailaddress)
        {
            try
            {

                var m = new MailAddress(emailaddress);
                return m.Address == emailaddress;
            }
            catch
            {
                return false;
            }
        }
        private bool ValidateFields()
        {
            Uri uriResult;
            bool result = Uri.TryCreate(textBoxUniUrl.Text, UriKind.Absolute, out uriResult);
            if (!(result && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) && textBoxUniUrl.Text.Length > 0)
                return false;
            result = Uri.TryCreate(textBoxFBurl.Text, UriKind.Absolute, out uriResult);
            if (!(result && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) && textBoxFBurl.Text.Length > 0)
                return false;
            result = Uri.TryCreate(textBoxLetterURL.Text, UriKind.Absolute, out uriResult);
            if (!(result && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)) && textBoxLetterURL.Text.Length > 0)
                return false;
            if ((!IsEmailValid(textBoxEmail.Text)) && !textBoxEmail.Text.Equals(""))
                return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBoxStudentBody.Text, "^[0-9]*$"))
                return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBoxAdmins.Text, "^[0-9]*$"))
                return false;
            return true;
        }

        //Searches for the uni name and outputs all the results on the list box
        private void searchButton_Click(object sender, EventArgs e)
        {
            if (notDeletedRadioButton.Checked) //Picks University names that were not deleted
            {
                string select = "SELECT Id, UniversityName FROM Zbox.University WHERE UniversityName like N'%" + uniNameSearch.Text + "%' and IsDeleted = 0 or IsDeleted IS NULL";
                var dataAdapter = new SqlDataAdapter(select, m_ConnectionString);
                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                m_GlobalTable = dataTable;
                uniListBox.DisplayMember = "UniversityName";
                uniListBox.ValueMember = "Id";
                uniListBox.DataSource = dataTable;

            }
            else //Picks all university names, deleted and non deleted alike
            {
                string select = "SELECT Id, UniversityName FROM Zbox.University WHERE UniversityName like N'%" + uniNameSearch.Text + "%'";
                //SqlConnection c = new SqlConnection();
                var dataAdapter = new SqlDataAdapter(select, m_ConnectionString);
                //SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                //dataAdapter.ToString();
                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                m_GlobalTable = dataTable;
                uniListBox.DisplayMember = "UniversityName";
                uniListBox.ValueMember = "Id";
                uniListBox.DataSource = dataTable;
            }

        }

        //Shows all universities that are marked as not active, without searching
        //Those that are not active are marked as false or NULL
        private void showNotActive_Click(object sender, EventArgs e)
        {
            const string @select = "SELECT id, UniversityName FROM Zbox.University WHERE (Active != 1 or Active IS NULL) and isdeleted=0";
            using (var dataAdapter = new SqlDataAdapter(select, m_ConnectionString))
            {

                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                m_GlobalTable = dataTable;
                uniListBox.DisplayMember = "UniversityName";
                uniListBox.ValueMember = "id";
                uniListBox.DataSource = dataTable;
            }
        }

        //Changes the data over each line whenever a different university is selected
        private async void uniListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            richTextBoxStats.ReadOnly = true;
            using (var c = new SqlConnection(m_ConnectionString))
            {
                await c.OpenAsync();
                using (var command = c.CreateCommand())
                {
                    command.Parameters.AddWithValue("@id", uniListBox.SelectedValue);
                    command.CommandText = "SELECT * FROM Zbox.University WHERE id = @id";
                    var ds = new DataSet();
                    using (var dataAdapter = new SqlDataAdapter { SelectCommand = command })
                    {

                        dataAdapter.Fill(ds);
                    }
                    var dataTable = ds.Tables[0];
                    textBoxUniName.Text = dataTable.Rows[0]["UniversityName"].ToString();
                    textBoxUniUrl.Text = dataTable.Rows[0]["WebSiteUrl"].ToString();
                    if (!string.IsNullOrEmpty(dataTable.Rows[0]["LargeImage"].ToString()))
                    {
                        pictureBoxUni.Load(dataTable.Rows[0]["LargeImage"].ToString());
                        pictureBoxUni.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    textBoxAdmins.Text = dataTable.Rows[0]["AdminNoOfPeople"].ToString();
                    textBoxEmail.Text = dataTable.Rows[0]["MailAddress"].ToString();
                    textBoxExtraSearch.Text = dataTable.Rows[0]["Extra"].ToString();
                    textBoxFBurl.Text = dataTable.Rows[0]["FacebookUrl"].ToString();
                    textBoxLetterURL.Text = dataTable.Rows[0]["LetterUrl"].ToString();
                    textBoxOrgName.Text = dataTable.Rows[0]["OrgName"].ToString();
                    textBoxStudentBody.Text = dataTable.Rows[0]["StudentBody"].ToString();
                    textBoxImage.Text = dataTable.Rows[0]["LargeImage"].ToString();
                    comboBoxCountry.Text = dataTable.Rows[0]["Country"].ToString();
                    m_Id = dataTable.Rows[0]["Id"].ToString();
                    m_NoOfStudents = dataTable.Rows[0]["NoOfUsers"].ToString();
                    if (dataTable.Rows[0]["Active"].ToString() == "True")
                        comboBox1.Text = "Active";
                    else
                        comboBox1.Text = "Not Active";
                    if (dataTable.Rows[0]["IsDeleted"].ToString() == "True")
                    {
                        LabelIsDeleted.ForeColor = Color.Red;
                        LabelIsDeleted.Text = "Deleted";
                    }

                    else
                    {
                        LabelIsDeleted.ForeColor = Color.Green;
                        LabelIsDeleted.Text = "Not-Deleted";
                    }

                    richTextBoxStats.Text = "Stats:\n\nCreated on: " + dataTable.Rows[0]["CreationTime"] +
                                            "\n\nStudents: " +
                                            dataTable.Rows[0]["NoOfUsers"] + "\n\nItems: " +
                                            dataTable.Rows[0]["NoOfItems"] +
                                            "\n\nBoxes: " + dataTable.Rows[0]["NoOfBoxes"] + "\n\nQuizes: " +
                                            dataTable.Rows[0]["NoOfQuizzes"] + "\n\nAdmin Score:" +
                                            dataTable.Rows[0]["AdminScore"] + "\n\nUniversityID:" +
                                            dataTable.Rows[0]["ID"];
                }
            }
        }

        private string RetrieveTextFromTextBox(TextBox t)
        {
            return string.IsNullOrEmpty(t.Text) ? null : t.Text;
        }

        //Saves the data as it is shown to the SQL database
        private async void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                MessageBox.Show("Some of the fields are in incorrect format.", "Error", MessageBoxButtons.OK);
                return;
            }

            //DialogResult result= MessageBox.Show("Are you sure you want to save? This action can't be undone.", "Save", MessageBoxButtons.OKCancel);
            //if (result == DialogResult.Cancel)
            //   return;


            using (var connection = new SqlConnection(m_ConnectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(
                     "UPDATE Zbox.University SET OrgName=@orgName, UniversityName=@uniName, WebSiteUrl=@uniUrl, AdminNoOfPeople=@admins, Country=@country, StudentBody=@stuBody, Extra=@extraSearch, MailAddress=@email, FacebookUrl=@fbUrl, LetterUrl=@letterUrl, Active=@active, isdirty=1, LargeImage=@largeImage WHERE Id=@id",
                     new
                     {
                         uniName = RetrieveTextFromTextBox(textBoxUniName),
                         orgName = RetrieveTextFromTextBox(textBoxOrgName),
                         country = comboBoxCountry.Text,
                         admins = textBoxAdmins.Text,
                         stuBody = textBoxStudentBody.Text,
                         extraSearch = RetrieveTextFromTextBox(textBoxExtraSearch),
                         uniUrl = RetrieveTextFromTextBox(textBoxUniUrl),
                         email = RetrieveTextFromTextBox(textBoxEmail),
                         fbUrl = RetrieveTextFromTextBox(textBoxFBurl),
                         letterUrl = RetrieveTextFromTextBox(textBoxLetterURL),
                         largeImage = RetrieveTextFromTextBox(textBoxImage),
                         id = m_Id,
                         active = comboBox1.Text == "Active" ? 1 : 0
                     });
                Refresh();
                MessageBox.Show("Done");
            }
        }


        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            if (m_NoOfStudents != "0")
            {

                var ms = new Move_Students(m_Id, m_NoOfStudents, m_ConnectionString);
                ms.ShowDialog();
                Refresh();
            }
            else
            {

                using (var connection = new SqlConnection(m_ConnectionString))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE Zbox.University SET IsDeleted=1 , Isdirty =1 , UpdatedUser='deleteTool' WHERE Id=" + m_Id;
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("The college was successfully deleted", "College Deleted", MessageBoxButtons.OK);
                        Refresh();
                    }
                }
            }



        }

        private void buttonChangePicture_Click(object sender, EventArgs e)
        {
            ChangePicture cp = new ChangePicture(m_Id);
            cp.ShowDialog();
            Refresh();

        }

        private void Refresh()
        {
            String name;
            name = textBoxUniName.Text;
            searchButton_Click(new object(), new EventArgs());
            //string select = "SELECT * FROM Zbox.University WHERE UniversityName like N'%" + uniListBox.GetItemText(uniListBox.SelectedItem)+ "%'";
            //string connectionString = "Data Source=LENOVO-PC;Initial Catalog=Zbox;Integrated Security=True";
            var c = new SqlConnection(m_ConnectionString);
            SqlCommand command = c.CreateCommand();
            command.Parameters.AddWithValue("@name", name);
            command.CommandText = "SELECT top 1 * FROM Zbox.University WHERE UniversityName like @name";
            //string select = command.CommandText;
            //SqlDataAdapter dataAdapter = new SqlDataAdapter(select, "Data Source=LENOVO-PC;Initial Catalog=Zbox;Integrated Security=True");
            SqlDataAdapter dataAdapter = new SqlDataAdapter { SelectCommand = command };
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            var ds = new DataSet();
            var dataTable = new DataTable();
            c.Open();
            dataAdapter.Fill(ds);
            c.Close();
            dataTable = ds.Tables[0];
            textBoxUniName.Text = dataTable.Rows[0]["UniversityName"].ToString();
            textBoxUniUrl.Text = dataTable.Rows[0]["WebSiteUrl"].ToString();
            try
            {
                if (!string.IsNullOrEmpty(dataTable.Rows[0]["LargeImage"].ToString()))
                {

                    pictureBoxUni.Load(dataTable.Rows[0]["LargeImage"].ToString());
                    pictureBoxUni.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch { }
            textBoxAdmins.Text = dataTable.Rows[0]["AdminNoOfPeople"].ToString();
            textBoxEmail.Text = dataTable.Rows[0]["MailAddress"].ToString();
            textBoxExtraSearch.Text = dataTable.Rows[0]["Extra"].ToString();
            textBoxFBurl.Text = dataTable.Rows[0]["FacebookUrl"].ToString();
            textBoxLetterURL.Text = dataTable.Rows[0]["LetterUrl"].ToString();
            textBoxOrgName.Text = dataTable.Rows[0]["OrgName"].ToString();
            textBoxStudentBody.Text = dataTable.Rows[0]["StudentBody"].ToString();
            textBoxImage.Text = dataTable.Rows[0]["LargeImage"].ToString();
            comboBoxCountry.Text = dataTable.Rows[0]["Country"].ToString();
            m_Id = dataTable.Rows[0]["Id"].ToString();
            m_NoOfStudents = dataTable.Rows[0]["NoOfUsers"].ToString();
            if (dataTable.Rows[0]["Active"].ToString() == "True")
                comboBox1.Text = "Active";
            else
                comboBox1.Text = "Not Active";
            if (dataTable.Rows[0]["IsDeleted"].ToString() == "True")
            {
                LabelIsDeleted.ForeColor = Color.Red;
                LabelIsDeleted.Text = "Deleted";
            }

            else
            {
                LabelIsDeleted.ForeColor = Color.Green;
                LabelIsDeleted.Text = "Not-Deleted";
            }

            richTextBoxStats.Text = "Stats:\n\nCreated on: " + dataTable.Rows[0]["CreationTime"] + "\n\nStudents: " + dataTable.Rows[0]["NoOfUsers"] + "\n\nItems: " + dataTable.Rows[0]["NoOfItems"] +
                "\n\nBoxes: " + dataTable.Rows[0]["NoOfBoxes"] + "\nQuizes: " + dataTable.Rows[0]["NoOfQuizzes"] + "\nAdmin Score:" + dataTable.Rows[0]["AdminScore"] + "\nuniversityID:" + dataTable.Rows[0]["ID"];
            richTextBoxStats.ReadOnly = true;
            int index = uniListBox.FindString(textBoxUniName.Text);
            if (index != -1)
                uniListBox.SetSelected(index, true);

        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                try
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.Parameters.AddWithValue("@email", textBoxUserEmailSearch.Text);
                        command.CommandText = "SELECT * FROM Zbox.Users WHERE Email=@email";
                        var dataAdapter = new SqlDataAdapter { SelectCommand = command };
                        await connection.OpenAsync();
                        using (var dataTable = new DataTable())
                        {
                            dataAdapter.Fill(dataTable);
                            m_GlobalUserTable = dataTable;
                            m_UserId = dataTable.Rows[0]["UserId"].ToString();
                            textBoxUserEmail.Text = dataTable.Rows[0]["Email"].ToString();
                            textBoxUserName.Text = dataTable.Rows[0]["UserName"].ToString();
                            comboBoxUserType.Text = dataTable.Rows[0]["UserType"].ToString() == "1"
                                ? "Admin"
                                : "Not Admin";
                            pictureBoxUser.InitialImage = null;

                            if (dataTable.Rows[0]["UserImageLarge"].ToString() != "NULL" &&
                                dataTable.Rows[0]["UserImageLarge"].ToString() != "")
                            {
                                pictureBoxUser.Load(dataTable.Rows[0]["UserImageLarge"].ToString());
                                pictureBoxUser.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            String uniName = "-None-";
                            if (dataTable.Rows[0]["UniversityId"].ToString() != "NULL" &&
                                dataTable.Rows[0]["UniversityId"].ToString() != "")
                            {
                                command.Parameters.AddWithValue("@uniId", dataTable.Rows[0]["UniversityId"].ToString());
                                command.CommandText = "SELECT UniversityName FROM Zbox.University WHERE Id=@uniId";
                                using (var dataAdapter2 = new SqlDataAdapter { SelectCommand = command })
                                {

                                    var dataTable2 = new DataTable();
                                    dataAdapter2.Fill(dataTable2);
                                    uniName = dataTable2.Rows[0]["UniversityName"].ToString();
                                }
                            }
                            String gender;
                            if (dataTable.Rows[0]["Sex"].ToString() == "True")
                                gender = "Female";
                            else
                                gender = "Male";
                            String mobile = "No";
                            if (dataTable.Rows[0]["MobileDevice"].ToString() == "1")
                                mobile = "Yes";
                            command.Parameters.AddWithValue("@userId", dataTable.Rows[0]["UserId"].ToString());
                            command.CommandText = "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId";
                            int boxes = (int)command.ExecuteScalar();

                            command.CommandText = "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId";
                            int quizzes = (int)command.ExecuteScalar();



                            richTextBoxUser.Text = "Stats\n\nCreated on:" + dataTable.Rows[0]["CreationTime"] +
                                                   "\n\nLast Activity: " + dataTable.Rows[0]["UpdateTime"] +
                                                   "\n\nUniversity Name: " + uniName +
                                                   "\n\nUrl: https://www.cloudents.com/"
                                                   + dataTable.Rows[0]["Url"] +
                                                   "\n\nFacebook: https://www.facebook.com" + "/" +
                                                   dataTable.Rows[0]["FacebookUserId"] + "\n\nReputation: " +
                                                   dataTable.Rows[0]["UserReputation"] + "\n\nCulture: "
                                                   + dataTable.Rows[0]["Culture"] + "\n\nGender: " + gender +
                                                   "\n\nMobile: " + mobile + "\n\nMobile Joined: " + "--" +
                                                   "\n\nBoxes: " +
                                                   boxes + "\n\nQuizzes: " + quizzes + "\n\nItems: ";
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Couldn't find the specified mail", "Error", MessageBoxButtons.OK);
                }


            }
        }

        private void buttonUserSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@email", textBoxUserEmail.Text);
                command.Parameters.AddWithValue("@userName", textBoxUserName.Text);
                String type;
                if (comboBoxUserType.Text == "Admin")
                    type = "1";
                else
                    type = "0";
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@userId", m_UserId);
                command.CommandText = "UPDATE Zbox.Users SET Email=@email, UserName=@userName, UserType=@type WHERE UserId=@userId";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            MessageBox.Show("Changes were saved", "Saved", MessageBoxButtons.OK);
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void buttonChangeUserImage_Click(object sender, EventArgs e)
        {
            ChangePictureUser cp = new ChangePictureUser(m_UserId);
            cp.ShowDialog();
            RefreshUser();
        }

        private void RefreshUser()
        {
            buttonSearch_Click(new int(), new EventArgs());
        }

        private string LoadScript()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Management_Application.DeleteUsers.sql"))
            {
                if (stream != null)
                {
                    var content = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(content, 0, (int)stream.Length);
                    return Encoding.UTF8.GetString(content);
                }
                return string.Empty;
            }
        }
        private async void buttonUserDelete_Click(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                await connection.OpenAsync();

                var script = LoadScript();
                if (string.IsNullOrEmpty(script))
                {
                    MessageBox.Show("Some problem loading script");
                    return;
                }
                await connection.ExecuteScalarAsync(script, new { Email = textBoxUserEmail.Text });
                MessageBox.Show("The user was deleted", "User Deleted", MessageBoxButtons.OK);
                textBoxUserEmail.Text = "";
                textBoxUserEmailSearch.Text = "";
                textBoxUserName.Text = "";
                richTextBoxUser.Text = "";
                pictureBoxUser.InitialImage = null;
                pictureBoxUser.Image = null;
                comboBoxUserType.Text = "";
            }
        }

        private void InitializeFlaggedItems()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("userrequests");
            TableQuery<UserrequestEntity> query = new TableQuery<UserrequestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagItem"));
            flaggedItemsListBox.Items.Clear();
            foreach (UserrequestEntity entity in table.ExecuteQuery(query))
            {
                flaggedItemsListBox.Items.Add(entity.ItemId);
                Console.WriteLine(entity.UserId);

                Console.WriteLine(entity.ItemId);
            }
        }
        public class UserrequestEntity : TableEntity
        {

            public UserrequestEntity(string partition, string row)
            {
                PartitionKey = partition;
                RowKey = row;
            }

            public UserrequestEntity() { }

            // public string Timestamp { get; set; }

            public string BadItem { get; set; }
            public Int64 ItemId { get; set; }
            public Int64 UserId { get; set; }
            public Guid PostId { get; set; }

        }
        private void flaggedItemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //pictureBoxFlaggedItem = new PictureBox();
            //pictureBoxUserThatFlagged = new PictureBox();
            pictureBoxUserThatFlagged.InitialImage = null;
            pictureBoxUserThatFlagged.Image = null;
            pictureBoxFlaggedItem.InitialImage = null;
            pictureBoxFlaggedItem.Image = null;
            richTextBoxFlaggedItem.Text = "";
            richTextBoxUserThatFlagged.Text = "";
            richTextBoxItemToReplace.Text = "";
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("userrequests");
                TableQuery<UserrequestEntity> query = new TableQuery<UserrequestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagItem"));
                m_GlobalFlaggedItem = null;
                foreach (UserrequestEntity entity in table.ExecuteQuery(query))
                {
                    if (entity.ItemId.ToString() == flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem))
                    {
                        m_GlobalFlaggedItem = entity;
                        break;
                    }

                    Console.WriteLine(flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem));
                    Console.WriteLine(entity.ItemId);
                    Console.WriteLine(entity.UserId);

                }
                //flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem) //--Item ID not users
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedItem.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = command;
                connection.Open();
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                dataTable = ds.Tables[0];
                connection.Close();
                try
                {
                    String uniName = "";
                    if (dataTable.Rows[0]["UniversityId"].ToString() != "NULL" && dataTable.Rows[0]["UniversityId"].ToString() != "")
                    {
                        command.Parameters.AddWithValue("@uniId", dataTable.Rows[0]["UniversityId"].ToString());
                        command.CommandText = "SELECT UniversityName FROM Zbox.University WHERE Id=@uniId";
                        SqlDataAdapter dataAdapter2 = new SqlDataAdapter();
                        dataAdapter2.SelectCommand = command;
                        DataTable dataTable2 = new DataTable();
                        dataAdapter2.Fill(dataTable2);
                        uniName = dataTable2.Rows[0]["UniversityName"].ToString();
                    }
                    String Gender = "";
                    if (dataTable.Rows[0]["Sex"].ToString() == "True")
                        Gender = "Female";
                    else
                        Gender = "Male";
                    String Mobile = "No";
                    if (dataTable.Rows[0]["MobileDevice"].ToString() == "1")
                        Mobile = "Yes";
                    int Boxes = 0;
                    command.Parameters.AddWithValue("@userId", dataTable.Rows[0]["UserId"].ToString());
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId";
                    connection.Open();
                    Boxes = (int)command.ExecuteScalar();
                    int Quizzes = 0;
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId";
                    Quizzes = (int)command.ExecuteScalar();
                    int Items = 0;
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Item WHERE UserId=@userId";
                    Items = (int)command.ExecuteScalar();


                    var urls = connection.Query<string>("SELECT url FROM Zbox.Item WHERE Itemid=@itemId", new { itemId = m_GlobalFlaggedItem.ItemId });
                    var itemUrl = urls.FirstOrDefault();


                    connection.Close();
                    richTextBoxUserThatFlagged.Text = "Created on:" + dataTable.Rows[0]["CreationTime"].ToString() + "\nLast Activity: " + dataTable.Rows[0]["UpdateTime"].ToString() + "\nUniversity Name: " + uniName + "\nUrl: https://www.cloudents.com/"
                        + dataTable.Rows[0]["Url"].ToString() + "\nFacebook: https://www.facebook.com" + "/" + dataTable.Rows[0]["FacebookUserId"].ToString() + "\nReputation: " + dataTable.Rows[0]["UserReputation"].ToString() + "\nCulture: "
                        + dataTable.Rows[0]["Culture"].ToString() + "\nGender: " + Gender + "\nMobile: " + Mobile + "\nMobile Joined: " + "--" + "\nBoxes: " + Boxes + "\nQuizzes: " + Quizzes + "\nItems: " + Items + "\nitemUrl: https://www.cloudents.com/" + itemUrl;

                    //richTextBoxUserThatFlagged.Text = ("Name: "+dataTable.Rows[0]["UserName"].ToString())+"\n\nEmail: "+dataTable.Rows[0]["Email"].ToString()+"\n\n"+dataTable.Rows[0][];
                    try
                    {
                        pictureBoxUserThatFlagged.Load(dataTable.Rows[0]["UserImageLarge"].ToString());
                        pictureBoxUserThatFlagged.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch (Exception)
                    {

                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("Check if the user exists", "User Problem", MessageBoxButtons.OK);
                }
                command.Parameters.AddWithValue("@ItemId", m_GlobalFlaggedItem.ItemId);
                command.CommandText = "SELECT*  FROM Zbox.Item WHERE ItemId=@ItemId";
                dataAdapter.SelectCommand = command;
                connection.Open();
                ds.Clear();
                dataAdapter.Fill(ds);
                dataTable = ds.Tables[0];
                connection.Close();
                try
                {
                    richTextBoxFlaggedItem.Text = "Item Name: " + dataTable.Rows[0]["name"] + "\n\nCreated User: " + dataTable.Rows[0]["CreatedUser"].ToString() + "\n\nUploaded On: " + dataTable.Rows[0]["CreationTime"].ToString() + "\n\nDownloads: " + dataTable.Rows[0]["NumberOfDownloads"].ToString() + "\n\nUrl: https://www.cloudents.com/" + dataTable.Rows[0]["Url"].ToString();
                    String boxId = dataTable.Rows[0]["BoxId"].ToString();
                    try
                    {
                        Console.WriteLine("https://www.cloudents.com/" + dataTable.Rows[0]["ThumbnailUrl"].ToString());
                        pictureBoxFlaggedItem.Load(dataTable.Rows[0]["ThumbnailUrl"].ToString());
                        pictureBoxUserThatFlagged.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Image Exception");
                    }
                    command.Parameters.AddWithValue("@boxId", boxId);
                    command.CommandText = "SELECT * FROM Zbox.Box WHERE BoxId=@boxId";
                    dataAdapter.SelectCommand = command;
                    ds.Clear();
                    connection.Open();
                    dataAdapter.Fill(ds);

                    dataTable = ds.Tables[0];
                    connection.Close();
                    Console.WriteLine("There");

                    richTextBoxFlaggedItem.Text += "\n\nBox Name: " + dataTable.Rows[0]["BoxName"].ToString();
                    richTextBoxItemToReplace.Text = richTextBoxFlaggedItem.Text;


                }
                catch (Exception)
                {
                    //pictureBoxFlaggedItem.Load("C:\\Users\\USER\\Desktop\\Android\\palette.png");
                    //pictureBoxUserThatFlagged.SizeMode = PictureBoxSizeMode.Zoom;
                }
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void InitializeFlaggedPosts()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("userrequests");
            TableQuery<UserrequestEntity> Query = new TableQuery<UserrequestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagPost"));
            listBoxFlaggedPosts.Items.Clear();
            foreach (UserrequestEntity entity in table.ExecuteQuery(Query))
            {
                listBoxFlaggedPosts.Items.Add(entity.PostId.ToString());
                Console.WriteLine(entity.UserId);

                Console.WriteLine(entity.PostId);
            }
        }

        private void listBoxFlaggedPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //richTextBoxFlaggedPost.Text = "";
            richTextBoxUserFlaggedPost.Text = "";
            pictureBoxUserFlaggedPost.InitialImage = null;
            pictureBoxUserFlaggedPost.Image = null;
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("userrequests");
                TableQuery<UserrequestEntity> Query = new TableQuery<UserrequestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagPost"));
                m_GlobalFlaggedPost = null;
                foreach (UserrequestEntity entity in table.ExecuteQuery(Query))
                {
                    if (entity.PostId.ToString() == listBoxFlaggedPosts.GetItemText(listBoxFlaggedPosts.SelectedItem))
                    {
                        Console.WriteLine(entity.PostId);

                        m_GlobalFlaggedPost = entity;
                        break;
                    }

                    //Console.WriteLine(flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem));
                    //Console.WriteLine(entity.UserId);

                }
                //globalFlaggedItem.ItemId  itemid
                //flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem) //--Item ID not users
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedPost.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = command;
                connection.Open();
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                dataTable = ds.Tables[0];
                connection.Close();
                try
                {
                    //richTextBoxUserFlaggedPost.Text = "Email: "+dataTable.Rows[0]["Email"].ToString() + "\n\nName: " + (dataTable.Rows[0]["UserName"].ToString()) + "\n\nCulture: " + dataTable.Rows[0]["Culture"].ToString();
                    String uniName = "";
                    if (dataTable.Rows[0]["UniversityId"].ToString() != "NULL" && dataTable.Rows[0]["UniversityId"].ToString() != "")
                    {
                        command.Parameters.AddWithValue("@uniId", dataTable.Rows[0]["UniversityId"].ToString());
                        command.CommandText = "SELECT UniversityName FROM Zbox.University WHERE Id=@uniId";
                        SqlDataAdapter dataAdapter2 = new SqlDataAdapter();
                        dataAdapter2.SelectCommand = command;
                        DataTable dataTable2 = new DataTable();
                        dataAdapter2.Fill(dataTable2);
                        uniName = dataTable2.Rows[0]["UniversityName"].ToString();
                    }
                    String Gender = "";
                    if (dataTable.Rows[0]["Sex"].ToString() == "True")
                        Gender = "Female";
                    else
                        Gender = "Male";
                    String Mobile = "No";
                    if (dataTable.Rows[0]["MobileDevice"].ToString() == "1")
                        Mobile = "Yes";
                    var Boxes = 0;
                    command.Parameters.AddWithValue("@userId", dataTable.Rows[0]["UserId"].ToString());
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId";
                    connection.Open();
                    Boxes = (int)command.ExecuteScalar();
                    int Quizzes = 0;
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId";
                    Quizzes = (int)command.ExecuteScalar();
                    int Items = 0;
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Item WHERE UserId=@userId";
                    Items = (int)command.ExecuteScalar();

                    var urls = connection.Query<string>("select url from zbox.box b inner join zbox.question q on b.boxid = q.boxid where questionid = @items", new { items = m_GlobalFlaggedPost.PostId });
                    var url = urls.FirstOrDefault();


                    connection.Close();
                    richTextBoxUserFlaggedPost.Text = "Created on:" + dataTable.Rows[0]["CreationTime"].ToString() + "\nLast Activity: " + dataTable.Rows[0]["UpdateTime"].ToString() + "\nUniversity Name: " + uniName + "\nUrl: https://www.cloudents.com/"
                        + dataTable.Rows[0]["Url"].ToString() + "\nFacebook: https://www.facebook.com" + "/" + dataTable.Rows[0]["FacebookUserId"].ToString() + "\nReputation: " + dataTable.Rows[0]["UserReputation"].ToString() + "\nCulture: "
                        + dataTable.Rows[0]["Culture"].ToString() + "\nGender: " + Gender + "\nMobile: " + Mobile + "\nMobile Joined: " + "--" + "\nBoxes: " + Boxes + "\nQuizzes: " + Quizzes + "\nItems: " + Items + "\nURL: https://www.cloudents.com/" + url;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("User doesn't exist", "User Problem", MessageBoxButtons.OK);
                }
                try
                {

                    pictureBoxUserFlaggedPost.Load(dataTable.Rows[0]["UserImageLarge"].ToString());
                    pictureBoxUserFlaggedPost.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception)
                {

                }


                command.Parameters.AddWithValue("@PostId", m_GlobalFlaggedPost.PostId.ToString());
                command.CommandText = "SELECT * FROM Zbox.Question WHERE QuestionId=@PostId";
                //command.CommandText = "SELECT * FROM Zbox.Answer WHERE AnswerId=@PostId";
                dataAdapter.SelectCommand = command;
                connection.Open();

                try
                {
                    ds.Clear();
                    dataAdapter.Fill(ds);
                    dataTable = ds.Tables[0];
                    Console.WriteLine(ds.Tables[0].Rows.Count);
                    if (ds.Tables[0].Rows.Count == 0)
                        throw new Exception();
                }
                catch (Exception)
                {
                    command.CommandText = "SELECT * FROM Zbox.Answer WHERE AnswerId=@PostId";
                    dataAdapter.SelectCommand = command;
                    ds.Clear();
                    dataAdapter.Fill(ds);
                    dataTable = ds.Tables[0];
                    Console.WriteLine("Answer");
                }
                connection.Close();
                try
                {
                    richTextBoxFlaggedPostContent.Text = dataTable.Rows[0]["Text"].ToString();// +"\n\n" + dataTable.Rows[0]["UserId"].ToString();

                }
                catch (Exception)
                {

                }
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void buttonDeleteFlag_Click(object sender, EventArgs e)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("userrequests");
            TableQuery<UserrequestEntity> Query = new TableQuery<UserrequestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagItem"));
            m_GlobalFlaggedItem = null;
            foreach (UserrequestEntity entity in table.ExecuteQuery(Query))
            {
                if (entity.ItemId.ToString() == flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem))
                {
                    TableOperation deleteOperation = TableOperation.Delete(entity);
                    table.Execute(deleteOperation);
                    break;
                }

            }
            MessageBox.Show("Flag deleted", "Deleted", MessageBoxButtons.OK);
            InitializeFlaggedItems();
            richTextBoxUserThatFlagged.Text = "";
            richTextBoxFlaggedItem.Text = "";
            pictureBoxUserThatFlagged.Image = null;
            pictureBoxUserThatFlagged.InitialImage = null;
            pictureBoxFlaggedItem.Image = null;
            pictureBoxFlaggedItem.InitialImage = null;
        }

        private void buttonDeleteFlaggedItem_Click(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@ItemId", m_GlobalFlaggedItem.ItemId);
                command.CommandText = "UPDATE Zbox.Item SET IsDeleted=1 WHERE ItemId=@ItemId";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private async void buttonDeletePost_Click(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        await connection.OpenAsync();
                        command.Parameters.AddWithValue("@PostId", m_GlobalFlaggedPost.PostId);
                        command.CommandText = "DELETE FROM Zbox.Answer WHERE AnswerId=@PostId";
                        await command.ExecuteNonQueryAsync();
                        command.CommandText = "DELETE FROM Zbox.Question WHERE QuestionId=@PostId";
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error " + ex.Message);
                    }

                }
            }
        }

        private void buttonDeleteFlagPost_Click(object sender, EventArgs e)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("userrequests");
            TableQuery<UserrequestEntity> query = new TableQuery<UserrequestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagPost"));
            m_GlobalFlaggedItem = null;
            foreach (UserrequestEntity entity in table.ExecuteQuery(query))
            {
                if (entity.PostId.ToString() == listBoxFlaggedPosts.GetItemText(listBoxFlaggedPosts.SelectedItem))
                {
                    TableOperation deleteOperation = TableOperation.Delete(entity);
                    table.Execute(deleteOperation);
                    break;
                }

            }
            MessageBox.Show("Flag deleted", "Deleted", MessageBoxButtons.OK);
            //richTextBoxFlaggedPost.Text = "";
            richTextBoxUserFlaggedPost.Text = "";
            pictureBoxUserFlaggedPost.InitialImage = null;
            pictureBoxUserFlaggedPost.Image = null;
            InitializeFlaggedPosts();
        }

        private void buttonReportUserPost_Click(object sender, EventArgs e)
        {
            const string sendGridUserName = "cloudents";
            const string sendGridPassword = "zbangitnow";
            var credentials = new NetworkCredential(sendGridUserName, sendGridPassword);
            var FlagPostMessage = new SendGridMessage();
            String mailTo;
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedPost.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                var dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = command;
                connection.Open();
                var dataTable = new DataTable();
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                dataTable = ds.Tables[0];
                connection.Close();
                mailTo = dataTable.Rows[0]["Email"].ToString();
            }

            FlagPostMessage.AddTo(mailTo);
            FlagPostMessage.From = new MailAddress("cloudents@gmail.com");
            FlagPostMessage.Subject = "Flag Report";
            FlagPostMessage.Text = "We have received the flag and dealt with it accordingly.\n Thank you for contributing to the Cloudents community.";

            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(FlagPostMessage);

        }

        private void buttonReportBackUser_Click(object sender, EventArgs e)
        {
            const string sendGridUserName = "cloudents";
            const string sendGridPassword = "zbangitnow";
            var credentials = new NetworkCredential(sendGridUserName, sendGridPassword);
            var FlagPostMessage = new SendGridMessage();
            String mailTo;
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedItem.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                var dataAdapter = new SqlDataAdapter { SelectCommand = command };
                connection.Open();
                var dataTable = new DataTable();
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                dataTable = ds.Tables[0];
                connection.Close();
                mailTo = dataTable.Rows[0]["Email"].ToString();
            }

            FlagPostMessage.AddTo(mailTo);
            FlagPostMessage.From = new MailAddress("cloudents@gmail.com");
            FlagPostMessage.Subject = "Flag Report";
            FlagPostMessage.Text = "We have received the flag and dealt with it accordingly.\n Thank you for contributing to the Cloudents community.";
            MessageBox.Show("Mail sent to " + mailTo, "Sent", MessageBoxButtons.OK);
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(FlagPostMessage);
        }

        private void buttonBrowseAttachments_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialogReplaceItem.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialogReplaceItem.FileName;
                textBoxAttachment.Text = file;
            }
        }
        private String GetBlobName(String itemId)
        {
            var ret = "";
            const string queryString = "SELECT BlobName FROM Zbox.Item WHERE ItemId = @NM";

            using (var connection = new SqlConnection(m_ConnectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@NM", itemId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        ret = reader["BlobName"].ToString();
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            return ret;
        }
        private void buttonReplaceItem_Click(object sender, EventArgs e)
        {
            //string ItemId = textBox1.Text;
            string itemId = m_GlobalFlaggedItem.ItemId.ToString(CultureInfo.InvariantCulture);
            //Console.WriteLine(getBlobName(ItemName));
            //string blobName = textBox1.Text;
            string blobName = GetBlobName(itemId);
            blobName = Path.GetFileNameWithoutExtension(blobName);
            // blobName = blobName.Remove(blobName.IndexOf('.'));
            String fileLocation = textBoxAttachment.Text;
            //CloudStorageAccount.DevelopmentStorageAccount;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("zboxcahce");
            foreach (IListBlobItem item in container.ListBlobs(blobName, false))
            {
                var blob = (CloudBlockBlob)item;

                blob.Delete();
            }

            string fileName = "";
            int i = fileLocation.Length;
            while (fileLocation[i - 1] != '\\')
            {
                fileName += fileLocation[i - 1];
                i--;
            }
            string fileName2 = "";
            i = fileName.Length;
            while (i > 0)
            {
                fileName2 += fileName[i - 1];
                i--;
            }
            Console.WriteLine(fileName2);
            UploadBlob(blobName, fileLocation);
            MessageBox.Show("File has been replaced, press OK to continue.", "File Replaced", MessageBoxButtons.OK);
            textBoxAttachment.Text = "";
        }

        private void UploadBlob(String blobName, String fileLocation)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("zboxfiles");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            if (blockBlob.Exists())
            {
                blockBlob.FetchAttributes();
                blockBlob.Metadata.Clear();
            }

            //blockBlob.BeginFetchAttributes();
            //Check how to fetch the properties, for metadata and type
            using (var fileStream = File.OpenRead(fileLocation))
            {
                var extension = Path.GetExtension(fileLocation);
                var registryKey = Registry.ClassesRoot.OpenSubKey(extension);
                var value = registryKey.GetValue("Content Type") as string;
                blockBlob.Properties.ContentType = value;
                blockBlob.UploadFromStream(fileStream);
            }

        }




        private async void buttonMergeBoxes_Click(object sender, EventArgs e)
        {
            var boxIdFrom = Int64.Parse(textBoxBoxFrom.Text);
            var boxIdTo = Int64.Parse(textBoxBoxTo.Text);

            const string sql1 =
                "Select Boxid as id, boxname as name,[MembersCount] as memberCount, [ItemCount] as itemCount from zbox.box where boxid in (@fromid, @toid) and isdeleted = 0 ";

            using (var conn = new SqlConnection(m_ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand command = conn.CreateCommand())
                {

                    command.Parameters.AddWithValue("@fromid", boxIdFrom);
                    command.Parameters.AddWithValue("@toid", boxIdTo);
                    command.CommandText = sql1;
                    var ds = new DataSet();
                    using (var dataAdapter = new SqlDataAdapter { SelectCommand = command })
                    {
                        dataAdapter.Fill(ds);
                    }
                    var dataTable = ds.Tables[0];
                    dataGridViewMergeBoxes.DataSource = dataTable;
                    dataGridViewMergeBoxes.Visible = true;
                    buttonMerge.Visible = true;
                    buttonMerge.Enabled = true;
                }

            }
        }

        private async void buttonMergeBoxesMakeItSo_Click(object sender, EventArgs e)
        {
            var boxIdFrom = Int64.Parse(textBoxBoxFrom.Text);
            var boxIdTo = Int64.Parse(textBoxBoxTo.Text);

            using (var conn = new SqlConnection(m_ConnectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteScalarAsync(@"
begin transaction
go
Update zbox.UserBoxRel set boxid=@toid, UpdatedUser = 'mergeTool'
where boxid=@fromid and userid in 
(Select Userid from zbox.Userboxrel where boxid=@fromid
except 
Select Userid from zbox.Userboxrel where boxid=@toid);
Delete from zbox.UserBoxRel where boxid=@fromid;

update zbox.Invite set BoxId =@toid where BoxId = @fromid;
update zbox.item set boxid=@toid, [IsDirty]=1, UpdatedUser = 'mergeTool' where boxid=@fromid;
update [Zbox].[Comment] set boxid=@toid where boxid=@fromid;

update [Zbox].[Quiz] set boxid=@toid, [IsDirty]=1, UpdatedUser = 'mergeTool' where boxid=@fromid;
update [Zbox].[Question] set boxid=@toid, UpdatedUser = 'mergeTool' where boxid=@fromid;
update zbox.Answer set boxid=@toid, UpdatedUser = 'mergeTool' where boxid=@fromid;

Update zbox.box set [IsDirty]=1, [IsDeleted]=1, LibraryId = null, UpdatedUser = 'mergeTool' where boxid=@fromid;
Update zbox.box set [IsDirty]=1, 
MembersCount = (select count(*) from zbox.UserBoxRel where BoxId = @toid),
ItemCount = (select count(*) from zbox.item where BoxId = @toid and IsDeleted = 0),
CommentCount = (select count(*) from zbox.Question where BoxId = @toid),
QuizCount = (select count(*) from zbox.Quiz where BoxId = @toid and IsDeleted = 0 and Publish = 1),
UpdatedUser = 'mergeTool' where boxid=@toid;
update zbox.Message set BoxId = @toid where BoxId = @fromid;

delete from [Zbox].[Question] where boxid = @fromid;
delete from zbox.NewUpdates where BoxId = @fromid;
commit transaction", new { fromid = boxIdFrom, toid = boxIdTo });

                MessageBox.Show("Done");

                dataGridViewMergeBoxes.Visible = false;
                buttonMerge.Visible = false;
                buttonMerge.Enabled = false;
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var boxId = long.Parse(textBoxDeleteBox.Text);
            var iocFactory = IocFactory.IocWrapper;

            var writeService = iocFactory.Resolve<IZboxWorkerRoleService>();
            await writeService.DeleteBoxAsync(new Zbang.Zbox.Domain.Commands.DeleteBoxCommand(boxId));


            //            using (var conn = new SqlConnection(m_ConnectionString))
            //            {
            //                await conn.OpenAsync();
            //                await conn.ExecuteScalarAsync(@"update zbox.box set IsDeleted=1, IsDirty=1, LibraryId = null, UpdatedUser = 'deleteTool' where boxid=@deleteid;
            //    update zbox.item set IsDeleted=1, IsDirty=1, UpdatedUser = 'deleteTool', QuestionId = null where boxid=@deleteid;
            //    update zbox.Quiz set [IsDeleted]=1 , isdirty=1, UpdatedUser = 'deleteTool' where BoxId=@deleteid;

            //    delete from zbox.NewUpdates where BoxId = @deleteid;
            //    delete from zbox.Invite where BoxId = @deleteid;
            //    delete from zbox.Answer where boxid = @deleteid
            //    delete from zbox.Question where boxid = @deleteid;
            //    delete zbox.CommentReplies where ParentId in ( select CommentId from zbox.Comment where boxid = @deleteid);
            //    delete [Zbox].[Comment] where BoxId=@deleteid;
            //    delete zbox.Invite where BoxId=@deleteid;
            //    delete from zbox.UserBoxRel where boxid=@deleteid;
            //    delete from zbox.Message where boxid = @deleteid;", 
            //new { deleteid = boxId });

            MessageBox.Show("Done");
            //            }
        }

        private void dataGridViewMergeBoxes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (e.Value != null && dataGridView1.Columns[e.ColumnIndex] == theRelevantColumn)
            //{
            //    e.Value = e.Value.ToString();
            //}
        }




    }
}