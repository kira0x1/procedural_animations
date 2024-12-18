namespace Kira.Procgen;

using System.Numerics;
using Sandbox;
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
        var nodeGo = new GameObject(SkeletonRoot, true, "node");
        var node = new PNode();
        node.Position = nodeGo.LocalPosition;
        node.Rotation = nodeGo.LocalRotation;
        node.GameObject = nodeGo;
        Nodes.Add(node);
    }

    public void RefreshNodes()
    {
        Nodes.Clear();

        foreach (GameObject node in SkeletonRoot.Children)
        {
            var pn = new PNode();
            pn.Position = node.LocalPosition;
            pn.Rotation = node.LocalRotation;
            pn.GameObject = node;
            Nodes.Add(pn);
        }
    }

    public void ClearNodes()
    {
        foreach (GameObject skeletonChild in SkeletonRoot.Children)
        {
            skeletonChild.Destroy();
        }

        Nodes.Clear();
    }
}

public class PNode
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public GameObject GameObject { get; set; }

    public float DesiredDistance { get; set; } = 4f;

    public void UpdateTransform()
    {
        Position = GameObject.LocalPosition;
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
        GameObject.LocalPosition = position;
    }
}