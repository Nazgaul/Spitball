using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

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

        private async void button1_Click(object sender, EventArgs e)
        {
            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;
            var lucenewire = iocFactory.Resolve<IUniversityWriteSearchProvider>();
            await lucenewire.BuildUniversityData();
            textBox2.Text = "Complete";
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;
            var luceneRead = iocFactory.Resolve<IUniversityReadSearchProvider>();
            var sw = new Stopwatch();
            sw.Start();
            var retVal = await luceneRead.SearchUniversity(new UniversitySearchQuery(textBox1.Text));
            sw.Stop();
            textBox2.Text = string.Empty;
            textBox2.Text = "took " + sw.ElapsedMilliseconds + "\r\n";
            if (retVal != null)
            {
                foreach (var item in retVal)
                {
                    textBox2.Text += string.Format("id: {0} name: {1} ", item.Id, item.Name);
                    textBox2.Text += "\r\n";
                }
            }
            //textBox2.Text  = string .Format("",)
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var iocFactory = IocFactory.Unity;
            var read = iocFactory.Resolve<IBoxReadSearchProvider>();//(new IocParameterOverride("shouldUseProduction", true));
            var sw = new Stopwatch();
            sw.Start();
            var retVal = await read.SearchBox(new BoxSearchQuery(textBox1.Text, Convert.ToInt64(textBoxUserId.Text),
                Convert.ToInt64(textBoxUniversityName.Text)));
            sw.Stop();
            textBox2.Text = string.Empty;
            textBox2.Text = "took " + sw.ElapsedMilliseconds + "\r\n";
            if (retVal != null)
            {
                foreach (var item in retVal)
                {
                    textBox2.Text += string.Format("id: {0} name: {1} ", item.Id, item.Name);
                    textBox2.Text += "\r\n";
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FileStream originalFileStream = new FileStream(@"C:\Users\Ram\Desktop\46eb10c2-34a4-4ac8-a203-7a15c7d49fd9.thumbnailV3.jpg", FileMode.Open))
            {
                using (FileStream compressedFileStream = File.Create(@"C:\Users\Ram\Desktop\46eb10c2-34a4-4ac8-a203-7a15c7d49fd9.thumbnailV32.jpg"))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                       CompressionMode.Compress))
                    {
                        originalFileStream.CopyTo(compressionStream);

                    }
                }


            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var i = int.Parse(textBox3.Text);
            Base62 v = new Base62(i);
            textBox2.Text = v.ToString();
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            var iocFactory = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;
            var read = iocFactory.Resolve<IItemReadSearchProvider>();
            var sw = new Stopwatch();
            sw.Start();
            var retVal = await read.SearchItem(new ItemSearchQuery(textBox1.Text, Convert.ToInt64(textBoxUserId.Text),
                Convert.ToInt64(textBoxUniversityName.Text)));
            sw.Stop();
            textBox2.Text = string.Empty;
            textBox2.Text = "took " + sw.ElapsedMilliseconds + "\r\n";
            if (retVal != null)
            {
                foreach (var item in retVal)
                {
                    textBox2.Text += string.Format("id: {0} name: {1} ", item.Id, item.Name);
                    textBox2.Text += "\r\n";
                }
            }
        }

    }
}
