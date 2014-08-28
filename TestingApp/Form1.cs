using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Storage;

namespace TestingApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();

            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;
            var lucenewire = iocFactory.Resolve<IUniversityWriteSearchProvider>("azureSearch");
            lucenewire.BuildUniversityData();
            // textBox2.Text = "Complete";
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;
            var luceneRead = iocFactory.Resolve<IUniversityReadSearchProvider>("azureSearch");
            var sw = new Stopwatch();
            sw.Start();
            var retVal = await luceneRead.SearchUniversity(textBox1.Text);
            sw.Stop();
            textBox2.Text = string.Empty;
            textBox2.Text = "took " + sw.ElapsedMilliseconds + "\r\n";
            foreach (var item in retVal)
            {
                textBox2.Text += string.Format("id: {0} name: {1} ", item.Id, item.Name);
                textBox2.Text += "\r\n";
            }
            //textBox2.Text  = string .Format("",)
        }
        
    }
}
