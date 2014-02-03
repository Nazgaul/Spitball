using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Ioc;

namespace UniversityCreator
{
    public partial class Form1 : Form
    {
        IocFactory Unity;
        public Form1()
        {
            InitializeComponent();
            comboBoxCountry.DisplayMember = "countryName";
            comboBoxCountry.ValueMember = "countryCode";
            comboBoxCountry.DataSource = GetCountryList();

            RegisterIoc();

            textBoxEmail.Text = string.Format("{0}@cloudents.com", Guid.NewGuid().ToString());
        }

        public void RegisterIoc()
        {
            Unity = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;


            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
        }

        public DataTable GetCountryList()
        {
            var dt = new DataTable();
            dt.Columns.Add("countryName");
            dt.Columns.Add("countryCode");

            Dictionary<string, string> cultureList = new Dictionary<string, string>();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures.OrderBy(o => o.Name))
            {
                RegionInfo region = new RegionInfo(culture.LCID);

                if (!(cultureList.ContainsValue(region.EnglishName)))
                {
                    //var dr = dt.NewRow();
                    //dr["countryName"] = region.EnglishName;
                    //dr["countryCode"] = region.Name;
                    //dt.Rows.Add(dr);
                    if (!cultureList.ContainsKey(region.Name))
                    {
                        cultureList.Add(region.Name, region.EnglishName);
                    }
                    //        cultureList.Add(region.EnglishName);
                }
            }
            foreach (var country in cultureList.OrderBy(o => o.Value))
            {
                var dr = dt.NewRow();
                dr["countryName"] = country.Value;
                dr["countryCode"] = country.Key;
                dt.Rows.Add(dr);
            }


            return dt; //cultureList.OrderBy(s => s).ToList();
        }

        public DataTable GetLanguageList()
        {
            var dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Culture");

            foreach (var item in Languages.SupportedCultures)
            {
                var dr = dt.NewRow();
                dr["Name"] = item.Name;
                dr["Culture"] = item.Culture;
                dt.Rows.Add(dr);
            }

            return dt;

        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            var dr = (System.Data.DataRowView)comboBoxCountry.SelectedItem;
            var countryCode = dr["countryCode"].ToString();

            var command = new CreateUniversityCommand(textBoxName.Text, textBoxEmail.Text, countryCode);

            var service = Unity.Resolve<IZboxServiceBackgroundApp>();
            service.CreateUniversity(command);

            textBoxEmail.Text = string.Format("{0}@cloudents.com", Guid.NewGuid().ToString());
            MessageBox.Show("DONE");
            //var dr2 = (System.Data.DataRowView)comboBoxCulture.SelectedItem;
            //var language = dr2["Culture"];



        }
    }
}
