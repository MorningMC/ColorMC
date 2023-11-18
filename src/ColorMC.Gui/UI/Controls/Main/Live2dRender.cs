﻿using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using ColorMC.Core.Utils;
using ColorMC.Gui.UI.Model.Main;
using ColorMC.Gui.Utils;
using Live2DCSharpSDK.App;
using Live2DCSharpSDK.Framework.Motion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ColorMC.Gui.UI.Controls.Main;

public class Live2dRender : OpenGlControlBase
{
    private LAppDelegate _lapp;
    private LAppModel _model;

    private DateTime _time;
    private bool _change;
    private bool _delete;
    private bool _init = false;
    private bool _first = false;

    public bool HaveModel
    {
        get
        {
            if (_lapp == null)
            {
                return false;
            }
            return _lapp.Live2dManager.GetModelNum() != 0;
        }
    }

    public Live2dRender()
    {
        DataContextChanged += Live2dRender_DataContextChanged;
    }

    private void Live2dRender_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainModel model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ModelChange")
        {
            _change = true;
            Dispatcher.UIThread.Post(RequestNextFrameRendering);
        }
        else if (e.PropertyName == "ModelDelete")
        {
            _delete = true;
        }
    }

    private void ChangeModel()
    {
        _lapp.Live2dManager.ReleaseAllModel();
        var model = GuiConfigUtils.Config.Live2D.Model;
        if (string.IsNullOrWhiteSpace(model))
        {
            return;
        }
        if (!File.Exists(model))
        {
            (DataContext as MainModel)!.Model.Show(App.Lang("MainWindow.Live2d.Error1"));
            return;
        }
        if (!GuiConfigUtils.Config.Live2D.Enable)
        {
            return;
        }
        var info = new FileInfo(model);
        try
        {
            _model = _lapp.Live2dManager.LoadModel(info.DirectoryName! + "/", info.Name.Replace(".model3.json", ""));
        }
        catch (Exception e)
        {
            string temp = App.Lang("MainWindow.Live2d.Error2");
            Logs.Error(temp, e);
            (DataContext as MainModel)!.Model.Show(temp);
        }
    }

    private static void CheckError(GlInterface gl)
    {
        int err;
        while ((err = gl.GetError()) != GlConsts.GL_NO_ERROR)
            Console.WriteLine(err);
    }

    protected override unsafe void OnOpenGlInit(GlInterface gl)
    {
        if (_first)
            return;
        _first = true;
        if (_init)
            return;
        CheckError(gl);

        try
        {
            _lapp = new(new AvaloniaApi(this, gl), Logs.Info);
            _change = true;
            CheckError(gl);
            _init = true;
        }
        catch (Exception e)
        {
            (DataContext as MainModel)!.ChangeModelDone();
            Logs.Error(App.Lang("Gui.Error31"), e);
        }
    }

    protected override void OnOpenGlDeinit(GlInterface GL)
    {
        _lapp?.Dispose();
        _lapp = null!;
        _init = false;
        _first = false;
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (!_init)
            return;
        var model = (DataContext as MainModel)!;
        if (_change)
        {
            _change = false;
            ChangeModel();
            model.ChangeModelDone();
            model.ShowMessage(App.Lang("Live2D.Text1"));
        }
        if (_delete)
        {
            _delete = false;
            _lapp.Live2dManager.ReleaseAllModel();
            model.ChangeModelDone();
        }

        int x = (int)Bounds.Width;
        int y = (int)Bounds.Height;

        if (VisualRoot is TopLevel window)
        {
            var screen = window.RenderScaling;
            x = (int)(Bounds.Width * screen);
            y = (int)(Bounds.Height * screen);
        }

        gl.Viewport(0, 0, x, y);
        var now = DateTime.Now;
        float span = 0;
        if (_time.Ticks == 0)
        {
            _time = now;
        }
        else
        {
            span = (float)(now - _time).TotalSeconds;
            _time = now;
        }
        _lapp.Run(span);
        CheckError(gl);
    }

    public void Pressed()
    {
        _lapp.OnMouseCallBack(true);
    }

    public void Release()
    {
        _lapp.OnMouseCallBack(false);
    }

    public void Moved(float x, float y)
    {
        _lapp.OnMouseCallBack(x, y);
    }

    public List<string> GetMotions()
    {
        return _model.Motions;
    }

    public List<string> GetExpressions()
    {
        return _model.Expressions;
    }

    public void PlayMotion(string name)
    {
        _model.StartMotion(name, MotionPriority.PriorityForce);
    }

    public void PlayExpression(string name)
    {
        _model.SetExpression(name);
    }
}
