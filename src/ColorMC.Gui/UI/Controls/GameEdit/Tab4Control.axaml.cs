using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.UI.Flyouts;
using ColorMC.Gui.UI.Model.GameEdit;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.UI.Controls.GameEdit;

public partial class Tab4Control : UserControl
{
    public Tab4Control()
    {
        InitializeComponent();

        DataGrid1.DoubleTapped += DataGrid1_DoubleTapped;
        DataGrid1.CellPointerPressed += DataGrid1_CellPointerPressed;

        AddHandler(DragDrop.DragEnterEvent, DragEnter);
        AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    public void Opened()
    {
        DataGrid1.SetFontColor();
    }

    private void DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            Grid2.IsVisible = true;
        }
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        Grid2.IsVisible = false;
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        Grid2.IsVisible = false;
        (DataContext as GameEditModel)!.DropMod(e.Data);
    }

    private void DataGrid1_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var items = DataGrid1.SelectedItems;
            var ev = e.PointerPressedEventArgs.GetCurrentPoint(this);
            if (ev.Properties.IsRightButtonPressed
            || (SystemInfo.Os == OsType.Android && ev.Properties.IsLeftButtonPressed))
            {
                _ = new GameEditFlyout1(this, items, (DataContext as GameEditModel)!);
            }
        });
    }

    private void DataGrid1_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        (DataContext as GameEditModel)!.DisEMod();
    }
}
