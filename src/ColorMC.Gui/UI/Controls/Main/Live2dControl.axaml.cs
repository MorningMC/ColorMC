using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using ColorMC.Gui.UI.Flyouts;
using ColorMC.Gui.UI.Model.Main;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ColorMC.Gui.UI.Controls.Main;

public partial class Live2dControl : UserControl
{
    private CancellationTokenSource _cancel = new();
    private FpsTimer _renderTimer;

    public Live2dControl()
    {
        InitializeComponent();

        Live2dTop.PointerPressed += Live2dTop_PointerPressed;
        Live2dTop.PointerReleased += Live2dTop_PointerReleased;
        Live2dTop.PointerMoved += Live2dTop_PointerMoved;

        DataContextChanged += Live2dControl_DataContextChanged;

        _renderTimer = new(Live2d)
        {
            FpsTick = FpsTick
        };

        App.OnClose += App_OnClose;
    }

    private void App_OnClose()
    {
        _renderTimer.Close();
    }

    private void Live2dControl_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
    }

    private void FpsTick(int fps)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (IsVisible)
            {
                Label1.Content = $"{fps}Fps";
            }
        });
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ModelText")
        {
            Dispatcher.UIThread.Post(() =>
            {
                _cancel.Cancel();
                _cancel = new();

                ShowMessage();
            });
        }
        else if (e.PropertyName == "ModelChangeDone")
        {
            if (Live2d.HaveModel)
            {
                IsVisible = true;
                _renderTimer.Pause = false;
            }
            else if (!Live2d.HaveModel)
            {
                IsVisible = false;
                _renderTimer.Pause = true;
            }
        }
        else if (e.PropertyName == "Render")
        {
            var model = (sender as MainModel)!;
            if (!model.Render)
            {
                _renderTimer.Pause = true;
            }
            else if (Live2d.HaveModel)
            {
                _renderTimer.Pause = false;
            }
        }
    }

    private void Live2dTop_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!Live2d.HaveModel)
        {
            return;
        }

        LongPressed.Cancel();

        var pro = e.GetCurrentPoint(this);
        if (pro.Properties.IsLeftButtonPressed)
            Live2d.Moved((float)pro.Position.X, (float)pro.Position.Y);
    }

    private void Live2dTop_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!Live2d.HaveModel)
        {
            return;
        }

        LongPressed.Released();
        Live2d.Release();
    }

    private void Live2dTop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!Live2d.HaveModel)
        {
            return;
        }

        var pro = e.GetCurrentPoint(this);
        if (pro.Properties.IsLeftButtonPressed)
        {
            Live2d.Pressed();
            Live2d.Moved((float)pro.Position.X, (float)pro.Position.Y);
        }
        else if (pro.Properties.IsRightButtonPressed)
        {
            Flyout((sender as Control)!);
        }
        else
        {
            LongPressed.Pressed(() => Flyout((sender as Control)!));
        }
    }

    private void Flyout(Control control)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _ = new Live2DFlyout(control, Live2d);
        });
    }

    private async void ShowMessage()
    {
        if (!Live2d.HaveModel)
        {
            return;
        }
        await App.CrossFade300.Start(null, Border1, _cancel.Token);
        if (_cancel.Token.IsCancellationRequested)
        {
            return;
        }
        await Task.Delay(TimeSpan.FromSeconds(5));
        if (_cancel.Token.IsCancellationRequested)
        {
            return;
        }
        await App.CrossFade300.Start(Border1, null, _cancel.Token);
    }
}
