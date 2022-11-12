﻿using ColorMC.Core.Config;
using ColorMC.Core.Http.Downloader;
using ColorMC.Core.Objs;
using ColorMC.Core.Path;

namespace ColorMC.Core;

public enum CoreRunState
{
    Init, GetInfo, Start, End,
    Error,
}

public static class CoreMain
{
    public const string Version = "1.0.0";

    public static Action<string, Exception, bool> OnError;
    public static Action NewStart;

    public static Action DownloadUpdate;
    public static Action<CoreRunState> DownloadState;
    public static Action<DownloadItem> DownloadStateUpdate;

    public static Func<GameSetting, bool> GameOverwirte;

    public static Action<CoreRunState> PackState;
    public static Action<int, int> PackUpdate;

    public static Func<LoaderInfo, bool> LostModLoader;

    public static void Init(string dir)
    {
        SystemInfo.Init();
        Logs.Init(dir);
        ConfigUtils.Init(dir);
        MCPath.Init(dir);
        DownloadManager.Init();
    }
}