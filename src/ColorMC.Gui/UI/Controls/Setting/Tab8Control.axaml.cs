using Avalonia.Controls;
using ColorMC.Gui.UI.Flyouts;
using ColorMC.Gui.UI.Model.Setting;

namespace ColorMC.Gui.UI.Controls.Setting;

public partial class Tab8Control : UserControl
{
    public Tab8Control()
    {
        InitializeComponent();

        DataGrid1.CellPointerPressed += DataGrid1_CellPointerPressed;
        DataGrid2.CellPointerPressed += DataGrid2_CellPointerPressed;
    }

    private void DataGrid1_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Flyout((sender as Control)!);
        }
        else
        {
            LongPressed.Pressed(() => Flyout((sender as Control)!));
        }
    }
    private void DataGrid2_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Flyout1((sender as Control)!);
        }
        else
        {
            LongPressed.Pressed(() => Flyout1((sender as Control)!));
        }
    }

    private void Flyout(Control control)
    {
        if (DataContext is not SettingModel model)
        {
            return;
        }
        _ = new SettingFlyout2(control, model, model.InputItem);
    }

    private void Flyout1(Control control)
    {
        if (DataContext is not SettingModel model)
        {
            return;
        }
        _ = new SettingFlyout2(control, model, model.InputAxisItem);
    }
}
