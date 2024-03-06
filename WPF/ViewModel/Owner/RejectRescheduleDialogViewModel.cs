using System;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Owner
{
    public class RejectRescheduleDialogViewModel : ViewModelBase
    {
        public string Explanation { get; set; }

        #region COMMANDS

        public ICommand RejectCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion
        
        #region EVENTS

        public bool Result { get; set; }
        public event EventHandler CloseDialogRequested;

        #endregion

        public RejectRescheduleDialogViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RejectCommand = new RelayCommand(Reject);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Reject(object obj)
        {
            CloseDialogRequested?.Invoke(this, EventArgs.Empty);
            Result = true;
        }

        private void Cancel(object obj)
        {
            CloseDialogRequested?.Invoke(this, EventArgs.Empty);
            Result = false;
        }
    }
}
