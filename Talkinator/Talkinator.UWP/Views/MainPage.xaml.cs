using System;

using Talkinator.UWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Talkinator.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }


        // Methods

    }
}
