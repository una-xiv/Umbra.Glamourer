using Dalamud.Interface;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Widgets;

namespace Umbra.Plugin.Glamourer;

[InteropToolbarWidget(
    "GlamourerRevertToGameButtonWidget",
    "Glamourer: Revert to Game",
    "A single icon-only button to revert a design of your character or that of a target back to the game default.",
    "Glamourer"
)]
public sealed class GlamourerRevertToGameButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon | StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Undo;

    private GlamourerApi   Api       { get; } = Framework.Service<GlamourerApi>();
    private ITargetManager TargetMgr { get; } = Framework.Service<ITargetManager>();

    protected override void OnLoad()
    {
        Node.OnClick      += _ => Api.RevertToDefault();
        Node.OnRightClick += _ => Api.RevertToDefault(TargetMgr.Target?.ObjectIndex ?? 0);
        Node.Tooltip      =  "Left click to revert your glamourer design to the game default.\nRight click to revert the target's glamourer design to the game default.";
    }
}
