namespace Kira.Procgen;

using System.Numerics;
using Vector3 = Vector3;

[Icon("boy")]
[Category("Procgen")]
[EditorHandle(Icon = "boy")]
public sealed class PBody : Component
{
    [Property]
    public List<PNode> Skeleton { get; set; } = new List<PNode>();
    public GameObject SkeletonRoot { get; set; }
    public List<PNode> Nodes { get; set; } = new List<PNode>();

    private void CreateSkeleton()
    {
        SkeletonRoot = new GameObject(GameObject, true, "skeleton");
    }

    public void AddNode()
    {
        if (!SkeletonRoot.IsValid())
        {
            CreateSkeleton();
        }

        var nodeGo = new GameObject(SkeletonRoot, true, $"node_{Nodes.Count}");
        var node = new PNode(nodeGo, Nodes.Count);

        node.Position = nodeGo.LocalPosition;
        node.Rotation = nodeGo.LocalRotation;
        node.GameObject = nodeGo;
        Nodes.Add(node);
    }

    public void RefreshNodes()
    {
        Nodes.Clear();

        for (var i = 0; i < SkeletonRoot.Children.Count; i++)
        {
            GameObject node = SkeletonRoot.Children[i];
            var pn = new PNode(node, i);
            pn.Position = node.LocalPosition;
            pn.Rotation = node.LocalRotation;
            pn.GameObject = node;
            Nodes.Add(pn);
        }
    }

    protected override void DrawGizmos()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].DrawGizmos();
        }
    }

    public void ClearNodes()
    {
        if (SkeletonRoot.IsValid())
        {
            SkeletonRoot.Destroy();
        }

        Nodes.Clear();
    }
}

public class PNode
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public GameObject GameObject { get; set; }

    public int Id { get; set; }
    public float DesiredDistance { get; set; } = 4f;

    public PNode(GameObject gameObject, int id = 0)
    {
        this.Id = id;
        this.GameObject = gameObject;
    }

    public void UpdateTransform()
    {
        Position = GameObject.LocalPosition;
    }

    public void DrawGizmos()
    {
        Gizmo.Draw.ScreenText($"{Id}", Gizmo.Camera.ToScreen(Position), "Poppins", 22, TextFlag.CenterTop);
        Gizmo.Control.BoundingBox("n", BBox.FromPositionAndSize(Position, 10f), out BBox box);
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
        GameObject.LocalPosition = position;
    }
}