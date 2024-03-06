using System.Windows.Input;
using System.Windows;
using System.Windows.Interactivity;

namespace SimsProject.WPF.ViewModel
{
    public class KeyBehavior : Behavior<UIElement>
    {
        public ICommand KeyCommand
        {
            get { return (ICommand)GetValue(KeyCommandProperty); }
            set { SetValue(KeyCommandProperty, value); }
        }

        public static readonly DependencyProperty KeyCommandProperty =
            DependencyProperty.Register("KeyCommand", typeof(ICommand), typeof(KeyBehavior), new UIPropertyMetadata(null));


        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += new KeyEventHandler(AssociatedObjectKeyUp);
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= new KeyEventHandler(AssociatedObjectKeyUp);
            base.OnDetaching();
        }

        private void AssociatedObjectKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return) e.Handled = true;
            if (KeyCommand != null)
            {
                KeyCommand.Execute(e.Key);
            }
        }
    }
}
