using SCMS.Common.DbData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMS.Common.UIData
{
   public class UICourse
    {
        public UICourse()
        {
        }

        public UICourse(Course course)
        {
            Id = course.Id;
            Name = course.Name;
            Teacher = course.Person?.Name;
            TeacherId = course.TeacherId;
            Year = course.Year;
            Term = course.Term;
        }

        public Course ToCourse()
        {
            return new Course()
            {
                Id = Id,
                Name = Name,
                Year = Year,
                Term = Term,
                TeacherId = TeacherId
            };
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public long? TeacherId { get; set; }
        public short Year { get; set; }
        public byte Term { get; set; }
    }
}
