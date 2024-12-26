namespace Kira.Procgen;

[Icon("boy")]
[Category("Procgen")]
[EditorHandle(Icon = "boy")]
public sealed class PBody : Component
{
    [Property]
    public GameObject SkeletonRoot { get; set; }

    [Property]
    public List<PNode> Descendants { get; set; } = new List<PNode>();

    private void CreateSkeleton()
    {
        Log.Info("Creating Skeleton");
        SkeletonRoot = new GameObject(GameObject, true, "skeleton");
    }

    /// <summary>
    /// Checks for skeleton
    /// </summary>
    public void Init()
    {
        var skel = GameObject.Children.Find(c => c.Name == "skeleton");
        if (!skel.IsValid() && GameObject.Children.Count >= 1)
        {
            skel = GameObject.Children[0];
        }

        SkeletonRoot = skel;
    }

    protected override void OnUpdate()
    {
        foreach (PNode node in Descendants)
        {
            // node.UpdatePosition();
        }
    }

    public PNode AddNode()
    {
        if (!SkeletonRoot.IsValid())
        {
            CreateSkeleton();
        }

        var nodeGo = new GameObject(SkeletonRoot, true, $"node_{SkeletonRoot.Children.Count}");
        var node = new PNode(nodeGo, this, SkeletonRoot.Children.Count);

        node.Position = nodeGo.LocalPosition;
        node.Rotation = nodeGo.LocalRotation;
        node.GameObject = nodeGo;

        Descendants.Add(node);

        return node;
    }

    public void DeleteNode(PNode node)
    {
        Descendants.Remove(node);

        node.DeleteNode();
    }

    public void RefreshNodes()
    {
        Descendants.Clear();

        for (var i = 0; i < SkeletonRoot.Children.Count; i++)
        {
            GameObject node = SkeletonRoot.Children[i];
            var pn = new PNode(node, this, GameObject.Children.Count);
            pn.Position = node.LocalPosition;
            pn.Rotation = node.LocalRotation;
            pn.GameObject = node;

            pn.RefreshChildren();
            Descendants.Add(pn);
        }
    }

    public void ClearNodes()
    {
        if (SkeletonRoot.IsValid())
        {
            SkeletonRoot.Destroy();
        }

        Descendants.Clear();
    }
}