using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;
using GitProxyManager.ViewModels;

namespace GitProxyManager;

public partial class MainWindow : MetroWindow
{
    private MainViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel = DataContext as MainViewModel;
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel == null) return;

        switch (e.PropertyName)
        {
            case nameof(MainViewModel.MasterToggle):
                PulseScale(MasterScale);
                break;
            case nameof(MainViewModel.IsSystemEnabled):
                PulseScale(SystemScale);
                break;
            case nameof(MainViewModel.IsGitEnabled):
                PulseScale(GitScale);
                break;
            case nameof(MainViewModel.SystemStatusText):
            case nameof(MainViewModel.GitStatusText):
                PulseScale(InfoScale);
                break;
        }
    }

    private void PulseScale(ScaleTransform scale)
    {
        var animDown = new DoubleAnimation(0.97, TimeSpan.FromMilliseconds(120))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };
        animDown.Completed += (s, e) =>
        {
            var animUp = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(180))
            {
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 3 }
            };
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, animUp);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, animUp);
        };
        scale.BeginAnimation(ScaleTransform.ScaleXProperty, animDown);
        scale.BeginAnimation(ScaleTransform.ScaleYProperty, animDown);
    }
}