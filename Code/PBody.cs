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
        Nodes.Add(node);
    }

    public void RefreshNodes()
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
}