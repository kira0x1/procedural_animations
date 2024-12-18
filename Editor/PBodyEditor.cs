namespace Kira.Procgen.Editor;

[EditorTool]
[Title("PBodyEditor")]
[Icon("boy")]
public class PbodyEditor : EditorTool<PBody>
{
    private PBody Target { get; set; }
    private int NodeCount { get; set; } = 0;

    public override void OnEnabled()
    {
        AllowGameObjectSelection = false;

        // create a widget window. This is a window that  
        // can be dragged around in the scene view
        var window = new WidgetWindow(SceneOverlay, "Body");
        window.Layout = Layout.Column();
        window.Layout.Spacing = 16;
        window.Layout.Margin = 14;
        window.MinimumSize = new Vector2(230, 110);

        // var button = new Button("Shoot Rocket");
        // button.Pressed = () => Log.Info("Rocket Has Been Shot!");
        // window.Layout.Add(button);

        var nodesLabel = new Label($"Nodes: {NodeCount}");
        window.Layout.Add(nodesLabel);

        var btnsContainer = window.Layout.AddRow();
        btnsContainer.Spacing = 10;

        var addNodeBtn = new Button("Create Node");
        // addNodeBtn.HorizontalSizeMode = SizeMode.Ignore;
        addNodeBtn.Pressed = () => AddNode();
        btnsContainer.Add(addNodeBtn);

        var refreshBtn = new Button("Refresh");
        // refreshBtn.HorizontalSizeMode = SizeMode.Ignore;
        refreshBtn.Pressed = () => RefreshNodes();
        btnsContainer.Add(refreshBtn);

        AddOverlay(window, TextFlag.CenterBottom, 20);
    }

    private void AddNode()
    {
        Target.AddNode();
    }

    private void RefreshNodes()
    {
        Target.RefreshNodes();
    }

    public override void OnUpdate()
    {
        if (!Target.IsValid()) return;
        if (!Target.SkeletonRoot.IsValid()) return;

        var skeleton = Target.SkeletonRoot;
        var nodes = Target.Nodes;

        NodeCount = nodes.Count;

        for (var i = 0; i < nodes.Count; i++)
        {
            var n = nodes[i];
            Gizmo.Draw.LineSphere(n.Position, 4f);
        }

        using (Gizmo.Scope("cursor"))
        {
            Gizmo.Transform = new Transform(Gizmo.CurrentRay.Project(Gizmo.Camera.Position.z), new Angles(90, 0, 0).ToRotation());
            Gizmo.Draw.LineCircle(0, 20);
        }
    }

    public override void OnSelectionChanged()
    {
        AllowGameObjectSelection = false;
        Target = GetSelectedComponent<PBody>();
        if (!Target.IsValid()) return;

        if (!Target.SkeletonRoot.IsValid())
        {
            Target.SkeletonRoot = new GameObject(Target.GameObject, true, "skeleton");
        }
    }
}