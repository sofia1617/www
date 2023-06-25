using CV19.Infrastructure.Commands;
using CV19.Models;
using CV19.Models.Decanat;
using CV19.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CV19.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public ObservableCollection<Group> Groups { get; }

        #region SelectedGroup:Group - Выбранная группа
        private Group _SelectedGroup;
        /// <summary>Выбранная группа</summary>
        public Group SelectedGroup
        {
            get => _SelectedGroup;
            set
            {
                if (!Set(ref _SelectedGroup, value))
                {
                    return;
                }
                _SelectedGroupStudents.Source = value?.Students;
                OnPropertyChanged(nameof(SelectedGroupStudents));
            }

        }
        #endregion

        #region StudentFilterText:string - Текст фильтра студентов
        private string _StudentFilterText;
        /// <summary>Текст фильтра студентов</summary>
        public string StudentFilterText
        {
            get => _StudentFilterText;
            set
            {
                if (!Set(ref _StudentFilterText, value))
                {
                    return;
                }
                _SelectedGroupStudents.View.Refresh();
            }
        }
        #endregion

        #region SelectedGroupStudents
        private readonly CollectionViewSource _SelectedGroupStudents = new CollectionViewSource();
        public ICollectionView? SelectedGroupStudents => _SelectedGroupStudents?.View;
        private void OnStudentFiltred(object sender, FilterEventArgs e)
        {
            if (e.Item is not Student student)
            {
                e.Accepted = false;
                return;
            }

            var text_filer = _StudentFilterText;
            if (string.IsNullOrEmpty(text_filer))
            {
                return;
            }

            if (student.Name is null || student.Surname is null || student.Patronymic is null)
            {
                e.Accepted = false;
                return;
            }

            if (student.Name.Contains(text_filer, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (student.Surname.Contains(text_filer, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (student.Patronymic.Contains(text_filer, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            e.Accepted = false;
        }
        #endregion

        public object[] CompositeCollection { get; }

        #region SelectedCompositeValue:object - Выбранный непонятный элемент
        private object _SelectedCompositeValue;
        /// <summary>Выбранный непонятный элемент</summary>
        public object SelectedCompositeValue
        {
            get => _SelectedCompositeValue;
            set => Set(ref _SelectedCompositeValue, value);
        }
        #endregion

        #region SelectedPageIndex:int - Номер выбранной вкладки
        private int _SelectedPageIndex = 0;
        /// <summary>Номер выбранной вкладки</summary>
        public int SelectedPageIndex
        {
            get => _SelectedPageIndex;
            set => Set(ref _SelectedPageIndex, value);
        }
        #endregion

        #region DataPoint:IEnumerable<DataPoint> - Набор данных для визуализации
        private IEnumerable<DataPoint>? _TestDataPoint;
        /// <summary>Набор данных для визуализации</summary>
        public IEnumerable<DataPoint>? TestDataPoint
        {
            get => _TestDataPoint;
            set => Set(ref _TestDataPoint, value);
        }
        #endregion

        #region Заголовок окна
        private string _title = "Анализ статистики";
        /// <summary>Заголовок окна</summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion

        #region Status:string - Статус
        private string _Status = string.Empty;
        /// <summary>Статус</summary>
        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }
        #endregion

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region ChangetTabIndexCommand
        public ICommand ChangetTabIndexCommand { get; }
        private bool CanChangetTabIndexCommandExecute(object p) => SelectedPageIndex >= 0;
        private void OnChangetTabIndexCommandExecuted(object p)
        {
            if (p is null)
            {
                return;
            }
            SelectedPageIndex += Convert.ToInt32(p);
        }
        #endregion

        #region CreateGroupCommand
        public ICommand CreateGroupCommand { get; }
        private bool CanCreateGroupCommandExecute(object p) => true;
        private void OnCreateGroupCommandExecuted(object p)
        {
            var groupMaxIndex = Groups.Count + 1;
            var newGroup = new Group
            {
                Name = $"Группа {groupMaxIndex}",
                Students = new ObservableCollection<Student>()
            };

            Groups.Add(newGroup);

            SelectedGroup = newGroup;
        }
        #endregion

        #region DeleteGroupCommand
        public ICommand DeleteGroupCommand { get; }
        private bool CanDeleteGroupCommandExecute(object p) => p is Group group && Groups.Contains(group);
        private void OnDeleteGroupCommandExecuted(object p)
        {
            if (p is not Group group)
                return;

            var groupIndex = Groups.IndexOf(group);
            Groups.Remove(group);
            if (groupIndex < Groups.Count)
            {
                SelectedGroup = Groups[groupIndex];
            }
        }
        #endregion

        #endregion

        public IEnumerable<Student> TestStudents => Enumerable.Range(1, App.IsDesignMode ? 10 : 100_000).Select(i => new Student()
        {
            Name = $"Name {i}",
            Surname = $"Surname {i}"
        });


        public DirectoryViewModel DiskRootDir { get; } = new DirectoryViewModel("C:\\");

        #region SelectedDirectory:DirectoryViewModel - Выбранная директория
        private DirectoryViewModel _SelectedDirectory;
        /// <summary>Выбранная директория</summary>
        public DirectoryViewModel SelectedDirectory
        {
            get => _SelectedDirectory;
            set => Set(ref _SelectedDirectory, value);
        }
        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            ChangetTabIndexCommand = new LambdaCommand(OnChangetTabIndexCommandExecuted, CanChangetTabIndexCommandExecute);
            CreateGroupCommand = new LambdaCommand(OnCreateGroupCommandExecuted, CanCreateGroupCommandExecute);
            DeleteGroupCommand = new LambdaCommand(OnDeleteGroupCommandExecuted, CanDeleteGroupCommandExecute);

            #endregion


            var student_index = 1;
            var students = Enumerable.Range(1, 10).Select(i => new Student()
            {
                Name = $"Name {student_index}",
                Surname = $"Surname {student_index}",
                Patronymic = $"Patronymic {student_index++}",
                Birthday = DateTime.Now,
                Rating = 0
            });
            var groups = Enumerable.Range(1, 20).Select(i => new Group()
            {
                Name = $"Group {i}",
                Students = new ObservableCollection<Student>(students)
            });
            Groups = new ObservableCollection<Group>(groups);

            var group = Groups[1];

            var dataList = new List<object> {
                "string ",
                42,
                group,
                group.Students.First()
            };

            CompositeCollection = dataList.ToArray();


            _SelectedGroupStudents.Filter += OnStudentFiltred;

        }
    }
}