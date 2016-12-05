using System.Reflection;
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
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using System.Threading;
using Zbang.Zbox.Infrastructure.Azure.Entities;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Enums;

namespace Management_Application
{


    public partial class Form1 : Form
    {
        private const string QuizUrl = "https://api.quizlet.com/2.0/sets/";
        DataTable m_GlobalUserTable;
        DataTable m_GlobalTable;
        string m_Id;
        string m_NoOfStudents;
        readonly string m_ConnectionString;
        long m_UserId;
        FlagItem m_GlobalFlaggedItem;
        FlagCommentOrReply m_GlobalFlaggedPost;
        //private readonly IIdGenerator m_IdGenerator;

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
            //tabUniversity.Ta
            //InitializeFlaggedItems();
            //InitializeFlaggedPosts();
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
            const string select = "SELECT id, UniversityName FROM Zbox.University WHERE (Active != 1 or Active IS NULL) and isdeleted=0";
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
                    comboBox1.Text = dataTable.Rows[0]["Active"].ToString() == "True" ? "Active" : "Not Active";
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

#pragma warning disable CS0114 // 'Form1.Refresh()' hides inherited member 'Control.Refresh()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        private void Refresh()
#pragma warning restore CS0114 // 'Form1.Refresh()' hides inherited member 'Control.Refresh()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        {
            var uniName = textBoxUniName.Text;
            searchButton_Click(new object(), new EventArgs());
            var c = new SqlConnection(m_ConnectionString);
            SqlCommand command = c.CreateCommand();
            command.Parameters.AddWithValue("@name", uniName);
            command.CommandText = "SELECT top 1 * FROM Zbox.University WHERE UniversityName like @name";
            SqlDataAdapter dataAdapter = new SqlDataAdapter { SelectCommand = command };
            var ds = new DataSet();
            c.Open();
            dataAdapter.Fill(ds);
            c.Close();
            var dataTable = ds.Tables[0];
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
            comboBox1.Text = dataTable.Rows[0]["Active"].ToString() == "True" ? "Active" : "Not Active";
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
                "\n\nBoxes: " + dataTable.Rows[0]["NoOfBoxes"] + "\nQuizes: " + dataTable.Rows[0]["NoOfQuizzes"] + "\nAdmin Score:" + dataTable.Rows[0]["AdminScore"] + "\n universityID:" + dataTable.Rows[0]["ID"];
            richTextBoxStats.ReadOnly = true;
            int index = uniListBox.FindString(textBoxUniName.Text);
            if (index != -1)
                uniListBox.SetSelected(index, true);

        }

        private async void buttonSearch_Click(object sender, EventArgs e)
        {
            using (var connection = await DapperConnection.OpenConnectionAsync())
            {
                const string startOfsql =
                    @"select userid,email,UserName,UserType,UserImageLarge,Sex,MobileDevice,u.CreationTime, u.url,FacebookUserId,
score,Culture,uu.UniversityName
from zbox.Users u
left join zbox.University uu on u.UniversityId = uu.Id ";
                long temp;
                var sql = startOfsql + "where email = @email";
                if (long.TryParse(textBoxUserEmailSearch.Text, out temp))
                {
                    sql = startOfsql + "where userid = @email";
                }
                var data = await connection.QueryFirstOrDefaultAsync<User>(sql, new { email = textBoxUserEmailSearch.Text });
                if (data == null)
                {
                    MessageBox.Show("No User");
                    return;
                }
                m_UserId = data.UserId;
                textBoxUserEmail.Text = data.Email;
                textBoxUserName.Text = data.UserName;
                comboBoxUserType.Text = data.UserType == UserType.TooHighScore
                    ? "Admin"
                    : "Not Admin";
                pictureBoxUser.InitialImage = null;

                if (!string.IsNullOrEmpty(data.UserImageLarge))
                {
                    pictureBoxUser.LoadAsync(data.UserImageLarge);
                    pictureBoxUser.SizeMode = PictureBoxSizeMode.Zoom;
                }

                var gender = data.Sex.GetEnumDescription();
                var mobile = data.MobileDevice;
                var boxes = 0;
                var quizzes = 0;
                using (var grid = await connection.QueryMultipleAsync(
                    "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId;" +
                    "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId and isdeleted = 0 and publish =1 ;"
                    , new { userId = data.UserId }))
                {
                    boxes = await grid.ReadFirstOrDefaultAsync<int>();
                    quizzes = await grid.ReadFirstOrDefaultAsync<int>();
                }

                //command.Parameters.AddWithValue("@userId", dataTable.Rows[0]["UserId"].ToString());
                //command.CommandText = "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId";
                //int boxes = (int)command.ExecuteScalar();

                //command.CommandText = "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId";
                //int quizzes = (int)command.ExecuteScalar();



                richTextBoxUser.Text = "Stats\n\nCreated on:" + data.CreationTime +
                                       // "\n\nLast Activity: " + data.UpdateTime +
                                       "\n\nUniversity Name: " + data.UniversityName +
                                       "\n\nUrl: https://www.spitball.co/"
                                       + data.Url +
                                       "\n\nFacebook: https://www.facebook.com" + "/" +
                                       data.FacebookUserId + "\n\nReputation: " +
                                       data.Score + "\n\nCulture: "
                                       + data.Culture + "\n\nGender: " + gender +
                                       "\n\nMobile: " + mobile + "\n\nMobile Joined: " + "--" +
                                       "\n\nBoxes: " +
                                       boxes + "\n\nQuizzes: " + quizzes;
                //+ "\n\nItems: ";
            }

        }

        private void buttonUserSave_Click(object sender, EventArgs e)
        {
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@email", textBoxUserEmail.Text);
                    command.Parameters.AddWithValue("@userName", textBoxUserName.Text);
                    var type = comboBoxUserType.Text == "Admin" ? "1" : "0";
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@userId", m_UserId);
                    command.CommandText =
                        "UPDATE Zbox.Users SET Email=@email, UserName=@userName, UserType=@type WHERE UserId=@userId";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
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

        private static string LoadScript()
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
            var query = new TableQuery<FlagItem>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagItem"));
            flaggedItemsListBox.Items.Clear();
            foreach (var entity in table.ExecuteQuery(query))
            {
                flaggedItemsListBox.Items.Add(entity.ItemId);
            }
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
            m_GlobalFlaggedItem = null;
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("userrequests");
            TableQuery<FlagItem> query = new TableQuery<FlagItem>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagItem"));

            //m_GlobalFlaggedItem = null;
            foreach (var entity in table.ExecuteQuery(query))
            {
                if (entity.ItemId.ToString() == flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem))
                {
                    m_GlobalFlaggedItem = entity;
                    break;
                }

                //Console.WriteLine(flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem));
                //Console.WriteLine(entity.ItemId);
                //Console.WriteLine(entity.UserId);

            }
            if (m_GlobalFlaggedItem == null)
            {
                MessageBox.Show("Something not right");
                return;
            }
            using (var connection = new SqlConnection(m_ConnectionString))
            {

                //flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem) //--Item ID not users
                var command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedItem.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                var dataAdapter = new SqlDataAdapter { SelectCommand = command };
                connection.Open();
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                var dataTable = ds.Tables[0];
                connection.Close();
                try
                {
                    var uniName = "";
                    if (dataTable.Rows[0]["UniversityId"].ToString() != "NULL" && dataTable.Rows[0]["UniversityId"].ToString() != "")
                    {
                        command.Parameters.AddWithValue("@uniId", dataTable.Rows[0]["UniversityId"].ToString());
                        command.CommandText = "SELECT UniversityName FROM Zbox.University WHERE Id=@uniId";
                        SqlDataAdapter dataAdapter2 = new SqlDataAdapter { SelectCommand = command };
                        DataTable dataTable2 = new DataTable();
                        dataAdapter2.Fill(dataTable2);
                        uniName = dataTable2.Rows[0]["UniversityName"].ToString();
                    }
                    var gender = dataTable.Rows[0]["Sex"].ToString() == "True" ? "Female" : "Male";
                    string mobile = "No";
                    if (dataTable.Rows[0]["MobileDevice"].ToString() == "1")
                        mobile = "Yes";
                    command.Parameters.AddWithValue("@userId", dataTable.Rows[0]["UserId"].ToString());
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId";
                    connection.Open();
                    var boxes = (int)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId";
                    var quizzes = (int)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Item WHERE UserId=@userId";
                    var items = (int)command.ExecuteScalar();

                    connection.Close();
                    richTextBoxUserThatFlagged.Text = "Created on:" + dataTable.Rows[0]["CreationTime"] + "\nLast Activity: " + dataTable.Rows[0]["UpdateTime"] + "\nUniversity Name: " + uniName + "\nUrl: https://www.spitball.co/"
                        + dataTable.Rows[0]["Url"] + "\nFacebook: https://www.facebook.com" + "/" + dataTable.Rows[0]["FacebookUserId"] + "\nReputation: " + dataTable.Rows[0]["score"] + "\nCulture: "
                        + dataTable.Rows[0]["Culture"] + "\nGender: " + gender + "\nMobile: " + mobile + "\nMobile Joined: " + "--" + "\nBoxes: " + boxes + "\nQuizzes: " + quizzes + "\nItems: " + items;

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
                command.CommandText = "SELECT *  FROM Zbox.Item WHERE ItemId=@ItemId";
                dataAdapter.SelectCommand = command;
                connection.Open();
                ds.Clear();
                dataAdapter.Fill(ds);
                dataTable = ds.Tables[0];
                connection.Close();
                try
                {
                    richTextBoxFlaggedItem.Text = "Item Name: " + dataTable.Rows[0]["name"] + "\n\nCreated User: "
                        + dataTable.Rows[0]["CreatedUser"] + "\n\nUploaded On: "
                        + dataTable.Rows[0]["CreationTime"] + "\n\nDownloads: "
                        + dataTable.Rows[0]["NumberOfDownloads"]
                        + "\n\nUrl: https://www.spitball.co/" + dataTable.Rows[0]["Url"] + "\n\nreason:" + m_GlobalFlaggedItem.BadItem;
                    string boxId = dataTable.Rows[0]["BoxId"].ToString();
                    try
                    {
                        pictureBoxFlaggedItem.Load($"https://az779114.vo.msecnd.net:443/preview/{dataTable.Rows[0]["BlobName"]}.jpg?width=209&height=157");
                        //pictureBoxUserThatFlagged.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Image Exception");
                    }
                    command.Parameters.AddWithValue("@boxId", boxId);
                    command.CommandText = "SELECT * FROM Zbox.Box WHERE BoxId=@boxId";
                    dataAdapter.SelectCommand = command;
                    ds.Clear();
                    connection.Open();
                    dataAdapter.Fill(ds);

                    dataTable = ds.Tables[0];
                    connection.Close();
                    //Console.WriteLine("There");

                    richTextBoxFlaggedItem.Text += "\n\nBox Name: " + dataTable.Rows[0]["BoxName"];
                    richTextBoxItemToReplace.Text = richTextBoxFlaggedItem.Text;


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void InitializeFlaggedPosts()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("userrequests");
            var query = new TableQuery<FlagCommentOrReply>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagPost"));
            listBoxFlaggedPosts.Items.Clear();
            foreach (var entity in table.ExecuteQuery(query))
            {
                listBoxFlaggedPosts.Items.Add(entity.PostId.ToString());
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
                TableQuery<FlagCommentOrReply> query = new TableQuery<FlagCommentOrReply>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagPost"));
                m_GlobalFlaggedPost = null;
                foreach (var entity in table.ExecuteQuery(query))
                {
                    if (entity.PostId.ToString() == listBoxFlaggedPosts.GetItemText(listBoxFlaggedPosts.SelectedItem))
                    {
                        // Console.WriteLine(entity.PostId);

                        m_GlobalFlaggedPost = entity;
                        break;
                    }

                    //Console.WriteLine(flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem));
                    //Console.WriteLine(entity.UserId);

                }
                if (m_GlobalFlaggedPost == null)
                {
                    MessageBox.Show("Something wrong");
                    return;
                }
                //globalFlaggedItem.ItemId  itemid
                //flaggedItemsListBox.GetItemText(flaggedItemsListBox.SelectedItem) //--Item ID not users
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedPost.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                SqlDataAdapter dataAdapter = new SqlDataAdapter { SelectCommand = command };
                connection.Open();
                //SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds);
                var dataTable = ds.Tables[0];
                connection.Close();
                try
                {
                    //richTextBoxUserFlaggedPost.Text = "Email: "+dataTable.Rows[0]["Email"].ToString() + "\n\nName: " + (dataTable.Rows[0]["UserName"].ToString()) + "\n\nCulture: " + dataTable.Rows[0]["Culture"].ToString();
                    String uniName = "";
                    if (dataTable.Rows[0]["UniversityId"].ToString() != "NULL" && dataTable.Rows[0]["UniversityId"].ToString() != "")
                    {
                        command.Parameters.AddWithValue("@uniId", dataTable.Rows[0]["UniversityId"].ToString());
                        command.CommandText = "SELECT UniversityName FROM Zbox.University WHERE Id=@uniId";
                        SqlDataAdapter dataAdapter2 = new SqlDataAdapter { SelectCommand = command };
                        DataTable dataTable2 = new DataTable();
                        dataAdapter2.Fill(dataTable2);
                        uniName = dataTable2.Rows[0]["UniversityName"].ToString();
                    }
                    var gender = dataTable.Rows[0]["Sex"].ToString() == "True" ? "Female" : "Male";
                    var mobile = "No";
                    if (dataTable.Rows[0]["MobileDevice"].ToString() == "1")
                        mobile = "Yes";
                    command.Parameters.AddWithValue("@userId", dataTable.Rows[0]["UserId"].ToString());
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.UserBoxRel WHERE UserId=@userId";
                    connection.Open();
                    var boxes = (int)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Quiz WHERE UserId=@userId";
                    var quizzes = (int)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM Zbox.Item WHERE UserId=@userId";
                    var items = (int)command.ExecuteScalar();

                    var urls = connection.Query<string>("select url from zbox.box b inner join zbox.question q on b.boxid = q.boxid where questionid = @items", new { items = m_GlobalFlaggedPost.PostId });
                    var url = urls.FirstOrDefault();


                    connection.Close();
                    richTextBoxUserFlaggedPost.Text = "Created on:" + dataTable.Rows[0]["CreationTime"] + "\nLast Activity: " + dataTable.Rows[0]["UpdateTime"] + "\nUniversity Name: " + uniName + "\nUrl: https://www.spitball.co/"
                        + dataTable.Rows[0]["Url"] + "\nFacebook: https://www.facebook.com" + "/" + dataTable.Rows[0]["FacebookUserId"] + "\nReputation: " + dataTable.Rows[0]["score"] + "\nCulture: "
                        + dataTable.Rows[0]["Culture"] + "\nGender: " + gender + "\nMobile: " + mobile + "\nMobile Joined: " + "--" + "\nBoxes: " + boxes + "\nQuizzes: " + quizzes + "\nItems: " + items + "\nURL: https://www.spitball.co/" + url;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("User doesn't exist" + ex, "User Problem", MessageBoxButtons.OK);
                }
                try
                {

                    pictureBoxUserFlaggedPost.Load(dataTable.Rows[0]["UserImageLarge"].ToString());
                    pictureBoxUserFlaggedPost.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
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
                    // Console.WriteLine(ds.Tables[0].Rows.Count);
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
            var query = new TableQuery<FlagItem>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagItem"));
            //m_GlobalFlaggedItem = null;
            foreach (var entity in table.ExecuteQuery(query))
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

        private async void buttonDeleteFlaggedItem_Click(object sender, EventArgs e)
        {
            var itemId = m_GlobalFlaggedItem.ItemId;
            var service = IocFactory.IocWrapper.Resolve<IZboxWriteService>();
            await service.DeleteItemAsync(new DeleteItemCommand(itemId, 1));
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

        private async void buttonDeleteFlagPost_Click(object sender, EventArgs e)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("userrequests");
            var query = new TableQuery<FlagCommentOrReply>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "FlagPost"));
            //m_GlobalFlaggedItem = null;
            foreach (var entity in table.ExecuteQuery(query))
            {
                if (entity.PostId.ToString() == listBoxFlaggedPosts.GetItemText(listBoxFlaggedPosts.SelectedItem))
                {
                    TableOperation deleteOperation = TableOperation.Delete(entity);
                    await table.ExecuteAsync(deleteOperation);
                    break;
                }

            }
            MessageBox.Show("Flag deleted", "Deleted", MessageBoxButtons.OK);
            richTextBoxUserFlaggedPost.Text = "";
            pictureBoxUserFlaggedPost.InitialImage = null;
            pictureBoxUserFlaggedPost.Image = null;
            InitializeFlaggedPosts();
        }

        private async void buttonReportUserPost_Click(object sender, EventArgs e)
        {
            const string sendGridUserName = "cloudents";
            const string sendGridPassword = "zbangitnow";
            var credentials = new NetworkCredential(sendGridUserName, sendGridPassword);
            var flagPostMessage = new SendGridMessage();
            string mailTo;
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", m_GlobalFlaggedPost.UserId);
                command.CommandText = "SELECT * FROM Zbox.Users WHERE UserId=@id";
                var dataAdapter = new SqlDataAdapter { SelectCommand = command };
                connection.Open();
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                var dataTable = ds.Tables[0];
                connection.Close();
                mailTo = dataTable.Rows[0]["Email"].ToString();
            }

            flagPostMessage.AddTo(mailTo);
            flagPostMessage.From = new MailAddress("cloudents@gmail.com");
            flagPostMessage.Subject = "Flag Report";
            flagPostMessage.Text = "We have received the flag and dealt with it accordingly.\n Thank you for contributing to the Cloudents community.";

            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(flagPostMessage);

        }

        private async void buttonReportBackUser_Click(object sender, EventArgs e)
        {
            const string sendGridUserName = "cloudents";
            const string sendGridPassword = "zbangitnow";
            var credentials = new NetworkCredential(sendGridUserName, sendGridPassword);
            var flagPostMessage = new SendGridMessage();
            string mailTo;
            using (var connection = new SqlConnection(m_ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();

                //command.Parameters.AddWithValue("@id", m_GlobalFlaggedItem.UserId);
                command.CommandText = "SELECT Email FROM Zbox.Users WHERE UserId=@id";
                var dataAdapter = new SqlDataAdapter { SelectCommand = command };
                connection.Open();
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                var dataTable = ds.Tables[0];
                connection.Close();
                mailTo = dataTable.Rows[0]["Email"].ToString();
            }

            flagPostMessage.AddTo(mailTo);
            flagPostMessage.From = new MailAddress("cloudents@gmail.com");
            flagPostMessage.Subject = "Flag Report";
            flagPostMessage.Text = "We have received the flag and dealt with it accordingly.\n Thank you for contributing to the Cloudents community.";
            MessageBox.Show("Mail sent to " + mailTo, "Sent", MessageBoxButtons.OK);
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(flagPostMessage);
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
        private async void buttonReplaceItem_Click(object sender, EventArgs e)
        {
            //string ItemId = textBox1.Text;
            string itemId = m_GlobalFlaggedItem.ItemId.ToString(CultureInfo.InvariantCulture);
            string blobName = GetBlobName(itemId);
            blobName = Path.GetFileNameWithoutExtension(blobName);
            string fileLocation = textBoxAttachment.Text;
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("zboxcahce");
            foreach (IListBlobItem item in container.ListBlobs(blobName))
            {
                var blob = (CloudBlockBlob)item;
                blob.Delete();
            }


            await UploadBlobAsync(blobName, fileLocation);
            MessageBox.Show("File has been replaced, press OK to continue.", "File Replaced", MessageBoxButtons.OK);
            textBoxAttachment.Text = "";
        }

        private async Task UploadBlobAsync(string blobName, string fileLocation)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
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
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

        }




        private async void buttonMergeBoxes_Click(object sender, EventArgs e)
        {
            var boxIdFrom = long.Parse(textBoxBoxFrom.Text);
            var boxIdTo = long.Parse(textBoxBoxTo.Text);

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
update zbox.item set [ItemTabId]=null, [IsDirty]=1, UpdatedUser = 'mergeTool' where boxid=@fromid;
update zbox.item set boxid=@toid, [IsDirty]=1, UpdatedUser = 'mergeTool' where boxid=@fromid;

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
delete from [Zbox].[ItemTab] where BoxId = @fromid;
commit transaction", new { fromid = boxIdFrom, toid = boxIdTo });
                var iocService = IocFactory.IocWrapper.Resolve<IZboxWorkerRoleService>();
                iocService.UpdateItemUrl();
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
            await writeService.DeleteBoxAsync(new DeleteBoxCommand(boxId));


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

        private void buttonChangeDepartment_Click(object sender, EventArgs e)
        {
            var boxIdstr = textBoxChangeDepartmentBoxId.Text?.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (boxIdstr == null)
            {
                return;
            }
            var boxIds = boxIdstr.Select(long.Parse);

            var departmentId = GuidEncoder.TryParseNullableGuid(textBoxChangeDepartmentDepartmentId.Text);
            if (!departmentId.HasValue)
            {
                return;
            }
            var command = new ChangeBoxLibraryCommand(boxIds, departmentId.Value);
            var writeService = IocFactory.IocWrapper.Resolve<IZboxWorkerRoleService>();
            writeService.ChangeBoxDepartment(command);
            MessageBox.Show("Done");
        }

        private async void buttonAddComment_Click(object sender, EventArgs e)
        {
            var boxIdstr = textBoxBoxIds.Text?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (boxIdstr == null)
            {
                return;
            }
            var boxIds = boxIdstr.Select(long.Parse);
            var commentUserId = long.Parse(textBoxCommentUserId.Text);
            var needReply = false;
            var replyUserId = 0L;
            if (!string.IsNullOrEmpty(textBoxReply.Text))
            {
                needReply = true;
                replyUserId = long.Parse(textBoxReplyUserId.Text);

            }


            var writeService = IocFactory.IocWrapper.Resolve<IZboxWriteService>();
            var idGenerator = IocFactory.IocWrapper.Resolve<IGuidIdGenerator>();
            //var cache = IocFactory.IocWrapper.Resolve<ICache>();

            foreach (var boxId in boxIds)
            {
                var questionId = idGenerator.GetId();
                var command = new AddCommentCommand(commentUserId, boxId, textBoxComment.Text, questionId, null, false);
                await writeService.AddCommentAsync(command);

                if (needReply)
                {
                    var answerId = idGenerator.GetId();
                    var replyCommand = new AddReplyToCommentCommand(replyUserId, boxId, textBoxReply.Text, answerId, questionId, null);
                    await writeService.AddReplyAsync(replyCommand);
                }
                // await cache.RemoveFromCacheAsyncSlowAsync(CacheRegions.BuildFeedRegion(boxId));

            }




            MessageBox.Show("Done");
        }

        private void openFileDialogReplaceItem_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private async void buttonImportQuiz_Click(object sender, EventArgs e)
        {
            importQuizResult.Text = string.Empty;
            // Split box ids
            var boxIdstr = quizBoxID.Text?.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (boxIdstr == null)
            {
                return;
            }
            var boxIds = boxIdstr.Select(long.Parse);
            var userId = long.Parse(quizUserID.Text);
            importQuizResult.Text = await ImportQuizAsync(quizUrlId.Text, quizName.Text, boxIds, userId);
            quizBoxID.Text = string.Empty;
            quizUrlId.Text = string.Empty;
        }
        private static async Task<string> ImportQuizAsync(string quizUrl, string quizName, IEnumerable<long> boxIds, long userId)
        {
            var httpClient = new HttpClient();
            var output = "";
            //Validate that the link is valid
            try
            {
                using (var sr = await httpClient.GetAsync(quizUrl))
                {
                    sr.EnsureSuccessStatusCode();
                    //Get the id of the quid for the json data query
                    var currentQuizId = quizUrl.Split('/')[3];
                    using (var response = await httpClient.GetAsync(string.Format(QuizUrl + "{0}" + "?client_id=53m5PP5tK3&whitespace=1&format=json", currentQuizId)))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var cardsList = (JObject.Parse(responseBody)).SelectToken("terms");
                        var cardImages = cardsList.Where(card => card.SelectToken("image").HasValues);
                        string imageLink;
                        var dictionaryImage = new Dictionary<long, string>();
                        var imagesTask = new List<Task>();
                        //go over the cards from json and convert them to Flashcard cards structure
                        foreach (var card in cardImages)
                        {
                            imagesTask.Add(SaveImageAsync((long)card.SelectToken("id"), (string)card.SelectToken("image.url"), dictionaryImage));
                        }
                        await Task.WhenAll(imagesTask);

                        //create and add card object to the list according the data
                        var cardsConverters2 = cardsList.Select(card =>
                            new Zbang.Zbox.Domain.Card
                            {
                                Cover = new Zbang.Zbox.Domain.CardSlide
                                {
                                    Text = (string)card.SelectToken("definition"),
                                    Image = (!dictionaryImage.TryGetValue((long)card.SelectToken("id"), out imageLink)) ? null : imageLink
                                },
                                Front = new Zbang.Zbox.Domain.CardSlide
                                {
                                    Image = null,
                                    Text = (string)card.SelectToken("term")
                                }
                            }
                       ).ToList();

                        //var tasks = new List<Task>();
                        foreach (var box in boxIds)
                        {
                            var retVal = await CreateFlashCardAsync(userId, quizName, cardsConverters2, box);
                            output += "\nboxId:#" + retVal;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }

            return output;
        }
        private static async Task SaveImageAsync(long id, string url, IDictionary<long, string> dictionary)
        {
            var client = new HttpClient();
            var name = url.Split('/').Last();
            //var dictionary = new Dictionary<long, string>();
            using (var res = await client.GetAsync(url))
            {
                var flash = IocFactory.IocWrapper.Resolve<IBlobProvider2<FlashcardContainerName>>();
                var fileName = Guid.NewGuid() + Path.GetExtension(name);
                await flash.UploadStreamAsync(fileName, await res.Content.ReadAsStreamAsync(), res.Content.Headers.ContentType.ToString(), default(CancellationToken));
                dictionary.Add(id, flash.GetBlobUrl(fileName, true).ToString());
                //return flash.GetBlobUrl(fileName, true).ToString();
            }
        }

        private static async Task<string> CreateFlashCardAsync(long userId, string quizName, IEnumerable<Zbang.Zbox.Domain.Card> cardsList, long boxId)
        {
            var output = boxId.ToString();
            try
            {
                var id = IocFactory.IocWrapper.Resolve<IIdGenerator>().GetId(Zbang.Zbox.Infrastructure.Consts.IdContainer.FlashcardScope);
                var flashCard = new Zbang.Zbox.Domain.Flashcard(id)
                {
                    BoxId = boxId,
                    UserId = userId,
                    Name = quizName,
                    Publish = true,
                    DateTime = DateTime.UtcNow,
                    Cards = cardsList
                };
                //Write the new Flashcard to the db
                var zboxWriteService = IocFactory.IocWrapper.Resolve<IZboxWriteService>();
                var command = new AddFlashcardCommand(flashCard);
                await zboxWriteService.AddFlashcardAsync(command);
                await zboxWriteService.PublishFlashcardAsync(new PublishFlashcardCommand(flashCard));
                return output + " done";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return output + " " + ex.Message;
            }

        }

        private async void buttonItemDelete_Click(object sender, EventArgs e)
        {
            var itemIdstr = textBoxItemsToDelete2.Text?.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (itemIdstr == null)
            {
                return;
            }
            var itemIds = itemIdstr.Select(long.Parse);
            var userId = long.Parse(textBoxItemDeleteUserId.Text);
            var zboxWriteService = IocFactory.IocWrapper.Resolve<IZboxWriteService>();
            foreach (var boxId in itemIds)
            {
                await zboxWriteService.DeleteItemAsync(new DeleteItemCommand(boxId, userId));
            }
            MessageBox.Show("Done");
        }

        private void tabUniversity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabUniversity.SelectedIndex == 2)
            {
                InitializeFlaggedItems();
            }
            if (tabUniversity.SelectedIndex == 3)
            {
                InitializeFlaggedPosts();
            }
        }

        private async void buttonUploadFile_Click(object sender, EventArgs e)
        {
            openFileDialogReplaceItem = new OpenFileDialog
            {
                Multiselect = false
            };
            if (openFileDialogReplaceItem.ShowDialog() == DialogResult.OK)
            {
                using (var stream = openFileDialogReplaceItem.OpenFile())
                {
                    if (stream == null)
                    {
                        return;
                    }

                    CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), false);
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    var mailContainer = blobClient.GetContainerReference("mailcontainer");
                    var blob = mailContainer.GetBlockBlobReference(Path.GetFileName(openFileDialogReplaceItem.FileName));
                    if (await blob.ExistsAsync())
                    {
                        MessageBox.Show("file with that name exists");
                        return;
                    }
                    await blob.UploadFromStreamAsync(stream);
                    textBoxUrl.Text = blob.Uri.AbsoluteUri;


                }

            }
        }
    }
}