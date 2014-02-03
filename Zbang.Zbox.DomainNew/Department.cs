using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class Department
    {
        protected Department()
        {
            Id = Guid.NewGuid();
        }

        public Department(University university, string departmentName)
            : this()
        {
            Name = departmentName.ToLower().Trim();
            University = university;
            DateTimeUser = new UserTimeDetails("sys");
        }
        public virtual Guid Id { get; protected set; }
        public virtual string Name { get; set; }

        public virtual University University { get; set; }

        public virtual UserTimeDetails DateTimeUser { get; set; }

        #region Nhibernate

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var professor = obj as Department;
            if (professor == null) return false;

            if (Name != professor.Name) return false;
            if (University != professor.University) return false;
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result;
                result = 11 * Name.GetHashCode();
                result += 13 * University.GetHashCode();
                return result;
            }
        }
        #endregion
    }
}
