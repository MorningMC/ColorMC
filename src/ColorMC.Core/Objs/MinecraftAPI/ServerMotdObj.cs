﻿using ColorMC.Core.Net.Motd;
using Newtonsoft.Json;

namespace ColorMC.Core.Objs.MinecraftAPI;

public enum StateType
{
    Ok,
    NoData,
    ConnectFail,
    Error
}

public class Chat
{
    [JsonProperty("text")]
    public string Text { get; set; } = "";

    [JsonProperty("bold")]
    public bool Bold { get; set; }

    [JsonProperty("italic")]
    public bool Italic { get; set; }

    [JsonProperty("underlined")]
    public bool Underlined { get; set; }

    [JsonProperty("strikethrough")]
    public bool Strikethrough { get; set; }

    [JsonProperty("obfuscated")]
    public bool Obfuscated { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("extra")]
    public List<Chat> Extra { get; set; }

    public override string ToString()
    {
        return ServerMotd.CleanFormat(this.ToPlainTextString());
    }
}

public record ServerVersionInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("protocol")]
    public int Protocol { get; set; }
}

public record ServerPlayerInfo
{
    [JsonProperty("max")]
    public int Max { get; set; }

    [JsonProperty("online")]
    public int Online { get; set; }

    [JsonProperty("sample")]
    public List<Player> Sample { get; set; }
}

public record Player
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public record ModInfo
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("modList")]
    public List<Mod> ModList { get; set; }
}

public record Mod
{
    [JsonProperty("modid")]
    public string ModId { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }
}

public class ServerMotdObj
{
    /// <summary>
    /// Server's address, support srv.
    /// </summary>
    [JsonIgnore]
    public string ServerAddress { get; set; }

    /// <summary>
    /// Server's runing port
    /// </summary>
    [JsonIgnore]
    public ushort ServerPort { get; set; }

    /// <summary>
    /// The server version info such name or protocol
    /// </summary>
    [JsonProperty("version")]
    public ServerVersionInfo Version { get; set; }

    /// <summary>
    /// The server player info such max or current player count and sample.
    /// </summary>
    [JsonProperty("players")]
    public ServerPlayerInfo Players { get; set; }

    /// <summary>
    /// Server's description (aka motd)
    /// </summary>
    [JsonProperty("description")]
    [JsonConverter(typeof(ServerDescriptionJsonConverter))]
    public Chat Description { get; set; }

    /// <summary>
    /// server's favicon. is a png image that is base64 encoded
    /// </summary>
    [JsonProperty("favicon")]
    public string Favicon { get; set; }

    /// <summary>
    /// Server's mod info including mod type and mod list (if is avaliable)
    /// </summary>
    [JsonProperty("modinfo")]
    public ModInfo ModInfo { get; set; }

    [JsonIgnore]
    public byte[] FaviconByteArray
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Favicon))
            {
                return Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7");
            }
            else
            {
                return Convert.FromBase64String(Favicon.Replace("data:image/png;base64,", ""));
            }
        }
    }

    /// <summary>
    /// The ping delay time.(ms)
    /// </summary>
    [JsonIgnore]
    public long Ping { get; set; }

    /// <summary>
    /// The handshake state
    /// </summary>
    [JsonIgnore]
    public StateType State { get; set; }

    /// <summary>
    /// The handshake message
    /// </summary>
    [JsonIgnore]
    public string Message { get; set; }

    [JsonIgnore]
    public bool AcceptTextures { get; set; }

    public ServerMotdObj()
    {

    }

    public ServerMotdObj(string ip, ushort port)
    {
        ServerAddress = ip;
        ServerPort = port;
    }
}
