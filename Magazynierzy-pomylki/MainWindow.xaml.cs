using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Magazynierzy_pomylki.Helpers;
using Magazynierzy_pomylki.Migrations;
using Magazynierzy_pomylki.Models;
using Magazynierzy_pomylki.Services;
using Microsoft.Win32;

namespace Magazynierzy_pomylki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _firstRun = true;
        private bool _buttonEnabled;
        private IssueModel _selectedIssue;
        private string _userName;
        private string _userNameFilter;
        private string _statusFilter;
        private string _changeFilter;
        private string _change;
        private DateTime _date;
        private string _description;
        private DateTime _showFor;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private OperatorModel _currentOperator;
        private string _currentPassword;
        private bool _accessGranted;

        public bool ButtonEnabled
        {
            get => _buttonEnabled;
            set
            {
                _buttonEnabled = value;
                OnPropertyChanged(nameof(ButtonEnabled));
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string UserNameFilter
        {
            get => _userNameFilter;
            set
            {
                _userNameFilter = value;
                OnPropertyChanged(nameof(UserNameFilter));
            }
        }

        public string StatusFilter
        {
            get => _statusFilter;
            set
            {
                _statusFilter = value;
                OnPropertyChanged(nameof(StatusFilter));
            }
        }

        public string ChangeFilter
        {
            get => _changeFilter;
            set
            {
                _changeFilter = value;
                OnPropertyChanged(nameof(ChangeFilter));
            }
        }

        public string Change
        {
            get => _change;
            set
            {
                _change = value;
                OnPropertyChanged(nameof(Change));
            }
        }

        public int ChangeAsNumber
        {
            get
            {
                if (Change.Contains("Dzienna"))
                    return 1;
                else
                    return 2;
            }
        }
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public IssueModel SelectedIssue
        {
            get => _selectedIssue;
            set
            {
                _selectedIssue = value;
                if (value == null)
                {
                    ButtonEnabled = false;
                }
                else
                {
                    ButtonEnabled = true;
                }
            }
        }

        public DateTime DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged(nameof(DateFrom));
            }
        }
        public DateTime DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged(nameof(DateTo));
            }
        }
        public DateTime ShowFor
        {
            get => _showFor;
            set
            {
                _showFor = value;
                OnPropertyChanged(nameof(ShowFor));
            }
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value;
                OnPropertyChanged(nameof(CurrentPassword));

                if (CurrentOperator != null)
                {
                    if (CurrentOperator.Password == value)
                    {
                        AccessGranted = true;
                    }
                    else
                    {
                        AccessGranted = false;
                    }
                }
            }
        }

        public OperatorModel CurrentOperator
        {
            get => _currentOperator;
            set
            {
                if (_currentOperator != null && _currentOperator.Name != value.Name)
                {
                    AccessGranted = false;
                    CurrentPassword = String.Empty;
                }

                _currentOperator = value;
                OnPropertyChanged(nameof(CurrentOperator));
            }
        }

        public bool AccessGranted
        {
            get => _accessGranted;
            set
            {
                _accessGranted = value;
                OnPropertyChanged(nameof(AccessGranted));
            }
        }

        public UsersService UsersService { get; set; }
        public IssuesService IssuesService { get; set; }
        public ObservableCollection<OperatorModel> Operators { get; set; } = new ObservableCollection<OperatorModel>();
        public ObservableCollection<string> Users { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> UsersFilter { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> StatusFilterOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> StatusChangeOptions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<IssueModel> Issues { get; set; } = new ObservableCollection<IssueModel>();

        public MainWindow()
        {
            _firstRun = true;
            MigrationManager.Migrate();

            UsersService = new UsersService();
            IssuesService = new IssuesService();

            InitializeComponent();
            Date = DateTime.Now;
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            ShowFor = DateTime.Now;
            Change = "Dzienna";
            StatusFilter = ConstHelper.EMPTY_LABEL_STATUS;
            ChangeFilter = ConstHelper.EMPTY_LABEL_STATUS;

            PrepareStatusFilterOptions();
            PrepareStatusChangeOptions();
            GetUsers();
            GetOperators();

            _firstRun = false;
            GetIssues();

        }

        private void GetOperators()
        {
            var operators = UsersService.GetOperators();
            Operators.Clear();
            foreach (var temoOperator in operators)
            {
                Operators.Add(temoOperator);
            }
        }

        private void PrepareStatusChangeOptions()
        {
            StatusChangeOptions.Add(ConstHelper.EMPTY_LABEL_STATUS);
            StatusChangeOptions.Add(ConstHelper.FIRST_CHANGE_FILTER);
            StatusChangeOptions.Add(ConstHelper.SECOND_CHANGE_FILTER);
        }

        private void PrepareStatusFilterOptions()
        {
            StatusFilterOptions.Add(ConstHelper.EMPTY_LABEL_STATUS);
            StatusFilterOptions.Add(ConstHelper.WITHOUT_EXPLAIN_NEED_FILTER);
            StatusFilterOptions.Add(ConstHelper.TO_EXPLAIN_FILTER);
            StatusFilterOptions.Add(ConstHelper.EXPLAINED_FILTER);
        }

        private void GetUsers()
        {
            var users = UsersService.GetUsers();
            UsersFilter.Add(ConstHelper.EMPTY_LABEL_WAREHOUSEMEN);
            foreach (var user in users)
            {
                Users.Add(user);
                UsersFilter.Add(user);
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Change) && Date != DateTime.MinValue)
            {
                var model = new AddIssueModel();
                model.UserName = UserName;
                model.Change = ChangeAsNumber;
                model.Description = Description;
                model.Date = Date;
                model.CreatedBy = CurrentOperator?.Name;

                try
                {
                    IssuesService.AddNewIssue(model);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Wystąpił problem przy dodawaniu wpisu. {0}"), ex.Message);
                    return;
                }

                GetIssues();

                Description = String.Empty;
            }
        }

        private void GetIssues()
        {
            if (_firstRun == false)
            {
                Issues.Clear();

                try
                {
                    var warheouseman = String.Empty;
                    if (!string.IsNullOrWhiteSpace(UserNameFilter) && UserNameFilter != ConstHelper.EMPTY_LABEL_WAREHOUSEMEN)
                    {
                        warheouseman = UserNameFilter;
                    }

                    var issues = IssuesService.GetIssuesForDate(ShowFor, warheouseman, StatusFilter, ChangeFilter);
                    foreach (var issue in issues)
                    {
                        Issues.Add(issue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił problem przy pobieraniu wpisów. {ex.Message}");
                    return;
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

            var issuesForDate = IssuesService.GetIssuesFromTo(DateFrom, DateTo);

            var newFile = $@"{DateFrom.ToString("dd.MM.yyyy")} - {DateTo.ToString("dd.MM.yyyy")}.xlsx";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = newFile;
            saveFileDialog.Filter = "Pliki Excel (*.xlsx)|*.xlsx|Wszystkie pliki (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                IssuesService.ExportToExcel(saveFileDialog.FileName, issuesForDate);
            }
        }

        private void ShowForDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ShowFor != DateTime.MinValue)
                GetIssues();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIssue != null && Issues.Any(x => x.Id == SelectedIssue.Id))
            {
                IssuesService.DeleteIssue(SelectedIssue.Id);

                GetIssues();
            }
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Filter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            GetIssues();
        }

        private void ToExpalinButton_Click(object sender, RoutedEventArgs e)
        {
            IssuesService.UpdateDriverExplainNeeded(SelectedIssue.Id, true);

            GetIssues();
        }

        private void AddExplainButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIssue != null)
            {
                var oldValue = SelectedIssue.DriverExplain;
                string inputRead = new InputBox("Wyjaśnienie", "Aktualizacja wyjaśnienia", "Arial", SelectedIssue.DriverExplain, 14).ShowDialog();
                if (oldValue != inputRead)
                {
                    IssuesService.UpdateDriverExplain(SelectedIssue.Id, inputRead);
                    IssuesService.UpdateDriverExplainNeeded(SelectedIssue.Id, false);


                    GetIssues();
                }

            }
        }

        private void CopyDescriptionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIssue != null)
            {
                Clipboard.SetText(SelectedIssue.Description);
            }
        }

        private void CopyDriverExplainMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIssue != null && SelectedIssue.DriverExplain != null)
            {
                Clipboard.SetText(SelectedIssue.DriverExplain);
            }
        }
    }
}
