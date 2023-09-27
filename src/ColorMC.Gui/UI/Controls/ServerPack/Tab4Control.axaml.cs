using Avalonia.Controls;
using Avalonia.Threading;
using ColorMC.Gui.UI.Flyouts;
using ColorMC.Gui.UI.Model.ServerPack;
using ColorMC.Gui.Utils;

namespace ColorMC.Gui.UI.Controls.ServerPack;

public partial class Tab4Control : UserControl
{
    public Tab4Control()
    {
        InitializeComponent();

        DataGrid1.CellPointerPressed += DataGrid1_CellPointerPressed;
    }

    public void Opened()
    {
        DataGrid1.SetFontColor();
    }

    private void DataGrid1_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Flyout((sender as Control)!);
        }

        LongPressed.Pressed(() => Flyout((sender as Control)!));
    }

    private void Flyout(Control control)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var model = (DataContext as ServerPackModel)!;
            _ = new ServerPackFlyout1(control, model, model.FileItem);
        });
    }
}
