using Avalonia.Controls;
using GeometryReflectionApp.ViewModels;

namespace GeometryReflectionApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GeometryReflectionViewModel();
        }
    }
}