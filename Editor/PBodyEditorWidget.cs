namespace Kira.Procgen.Editor;

using System;

[EditorTool]
[Title("PBodyEditor")]
[Icon("boy")]
public class PBodyEditorWidget : EditorTool
{
    private PBody Target { get; set; }
    private int NodeCount { get; set; } = 0;

    private Layout CreateLayout { get; set; }
    private bool IsCreated { get; set; }

    private WidgetWindow window;
    private WidgetWindow editWindow;
    //todo brushWindow

    private Label nodesLabel;

    public override void OnEnabled()
    {
        CreateWindow();
    }

    public override void OnUpdate()
    {
        if (!IsCreated || !Target.IsValid())
        {
            return;
        }

        for (var i = 0; i < Target.Nodes.Count; i++)
        {
            PNode node = Target.Nodes[i];

            // Log.Info(i);
            // Gizmo.Draw.SolidSphere(node.Position, 1f);
            // Gizmo.Control.Arrow("ar", Vector3.Zero, out float distance);

            using (Gizmo.Scope($"node{i}"))
            {
                Gizmo.Draw.Color = Gizmo.HasSelected ? Color.Cyan : Color.White;
                Gizmo.Transform = new Transform(node.Position, Rotation.Identity);

                Gizmo.Control.Position("pos", node.Position, out Vector3 newPos, squareSize: 2.5f);

                Gizmo.Control.Sphere("rd", node.DesiredDistance, out float radius, Color.Magenta);
                node.DesiredDistance = radius.Clamp(5f, 50f);

                var nodePos = node.Position;
                nodePos.z -= 3;
                Gizmo.Draw.ScreenText($"{i}", Camera.ToScreen(nodePos), "Poppins", 22, TextFlag.CenterTop);

                node.SetPosition(newPos);

                // Gizmo.Control.BoundingBox("drag", BBox.FromPositionAndSize(node.GameObject.WorldTransform.NormalToLocal(Vector3.Zero), node.DesiredDistance), out BBox box);
            }
        }
    }

    public override void OnSelectionChanged()
    {
        var goSelected = (GameObject)Selection.FirstOrDefault();
        Target = GetSelectedComponent<PBody>();

        if (goSelected.IsValid() && !Target.IsValid())
        {
            Target = goSelected.GetComponentInParent<PBody>();
        }

        if (!Target.IsValid())
        {
            if (IsCreated)
            {
                SwapToCreate();
            }

            UpdateNodeLabel();
            return;
        }

        if (!Target.SkeletonRoot.IsValid())
        {
            Target.SkeletonRoot = new GameObject(Target.GameObject, true, "skeleton");
        }

        UpdateNodeLabel();
        SwapToEdit();
    }

    private void CreateWindow()
    {
        // create a widget window. This is a window that  
        // can be dragged around in the scene view
        window = new WidgetWindow(SceneOverlay, "Body");
        window.Layout = Layout.Column();
        window.Layout.Spacing = 16;
        window.Layout.Margin = 14;
        window.MinimumSize = new Vector2(230, 110);

        // var button = new Button("Shoot Rocket");
        // button.Pressed = () => Log.Info("Rocket Has Been Shot!");
        // window.Layout.Add(button);

        var curSelection = GetSelectedComponent<PBody>();
        if (curSelection.IsValid())
        {
            Target = curSelection;
            NodeCount = Target.Nodes.Count;
        }

        nodesLabel = new Label($"Nodes: {NodeCount}");
        window.Layout.Add(nodesLabel);

        GridLayout btnsContainer = window.Layout.AddLayout(Layout.Grid());
        btnsContainer.Spacing = 10;

        var addNodeBtn = new Button("Create Node");
        // addNodeBtn.HorizontalSizeMode = SizeMode.Ignore;
        addNodeBtn.Pressed = () => AddNode();
        btnsContainer.AddCell(0, 0, addNodeBtn);

        var refreshBtn = new Button("Refresh");
        // refreshBtn.HorizontalSizeMode = SizeMode.Ignore;
        refreshBtn.Pressed = () => RefreshNodes();
        btnsContainer.AddCell(1, 0, refreshBtn);


        var clearBtn = new Button("Clear");
        clearBtn.Pressed = () => ClearNodes();
        btnsContainer.AddCell(0, 1, clearBtn);

        CreateLayout = btnsContainer;

        AddOverlay(window, TextFlag.CenterBottom, 20);
        CreateEditWindow();

        IsCreated = true;
    }

    private void CreateEditWindow()
    {
        editWindow = new WidgetWindow(SceneOverlay, "Edit");
        editWindow.Layout = Layout.Column();
        editWindow.Layout.Spacing = 16;
        editWindow.Layout.Margin = 14;
        editWindow.MinimumSize = new Vector2(230, 110);

        AddOverlay(editWindow, TextFlag.CenterBottom, 20);
        var pos = editWindow.Position;
        pos.x -= 240f;
        editWindow.Position = pos;
    }

    private void UpdateNodeLabel()
    {
        NodeCount = Target.IsValid() ? Target.Nodes.Count : 0;

        if (IsCreated)
        {
            nodesLabel.Text = $"Nodes: {NodeCount}";
            window.Update();
        }
    }

    private void AddChildNode()
    {
    }

    private void AddNode()
    {
        if (Target.IsValid())
        {
            Target.AddNode();
            UpdateNodeLabel();
        }
    }

    private void ClearNodes()
    {
        if (Target.IsValid())
        {
            Target.ClearNodes();
        }

        UpdateNodeLabel();
    }

    private void RefreshNodes()
    {
        if (Target.IsValid())
        {
            Target.RefreshNodes();
        }

        UpdateNodeLabel();
    }

    private void SwapToCreate()
    {
        CreateLayout.Enabled = true;
        editWindow.Visible = false;
    }

    private void SwapToEdit()
    {
        editWindow.Visible = true;
    }
}