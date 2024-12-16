namespace Kira;

public class Body
{
    public BodyNode HeadNode { get; set; }
    public List<BodyNode> Nodes { get; set; }

    public Body()
    {
        HeadNode = new BodyNode();
        Nodes = new List<BodyNode>();
    }

    public Body(Vector3 startPosition, int segments = 3)
    {
        HeadNode = new BodyNode(startPosition);
        Nodes = new List<BodyNode>();

        for (int i = 0; i < segments; i++)
        {
            AddNode();
        }
    }

    public void SetSpeed(float speed)
    {
        foreach (BodyNode node in Nodes)
        {
            node.Speed = speed;
        }
    }

    public void SetOffset(float offset)
    {
        foreach (BodyNode node in Nodes)
        {
            node.offset = offset;
        }
    }

    public void SetDesiredDistance(float desiredDistance)
    {
        foreach (BodyNode node in Nodes)
        {
            node.desiredDistance = desiredDistance;
        }
    }

    public void AddNode(BodyNode node)
    {
        var prevNode = Nodes.Count == 0 ? HeadNode : Nodes[^1];
        node.SetAnchor(prevNode);
        node.UpdatePosition();
    }

    public void AddNode()
    {
        var prevNode = Nodes.Count == 0 ? HeadNode : Nodes[^1];
        BodyNode node = new BodyNode(prevNode.position, prevNode);
        node.UpdatePosition();
        Nodes.Add(node);
    }

    public void AddNode(Vector3 offset, float desiredDistance = 3f)
    {
        var prevNode = Nodes.Count == 0 ? HeadNode : Nodes[^1];
        var pos = prevNode.position + offset;
        BodyNode node = new BodyNode(pos, prevNode);
        node.desiredDistance = desiredDistance;
        node.UpdatePosition();
        Nodes.Add(node);
    }

    public void ClearNodes()
    {
        Nodes.Clear();
    }

    public void SetHeadPos(Vector3 pos)
    {
        HeadNode.position = pos;

        if (Nodes.Count > 0)
        {
            Nodes[0].UpdatePosition();
        }

        foreach (BodyNode node in Nodes)
        {
            node.UpdatePosition();
        }
    }

    public void DrawBody()
    {
        Gizmo.Draw.LineCircle(HeadNode.position, 3f);

        for (var i = 0; i < Nodes.Count; i++)
        {
            BodyNode node = Nodes[i];
            Gizmo.Draw.Color = Color.Red;
            node.DrawNode();

            var endPos = node.anchor.position;

            using (Gizmo.Scope("line"))
            {
                Gizmo.Draw.LineThickness = 2f;
                Gizmo.Draw.Color = Color.Lerp(Color.Green, Color.Red, (i * 1f) / Nodes.Count);

                var dist = Vector3.DistanceBetween(node.position, endPos);
                var centerPoint = node.position - Vector3.Left * dist / 2f;
                // Gizmo.Draw.LineCircle(centerPoint, 1f);
                // Gizmo.Draw.Line(centerPoint - Vector3.Left * 2f, centerPoint + Vector3.Left * 2f);
            }
        }
    }
}