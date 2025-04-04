﻿using Avalonia.Controls;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Model.Dialog;
using ColorMC.Gui.UI.Model.Items;

namespace ColorMC.Gui.UI.Flyouts;

/// <summary>
/// 配置文件编辑页面
/// Nbt标签列表右键菜单
/// </summary>
public class ConfigFlyout2
{
    public ConfigFlyout2(Control con, NbtDialogEditModel model, NbtDataItemModel item)
    {
        _ = new FlyoutsControl(
        [
            new FlyoutMenuObj(App.Lang("Button.Delete"), true, () =>
            {
                model.DeleteItem(item);
            }),
        ], con);
    }
}
