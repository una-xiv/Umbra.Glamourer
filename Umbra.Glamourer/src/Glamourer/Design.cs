using System;

namespace Umbra.Plugin.Glamourer;

public readonly struct Design
{
    public Guid   Guid      { get; init; }
    public string Name      { get; init; }
    public string Directory { get; init; }
    public uint   Color     { get; init; }
    public bool   ShowInQdb { get; init; }
}
