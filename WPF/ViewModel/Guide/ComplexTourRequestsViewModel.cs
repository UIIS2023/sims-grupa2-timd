using SimsProject.Application.Interface;
using SimsProject.Domain.Model;
using SimsProject.WPF.View.Guide;
using SimsProject.WPF.ViewModel.Domain;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SimsProject.WPF.ViewModel.Guide
{
    public class ComplexTourRequestsViewModel : ViewModelBase
    {
        #region PROPERTIES

        private User LoggedInUser { get; }
        public ObservableCollection<ComplexTourRequestViewModel> ComplexTourRequests { get; set; }

        #endregion

        #region COMMANDS

        public ICommand OpenGuideOverviewCommand { get; set; }
        public ICommand AcceptTourRequestCommand { get; set; }

        #endregion

        #region SERVICES

        private IComplexTourRequestService _complexTourRequestService;

        #endregion

        #region EVENTS

        public static event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        public event EventHandler<OpenWindowEventArgs> OpenWindowRequested;

        #endregion

        public ComplexTourRequestsViewModel(User user)
        {
            LoggedInUser = user;

            InitializeServices();
            InitializeCollections();
            InitializeCommands();
        }

        private void InitializeServices()
        {
            _complexTourRequestService = Injector.CreateInstance<IComplexTourRequestService>();
        }

        private void InitializeCommands()
        {
            OpenGuideOverviewCommand = new RelayCommand(OpenGuideOverview);
            AcceptTourRequestCommand = new RelayCommand(AcceptTourRequest);
        }

        private void InitializeCollections()
        {
            ComplexTourRequests = new ObservableCollection<ComplexTourRequestViewModel>();

            foreach (var complexTourRequest in _complexTourRequestService.GetAll().Where(complexTourRequest => complexTourRequest.Status == ComplexRequestStatus.Pending))
            {
                ComplexTourRequests.Add(new ComplexTourRequestViewModel(complexTourRequest));
            }
        }

        private void AcceptTourRequest(object obj)
        {
            if (obj is not TourRequestViewModel selectedRequest) return;
            var createTour = new CreateTour(LoggedInUser, selectedRequest, null, null);
            OpenWindowRequested?.Invoke(this, new OpenWindowEventArgs(createTour));
            OnActionCompleted();
        }

        private void OpenGuideOverview(object obj)
        {
            MainWindow mainWindow = new(LoggedInUser);
            mainWindow.Show();
            OnActionCompleted();
        }

        private void OnActionCompleted()
        {
            var args = new ActionCompletedEventArgs();
            ActionCompleted?.Invoke(this, args);
        }
    }
}
