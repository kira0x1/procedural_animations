namespace Kira;

using System;

public class BodyNode
{
    public Vector3 position;

    public float segmentSize = 1f;
    public float desiredDistance = 1f;
    public float offset = 1f;

    public BodyNode anchor;
    private Vector3 anchorPoint;

    public float Speed { get; set; } = 3f;

    public BodyNode()
    {
    }

    public BodyNode(Vector3 position, float desiredDistance = 3f)
    {
        this.position = position;
        this.desiredDistance = desiredDistance;
    }

    public BodyNode(Vector3 position, BodyNode anchor, float desiredDistance = 3f)
    {
        this.position = position;
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
        UpdateAnchorPoint();

        var dir = Vector3.Direction(anchorPoint, anchor.position);
        // var targetPos = anchor.position - dir * desiredDistance * 2f;
        var targetPos = anchor.position - dir * (desiredDistance * offset + 1f);

        Gizmo.Draw.Arrow(position, targetPos, 2f, 1f);
        // Gizmo.Draw.LineSphere(targetPos, 1f);


        float dist = Vector3.DistanceBetween(position.WithX(0f), anchor.position.WithX(0f));

        if (dist < desiredDistance)
        {
            position = anchor.position - dir * desiredDistance;
        }

        position = Vector3.Lerp(position, targetPos, Speed * Time.Delta);
    }

    public Vector3 CalculateDirection()
    {
        var dir = Vector3.Direction(position, anchor.position);
        Vector3 res = dir * desiredDistance;
        return res;
    }

    public void DrawNode(float radius = 3f)
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
            // Gizmo.Draw.LineCircle(pos, desiredDistance / 4f);
        }

        // draw anchor point
        using (Gizmo.Scope("anchorPoint"))
        {
            Gizmo.Draw.Color = Gizmo.Colors.Active;
            Gizmo.Draw.LineThickness = 1f;
            Gizmo.Draw.LineCircle(anchorPoint, desiredDistance / 3f);
        }
    }
}