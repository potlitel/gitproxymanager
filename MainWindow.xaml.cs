using System.ComponentModel;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using GitProxyManager.ViewModels;

namespace GitProxyManager;

public partial class MainWindow : Window
{
    private MainViewModel? _viewModel;

    internal bool AllowClose { get; set; }

    public MainWindow()
        : this(new MainViewModel())
    {
    }

    public MainWindow(MainViewModel? viewModel)
    {
        InitializeComponent();

        DataContext = viewModel ?? new MainViewModel();
        _viewModel = DataContext as MainViewModel;
        if (_viewModel != null)
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        try
        {
            Icon = new WindowIcon(Avalonia.Platform.AssetLoader.Open(
                new Uri("avares://GitProxyManager/Resources/Icons/app-icon.png")));
        }
        catch
        {
            // El icono es decorativo; la app puede funcionar sin el.
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!AllowClose)
        {
            e.Cancel = true;
            Hide();
        }

        base.OnClosing(e);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel == null) return;

        switch (e.PropertyName)
        {
            case nameof(MainViewModel.MasterToggle):
                PulseScale(GetScale("MasterBorder"));
                break;
            case nameof(MainViewModel.IsSystemEnabled):
                PulseScale(GetScale("SystemBorder"));
                break;
            case nameof(MainViewModel.IsGitEnabled):
                PulseScale(GetScale("GitBorder"));
                break;
            case nameof(MainViewModel.SystemStatusText):
            case nameof(MainViewModel.GitStatusText):
                PulseScale(GetScale("InfoBorder"));
                break;
            case nameof(MainViewModel.ToastVisible):
                AnimateToast();
                break;
        }
    }

    private ScaleTransform? GetScale(string borderName)
        => this.FindControl<Border>(borderName)?.RenderTransform as ScaleTransform;

    private static async void PulseScale(ScaleTransform? scale)
    {
        if (scale == null) return;

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(260),
            Easing = new QuadraticEaseInOut(),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 0.97),
                        new Setter(ScaleTransform.ScaleYProperty, 0.97)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0)
                    }
                }
            }
        };

        await animation.RunAsync(scale);
    }

    private void AnimateToast()
    {
        var toast = this.FindControl<Border>("ToastBorder");
        if (toast != null)
            toast.Opacity = _viewModel?.ToastVisible == true ? 1 : 0;
    }
}
