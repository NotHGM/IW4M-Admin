using System;
using Data.Models;

namespace SharedLibraryCore.Dtos;

public class ClientInfoResult
{
    public int ClientId { get; set; }
    public string Name { get; set; }
    public string Level { get; set; }
    public long NetworkId { get; set; }
    public string GameName { get; set; }
    public string? Tag { get; set; }
    public DateTime FirstConnection { get; set; }
    public DateTime LastConnection { get; set; }
    public int TotalConnectionTime { get; set; }
    public int Connections { get; set; }
}
