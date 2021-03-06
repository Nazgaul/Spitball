﻿using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Resources;

namespace Cloudents.Command.CommandHandler
{
    public class AddTutorCalendarEventCommandHandler : ICommandHandler<AddTutorCalendarEventCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly ICalendarService _calendarService;
        private readonly IRepository<GoogleTokens> _googleTokenRepository;
        

        public AddTutorCalendarEventCommandHandler(IRegularUserRepository userRepository, ITutorRepository tutorRepository, 
            ICalendarService calendarService, IRepository<GoogleTokens> googleTokenRepository)
        {
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
            _calendarService = calendarService;
            _googleTokenRepository = googleTokenRepository;
        }

        public async Task ExecuteAsync(AddTutorCalendarEventCommand message, CancellationToken token)
        {
            //TODO : need to check universal time in all the process in here
            //TODO : need to check only one hour is booked
            //TODO : need to check if user have payment detail
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            if (tutor.TutorHours.Any())
            {
                if (!tutor.TutorHours.Any(a => a.AvailabilitySlot.Day == message.From.DayOfWeek
                                               && a.AvailabilitySlot.From <= message.From.TimeOfDay
                                               && message.To.TimeOfDay <= a.AvailabilitySlot.To))
                {
                    throw new ArgumentException("Slot is booked");
                }
            }

            // Tutor hours
            var appointments = await _calendarService.ReadCalendarEventsAsync(tutor.Id, tutor.Calendars.Select(s => s.Calendar.GoogleId), message.From.AddHours(-1), message.To.AddHours(1), token);
            if (appointments.Any(a =>
            {
                if (IsBetween(message.From, a.From, a.To))
                {
                    return true;
                }
                if (IsBetween(message.To, a.From, a.To))
                {
                    return true;
                }

                return false;
            }))
            {
                throw new ArgumentException("Google Slot is booked");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            tutor.User.AddFollower(user);
            var googleTokens = await _googleTokenRepository.GetAsync(message.TutorId.ToString(), token);
            
            if (googleTokens != null)
            {
                var resourceManager = new System.Resources.ResourceManager(typeof(CalendarResources));
                var eventName = resourceManager.GetString("TutorCalendarMessage", CultureInfo.CurrentUICulture) ?? "Tutor Session In Spitball";
                eventName = string.Format(eventName, tutor.User.Name, user.Name);
                var calendarEmail = _calendarService.GetEmailFromToken(googleTokens.Value);
                await _calendarService.SendCalendarInviteAsync(
                    new []{tutor.User.Email,user.Email,calendarEmail},
                    message.From,
                    message.To,
                    eventName,
                    null,
                    token
                    );
            }
        }

        private static bool IsBetween(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck > startDate && dateToCheck < endDate;
        }
    }
}