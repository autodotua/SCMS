using FzLib.Control.Dialog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCMS.Common;
using SCMS.Common.DbData;
using SCMS.Common.UIData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static SCMS.Common.ApiCommands;

namespace SCMS.Client.UI
{
    /// <summary>
    /// WinStudentManager.xaml 的交互逻辑
    /// </summary>
    public partial class PageStudent : Page,INotifyPropertyChanged, IPageClosing
    {
        public class Info
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public byte Score { get; set; }
        }

        public Person Student { get; }
        public ObservableCollection<UIStudentCourse> StudentCourses { get; } = new ObservableCollection<UIStudentCourse>();


        public event PropertyChangedEventHandler PropertyChanged;



        public PageStudent(Person person)
        {
            Student = person;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JObject json = await NetHelper.Send(new { Command = Student_Courses, ID = Student.Id });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("获取失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            foreach (JObject item in array)
            {
                UIStudentCourse course = item.ToObject<UIStudentCourse>();

                StudentCourses.Add(course);
            }

            //if(StudentCourses.Count>0)
            //{
            //    SelectedStudentCourse = TeacherCourses[0];
            //}
        }

        //private async  void initializeScores(Course course)
        //{
        //    Scores.Clear();
        //    JArray array = JArray.Parse(await NetHelper.Send(JsonConvert.SerializeObject(new { Command = ApiCommands.Teacher_StudentScoreList, ID = course.Id })));

        //    foreach (dynamic item in array)
        //    {
        //        Info score = new Info();
        //        score.Id = item.Id;
        //        score.Score = item.Grade;
        //        JObject stu=JObject.Parse(await NetHelper.Send(JsonConvert.SerializeObject(new { Command = ApiCommands.Student,ID = item.Student })));
        //        score.Student = stu["Name"].Value<string>();
        //        Scores.Add(score);
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Person person = null;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    person = ((DataGridRow)vis).DataContext as Person;
                    break;
                }
            }



        }

        public bool PageClosing()
        {
            return true;
        }
    }

}
