using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit.Indentation.CSharp;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Gui.UI.Model;
using ColorMC.Gui.UI.Model.GameConfigEdit;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.Utils;
using System.ComponentModel;
using System.IO;

namespace ColorMC.Gui.UI.Controls.ConfigEdit;

public partial class GameConfigEditControl : UserControl, IUserControl
{
    private readonly WorldObj _world;
    private readonly GameSettingObj _obj;

    public IBaseWindow Window => App.FindRoot(VisualRoot);

    public string Title
    {
        get
        {
            var model = (DataContext as GameConfigEditModel)!;
            if (model.World == null)
            {
                return string.Format(App.GetLanguage("ConfigEditWindow.Title"),
                    model.Obj?.Name);
            }
            else
            {
                return string.Format(App.GetLanguage("ConfigEditWindow.Title1"),
                    model.Obj?.Name, model.World?.LevelName);
            }
        }
    }

    public GameConfigEditControl()
    {
        InitializeComponent();

        NbtViewer.PointerPressed += NbtViewer_PointerPressed;
        NbtViewer.KeyDown += NbtViewer_KeyDown;

        TextEditor1.KeyDown += NbtViewer_KeyDown;
        TextEditor1.TextArea.Background = Brushes.Transparent;
        TextEditor1.Options.ShowBoxForControlCharacters = true;
        TextEditor1.TextArea.IndentationStrategy =
            new CSharpIndentationStrategy(TextEditor1.Options);

        DataGrid1.CellEditEnded += DataGrid1_CellEditEnded;
        DataGrid1.CellPointerPressed += DataGrid1_CellPointerPressed;
    }

    public GameConfigEditControl(WorldObj world) : this()
    {
        _world = world;
    }

    public GameConfigEditControl(GameSettingObj obj) : this()
    {
        _obj = obj;
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "TurnTo")
        {
            NbtViewer.Scroll!.Offset = new(0, (DataContext as GameConfigEditModel)!.TurnTo * 25);
        }
    }

    private void DataGrid1_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Dispatcher.UIThread.Post(() =>
            {
                (DataContext as GameConfigEditModel)!.Flyout(this);
            });
        }
    }

    private void DataGrid1_CellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            (DataContext as GameConfigEditModel)!.DataEdit();
        }
    }

    private void NbtViewer_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers == KeyModifiers.Control)
        {
            (DataContext as GameConfigEditModel)!.Save();
        }
    }

    private void NbtViewer_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Dispatcher.UIThread.Post(() =>
            {
                (DataContext as GameConfigEditModel)!.Pressed(this);
            });
        }
    }

    public void Opened()
    {
        Window.SetTitle(Title);

        DataGrid1.SetFontColor();

        var model = (DataContext as GameConfigEditModel)!;
        model.Load();

        var icon = model.Obj.GetIconFile();
        if (File.Exists(icon))
        {
            model.Model.SetIcon(new(icon));
        }
    }

    public void Closed()
    {
        string key;
        var model = (DataContext as GameConfigEditModel)!;
        if (model.World != null)
        {
            key = model.Obj.UUID + ":" + model.World.LevelName;
        }
        else
        {
            key = model.Obj.UUID;
        }
        App.ConfigEditWindows.Remove(key);
    }

    public void SetBaseModel(BaseModel model)
    {
        var amodel = new GameConfigEditModel(model, _obj, _world);
        amodel.PropertyChanged += Model_PropertyChanged;
        DataContext = amodel;
    }
}
