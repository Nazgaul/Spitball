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
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace DuplicateQuiz
{
    public partial class Form1 : Form
    {
        IocFactory Unity;
        public Form1()
        {
            InitializeComponent();
            RegisterIoc();
        }

        public void RegisterIoc()
        {
            Unity = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;


            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
        }

        private async void buttonDuplicate_Click(object sender, EventArgs e)
        {
            var boxids = new string[] { 
                textBoxBoxId.Text,
                textBoxBoxId2.Text,
            textBoxBoxId3.Text,
            textBoxBoxId4.Text,
            textBoxBoxId5.Text,
            textBoxBoxId6.Text,
            textBoxBoxId7.Text,
            textBoxBoxId8.Text,
            textBoxBoxId9.Text,
            textBoxBoxId10.Text,
            textBoxBoxId11.Text,
            textBoxBoxId12.Text,
            textBoxBoxId13.Text,
            textBoxBoxId14.Text,
            textBoxBoxId15.Text,
            textBoxBoxId16.Text,
            textBoxBoxId17.Text,
            textBoxBoxId18.Text,
            textBoxBoxId19.Text,
            textBoxBoxId20.Text};

            var boxIdIntLong = boxids.Select(s =>
            {
                long i;
                long.TryParse(s, out i);
                return i;
            });


            var retVal = new QuizWithDetailSolvedDto();
            using (var conn = await DapperConnection.OpenConnection())
            {
                using (var grid = conn.QueryMultiple(string.Format("{0} {1} {2}",
                   Quiz.QuizQuery,
                   Quiz.Question,
                   Quiz.Answer
                    ), new { QuizId = long.Parse(textBoxQuizId.Text) }))
                {
                    retVal.Quiz = grid.Read<QuizWithDetailDto>().First();
                    retVal.Quiz.Questions = grid.Read<QuestionWithDetailDto>();

                    var answers = grid.Read<AnswerWithDetailDto>();

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                    }


                }
            }
            var userId = long.Parse(ConfigurationManager.AppSettings["UserId"]);
            foreach (var item in boxIdIntLong.Where(w => w != 0))
            {

                var idGenerator = Unity.Resolve<Zbang.Zbox.Infrastructure.IdGenerator.IIdGenerator>();
                var zboxWriteService = Unity.Resolve<Zbang.Zbox.Domain.Common.IZboxWriteService>();

                var quizId = idGenerator.GetId(Zbang.Zbox.Infrastructure.IdGenerator.IdGenerator.QuizScope);

                var createQuizCommand = new CreateQuizCommand(
                    userId,
                   quizId,
                    retVal.Quiz.Name,
                    item);


                zboxWriteService.CreateQuiz(createQuizCommand);

                foreach (var question in retVal.Quiz.Questions)
                {
                    var questionId = idGenerator.GetId();
                    var createQuestionCommand = new CreateQuestionCommand(
                        question.Text, quizId, userId, questionId);
                    zboxWriteService.CreateQuestion(createQuestionCommand);

                    foreach (var answer in question.Answers)
                    {
                        var createAnswerCommand = new CreateAnswerCommand(
                            userId, idGenerator.GetId(), answer.Text,
                            answer.Id == question.CorrectAnswer, questionId
                            );
                        zboxWriteService.CreateAnswer(createAnswerCommand);
                    }
                }

                zboxWriteService.SaveQuiz(new SaveQuizCommand(userId, quizId));
            }
            MessageBox.Show("Done");
        }


    }
}
