using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCMS.Common;
using SCMS.Common.DbData;
using SCMS.Common.UIData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SCMS.Common.ApiCommands;

namespace SCMS.Server
{
    /*
        接受格式：
            string:Command
            自定义


        返回格式：
            bool:OK
            stirng:Message
            ?:Data
    */
    public class DatabaseHelper
    {
        internal static DatabaseHelper Instance { get; } = new DatabaseHelper();

        internal SCMSModel Model { get; set; }

        #region 管理员
        /// <summary>
        /// 获取所有学生列表
        /// </summary>
        /// <returns></returns>
        public string GetAllStudents()
        {
            try
            {
                List<Person> students = Model.Person.Where(p => p.Role == "学生").ToList();
                return JsonConvert.SerializeObject(new { OK = true, Data = students });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }
        /// <summary>
        /// 获取所有教师列表
        /// </summary>
        /// <returns></returns>
        public string GetAllTeachers()
        {
            try
            {
                List<Person> teachers = Model.Person.Where(p => p.Role == "教师").ToList();
                return JsonConvert.SerializeObject(new { OK = true, Data = teachers });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }
        /// <summary>
        /// 获取所有课程列表
        /// </summary>
        /// <returns></returns>
        public string GetAllCourses()
        {
            try
            {
                UICourse[] courses = Model.Course.ToList().Select(p => new UICourse(p)).ToArray();
                return JsonConvert.SerializeObject(new { OK = true, Data = courses });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }

        }
        /// <summary>
        /// 更新一些信息
        /// </summary>
        /// <typeparam name="T">实体模型</typeparam>
        /// <typeparam name="TPk">实体模型的逐渐类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="dbSet">数据库集合</param>
        /// <param name="getPrimaryKey">获取主键的方法</param>
        /// <param name="prepareData">获取实体数据后</param>
        /// <param name="modifiedAction">调整数据的方法</param>
        /// <param name="deletedAction">删除数据的方法</param>
        /// <returns></returns>
        private string Update<T, TPk>(JObject json,
   DbSet<T> dbSet,
   Func<T, TPk> getPrimaryKey,
   Action<T> prepareData,
   Action<T, T> modifiedAction,
   Action<T, T> deletedAction
   ) where T : class
        {
            //Json包含Modified和Deleted
            if (modifiedAction != null)
            {
                foreach (JToken jStu in json[Modified] as JArray)
                {
                    T reference = jStu.ToObject<T>();
                    prepareData(reference);
                    T entity = dbSet.Find(getPrimaryKey(reference));
                    if (entity != null)
                    {
                        modifiedAction(entity, reference);
                    }
                    else
                    {
                        dbSet.Add(reference);
                    }
                }
            }
            if (deletedAction != null)
            {
                foreach (JToken jStu in json[Deleted] as JArray)
                {
                    T reference = jStu.ToObject<T>();
                    T entity = dbSet.Find(getPrimaryKey(reference));
                    if (entity != null)
                    {
                        deletedAction(entity, reference);
                    }
                }
            }
            try
            {
                int count = Model.SaveChanges();
                return JsonConvert.SerializeObject(new { OK = true, Message = $"保存成功，修改了{count}条记录" });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                StringBuilder message = new StringBuilder();
                message.Append("保存失败：").Append(ex.Message);
                if (ex is DbEntityValidationException)
                {
                    message.AppendLine();
                    var error = (ex as DbEntityValidationException).EntityValidationErrors.First();

                    foreach (var detail in error.ValidationErrors)
                    {
                        message.AppendLine(detail.ErrorMessage);
                    }


                }
                return JsonConvert.SerializeObject(new { OK = false, Message = message.ToString() });
            }
        }
        /// <summary>
        /// 更新所有Person相关列表
        /// </summary>
        /// <returns></returns>
        public string UpdatePerson(JObject json)
        {
            return Update(json,
                Model.Person,
                p => p.Id,
                r =>
                {
                    //if(string.IsNullOrEmpty(r.Password))
                    //{
                    //    r.Password = "123456";
                    //}
                },
                (e, r) =>
                {
                    UpdatePropertyValues(e, r, new string[] { "Name", "StartYear", "Major", "Gender", "Born" });
                    Model.Entry(e).State = EntityState.Modified;
                },
               (e, r) =>
               {
                   Model.Entry(e).State = EntityState.Deleted;
                   foreach (var course in Model.StudentCourse.Where(p => p.StudentId == e.Id))
                   {
                       Model.Entry(course).State = EntityState.Deleted;
                   }
                   foreach (var course in Model.Course.Where(p => p.TeacherId == e.Id).ToArray())
                   {
                       foreach (var stCs in Model.StudentCourse.Where(p => p.CourseId == course.Id))
                       {
                           Model.Entry(stCs).State = EntityState.Deleted;
                       }
                       Model.Entry(course).State = EntityState.Deleted;
                   }
               });



        }
        /// <summary>
        /// 更新所有课程相关列表
        /// </summary>
        /// <returns></returns>
        public string UpdateCourse(JObject json)
        {
            return Update(json,
                Model.Course,
                p => p.Id,
                r => { },
                (e, r) =>
                {
                    UpdatePropertyValues(e, r, new string[] { "Name", "Year", "Term" });
                    Model.Entry(e).State = EntityState.Modified;
                },
               (e, r) =>
               {
                   Model.Entry(e).State = EntityState.Deleted;
                   foreach (var course in Model.StudentCourse.Where(p => p.CourseId == e.Id))
                   {
                       Model.Entry(course).State = EntityState.Deleted;
                   }

               });


        }

        /// <summary>
        /// 获取所有教师列表
        /// </summary>
        /// <returns></returns>
        public string GetCoursesOfStudent(JObject json)
        {
            try
            {
                long id = json[ID].Value<long>();
                Person student = Model.Person.Find(id);
                return JsonConvert.SerializeObject(new { OK = true, Data = student.StudentCourse.Select(p => p.CourseId).ToArray() });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }

        }
        /// <summary>
        /// 获取所有教师列表
        /// </summary>
        /// <returns></returns>
        public string GetCourseOfTeacher(JObject json)
        {
            try
            {
                long id = json[ID].Value<long>();
                Person teacher = Model.Person.Find(id);
                return JsonConvert.SerializeObject(new { OK = true, Data = teacher.Course.Select(p => p.Id).ToArray() });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }

        }
        /// <summary>
        /// 获取课程关联的教师和学生
        /// </summary>
        /// <returns></returns>
        public string GetStudentsAndTeacherOfCourse(JObject json)
        {
            try
            {
                long id = json[ID].Value<int>();
                Course course = Model.Course.Find(id);
                StudentCourse[] studentCourses = course.StudentCourse.ToArray();
                long[] Students = studentCourses.Select(p => p.Person.Id).ToArray();
                long Teacher = course.Person.Id;
                return JsonConvert.SerializeObject(new { OK = true, Data = new { Students, Teacher } });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public string ChangePassword(JObject json)
        {
            //格式：ID代表人员ID，OldPassword代表旧密码，NewPassword代表新密码
            try
            {
                long id = json[ID].Value<long>();
                string oldPassword = json[OldPassword].Value<string>();
                string newPassword = json[NewPassword].Value<string>();
                Person person = Model.Person.Find(id);
                if (person == null)
                {
                    throw new Exception("没有找到该人员");
                }
                if (person.Password != oldPassword)
                {
                    throw new Exception("旧密码错误");
                }

                person.Password = newPassword;
                Model.Entry(person).State = EntityState.Modified;
                Model.SaveChanges();
                return JsonConvert.SerializeObject(new { OK = true, Message = "修改成功" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }
        /// <summary>
        /// 设置课程对应的教师和学生
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string SetStudentsAndTeacherOfCourse(JObject json)
        {
            //格式：ID表示课程ID，Students包含一组学生的ID，Teacher表示教师ID
            try
            {
                int courseId = json[ID].Value<int>();
                IEnumerable<long> studentsId = (json[Students] as JArray).Select(p => p.Value<long>());
                long teacherId = json[Teacher].Value<long>();

                Course course = Model.Course.Find(courseId);
                if (course == null)
                {
                    throw new Exception("找不到课程");
                }

                foreach (var item in course.StudentCourse.ToArray())
                {
                    Model.Entry(item).State = EntityState.Deleted;
                }

                foreach (var item in studentsId)
                {
                    StudentCourse studentCourse = new StudentCourse()
                    {
                        CourseId = courseId,
                        StudentId = item,
                    };
                    Model.StudentCourse.Add(studentCourse);
                }
                if (course.TeacherId != teacherId)
                {
                    course.TeacherId = teacherId;
                    Model.Entry(course).State = EntityState.Modified;
                }

                int count = Model.SaveChanges();
                return JsonConvert.SerializeObject(new { OK = true, Message = $"设置成功，修改了{count}条记录" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }

        #endregion


        #region 教师
        /// <summary>
        /// 获取某一个教师需要教的课
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetTeacherCoursesList(JObject json)
        {
            try
            {
                List<Course> courses = Model.Person.Find(json[ID].Value<long>()).Course.ToList();
                return JsonConvert.SerializeObject(new { OK = true, Data = courses });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }
        /// <summary>
        /// 获取某一个教师需要教的课
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetStudentCoursesList(JObject json)
        {
            try
            {
                List<StudentCourse> studentCourses = Model.Person.Find(json[ID].Value<long>()).StudentCourse.ToList();
                List<UIStudentCourse> courses = new List<UIStudentCourse>();
                foreach (var c in studentCourses)
                {
                    courses.Add(new UIStudentCourse(c));
                }
                return JsonConvert.SerializeObject(new { OK = true, Data = courses });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }

        }

        /// <summary>
        /// 更新学生成绩
        /// </summary>
        /// <returns></returns>
        public string UpdateStudentsScore(JObject json)
        {
            return Update(json,
                Model.StudentCourse,
                p => p.Id,
                r => { },
                (e, r) =>
                {
                    UpdatePropertyValues(e, r, new string[] { "Score" });
                    Model.Entry(e).State = EntityState.Modified;
                },
               null);


        }

        #endregion


        #region 学生
        /// <summary>
        /// 获取学生选课表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetStudentsScoresList(JObject json)
        {
            try
            {
                List<StudentCourse> stuC = Model.Course.Find(json[ID].Value<long>()).StudentCourse.ToList();
                List<UITeacherCourse> courses = new List<UITeacherCourse>();
                foreach (var item in stuC)
                {
                    courses.Add(new UITeacherCourse(item));
                }
                return JsonConvert.SerializeObject(new { OK = true, Data = courses });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }

        }
        #endregion

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Login(JObject json)
        {
            try
            {
                Person person = Login(json[ID].Value<long>(), json[Password].Value<string>());
                //格式：信息 用户名 角色
                return JsonConvert.SerializeObject(new { OK = true, Person = person });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { OK = false, ex.Message });
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Person Login(long id, string password)
        {
            Person person = Model.Person.Find(id);
            if (person != null)
            {

                if (person.Password == password)
                {
                    return person;
                }
                throw new Exception("密码错误");
            }
            else
            {
                throw new Exception("用户名不存在");
            }
        }

        /// <summary>
        /// 根据参照对象来更新一个对象的属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="reference"></param>
        /// <param name="whiteList"></param>
        private void UpdatePropertyValues<T>(T obj, T reference, string[] whiteList)
        {
            Type type = reference.GetType();
            foreach (var prop in type.GetRuntimeProperties())
            {
                if (Array.IndexOf(whiteList, prop.Name) == -1)
                {
                    continue;
                }
                var value = prop.GetValue(reference);
                prop.SetValue(obj, value);
            }
        }
    }
}
