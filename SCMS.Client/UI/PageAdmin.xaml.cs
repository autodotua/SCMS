using FzLib.Control.Dialog;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCMS.Common;
using SCMS.Common.DbData;
using SCMS.Common.UIData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskDialog = FzLib.Control.Dialog.TaskDialog;
using static SCMS.Common.ApiCommands;
using SCMS.Client.UI.Dialog;

namespace SCMS.Client.UI
{
    /// <summary>
    /// WinStudentManager.xaml 的交互逻辑
    /// </summary>
    public partial class PageAdmin : Page, IPageClosing
    {
        #region 窗体和页面
        public PageAdmin(Person person)
        {
            Admin = person;
            InitializeComponent();
            //SetDataGridReadOnly(true);
            Students.CollectionChanged += CollectionChanged;
            Teachers.CollectionChanged += CollectionChanged;
            Courses.CollectionChanged += CollectionChanged;



        }
        public bool PageClosing()
        {
            if (WaitingForUpdating())
            {
                if (TaskDialog.ShowWithYesNoButtons("还有已修改的数据没有更新，是否退出？", "正在关闭程序") == true)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion

        #region 基本数据集合
        public Person Admin { get; }
        public ObservableCollection<Person> Students { get; } = new ObservableCollection<Person>();
        public ObservableCollection<Person> Teachers { get; } = new ObservableCollection<Person>();
        public ObservableCollection<UICourse> Courses { get; } = new ObservableCollection<UICourse>();


        public string[] Genders { get; } = { "男", "女", "未知" };
        #endregion

        #region 监听数据更改
        public HashSet<Person> DeletedPersons { get; } = new HashSet<Person>();
        public HashSet<Person> ModifiedPersons { get; } = new HashSet<Person>();
        public HashSet<UICourse> DeletedCourses { get; } = new HashSet<UICourse>();
        public HashSet<UICourse> ModifiedCourses { get; } = new HashSet<UICourse>();
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    if (item is Person)
                    {
                        DeletedPersons.Add(item as Person);
                        if((item as Person).Role=="教师")
                        {
                            Courses.CollectionChanged -= CollectionChanged;
                            foreach (var course in Courses.Where(p=>p.TeacherId==(item as Person).Id).ToArray())
                            {
                                Courses.Remove(course);
                            }
                            Courses.CollectionChanged += CollectionChanged;
                        }
                    }
                    else if (item is UICourse)
                    {
                        DeletedCourses.Add(item as UICourse);
                    }
                }
            }
        }
        private void DgrdStudentsRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (sender == dgrdStudents)
            {
                ModifiedPersons.Add(e.Row.DataContext as Person);
            }
            else if (sender == dgrdTeachers)
            {
                ModifiedPersons.Add(e.Row.DataContext as Person);
            }
            else if (sender == dgrdCourses)
            {
                ModifiedCourses.Add(e.Row.DataContext as UICourse);
            }
        }

        #endregion

        #region 加载
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStudents();
            await LoadTeachers();
            await LoadCourses();
        }

        /// <summary>
        /// 加载学生信息
        /// </summary>
        /// <returns></returns>
        private async Task LoadStudents()
        {
            Students.Clear();
            JObject json = await NetHelper.Send(new { Command = Admin_Students });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("加载学生列表失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            foreach (dynamic item in array)
            {
                Person person = new Person();
                person.Id = item.Id;
                person.Name = item.Name;
                person.Born = item.Born;
                person.Gender = item.Gender;
                person.StartYear = item.StartYear;
                person.Major = item.Major;

                Students.Add(person);
            }
        }
        /// <summary>
        /// 加载教师信息
        /// </summary>
        /// <returns></returns>
        private async Task LoadTeachers()
        {
            Teachers.Clear();
            JObject json = await NetHelper.Send(new { Command = ApiCommands.Admin_Teachers });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("加载教师列表失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            foreach (dynamic item in array)
            {
                Person person = new Person();
                person.Id = item.Id;
                person.Name = item.Name;
                person.Born = item.Born;
                person.Gender = item.Gender;
                person.StartYear = item.StartYear;
                person.Major = item.Major;
                person.Role = item.Role;

                Teachers.Add(person);
            }
        }
        /// <summary>
        /// 加载课程信息
        /// </summary>
        /// <returns></returns>
        private async Task LoadCourses()
        {
            Courses.Clear();
            JObject json = await NetHelper.Send(new { Command = Admin_Courses });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("加载课程列表失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            foreach (dynamic item in array)
            {
                UICourse course = new UICourse();
                course.Id = item.Id;
                course.Name = item.Name;
                course.Teacher = item.Teacher;
                course.TeacherId = item.TeacherId;
                course.Year = item.Year;
                course.Term = item.Term;

                Courses.Add(course);
            }
        }

        #endregion

        #region 更新
        /// <summary>
        /// 更新学生或教师信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdatePersonsButtonClick(object sender, RoutedEventArgs e)
        {
            if (ModifiedPersons.Count + DeletedPersons.Count == 0)
            {
                TaskDialog.ShowError("没有修改或删除的人员");
                return;
            }
            JObject json = await NetHelper.Send(new { Command = Admin_Update_Persons, Modified = ModifiedPersons, Deleted = DeletedPersons }) as JObject;

            if (json[OK].Value<bool>())
            {
                TaskDialog.Show(json[Message].Value<string>());
                ModifiedPersons.Clear();
                DeletedPersons.Clear();
            }
            else
            {
                TaskDialog.ShowError(json[Message].Value<string>());
            }
        }


        /// <summary>
        /// 更新课程信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateCourseButtonClick(object sender, RoutedEventArgs e)
        {
            if(ModifiedCourses.Count+DeletedCourses.Count==0)
            {
                TaskDialog.ShowError("没有修改或删除的课程");
                return;
            }
            JObject json = await NetHelper.Send(new { Command = Admin_Update_Courses, Modified = ModifiedCourses.Select(p => p.ToCourse()), Deleted = DeletedCourses.Select(p => p.ToCourse()) }) as JObject;
            if (json[OK].Value<bool>())
            {
                TaskDialog.Show(json[Message].Value<string>());
                ModifiedCourses.Clear();
                DeletedCourses.Clear();
            }
            else
            {
                TaskDialog.ShowError(json[Message].Value<string>());
            }

        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetPasswordButtonClick(object sender, RoutedEventArgs e)
        {
            Person person = null;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    person = ((DataGridRow)vis).DataContext as Person;
                    person.Password = "123456";
                    ModifiedPersons.Add(person);
                    (sender as Button).IsEnabled = false;
                    break;
                }
            }
        }

        #endregion

        #region 获取关联数据
        /// <summary>
        /// 判断现在是否有数据还未上传到服务器
        /// </summary>
        /// <returns></returns>
        private bool WaitingForUpdating()
        {
            return
                ModifiedPersons.Count + ModifiedCourses.Count
            + DeletedPersons.Count + DeletedCourses.Count > 0;
        }
        /// <summary>
        /// 获取一个学生所选的课程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetCoursesOfStudentButtonClick(object sender, RoutedEventArgs e)
        {
            if (WaitingForUpdating())
            {
                TaskDialog.ShowError("请先更新数据，然后进行相关操作");
                return;
            }
            if (dgrdStudents.SelectedItems.Count != 1)
            {
                TaskDialog.ShowError("请选择且仅选择一个学生");
                return;
            }
            JObject json = await NetHelper.Send(new { Command = Admin_CourseOfStudent, ID = (dgrdStudents.SelectedItem as Person).Id });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("获取失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            dgrdCourses.SelectedItems.Clear();
            foreach (JValue item in array)
            {
                dgrdCourses.SelectedItems.Add(Courses.First(p => p.Id == item.Value<int>()));
            }
        }
        /// <summary>
        /// 获取一位教师需要上的课程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetCoursesOfTeacherButtonClick(object sender, RoutedEventArgs e)
        {
            if (WaitingForUpdating())
            {
                TaskDialog.ShowError("请先更新数据，然后进行相关操作");
                return;
            }
            if (dgrdTeachers.SelectedItems.Count != 1)
            {
                TaskDialog.ShowError("请选择且仅选择一个教师");
                return;
            }
            JObject json = await NetHelper.Send(new { Command = Admin_CourseOfTeacher, ID = (dgrdTeachers.SelectedItem as Person).Id });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("获取失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            dgrdCourses.SelectedItems.Clear();
            foreach (JValue item in array)
            {
                dgrdCourses.SelectedItems.Add(Courses.First(p => p.Id == item.Value<int>()));
            }
        }
        /// <summary>
        /// 获取一个课程的上课教师和需要上课的学生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetStudentsAndTeacherButtonClick(object sender, RoutedEventArgs e)
        {
            if (WaitingForUpdating())
            {
                TaskDialog.ShowError("请先更新数据，然后进行相关操作");
                return;
            }
            if (dgrdCourses.SelectedItems.Count != 1)
            {
                TaskDialog.ShowError("请选择且仅选择一门课程");
                return;
            }
            JObject json = await NetHelper.Send(new { Command = Admin_StudentsAndTeacherOfCourses, ID = (dgrdCourses.SelectedItem as UICourse).Id });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("获取失败：" + json[Message].Value<string>());
                return;
            }
            JArray students = json[Data][ApiCommands.Students] as JArray;
            JValue teacher = json[Data][Teacher] as JValue;
            dgrdStudents.SelectedItems.Clear();
            dgrdTeachers.SelectedItems.Clear();
            foreach (JValue item in students)
            {
                dgrdStudents.SelectedItems.Add(Students.First(p => p.Id == item.Value<long>()));
            }
            dgrdTeachers.SelectedItem = Teachers.First(p => p.Id == teacher.Value<long>());
        }
        /// <summary>
        /// 设置一门课程需要上课的学生和上课教师
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetStudentsAndTeacherOfCourseButtonClick(object sender, RoutedEventArgs e)
        {
            if (WaitingForUpdating())
            {
                TaskDialog.ShowError("请先更新数据，然后进行相关操作");
                return;
            }
            if (dgrdStudents.SelectedItems.Count == 0)
            {
                TaskDialog.ShowError("请至少选择一位学生");
                return;
            }
            if (dgrdTeachers.SelectedItems.Count != 1)
            {
                TaskDialog.ShowError("请选择一位教师");
                return;
            }
            if (dgrdCourses.SelectedItems.Count != 1)
            {
                TaskDialog.ShowError("请选择一门课程");
                return;
            }
            int id = (dgrdCourses.SelectedItem as UICourse).Id;
            IEnumerable<long> studentsId = dgrdStudents.SelectedItems.Cast<Person>().Select(p => p.Id);
            long teacherId = (dgrdTeachers.SelectedItem as Person).Id;

            JObject json = await NetHelper.Send(new { Command = Admin_SetStudentsAndTeacherOfCourse, ID = id, Students = studentsId, Teacher = teacherId }) as JObject;

            TaskDialog.Show(json[Message].Value<string>(),
                icon: json[OK].Value<bool>() ? TaskDialogStandardIcon.None : TaskDialogStandardIcon.Error);
            await LoadCourses();
        }
        #endregion

        /// <summary>
        /// 单击新建条目按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewItemButtonClick(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            if (btn.Tag.Equals("2"))
            {
                AddNewCourseDialog dialog = new AddNewCourseDialog(Courses.Max(p => p.Id) + 1, "新增课程");
                if (dialog.ShowDialog() == true)
                {
                    Course course = dialog.Course;
                    if (Courses.Any(p => p.Id == course.Id))
                    {
                        TaskDialog.ShowError("该ID已存在");
                        return;
                    }
                    ModifiedCourses.Add(new UICourse(course));
                    Courses.Add(new UICourse(course));
                }

            }
            else
            {
                bool teacher = btn.Tag.Equals("1");
                AddNewPersonDialog dialog = new AddNewPersonDialog(teacher? Students.Max(p => p.Id) + 1:Teachers.Max(p=>p.Id)+1, teacher);
                if (dialog.ShowDialog() == true)
                {
                    Person person = dialog.Person;
                    if (Students.Any(p => p.Id == person.Id) || Teachers.Any(p => p.Id == person.Id))
                    {
                        TaskDialog.ShowError("该ID已存在");
                        return;
                    }
                    if (btn.Tag.Equals("0"))
                    {
                        Students.Add(person);
                    }
                    else
                    {
                        Teachers.Add(person);
                    }
                    ModifiedPersons.Add(person);
                }
            }
        }

        private void MenuDeleteClick(object sender, RoutedEventArgs e)
        {
            DataGrid dgrd = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DataGrid;
            foreach (var item in new System.Collections.ArrayList(dgrd.SelectedItems))
            {
                (dgrd.ItemsSource as System.Collections.IList).Remove(item);
            }
        }
    }
    /// <summary>
    /// 性别转换器
    /// </summary>
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "未知";
            }
            switch ((byte)value)
            {
                case 1:
                    return "男";
                case 2:
                    return "女";
                default:
                    return "未知";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "男":
                    return 1;
                case "女":
                    return 2;
                default:
                    return 0;

            }
        }
    }

}
