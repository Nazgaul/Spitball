﻿using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Reflection")]
    public partial class TutorMap
    {
        public class TutorCalendarMap : ClassMap<TutorCalendar>
        {
            public TutorCalendarMap()
            {
                Id(x => x.Id);

                Component(x => x.Calendar, y =>
                {
                    y.Map(x => x.GoogleId).Not.Nullable();
                    y.Map(x => x.Name).Not.Nullable();
                });

                References(x => x.Tutor).Not.Nullable();
            }
        }
    }
}