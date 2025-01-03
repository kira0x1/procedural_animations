using Kira;
using Kira.Procgen;

[Category("Kira")]
public sealed class ProceduralBlobs : Component
{
    [Property, Range(1, 20)]
    private int Resolution { get; set; } = 4;

    [Property, Range(0, 20)]
    private float Speed { get; set; } = 5f;

    [Property, Range(0, 10f)]
    private float Offset { get; set; } = 5f;

    [Property, Range(0, 10f)]
    private float DesiredDistance { get; set; } = 5f;

    private Vector3 anchorPos = Vector3.Zero;

    [Property]
    private PBody Body { get; set; }
    private List<BodyNode> nodes = new List<BodyNode>();

    private Body body = new Body();

    protected override void OnStart()
    {
    }

    protected override void OnUpdate()
    {
        // DisplayGizmos();
        // RefreshBodyNodes();
        // DrawNodeGizmos();

        UpdateAnchorPos();

        body.SetOffset(Offset);
        body.SetDesiredDistance(DesiredDistance);
        body.SetSpeed(Speed);
        body.SetHeadPos(anchorPos);

        if (Input.Pressed("Slot1"))
        {
            body.AddNode(Vector3.Left * Offset);
        }

        if (Input.Pressed("Slot2"))
        {
            body.ClearNodes();
        }

        body.DrawBody();
    }

    protected override void DrawGizmos()
    {
    }

    private void DrawNodeGizmos()
    {
        using (Gizmo.Scope("nodes"))
        {
            foreach (BodyNode node in nodes)
            {
                Gizmo.Draw.Color = new Color(90f, 100f, 150f);
                Gizmo.Draw.LineThickness = 2f;
                Gizmo.Draw.LineCircle(node.position, node.segmentSize, sections: 128);
            }
        }
    }

    private void UpdateAnchorPos()
    {
        var mouse = Mouse.Position;
        var camera = Scene.Camera;
        var ray = camera.ScreenPixelToRay(mouse);

        var tr = Scene.Trace.Ray(ray, 300f).Run();
        anchorPos = tr.EndPosition;
    }

    private void DisplayGizmos()
    {
        var pointPos = new Vector3(0, -60f, 30f);

        using (Gizmo.Scope("point"))
        {
            Gizmo.Draw.LineThickness = 3f;
            Gizmo.Draw.LineCircle(pointPos, 3f);
        }

        using (Gizmo.Scope("firstArrow"))
        {
            Gizmo.Draw.LineThickness = 2.2f;
            Gizmo.Draw.Color = new Color(30f, 60f, 30f);
            Gizmo.Draw.Arrow(anchorPos, pointPos, 1f, 0.5f);
        }

        using (Gizmo.Scope("innerPoint"))
        {
            // var dir = anchorPos + pointPos.Normal * 10f;
            Vector3 dir = anchorPos + Vector3.Direction(anchorPos, pointPos) * 10f;
            Gizmo.Draw.LineThickness = 3f;
            Gizmo.Draw.IgnoreDepth = true;
            Gizmo.Draw.Line(anchorPos, dir);
            Gizmo.Draw.SolidCircle(dir, 1.5f, sections: 16);
        }
    }
}