using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zbang.Zbox.Infrastructure.Consts;

namespace GenerateUrl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            bool alreadyFocused = false;

            textBoxResult.GotFocus += (s, e) =>
            {
                if (MouseButtons == MouseButtons.None)
                {
                    alreadyFocused = true;
                    textBoxResult.SelectAll();
                }
            };
            textBoxResult.Leave += (s, e) =>
            {
                alreadyFocused = false;
            };
            textBoxResult.MouseUp += (s, e) =>
            {
                if (!alreadyFocused && this.textBoxResult.SelectionLength == 0)
                {
                    alreadyFocused = true;
                    this.textBoxResult.SelectAll();
                }
            };
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sql = @"select boxid, boxname,Discriminator as boxType,
								case Discriminator when 2 then 
                                (select universityname from zbox.Users u where OwnerId = u.UserId)
								else null
								end as universityname from zbox.box where boxid = @BoxId";
                dynamic result = connection.Query(sql, new { BoxId = textBoxBoxId.Text }).FirstOrDefault();

                if (result.boxType == 2)
                {
                    textBoxResult.Text = string.Format(UrlConsts.CourseUrl,
                         result.boxid,
                         UrlConsts.NameToQueryString(result.boxname),
                         UrlConsts.NameToQueryString(result.universityname));
                }
                else
                {
                    textBoxResult.Text = string.Format(UrlConsts.BoxUrl, result.boxid,
                        UrlConsts.NameToQueryString(result.boxname));
                }

            }


        }
       
    }
}
