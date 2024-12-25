namespace Kira.Procgen.Editor;

using System;

[EditorTool]
[Title("PBodyEditor")]
[Icon("boy")]
public partial class PBodyEditorWidget : EditorTool
{
    private PBody Target { get; set; }
    private int NodeCount { get; set; } = 0;

    private Layout CreateLayout { get; set; }
    private bool HasCreatedWindow { get; set; }
    private bool IsNodeSelected { get; set; }

    private WidgetWindow window;
    private WidgetWindow editWindow;
    private Checkbox drawHandlesCheckbox;
    private Checkbox hideOtherGizmosCheckbox;

    //todo brushWindow

    private PNode nodeSelected;
    private Label nodesLabel;

    private static PBodyEditorWidget Instance { get; set; }

    public override void OnEnabled()
    {
        Instance = this;
        CreateWindow(true);
    }

    private void CreateWindow(bool firstTime = false)
    {
        if (firstTime)
        {
            window = new WidgetWindow(SceneOverlay, "Body");
            window.Layout = Layout.Column();
            window.Layout.Spacing = 16;
            window.Layout.Margin = 14;
            window.MinimumSize = new Vector2(360, 110);
        }
        else
        {
            window.Layout.Clear(true);
        }

        var curSelection = GetSelectedComponent<PBody>();
        if (curSelection.IsValid())
        {
            Target = curSelection;
            NodeCount = Target.Descendants.Count;
        }

        nodesLabel = new Label($"Nodes: {NodeCount}");

        window.Layout.Add(nodesLabel);

        // Display Selection
        var boneSelection = CreateBoneSelectionWidget();
        window.Layout.Add(boneSelection);

        #region Checkboxes

        // -- CHECKBOXES --
        var checkboxContainer = window.Layout.AddLayout(Layout.Grid());
        checkboxContainer.Spacing = 8;

        // SEPERATOR
        window.Layout.AddSeparator(true);

        // Toggle Other Gizmos

        hideOtherGizmosCheckbox = new Checkbox("Hide Other Gizmos");
        checkboxContainer.AddCell(0, 0, hideOtherGizmosCheckbox);

        // Toggle Handles
        drawHandlesCheckbox = new Checkbox("Toggle Handles");
        checkboxContainer.AddCell(1, 0, drawHandlesCheckbox);

        var boneGizmoMode = new Checkbox("Bone Display", window);
        checkboxContainer.AddCell(0, 1, boneGizmoMode);

        #endregion

        #region Buttons

        // -- BUTTONS --
        GridLayout btnsContainer = window.Layout.AddLayout(Layout.Grid());
        btnsContainer.Spacing = 10;

        // Add Button
        var addNodeBtn = new Button("Create Node");
        addNodeBtn.Pressed = () => AddNode();
        btnsContainer.AddCell(0, 0, addNodeBtn);

        // Clear Button
        var clearBtn = new Button("Clear");
        clearBtn.Pressed = () => ClearNodes();
        btnsContainer.AddCell(0, 1, clearBtn);

        // Refresh Button
        var refreshBtn = new Button("Refresh");
        refreshBtn.Pressed = () => RefreshNodes();
        btnsContainer.AddCell(1, 0, refreshBtn);

        CreateLayout = btnsContainer;

        #endregion

        if (firstTime)
        {
            AddOverlay(window, TextFlag.CenterBottom, 20);
            CreateEditWindow();
            editWindow.Enabled = false;
            editWindow.Visible = false;
        }

        HasCreatedWindow = true;
    }

    private Widget CreateBoneSelectionWidget()
    {
        var layout = Layout.Row();
        var sc = layout.Add(new SegmentedControl());
        sc.AddOption("Lines");
        sc.AddOption("Capsules");

        sc.SelectedIndex = (int)boneDisplayMode;

        sc.OnSelectedChanged = s =>
        {
            if (s == "Capsules")
            {
                boneDisplayMode = BoneDisplayMode.Capsule;
            }
            else if (s == "Lines")
            {
                boneDisplayMode = BoneDisplayMode.Line;
            }
        };

        sc.MaximumWidth = 200;
        sc.HorizontalSizeMode = SizeMode.CanGrow;

        return sc;
    }

    [EditorEvent.Hotload]
    public static void Rebuild()
    {
        Instance.CreateWindow(false);
    }

    private void UpdateNodeLabel()
    {
        if (!HasCreatedWindow) return;
        if (!Target.IsValid() || IsNodeSelected)
        {
            nodesLabel.Text = "";
            window.Update();
            return;
        }

        NodeCount = Target.Descendants.Count;

        if (HasCreatedWindow)
        {
            nodesLabel.Text = $"Nodes: {NodeCount}";
            window.Update();
        }
    }

    public override void OnSelectionChanged()
    {
        var goSelected = (GameObject)Selection.FirstOrDefault();
        Target = GetSelectedComponent<PBody>();
        IsNodeSelected = false;

        // Check if were selecting a child of the PBody
        if (goSelected.IsValid() && !Target.IsValid())
        {
            Target = goSelected.GetComponentInParent<PBody>();

            if (Target.IsValid())
            {
                PNode node = Target.Descendants.Find(c => c.GameObject == goSelected);

                if (node != null)
                {
                    nodeSelected = node;
                    IsNodeSelected = true;
                }
                else
                {
                    PNode parentNode = Target.Descendants.Find(c => c.IsChild(goSelected));
                    if (parentNode != null)
                    {
                        PNode child = parentNode.GetChild(goSelected);
                        if (child == null) return;

                        nodeSelected = child;
                        IsNodeSelected = true;
                    }
                }
            }
        }

        if (!Target.IsValid())
        {
            if (HasCreatedWindow)
            {
                SwapToCreate();
            }

            UpdateNodeLabel();
            return;
        }

        Target.Init();

        if (!Target.SkeletonRoot.IsValid())
        {
            Target.SkeletonRoot = new GameObject(Target.GameObject, true, "skeleton");
        }

        UpdateNodeLabel();
        SwapToEdit();
    }

    private void AddNode()
    {
        if (Target.IsValid())
        {
            var node = Target.AddNode();
            Selection.Set(node.GameObject);
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
        SceneOverlay.Update();
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
}