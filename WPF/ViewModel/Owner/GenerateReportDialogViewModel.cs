using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class GenerateReportDialogViewModel : ViewModelBase, IDataErrorInfo
    {
        #region PROPERTIES

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (value != _startDate)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (value != _endDate)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(StartDate));
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        #endregion

        #region VALIDATION

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(StartDate):
                        if (StartDate is null)
                        {
                            error = "Start date is required";
                        }
                        if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
                        {
                            error = "End date must be after start date.";
                        }
                        break;

                    case nameof(EndDate):
                        if (EndDate is null)
                        {
                            error = "End date is required";
                        }
                        if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
                        {
                            error = "End date must be after start date.";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error => null;

        private readonly string[] _canFindDatesValidatedProperties = { "StartDate", "EndDate" };

        #endregion

        #region COMMANDS

        public ICommand GenerateCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region EVENTS

        public bool Result { get; set; }
        public event EventHandler CloseDialogRequested;

        #endregion

        public GenerateReportDialogViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            GenerateCommand = new RelayCommand(Generate, CanGenerate);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Generate(object obj)
        {
            CloseDialogRequested?.Invoke(this, EventArgs.Empty);
            Result = true;
        }

        private bool CanGenerate(object obj)
        {
            return _canFindDatesValidatedProperties.All(property => string.IsNullOrEmpty(this[property]));
        }

        private void Cancel(object obj)
        {
            CloseDialogRequested?.Invoke(this, EventArgs.Empty);
            Result = false;
        }
    }
}
