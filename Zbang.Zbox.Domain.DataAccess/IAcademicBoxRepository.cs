﻿using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IAcademicBoxRepository: IRepository<AcademicBox> 
    {
        AcademicBox CheckIfExists(string courseCode, User universityUser, string professor, string courseName);
        //AcademicBox GetByNameAndProfessor(string boxName, Professor professor, User universityUser);
    }
}
