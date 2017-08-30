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
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace DuplicateQuiz
{
    public partial class Form1 : Form
    {
        IocFactory m_Unity;
        public Form1()
        {
            InitializeComponent();
#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#endif
            RegisterIoc();
        }

        public void RegisterIoc()
        {
            m_Unity = IocFactory.IocWrapper;


            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            m_Unity.Build();
        }

        private async void buttonDuplicate_Click(object sender, EventArgs e)
        {
            var boxids = textBoxBoxId.Text.Split(',');

            var quizIds = textBoxQuizId.Text.Split(',');



            var boxIdIntLong = boxids.Select(s =>
            {
                long i;
                long.TryParse(s, out i);
                return i;
            }).ToList();

            var quizIdIntLong = quizIds.Select(s =>
            {
                long i;
                long.TryParse(s, out i);
                return i;
            });
            var result = new List<string>();
            var userId = long.Parse(ConfigurationManager.AppSettings["UserId"]);
            foreach (var l in quizIdIntLong.Where(w => w != 0))
            {
                result.Add("processing quiz: " + l);
                textBoxProgress.Lines = result.ToArray();

                var retVal = new QuizWithDetailSolvedDto();
                using (var conn = await DapperConnection.OpenConnectionAsync())
                {
                    using (var grid = conn.QueryMultiple(string.Format("{0} {1} {2}",
                        Quiz.QuizQuery,
                        Quiz.Question,
                        Quiz.Answer
                        ), new { QuizId = l }))
                    {
                        retVal.Quiz = grid.Read<QuizWithDetailDto>().First();
                        retVal.Quiz.Questions = grid.Read<QuestionWithDetailDto>().ToList();

                        var answers = grid.Read<AnswerWithDetailDto>().ToList();

                        foreach (var question in retVal.Quiz.Questions)
                        {
                            question.Answers.AddRange(answers.Where(w => w.QuestionId == question.Id));
                        }


                    }
                }

                foreach (var item in boxIdIntLong.Where(w => w != 0))
                {
                    result.Add("duplicate in box : " + item);
                    textBoxProgress.Lines = result.ToArray();

                    var idGenerator = m_Unity.Resolve<Zbang.Zbox.Infrastructure.IdGenerator.IIdGenerator>();
                    var idGenerator2 = m_Unity.Resolve<Zbang.Zbox.Infrastructure.IdGenerator.IGuidIdGenerator>();
                    var zboxWriteService = m_Unity.Resolve<Zbang.Zbox.Domain.Common.IZboxWriteService>();

                    var quizId = idGenerator.GetId(Zbang.Zbox.Infrastructure.Consts.IdContainer.QuizScope);

                    var createQuizCommand = new CreateQuizCommand(
                        userId,
                        quizId,
                        retVal.Quiz.Name,
                        item);


                    await zboxWriteService.CreateQuizAsync(createQuizCommand);

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        var questionId = idGenerator2.GetId();
                        var createQuestionCommand = new CreateQuestionCommand(
                            question.Text, quizId, userId, questionId);
                        zboxWriteService.CreateQuestion(createQuestionCommand);

                        foreach (var answer in question.Answers)
                        {
                            var createAnswerCommand = new CreateAnswerCommand(
                                userId, idGenerator2.GetId(), answer.Text,
                                questionId
                                );
                            zboxWriteService.CreateAnswer(createAnswerCommand);
                            if (answer.Id == question.CorrectAnswer)
                            {
                                var command = new MarkAnswerCorrectCommand(createAnswerCommand.Id, userId);
                                zboxWriteService.MarkAnswerAsCorrect(command);
                            }
                        }
                    }

                    await zboxWriteService.SaveQuizAsync(new SaveQuizCommand(userId, quizId));
                }
            }
            MessageBox.Show("Done");
        }



    }
}
