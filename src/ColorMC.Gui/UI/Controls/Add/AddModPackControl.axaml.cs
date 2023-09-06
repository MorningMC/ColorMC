using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.Add;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.Utils;
using System.ComponentModel;

namespace ColorMC.Gui.UI.Controls.Add;

public partial class AddModPackControl : UserControl, IUserControl
{
    public IBaseWindow Window => App.FindRoot(VisualRoot);

    public string Title => App.GetLanguage("AddModPackWindow.Title");

    public AddModPackControl()
    {
        InitializeComponent();

        DataGridFiles.DoubleTapped += DataGridFiles_DoubleTapped;

        Grid1.PointerPressed += Grid1_PointerPressed;

        Input1.KeyDown += Input1_KeyDown;
    }

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            (DataContext as AddModPackControlModel)!.Reload1();
        }
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "DisplayList")
        {
            Dispatcher.UIThread.Post(ScrollViewer1.ScrollToHome);
        }
        else if (e.PropertyName == "Display")
        {
            Dispatcher.UIThread.Post(() =>
            {
                if ((DataContext as AddModPackControlModel)!.Display)
                {
                    App.CrossFade300.Start(null, Grid1);
                }
                else
                {
                    App.CrossFade300.Start(Grid1, null);
                }
            });
        }
        else if (e.PropertyName == "WindowClose")
        {
            Window.Close();
        }
    }

    private async void Grid1_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var ev = e.GetCurrentPoint(this);
        if (ev.Properties.IsXButton1Pressed)
        {
            await (DataContext as AddModPackControlModel)!.Download();
            e.Handled = true;
        }
    }

    private void Input1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            (DataContext as AddModPackControlModel)!.Reload();
        }
    }

    private async void DataGridFiles_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        await (DataContext as AddModPackControlModel)!.Download();
    }

    public void Closed()
    {
        App.AddModPackWindow = null;
    }

    public void Opened()
    {
        Window.SetTitle(Title);

        DataGridFiles.SetFontColor();

        (DataContext as AddModPackControlModel)!.Source = 0;
    }

    public void SetBaseModel(BaseModel model)
    {
        var amodel = new AddModPackControlModel(model);
        amodel.PropertyChanged += Model_PropertyChanged;
        DataContext = amodel;
    }
}
