using System;
using System.IO;
using ColorMC.Core.Config;
using ColorMC.Core.Helpers;
using ColorMC.Core.Utils;
using ColorMC.Gui.Manager;
using ColorMC.Gui.Objs;

using Newtonsoft.Json;

namespace ColorMC.Gui.Utils;

/// <summary>
/// GUI配置文件
/// </summary>
public static class GuiConfigUtils
{
    public static GuiConfigObj Config { get; set; }

    private static string s_local;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        s_local = Path.Combine(ColorMCGui.RunDir, GuiNames.NameGuiConfigFile);

        Load(s_local);
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="local">路径</param>
    /// <param name="quit">加载失败是否退出</param>
    /// <returns>是否加载成功</returns>
    public static bool Load(string local, bool quit = false)
    {
        if (File.Exists(local))
        {
            try
            {
                Config = JsonConvert.DeserializeObject<GuiConfigObj>(File.ReadAllText(local))!;
            }
            catch (Exception e)
            {
                Logs.Error(App.Lang("Config.Error2"), e);
            }

            if (Config == null)
            {
                if (quit)
                {
                    return false;
                }

                Config = MakeDefaultConfig();

                SaveNow();
                return true;
            }

            bool save = false;

            if (Config.ServerCustom == null)
            {
                Config.ServerCustom = MakeServerCustomConfig();
                save = true;
            }
            if (Config.ServerCustom.LockLogins == null)
            {
                Config.ServerCustom.LockLogins = [];
                save = true;
            }
            if (Config.Render == null
                || Config.Render.Windows == null
                || Config.Render.X11 == null)
            {
                Config.Render = MakeRenderConfig();
                save = true;
            }
            if (Config.Live2D == null)
            {
                Config.Live2D = MakeLive2DConfig();
                save = true;
            }
            if (Config.Style == null)
            {
                Config.Style = MakeStyleSettingConfig();
                save = true;
            }
            if (Config.Head == null)
            {
                Config.Head = MakeHeadSettingConfig();
                save = true;
            }
            if (Config.Input == null)
            {
                Config.Input = new();
                save = true;
            }
            if (Config.LogColor == null)
            {
                Config.LogColor = MakeLogColorConfig();
                save = true;
            }
            if (Config.Card == null)
            {
                Config.Card = MakeCardConfig();
                save = true;
            }
            if (Config.LaunchCheck == null)
            {
                Config.LaunchCheck = MakeLaunchCheckConfig();
                save = true;
            }

            if (save)
            {
                Logs.Info(LanguageHelper.Get("Core.Config.Info2"));
                SaveNow();
            }
        }
        else
        {
            Config = MakeDefaultConfig();

            SaveNow();
        }

        return true;
    }

    /// <summary>
    /// 立即保存
    /// </summary>
    public static void SaveNow()
    {
        Logs.Info(LanguageHelper.Get("Core.Config.Info2"));
        File.WriteAllText(s_local, JsonConvert.SerializeObject(Config));
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    public static void Save()
    {
        ConfigSave.AddItem(new()
        {
            Name = "gui.json",
            File = s_local,
            Obj = Config
        });
    }

    public static LaunchCheckSetting MakeLaunchCheckConfig()
    {
        return new()
        {
            CheckLoader = true,
            CheckMemory = true,
            CheckUser = true
        };
    }

    public static CardSetting MakeCardConfig()
    {
        return new CardSetting()
        {
            Last = true,
            News = true,
            Online = true
        };
    }

    public static HeadSetting MakeHeadSettingConfig()
    {
        return new()
        {
            Type = HeadType.Head3D_B,
            X = 15,
            Y = 65
        };
    }

    public static StyleSetting MakeStyleSettingConfig()
    {
        return new()
        {
            EnableAm = true,
            AmTime = 500,
            AmFade = true
        };
    }

    public static Live2DSetting MakeLive2DConfig()
    {
        return new()
        {
            Width = 30,
            Height = 50
        };
    }

    public static RenderSetting MakeRenderConfig()
    {
        return new()
        {
            Windows = new()
            {
                ShouldRenderOnUIThread = null
            },
            X11 = new()
            {
                UseDBusMenu = null,
                UseDBusFilePicker = null,
                OverlayPopups = null
            }
        };
    }

    public static GuiConfigObj MakeDefaultConfig()
    {
        return new()
        {
            ColorMain = ThemeManager.MainColorStr,
            RGBS = 100,
            RGBV = 100,
            ServerCustom = MakeServerCustomConfig(),
            FontDefault = true,
            Render = MakeRenderConfig(),
            BackLimitValue = 50,
            EnableBG = false,
            BackImage = "",
            Live2D = MakeLive2DConfig(),
            Style = MakeStyleSettingConfig(),
            Head = MakeHeadSettingConfig(),
            LogColor = MakeLogColorConfig(),
            Input = new(),
            Card = MakeCardConfig(),
            LaunchCheck = MakeLaunchCheckConfig(),
            CheckUpdate = true
        };
    }

    public static LogColorSetting MakeLogColorConfig()
    {
        return new()
        {
            Warn = "#8B8B00",
            Error = "#FF0000",
            Debug = "#BEBEBE"
        };
    }

    public static ServerCustomSetting MakeServerCustomConfig()
    {
        return new()
        {
            MotdColor = "White",
            MotdBackColor = "Black",
            Volume = 30,
            LockLogins = []
        };
    }
}