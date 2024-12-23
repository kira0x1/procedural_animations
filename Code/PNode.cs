namespace Kira.Procgen;

using System.Numerics;
using Vector3 = Vector3;

public class PNode
{
    public string Name => GameObject.Name;
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public GameObject GameObject { get; set; }
    public PBody Body { get; set; }

    public Vector3 WorldPos => GameObject.WorldPosition;
    public Vector3 LocalPos => GameObject.LocalPosition;

    public int Id { get; set; }
    public float DesiredDistance { get; set; } = 4f;

    public bool HasParent { get; set; }
    public PNode parent;
    public string NodeName { get; set; }


    public List<PNode> Children { get; set; } = new List<PNode>();

    public PNode(GameObject gameObject, PBody body, int id = 0)
    {
        this.Id = id;
        this.Body = body;
        this.GameObject = gameObject;
    }

    public PNode()
    {
    }

    public void GenerateName()
    {
        NodeName = HasParent ? $"{parent.Id}_node_{Id}" : $"node_{Id}";
    }

    public void SetParent(PNode parent)
    {
        this.parent = parent;
        HasParent = true;
    }

    public void UpdateTransform()
    {
        Position = GameObject.LocalPosition;
    }

    // called after the body has deleted this node so clean up any references
    public void DeleteNode()
    {
        if (HasParent)
        {
            parent.OnChildNodeDeleted(this);
        }

        if (GameObject.IsValid())
        {
            GameObject.Destroy();
        }
    }

    protected void OnChildNodeDeleted(PNode node)
    {
        Children.Remove(node);
    }


    private Vector3 GetAnchorPoint()
    {
        var center = GameObject.LocalPosition;
        var dir = Vector3.Direction(GameObject.LocalPosition, GameObject.Parent.LocalPosition);
        Vector3 res = dir * DesiredDistance;
        var anchorPoint = center + res;
        return anchorPoint;
    }

    /// <summary>
    /// Updates the nodes constrained position relative to its anchor 
    /// </summary>
    public void UpdatePosition()
    {
        var pos = GameObject.LocalPosition;
        var anchor = Body;
        var dir = Vector3.Direction(GetAnchorPoint(), anchor.LocalPosition);
        // var targetPos = anchor.position - dir * desiredDistance * 2f;

        const float offset = 1f;
        var targetPos = anchor.LocalPosition - dir * (DesiredDistance * offset + 1f);

        Gizmo.Draw.Arrow(pos, targetPos, 2f, 1f);
        // Gizmo.Draw.LineSphere(targetPos, 1f);


        float dist = Vector3.DistanceBetween(pos.WithX(0f), anchor.LocalPosition.WithX(0f));

        if (dist < DesiredDistance)
        {
            pos = anchor.LocalPosition - dir * DesiredDistance;
        }

        const float Speed = 2f;
        Position = Vector3.Lerp(pos, targetPos, Speed * Time.Delta);
    }

    public void DrawGizmos()
    {
        float fontSize = HasParent ? 16f : 22f;

        using (Gizmo.Scope("lines"))
        {
            if (!HasParent)
            {
                // Gizmo.Draw.Line(LocalPos, Body.SkeletonRoot.WorldPosition);
            }

            foreach (PNode cnode in Children)
            {
                // Gizmo.Draw.Line(LocalPos, cnode.LocalPos);
            }
        }


        using (Gizmo.Scope("node_text"))
        {
            Gizmo.Draw.Color = HasParent ? Color.White : new Color(0.2f, 0.71f, 0.8f);
            Gizmo.Draw.ScreenText(GameObject.Name, Gizmo.Camera.ToScreen(LocalPos - new Vector3(0, 0, DesiredDistance * 1.1f)), "Poppins", fontSize, TextFlag.CenterTop);
        }

        foreach (PNode node in Children)
        {
            node.DrawGizmos();
        }
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
        GameObject.LocalPosition = position;
    }

    public PNode AddChild()
    {
        var nodeGo = new GameObject(this.GameObject, true, $"c_node_{GameObject.Children.Count}");
        nodeGo.LocalPosition = LocalPos;

        var node = new PNode(nodeGo, Body, Children.Count);
        node.Position = nodeGo.LocalPosition;
        node.Rotation = nodeGo.LocalRotation;
        node.GameObject = nodeGo;
        node.SetParent(this);

        Children.Add(node);
        Body.Descendants.Add(node);
        return node;
    }

    public bool IsChild(GameObject gameObject)
    {
        return Children.Find(c => c.GameObject == gameObject) != null;
    }

    public PNode GetChild(GameObject gameObject)
    {
        return Children.Find(c => c.GameObject == gameObject);
    }
}