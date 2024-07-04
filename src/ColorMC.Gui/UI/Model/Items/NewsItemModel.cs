﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ColorMC.Core.Objs.MinecraftAPI;
using ColorMC.Core.Utils;
using ColorMC.Gui.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ColorMC.Gui.UI.Model.Items;

public partial class NewsItemModel(MinecraftNewObj.ArticleObj item) : ObservableObject
{
    public Task<Bitmap?> Image => GetImage();

    private Bitmap? _img;

    public string Title => item.DefaultTile.Title;
    public string SubTitle => item.DefaultTile.SubHeader;
    public string Category => item.PrimaryCategory;

    private async Task<Bitmap?> GetImage()
    {
        if (_img != null)
        {
            return _img;
        }
        if (item.DefaultTile.Image.ImageURL == null)
        {
            return null;
        }
        try
        {
            await Task.Run(() =>
            {
                _img = ImageUtils.Load(item.DefaultTile.Image.ImageURL).Result;
            });
            return _img;
        }
        catch (Exception e)
        {
            Logs.Error(App.Lang("AddModPackWindow.Error5"), e);
        }

        return null;
    }
}
