using SCMS.Common.DbData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMS.Common.UIData
{
    public class UITeacherCourse
    {
        public UITeacherCourse()
        {
        }

        public UITeacherCourse(StudentCourse course)
        {
            StudentId = course.Person.Id;
            StudentName = course.Person.Name;
            Score = course.Score;
            CourseId = course.CourseId;
            Id = course.Id;
        }

        public StudentCourse ToStudentCourse()
        {
            return new StudentCourse()
            {
                CourseId = CourseId,
                StudentId = StudentId,
                Id = Id,
                Score = Score,
            };
        }
        public int Id { get; set; }
        public long StudentId { get; set; }
        public int CourseId { get; set; }
        public string StudentName { get; set; }
        public byte? Score { get; set; }
    }
}
