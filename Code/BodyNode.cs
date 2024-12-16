namespace Kira;

public class BodyNode
{
    public Vector3 position;
    private Vector3 startPosition;

    public float segmentSize = 1f;

    public float desiredDistance;
    public BodyNode anchor;
    private Vector3 anchorPoint;

    public BodyNode()
    {
    }

    public BodyNode(Vector3 position, float desiredDistance = 3f)
    {
        this.position = position;
        this.startPosition = position;
        this.desiredDistance = desiredDistance;
    }

    public BodyNode(Vector3 position, BodyNode anchor, float desiredDistance = 3f)
    {
        this.position = position;
        this.startPosition = position;
        this.anchor = anchor;
        this.desiredDistance = desiredDistance;
    }

    public void SetAnchor(BodyNode node)
    {
        anchor = node;
    }

    private void UpdateAnchorPoint()
    {
        var center = position;
        var dir = CalculateDirection();
        anchorPoint = center + dir;
    }

    /// <summary>
    /// Updates the nodes constrained position relative to its anchor 
    /// </summary>
    public void UpdatePosition()
    {
        // var dir = CalculateDirection();
        UpdateAnchorPoint();
        var pos = anchor.position;

        // offset
        pos += Vector3.Left * 10f;
        // pos.y -= dir.y;

        var dir = Vector3.Direction(anchorPoint, anchor.position);
        var targetPos = Vector3.Lerp(anchorPoint, anchor.position, 0.5f);

        Gizmo.Draw.LineSphere(targetPos, 1f);
        position = Vector3.Lerp(position,targetPos, 5f * Time.Delta);
    }

    public Vector3 CalculateDirection()
    {
        var dir = Vector3.Direction(position, anchor.position);
        Vector3 res = dir * desiredDistance;
        return res;
    }

    public void DrawNode()
    {
        UpdateAnchorPoint();

        using (Gizmo.Scope("dist"))
        {
            Gizmo.Draw.Color = Color.Magenta;
            var pos = position;

            // Gizmo.Draw.LineThickness = 0.8f;
            Gizmo.Draw.LineCircle(pos, desiredDistance);
            // Gizmo.Draw.LineSphere(pos, desiredDistance, 3);
        }

        using (Gizmo.Scope("node"))
        {
            Gizmo.Draw.Color = Gizmo.Colors.Blue;
            var pos = position;
            Gizmo.Draw.LineCircle(pos, 0.5f);
        }

        // draw anchor point
        using (Gizmo.Scope("anchorPoint"))
        {
            Gizmo.Draw.Color = Gizmo.Colors.Active;
            Gizmo.Draw.LineThickness = 1f;
            Gizmo.Draw.LineCircle(anchorPoint, 1f);
        }
    }
}