namespace Kira.Procgen.Editor;

public partial class PBodyEditorWidget
{
    private Button DeleteNodeBtn { get; set; }

    private void CreateEditWindow()
    {
        editWindow = new WidgetWindow(SceneOverlay, "Edit");
        editWindow.Layout = Layout.Column();
        editWindow.Layout.Spacing = 16;
        editWindow.Layout.Margin = 14;
        editWindow.MinimumSize = new Vector2(230, 110);


        GridLayout btnsContainer = editWindow.Layout.AddLayout(Layout.Grid());
        btnsContainer.Spacing = 10;

        // AddChild Button
        // add to edit window instead
        var addChildBtn = new Button("Add Child");
        addChildBtn.Pressed = () => AddChildNode();

        var deleteNodeBtn = new Button("Delete Node");
        deleteNodeBtn.Pressed = () => DeleteNode();
        DeleteNodeBtn = deleteNodeBtn;

        btnsContainer.Alignment = TextFlag.Top;
        btnsContainer.AddCell(0, 0, addChildBtn);
        btnsContainer.AddCell(1, 0, deleteNodeBtn);

        AddOverlay(editWindow, TextFlag.CenterBottom, 20);

        var pos = editWindow.Position;
        pos.x -= 240f;
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
        }
        else if (IsNodeSelected)
        {
            DeleteNodeBtn.Enabled = true;
        }

        editWindow.WindowTitle = IsNodeSelected ? $"Edit: {nodeSelected.Name}" : $"Edit";
        editWindow.Update();
    }

    private void AddChildNode()
    {
        if (IsNodeSelected)
        {
            nodeSelected.AddChild();
        }
        else
        {
            Target.AddNode();
        }
    }

    private void DeleteNode()
    {
        if (!Target.IsValid() || !IsNodeSelected) return;
        Target.DeleteNode(nodeSelected);
        IsNodeSelected = false;
        UpdateEditWindow();
    }
}