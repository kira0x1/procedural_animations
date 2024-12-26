namespace Kira.Procgen.Editor;

public partial class PBodyEditorWidget
{
    private Button DeleteNodeBtn { get; set; }
    private Button AddSiblingBtn { get; set; }
    private Checkbox ShowDistanceControl { get; set; }
    private Checkbox ShowDistanceRadius { get; set; }

    private void CreateEditWindow()
    {
        editWindow = new WidgetWindow(SceneOverlay, "Edit");
        editWindow.Layout = Layout.Column();
        editWindow.Layout.Spacing = 16;
        editWindow.Layout.Margin = 14;
        editWindow.MinimumSize = new Vector2(230, 110);

        var checkboxContainer = editWindow.Layout.AddColumn();

        ShowDistanceControl = new Checkbox("Show Distance Control");
        ShowDistanceRadius = new Checkbox("Show Distance Radius");
        checkboxContainer.Add(ShowDistanceControl);
        checkboxContainer.Add(ShowDistanceRadius);

        GridLayout btnsContainer = editWindow.Layout.AddLayout(Layout.Grid());
        btnsContainer.Spacing = 10;

        // AddChild Button
        // add to edit window instead
        var addChildBtn = new Button("Add Child");
        addChildBtn.Pressed = () => AddChildNode();

        var deleteNodeBtn = new Button("Delete Node");
        deleteNodeBtn.Pressed = () => DeleteNode();
        DeleteNodeBtn = deleteNodeBtn;

        var addSiblingBtn = new Button("Add Sibling");
        addSiblingBtn.Pressed = () => AddSiblingNode();
        AddSiblingBtn = addSiblingBtn;

        btnsContainer.Alignment = TextFlag.Top;
        btnsContainer.AddCell(0, 0, addChildBtn);
        btnsContainer.AddCell(1, 0, deleteNodeBtn);
        btnsContainer.AddCell(1, 1, addSiblingBtn);


        AddOverlay(editWindow, TextFlag.CenterBottom, 20);

        var pos = editWindow.Position;
        pos.x -= 320f;
        editWindow.Position = pos;
    }


    private void SwapToEdit()
    {
        editWindow.Enabled = true;
        editWindow.Visible = true;
        UpdateEditWindow();
    }

    private void UpdateEditWindow()
    {
        if (!IsNodeSelected && DeleteNodeBtn.IsValid())
        {
            DeleteNodeBtn.Enabled = false;
            AddSiblingBtn.Enabled = false;
        }
        else if (IsNodeSelected)
        {
            DeleteNodeBtn.Enabled = true;
            AddSiblingBtn.Enabled = nodeSelected.parent != null || nodeSelected.GameObject.Parent.Name == "skeleton";
        }


        editWindow.WindowTitle = IsNodeSelected ? $"Edit: {nodeSelected.Name}" : $"Edit";
        editWindow.Update();
    }

    private void AddChildNode()
    {
        if (IsNodeSelected)
        {
            var node = nodeSelected.AddChild();
            Selection.Set(node.GameObject);
        }
        else
        {
            AddNode();
        }
    }

    private void AddSiblingNode()
    {
        if (!IsNodeSelected)
        {
            Log.Warning("cant add sibling, select a node first");
            return;
        }

        if (nodeSelected.parent == null)
        {
            if (nodeSelected.GameObject.Parent.Name == "skeleton")
            {
                AddNode();
                return;
            }

            Log.Warning($"cant add sibling, {nodeSelected.Name} does not have a parent");
            return;
        }

        var node = nodeSelected.parent.AddChild();
        Selection.Set(node.GameObject);
    }

    private void DeleteNode()
    {
        if (!Target.IsValid() || !IsNodeSelected) return;
        Target.DeleteNode(nodeSelected);
        IsNodeSelected = false;
        UpdateEditWindow();
    }
}