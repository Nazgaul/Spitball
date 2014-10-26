using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class PartnersEmail : IJob
    {
        private bool m_KeepRunning;

        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;
        private readonly TimeSpan m_TimeToSleepAfterExcecuting = TimeSpan.FromDays(1);

        class Partners
        {
            public Partners()
            {
                Emails = new List<string> { "lennard@cloudents.com", "eidan@cloudents.com" };
            }
            public string SchoolName { get; set; }
            public long Id { get; set; }
            public IEnumerable<string> Emails { get; private set; }
        }

        private readonly List<Partners> m_Partners = new List<Partners> {
            new Partners { Id=10588, SchoolName = "Universiteit van Amsterdam"},
            new Partners { Id=10691, SchoolName = "Hogeschool van Amsterdam"},
            new Partners { Id=11279, SchoolName = "Vrije Universiteit Amsterdam"},
            new Partners { Id=11270, SchoolName = "Rijksuniversiteit Groningen"},
            new Partners { Id=12793, SchoolName = "Hanzehogeschool Groningen"},
            new Partners { Id=11285, SchoolName = "University College Utrecht"},
            new Partners { Id=11271, SchoolName = "Radboud Universiteit Nijmegen"},
            new Partners { Id=12795, SchoolName = "Hogeschool Arnhem en Nijmegen "}
       };

        public PartnersEmail(IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent)
        {
            m_ZboxReadService = zboxReadService;
            m_MailComponent = mailComponent;
        }
        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                Execute();
            }
        }

        private void Execute()
        {
            if (!ShouldRunReport())
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));
                return;
            }
            foreach (var partner in m_Partners)
            {
                var data = m_ZboxReadService.GetPartnersEmail(partner.Id).Result;

                var parameters = new Infrastructure.Mail.EmailParameters.Partners(
                    CultureInfo.GetCultureInfo("en-US"),
                    partner.SchoolName,
                    data.LastWeekUsers,
                    data.AllUsers,
                    data.LastWeekCourses,
                    data.AllCourses,
                    data.LastWeekItems,
                    data.AllItems,
                    data.LastWeekQnA,
                    data.AllQnA,
                    data.Univeristies.Select(s => new Infrastructure.Mail.EmailParameters.Partners.University
                    {
                        Name = s.Name,
                        StudentsCount = s.Students
                    }));

                m_MailComponent.GenerateAndSendEmail(partner.Emails, parameters);


            }
            Thread.Sleep(m_TimeToSleepAfterExcecuting);
        }

        private bool ShouldRunReport()
        {
            return DateTime.UtcNow.Hour == 5 && DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
