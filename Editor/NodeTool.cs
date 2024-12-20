namespace Sandbox;

public class NodeTool : EditorTool<ProceduralBlobs>
{
    private ProceduralBlobs Target { get; set; }

    public override void OnEnabled()
    {
        // CreateWindow();
    }

    public override void OnUpdate()
    {
        if (!Target.IsValid()) return;
    }

    private void CreateWindow()
    {
        var window = new WidgetWindow(SceneOverlay);
        window.FixedWidth = 400f;
        window.Layout = Layout.Column();
        window.Layout.Margin = 22;

        // Create a button for us to press
        var btn = CreateBtn("Add Bone");
        btn.Pressed = () => AddBone();

        var btn2 = CreateBtn("Create Skeleton");
        window.Layout.Spacing = 10;

        // Add the button to the window's layout
        window.Layout.Add(btn);
        window.Layout.Add(btn2);

        // Calling this function means that when your tool is deleted,
        // ui will get properly deleted too. If you don't call this and
        // you don't delete your UI in OnDisabled, it'll hang around forever.
        AddOverlay(window, TextFlag.CenterBottom, 10);
    }

    private Button CreateBtn(string content)
    {
        var btn = new Button(content);
        btn.HorizontalSizeMode = SizeMode.Ignore;
        return btn;
    }

    private void AddBone()
    {
        Log.Info("adding bone");
    }

    public override void OnSelectionChanged()
    {
        Target = GetSelectedComponent<ProceduralBlobs>();
    }
}