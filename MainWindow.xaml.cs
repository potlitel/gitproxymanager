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
        Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
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
            case nameof(MainViewModel.ToastVisible):
                AnimateToast();
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

    private void AnimateToast()
    {
        if (_viewModel == null) return;

        if (_viewModel.ToastVisible)
        {
            ToastTranslate.Y = 20;
            ToastBorder.Opacity = 0;

            var slideIn = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(250))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));

            ToastTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideIn);
            ToastBorder.BeginAnimation(OpacityProperty, fadeIn);
        }
        else
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            ToastBorder.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}