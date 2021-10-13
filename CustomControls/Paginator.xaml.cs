﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls
{
    /// <summary>
    /// Логика взаимодействия для Paginator.xaml
    /// </summary>
    public partial class Paginator : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty RangeProperty;
        public static readonly DependencyProperty CurrentPageProperty;
        public static readonly DependencyProperty MinPageProperty;
        public static readonly DependencyProperty MaxPageProperty;

        static Paginator()
        {
            RangeProperty = DependencyProperty.Register(nameof(Range), typeof(int), typeof(Paginator), new PropertyMetadata(2));
            CurrentPageProperty = DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(Paginator), new PropertyMetadata(1));
            MinPageProperty = DependencyProperty.Register(nameof(MinPageProperty), typeof(int), typeof(Paginator), new PropertyMetadata(1));
            MaxPageProperty = DependencyProperty.Register(nameof(MaxPageProperty), typeof(int), typeof(Paginator), new PropertyMetadata(1));
        }

        public Paginator()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        /// Диапазон страниц. Работает от центральной страницы, то есть, если диапазон - 2, то отображается 5 страниц (2 + 1 + 2)
        /// </summary>
        public int Range
        {
            get => (int)GetValue(RangeProperty);
            set
            {
                SetValue(RangeProperty, value);
                OnPropertyChanged("Pages");
            }
        }

        /// <summary>
        /// Минимальная страница
        /// </summary>
        public int MinPage
        {
            get => (int)GetValue(MinPageProperty);
            set
            {
                SetValue(MinPageProperty, value);
                OnPropertyChanged("Pages");
            }
        }

        /// <summary>
        /// Максимальная страница
        /// </summary>
        public int MaxPage
        {
            get => (int)GetValue(MaxPageProperty);
            set
            {
                SetValue(MaxPageProperty, value);
                OnPropertyChanged("Pages");
            }
        }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            private set
            {
                SetValue(CurrentPageProperty, value);
                OnPropertyChanged("Pages");
            }
        }

        /// <summary>
        /// Возвращает список страниц. Возвращаемые страницы зависят от текущей страницы и диапазона.
        /// Прожорливая штука, лучше найти замену ей.
        /// </summary>
        public IEnumerable<PageRecord> Pages
        {
            get
            {

                var start = 1;
                var end = 1;
                var min = MinPage + Range;
                var max = MaxPage - Range;

                if (CurrentPage < min)
                {
                    start = MinPage;
                    end = MinPage + Range * 2;
                }
                else if (CurrentPage > max)
                {
                    end = MaxPage;
                    start = MaxPage - Range * 2;
                }
                else
                {
                    start = CurrentPage - Range;
                    end = CurrentPage + Range;
                }

                if (start < MinPage)
                    start = MinPage;
                if (end > MaxPage)
                    end = MaxPage;

                for (int page = start; page <= end && page <= MaxPage; page++)
                {
                    yield return new PageRecord(page, page == CurrentPage);
                }
            }
        }

        #endregion
        #region Buttons

        private void GoFirst(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != MinPage)
                CurrentPage = MinPage;
        }
        private void GoPrevious(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > MinPage)
                CurrentPage--;
        }
        private void GoNext(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < MaxPage)
                CurrentPage++;
        }
        private void GoLast(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != MaxPage)
                CurrentPage = MaxPage;
        }

        #endregion
        #region Events
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Обработчик кликов по страницам в ListView
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы</param>
        private void ClickOnPage(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var send = (ListViewItem)sender;
            CurrentPage = ((PageRecord)send.Content).Number;
        }

        #endregion

    }

    public record PageRecord
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Выбрана она или нет. От этого свойства зависит подстветка на UI.
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number">Номер страницы</param>
        /// <param name="isSelected">Выбрана?</param>
        public PageRecord(int number, bool isSelected)
        {
            Number = number;
            IsSelected = isSelected;
        }
    }
}
