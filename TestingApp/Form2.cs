using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Ioc;

namespace TestingApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();


            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();

            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            //Zbang.Zbox.Infrastructure.Mail.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
        }

        private async void buttonGenerate_Click(object sender, EventArgs e)
        {
            var idGenerator = IocFactory.Unity.Resolve<IIdGenerator>();
            var writeService = IocFactory.Unity.Resolve<IZboxWriteService>();
            var userId = Convert.ToInt64(textBoxUserId.Text);
            var boxes = textBoxBoxes.Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt64(s));
            foreach (var box in boxes)
            {

                var questionId = idGenerator.GetId();
                var command = new Zbang.Zbox.Domain.Commands.AddCommentCommand(userId, box, textBox1.Text, questionId,
                    null);
                command.GetType().GetProperty("ShouldEncode").SetValue(command, false);
               await writeService.AddQuestionAsync(command);
            }

        }
    }
}
