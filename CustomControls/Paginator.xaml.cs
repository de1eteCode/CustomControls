using System.Collections.Generic;
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
        private int _range = 2;
        private int _minPage = 1;
        private int _currentPage = 1;
        private int _maxPage = 1;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Paginator()
        {
            InitializeComponent();
        }

        public int Range
        {
            get => _range;
            set
            {
                _range = value;
                OnPropertyChanged("Pages");
            }
        }
        public int MinPage
        {
            get => _minPage;
            set => _minPage = value;
        }
        public int MaxPage
        {
            get => _maxPage;
            set
            {
                _maxPage = value;
                OnPropertyChanged("Pages");
            }
        }
        public int CurrentPage
        {
            get => _currentPage;
            private set
            {
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged("Pages");
            }
        }
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ClickOnPage(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var send = (ListViewItem)sender;
            CurrentPage = ((PageRecord)send.Content).Number;
        }
    }

    public record PageRecord
    {
        public int Number { get; private set; }
        public bool IsSelected { get; private set; }

        public PageRecord(int number, bool isSelected)
        {
            Number = number;
            IsSelected = isSelected;
        }
    }
}
