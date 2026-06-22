using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using GitProxyManager.ViewModels;

namespace GitProxyManager;

public partial class MainWindow : MetroWindow
{
    private MainViewModel? _viewModel;
    private string _lastToggleLabel = string.Empty;
    private string _lastConfigInfo = string.Empty;

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
            _lastToggleLabel = _viewModel.ToggleLabel;
            _lastConfigInfo = _viewModel.CurrentProxyInfo;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.ToggleLabel) && _viewModel != null)
        {
            var newLabel = _viewModel.ToggleLabel;
            if (newLabel != _lastToggleLabel)
            {
                _lastToggleLabel = newLabel;
                AnimateFade(ToggleBorder);
            }
        }
        else if (e.PropertyName == nameof(MainViewModel.CurrentProxyInfo) && _viewModel != null)
        {
            var newInfo = _viewModel.CurrentProxyInfo;
            if (newInfo != _lastConfigInfo)
            {
                _lastConfigInfo = newInfo;
                AnimateFade(ConfigInfoBorder);
            }
        }
    }

    private void AnimateFade(FrameworkElement element)
    {
        var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromSeconds(0.15));
        fadeOut.Completed += (s, e) =>
        {
            var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(0.15));
            element.BeginAnimation(OpacityProperty, fadeIn);
        };
        element.BeginAnimation(OpacityProperty, fadeOut);
    }
}
