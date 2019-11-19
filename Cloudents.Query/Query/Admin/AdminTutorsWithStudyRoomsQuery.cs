using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminTutorsWithStudyRoomsQuery : IQueryAdmin<IEnumerable<TutorInfoDto>>
    {
        public AdminTutorsWithStudyRoomsQuery(string country)
        {
            Country = country;
        }
        public string Country { get; }

        internal sealed class AdminTutorsWithStudyRoomsQueryHandler : IQueryHandler<AdminTutorsWithStudyRoomsQuery, IEnumerable<TutorInfoDto>>
        {
            private readonly IDapperRepository _dapper;

            public AdminTutorsWithStudyRoomsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<TutorInfoDto>> GetAsync(AdminTutorsWithStudyRoomsQuery query, CancellationToken token)
            {
                var sql = @"	select u.Id, u.Name, u.Email, u.PhoneNumberHash as PhoneNumber,
                            (select cast(sum(datediff(MINUTE, srs.created, srs.ended)) as float)/60
                            from sb.StudyRoom sr 
                            join sb.StudyRoomSession srs
	                            on sr.Id = srs.StudyRoomId
                            where sr.TutorId = t.Id
                            ) as TotalHours,
                            (select count(distinct sru.UserId) 
                            from sb.StudyRoom sr
                            join sb.StudyRoomUser sru
	                            on sr.Id = sru.StudyRoomId and sru.UserId != sr.TutorId
                            where sr.TutorId = t.Id 
                            ) as TotalStudents,
							case when Exists (select sr.Id from sb.StudyRoom sr 
												 join sb.StudyRoomSession srs
												     on sr.Id = srs.StudyRoomId
												 where sr.TutorId = t.Id and Receipt is null
												 ) then 1 else 0 end as NeedToPay, 
                            t.Price as Price
                            from sb.Tutor t
                            join sb.[User] u
	                            on u.Id = t.Id
                            where (
		                            select count(distinct sru.UserId) 
		                            from sb.StudyRoom sr
		                            join sb.StudyRoomUser sru
			                            on sr.Id = sru.StudyRoomId and sru.UserId != sr.TutorId
		                            where sr.TutorId = t.Id 
		                            ) > 0 
									and (u.Country = @Country or @Country is null)";

                using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QueryAsync<TutorInfoDto>(sql, new { query.Country });
                    //return res.AsList();
                }
                //TutorInfoDto dto = null;
                //Cloudents.Core.Entities.Tutor tutorAlias = null;
                //User userAlias = null;
                //StudyRoom studyRoomAlias = null;
                //StudyRoomSession studyRoomSessionAlias = null;
                //StudyRoomUser studyRoomUserAlias = null;

                //var res = await _session.QueryOver(() => studyRoomAlias)
                //    .JoinAlias(() => studyRoomAlias.Sessions, () => studyRoomSessionAlias)
                //    .JoinAlias(() => studyRoomAlias.Users, () => studyRoomUserAlias)
                //    .JoinAlias(x => x.Tutor, () => tutorAlias)
                //    .JoinAlias(x => studyRoomUserAlias.User, () => userAlias).Where(w => userAlias.Id == studyRoomAlias.Tutor.Id)
                //    .SelectList(
                //    l =>
                //        l.Select(Projections.Group(() => userAlias.Name)).WithAlias(() => dto.Name)
                //        .Select(Projections.Group(() => userAlias.PhoneNumber)).WithAlias(() => dto.PhoneNumber)
                //        .Select(Projections.Group(() => userAlias.Email)).WithAlias(() => dto.Email)
                //        .Select(Projections.Group(() => tutorAlias.Price)).WithAlias(() => dto.Rate)
                //        .Select(Projections.Sum<StudyRoomSession>(_ => studyRoomSessionAlias.Duration)).WithAlias(() => dto.TotalHours)
                //        .Select(Projections.CountDistinct<StudyRoomUser>(_ => studyRoomUserAlias.Id)).WithAlias(() => dto.TotalStudents)
                //    ).ListAsync<TutorInfoDto>(token);
                //return res;


            }
        }
    }
}
