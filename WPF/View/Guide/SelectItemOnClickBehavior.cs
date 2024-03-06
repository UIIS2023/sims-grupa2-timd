using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimsProject.WPF.View.Guide
{
    public static class SelectItemOnClickBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(SelectItemOnClickBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                button.Click += Button_Click;
            }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: { } } button)
            {
                var command = GetCommand(button);
                if (command != null)
                {
                    var listBox = FindParent<ListBox>(button);
                    if (listBox != null)
                    {
                        listBox.SelectedItem = button.DataContext;
                    }

                    command.Execute(button.DataContext);
                }
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(child);
                switch (parent)
                {
                    case null:
                        return null;
                    case T typedParent:
                        return typedParent;
                    default:
                        child = parent;
                        break;
                }
            }
        }
    }
}
