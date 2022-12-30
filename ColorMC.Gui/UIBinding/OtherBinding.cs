﻿using ColorMC.Core.Http.Downloader;
using ColorMC.Core.LaunchPath;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorMC.Gui.UIBinding;

public static class OtherBinding
{
    public static bool LoadAuthDatabase(string dir)
    {
        return AuthDatabase.LoadData(dir);
    }

    public static bool LoadConfig(string dir)
    {
        return ConfigUtils.Load(dir, true);
    }

    public static List<GameSettingObj> GetGames()
    {
        return InstancesPath.Games;
    }

    public static List<string> GetGameVersion(bool? type1, bool? type2, bool? type3)
    {
        var list = new List<string>();
        if (VersionPath.Versions == null)
            return list;

        foreach (var item in VersionPath.Versions.versions)
        {
            if (item.type == "release")
            {
                if (type1 == true)
                {
                    list.Add(item.id);
                }
            }
            else if (item.type == "snapshot")
            {
                if (type2 == true)
                {
                    list.Add(item.id);
                }
            }
            else
            {
                if (type3 == true)
                {
                    list.Add(item.id);
                }
            }
        }

        return list;
    }

    public static async Task<bool> AddGame(string name, string version, 
        Loaders loaders, string? loaderversion = null)
    {
        var game = new GameSettingObj()
        {
            Name = name,
            Version = version,
            Loader = loaders,
            LoaderVersion = loaderversion
        };

        game = await InstancesPath.CreateVersion(game);

        return game != null;
    }

    public static Task<bool> AddPack(string dir, PackType type)
    {
        return InstancesPath.LoadFromZip(dir, type);
    }
}
