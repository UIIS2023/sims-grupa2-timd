using System;
using System.Windows;

namespace SimsProject.WPF.View.Guide
{
    public class OpenWindowEventArgs : EventArgs
    {
        public Window Window { get; }

        public OpenWindowEventArgs(Window window)
        {
            Window = window;
        }
    }
}
