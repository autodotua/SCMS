using SCMS.Common.DbData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMS.Common.UIData
{
   public class UIStudentCourse
    {
        public UIStudentCourse()
        {
        }

        public UIStudentCourse(StudentCourse stuCourse)
        {
            Course course = stuCourse.Course;
            CourseId = course.Id;
            CourseName = course.Name;
            CourseTeacher = course.Person.Name;

            Year = course.Year;
            Term = course.Term;
            Score = stuCourse.Score;
            Credit = course.Credit;
            GradePoint = Score.HasValue ? (Score<60?0:(Score/10.0-5)) : null;
        }

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseTeacher { get; set; }
        public int Year { get; set; }
        public byte Term { get; set; }
        public byte? Score { get; set; }
        public byte? Credit { get; set; }
        public double? GradePoint { get; set; }
    }
}
