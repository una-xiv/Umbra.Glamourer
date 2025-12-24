using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Plugin.Glamourer;

public class GlamourerPopup : WidgetPopup
{
    protected sealed override Node Node { get; }

    private UdtDocument    Document      { get; }
    private GlamourerApi   Glamourer     { get; }
    private ITargetManager TargetManager { get; }

    private bool         _shouldUseTarget;
    private string       _filterString   = string.Empty;
    private List<Design> _allDesigns     = [];
    private List<Design> _visibleDesigns = [];

    public GlamourerPopup()
    {
        Document      = UdtLoader.Load("widget.xml");
        Glamourer     = Framework.Service<GlamourerApi>();
        TargetManager = Framework.Service<ITargetManager>();
        Node          = Document.RootNode!;

        foreach (var btn in Node.QuerySelectorAll(".apply-btn")) {
            btn.OnClick += OnApplyButtonClicked;
        }
        
        Node.QuerySelector("#btn-revert-default")!.OnClick += _ => Glamourer.RevertToDefault(GetTargetId());
        Node.QuerySelector("#btn-revert-auto")!.OnClick    += _ => Glamourer.RevertToAutomation(GetTargetId());
        Node.QuerySelector("#btn-open-glamourer")!.OnClick += _ => Glamourer.OpenGlamourerWindow();
        
        Node.QuerySelector<StringInputNode>("#filter-input")!.OnValueChanged += str => {
            _filterString = str.Trim().ToLowerInvariant();
            UpdateVisibleDesigns();
        };
    }

    protected override void OnOpen()
    {
        _allDesigns = Glamourer.GetDesignList();
        CreateDesignNodes();
    }

    private void OnApplyButtonClicked(Node node)
    {
        _shouldUseTarget = node.Id == "apply-target";

        foreach (var btn in Node.QuerySelectorAll(".apply-btn")) {
            btn.ToggleClass("selected", btn.Id == node.Id);
        }
    }

    private int GetTargetId()
    {
        return _shouldUseTarget ? TargetManager.Target?.ObjectIndex ?? 0 : 0;
    }

    private void UpdateVisibleDesigns()
    {
        List<string> visibleGuids = _allDesigns.Where(d => _filterString.Length == 0 || (
            d.Name.Contains(_filterString, System.StringComparison.InvariantCultureIgnoreCase) ||
            d.Directory.Contains(_filterString, System.StringComparison.InvariantCultureIgnoreCase)
        )).Select(d => $"di-{d.Guid:D}").ToList();

        foreach (Node node in Node.QuerySelectorAll(".design-item")) {
            node.Style.IsVisible = visibleGuids.Contains(node.Id!);
        }
    }

    private void CreateDesignNodes()
    {
        Node.QuerySelector("#design-list")!.Clear();

        foreach (var design in _allDesigns) {
            Node designNode = Document.CreateNodeFromTemplate("design-item", new() {
                ["name"]      = design.Name,
                ["directory"] = design.Directory,
            });

            designNode.Id = $"di-{design.Guid:D}";
            
            if (design.Color > 0) {
                designNode.QuerySelector(".name")!.Style.TextShadowSize  = 3.0f;
                designNode.QuerySelector(".name")!.Style.TextShadowColor = new(design.Color);
            }
            
            Node.QuerySelector("#design-list")!.AppendChild(designNode);
            
            designNode.OnClick += _ => {
                designNode.QuerySelector(".bg")!.ToggleClass("clicked", true);
                Glamourer.ApplyDesign(design.Guid, GetTargetId());
            };
        }
    }
}
