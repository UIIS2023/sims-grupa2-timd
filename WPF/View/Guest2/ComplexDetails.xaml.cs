﻿using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Guest2;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for ComplexDetails.xaml
    /// </summary>
    public partial class ComplexDetails : Page
    {
        public ComplexDetails(User LoggedInUser,ComplexTourRequest Selected)
        {
            InitializeComponent();
            this.DataContext = new ComplexDetailsViewModel(LoggedInUser,this,Selected);
        }
    }
}
