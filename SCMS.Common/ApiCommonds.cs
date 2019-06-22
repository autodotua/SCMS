namespace SCMS.Common
{
    public static class ApiCommands
    {
        /// <summary>
        /// 登录
        /// </summary>
        public static readonly string Login = nameof(Login);
        /// <summary>
        /// 管理员获取学生
        /// </summary>
        public static readonly string Admin_Students = nameof(Admin_Students);
        /// <summary>
        /// 管理员获取教师
        /// </summary>
        public static readonly string Admin_Teachers = nameof(Admin_Teachers);
        /// <summary>
        /// 管理员获取课程
        /// </summary>
        public static readonly string Admin_Courses = nameof(Admin_Courses);

        /// <summary>
        /// 管理员更新人员
        /// </summary>
        public static readonly string Admin_Update_Persons = nameof(Admin_Update_Persons);
        /// <summary>
        /// 管理员更新课程
        /// </summary>
        public static readonly string Admin_Update_Courses = nameof(Admin_Update_Courses);

        /// <summary>
        /// 管理员获取选课的学生
        /// </summary>
        public static readonly string Admin_CourseOfStudent = nameof(Admin_CourseOfStudent);
        /// <summary>
        /// 管理员获取课程的教师
        /// </summary>
        public static readonly string Admin_CourseOfTeacher = nameof(Admin_CourseOfTeacher);
        /// <summary>
        /// 管理员获取某一门课程的教师和选课的学生
        /// </summary>
        public static readonly string Admin_StudentsAndTeacherOfCourses = nameof(Admin_StudentsAndTeacherOfCourses);

        /// <summary>
        /// 管理员更新某一门课程的教师和选课的学生
        /// </summary>
        public static readonly string Admin_SetStudentsAndTeacherOfCourse = nameof(Admin_SetStudentsAndTeacherOfCourse);

        /// <summary>
        /// 教师获取自己的课程
        /// </summary>
        public static readonly string Teacher_Courses = nameof(Teacher_Courses);
        /// <summary>
        /// 教师获取某一门课程的学生
        /// </summary>
        public static readonly string Teacher_StudentScoreList = nameof(Teacher_StudentScoreList);
        /// <summary>
        /// 教师更新某一门课程的学生成绩
        /// </summary>
        public static readonly string Teacher_UpdateStudentScore = nameof(Teacher_UpdateStudentScore);

        /// <summary>
        /// 学生获取课程信息和成绩
        /// </summary>
        public static readonly string Student_StudentCourses = nameof(Student_StudentCourses);

        public static readonly string Student = nameof(Student);
        public static readonly string Students = nameof(Students);
        public static readonly string Student_Courses = nameof(Student_Courses);

        public static readonly string Teacher = nameof(Teacher);
        public static readonly string Course = nameof(Course);

        public static readonly string ChangePassword = nameof(ChangePassword);
        public static readonly string OldPassword = nameof(OldPassword);
        public static readonly string NewPassword = nameof(NewPassword);


        public static readonly string ID = nameof(ID);
        public static readonly string OK = nameof(OK);
        public static readonly string Person = nameof(Person);
        public static readonly string Data = nameof(Data);
        public static readonly string Password = nameof(Password);
        public static readonly string Message = nameof(Message);
        public static readonly string Command = nameof(Command);
        public static readonly string Modified = nameof(Modified);
        public static readonly string Deleted = nameof(Deleted);

    }
}
