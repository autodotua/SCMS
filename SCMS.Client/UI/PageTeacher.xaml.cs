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
    public partial class PageTeacher : Page,INotifyPropertyChanged, IPageClosing
    {
        public Person Teacher { get; }
        public ObservableCollection<Course> TeacherCourses { get; } = new ObservableCollection<Course>();
        public ObservableCollection<UITeacherCourse> Scores { get; } = new ObservableCollection<UITeacherCourse>();

        public HashSet<UITeacherCourse> ModifiedScores { get; } = new HashSet<UITeacherCourse>();
        private Course selectedStudentCourse;

        public event PropertyChangedEventHandler PropertyChanged;

        public Course SelectedStudentCourse
        {
            get => selectedStudentCourse;
            set
            {
                selectedStudentCourse = value;
                initializeScores(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStudentCourse)));
            }
        }

        public PageTeacher(Person person)
        {
            Teacher = person;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JObject json = await NetHelper.Send(new { Command = Teacher_Courses, ID=Teacher.Id});
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("获取失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            foreach (JObject item in array)
            {
                Course course = item.ToObject<Course>();

                TeacherCourses.Add(course);
            }

            if(TeacherCourses.Count>0)
            {
                SelectedStudentCourse = TeacherCourses[0];
            }
        }

        private async  void initializeScores(Course course)
        {
            Scores.Clear();
            JObject json = await NetHelper.Send(new { Command = Teacher_StudentScoreList, ID = course.Id });
            if (!json[OK].Value<bool>())
            {
                TaskDialog.ShowError("获取失败：" + json[Message].Value<string>());
                return;
            }
            JArray array = json[Data] as JArray;
            foreach (JObject item in array)
            {
                UITeacherCourse score = item.ToObject<UITeacherCourse>();
                Scores.Add(score);
            }
        }

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

        private async void UpdateScoreButtonClick(object sender, RoutedEventArgs e)
        {
            JObject json = await NetHelper.Send(new { Command = Teacher_UpdateStudentScore, Modified = ModifiedScores.Select(p=>p.ToStudentCourse())}) as JObject;

            if (json[OK].Value<bool>())
            {
                TaskDialog.Show(json[Message].Value<string>());
                ModifiedScores.Clear();
            }
            else
            {
                TaskDialog.ShowError(json[Message].Value<string>());
            }
        }

        private void ScoresRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            ModifiedScores.Add(e.Row.DataContext as UITeacherCourse);
        }
    }

}
