namespace Sandbox;

[EditorTool]
[Title("Node")]
[Icon("lock")]
public class NodeToolWidget : EditorTool
{
    private ProceduralBlobs Body { get; set; }

    public override void OnEnabled()
    {
        CreateWindow();
    }

    private void CreateWindow()
    {
        // create a widget window. This is a window that  
        // can be dragged around in the scene view
        var window = new WidgetWindow(SceneOverlay);
        window.Layout = Layout.Column();
        window.Layout.Margin = 16;

        bool foundBody = false;
        var selection = Selection.FirstOrDefault();

        if (selection != null && selection.GetType() == typeof(GameObject))
        {
            GameObject obj = (GameObject)selection;
            var body = obj.GetComponent<ProceduralBlobs>();
            if (body.IsValid())
            {
                Body = body;
                foundBody = true;
            }
        }

        if (!foundBody)
        {
            var createBtn = new Button("Create Body");
            createBtn.Pressed = () => Log.Info("Create Body");
            window.Layout.Add(createBtn);
        }

        // Create a button for us to press
        // var button = new Button("Shoot Rocket");
        // button.Pressed = () => Log.Info("Rocket Has Been Shot!");

        // Add the button to the window's layout
        // window.Layout.Add(button);

        // Calling this function means that when your tool is deleted,
        // ui will get properly deleted too. If you don't call this and
        // you don't delete your UI in OnDisabled, it'll hang around forever.
        AddOverlay(window, TextFlag.RightTop, 10);
    }

    public void UpdateWindow()
    {
    }

    public override void OnSelectionChanged()
    {
        UpdateWindow();
    }
}