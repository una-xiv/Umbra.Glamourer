using Dalamud.Plugin.Services;
using Glamourer.Api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Glamourer.Api.IpcSubscribers;
using Umbra.Common;

namespace Umbra.Plugin.Glamourer;

[Service]
public sealed class GlamourerApi : IDisposable
{
    #region Glamourer API

    private ApiVersion            _getApiVersion         = new(Framework.DalamudPlugin);
    private ApplyDesign           _applyDesign           = new(Framework.DalamudPlugin);
    private RevertState           _revertState           = new(Framework.DalamudPlugin);
    private RevertToAutomation    _revertToAutomation    = new(Framework.DalamudPlugin);
    private GetDesignList         _getDesignList         = new(Framework.DalamudPlugin);
    private GetDesignListExtended _getDesignListExtended = new(Framework.DalamudPlugin);

    #endregion

    public List<Design> GetDesignList()
    {
        return IsExtendedDesignListAvailable()
            ? GetDesignListExtended()
            : GetDesignListSimple();
    }

    public void OpenGlamourerWindow()
    {
        Framework.Service<ICommandManager>().ProcessCommand("/glamourer");
    }
    
    public GlamourerApiEc ApplyDesign(Guid designGuid, int objectId = 0)
    {
        return _applyDesign.Invoke(designGuid, objectId);
    }

    public GlamourerApiEc RevertToDefault(int objectId = 0)
    {
        return _revertState.Invoke(objectId);
    }

    public GlamourerApiEc RevertToAutomation(int objectId = 0)
    {
        return _revertToAutomation.Invoke(objectId);
    }

    public void Dispose()
    {
        _getApiVersion         = null!;
        _applyDesign           = null!;
        _revertState           = null!;
        _revertToAutomation    = null!;
        _getDesignList         = null!;
        _getDesignListExtended = null!;
    }

    private bool IsExtendedDesignListAvailable()
    {
        return _getApiVersion.Invoke() is { Major: >= 2, Minor: >= 5 };
    }

    private List<Design> GetDesignListSimple()
    {
        List<Design> designs = [];

        foreach (var (guid, name) in _getDesignList.Invoke()) {
            designs.Add(new Design {
                Guid      = guid,
                Name      = name,
                Directory = "",
                Color     = 0,
                ShowInQdb = false,
            });
        }

        return designs.OrderBy(design => design.Name).ToList();
    }

    private List<Design> GetDesignListExtended()
    {
        List<Design> designs = [];

        foreach (var (guid, data) in _getDesignListExtended.Invoke()) {
            designs.Add(new Design {
                Guid      = guid,
                Name      = data.DisplayName,
                Directory = data.FullPath,
                Color     = data.DisplayColor,
                ShowInQdb = data.ShownInQdb,
            });
        }

        return designs.OrderBy(design => design.Directory).ThenBy(design => design.Name).ToList();
    }
}
