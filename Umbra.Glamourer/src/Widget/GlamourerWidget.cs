using System.Collections.Generic;
using Umbra.Common;
using Umbra.Widgets;

namespace Umbra.Plugin.Glamourer;

[InteropToolbarWidget(
    "Glamourer",
    "Glamourer",
    "Quickly swap between stored glamourer designs, straight from your toolbar!",
    "Glamourer"
)]
public sealed class GlamourerWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override GlamourerPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    private GlamourerApi Glamourer { get; } = Framework.Service<GlamourerApi>();

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new StringWidgetConfigVariable(
                "CustomLabel",
                I18N.Translate("Widget.CustomButton.Config.Label.Name"),
                I18N.Translate("Widget.CustomButton.Config.Label.Description"),
                Info.Name,
                100
            ),
        ];
    }

    protected override void OnLoad()
    {
    }

    protected override void OnDraw()
    {
        SetText(GetConfigValue<string>("CustomLabel"));
    }

    protected override void OnUnload()
    {
    }

    // private void RefreshDesignsList()
    // {
    //     foreach (var btn in DesignButtons) DesignsGroup.Remove(btn, true);
    //     DesignButtons.Clear();
    //
    //     var prefix = GetConfigValue<string>("DesignPrefixFilter");
    //
    //     foreach (var design in Glamourer.GetDesignList())
    //     {
    //         if (prefix.Length > 0 && !design.Name.StartsWith(prefix)) continue;
    //
    //         MenuPopup.Button button = new(design.Name)
    //         {
    //             OnClick           = () => Glamourer.ApplyDesign(design.Guid, GetTargetId()),
    //             ClosePopupOnClick = false,
    //         };
    //
    //         DesignButtons.Add(button);
    //         DesignsGroup.Add(button);
    //     }
    // }
    //
    // private int GetTargetId()
    // {
    //     return _applyToTarget ? Framework.Service<ITargetManager>().Target?.ObjectIndex ?? 0 : 0;
    // }
    //
    // private void SetApplyTargetSelf()
    // {
    //     ApplyToGroup.Get<MenuPopup.Button>("applyToSelf").Icon   = FontAwesomeIcon.CheckCircle;
    //     ApplyToGroup.Get<MenuPopup.Button>("applyToTarget").Icon = null;
    //
    //     _applyToTarget = false;
    // }
    //
    // private void SetApplyTargetTarget()
    // {
    //     ApplyToGroup.Get<MenuPopup.Button>("applyToSelf").Icon   = null;
    //     ApplyToGroup.Get<MenuPopup.Button>("applyToTarget").Icon = FontAwesomeIcon.CheckCircle;
    //
    //     _applyToTarget = true;
    // }
}
