using Kira;

[Category("Kira")]
public sealed class ProceduralBlobs : Component
{
    [Property, Range(1, 20)]
    private int Resolution { get; set; } = 4;


    [Property, Range(0, 50f)]
    private float Offset { get; set; } = 5f;

    [Property, Range(0, 20f)]
    private float SegmentSize { get; set; } = 5f;

    private Vector3 anchorPos = Vector3.Zero;

    private List<BodyNode> nodes = new List<BodyNode>();


    private void RefreshBodyNodes()
    {
        nodes.Clear();

        for (int i = 0; i < Resolution; i++)
        {
            var node = new BodyNode();
            node.SegmentSize = SegmentSize;
            var pos = anchorPos;

            // move to outer circle of anchor
            // Add the radius of anchor
            pos.y += SegmentSize * 3.2f;

            // add offsets
            
            // offset by each segments radius
            pos.y += i * SegmentSize * 2.2f;
            
            // add additional offset from offset variable
            pos.y += i * Offset;


            node.Position = pos;
            // node.Position += i * Offset + 5f;
            nodes.Add(node);
        }
    }

    protected override void OnUpdate()
    {
        DisplayGizmos();
        RefreshBodyNodes();

        using (Gizmo.Scope("nodes"))
        {
            foreach (BodyNode node in nodes)
            {
                Gizmo.Draw.Color = new Color(90f, 100f, 150f);
                Gizmo.Draw.LineThickness = 2f;
                Gizmo.Draw.LineCircle(node.Position, node.SegmentSize, sections: 128);
            }
        }

        UpdateAnchorPos();
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

        using (Gizmo.Scope("anchor"))
        {
            Gizmo.Draw.Color = new Color(130f, 20f, 100f);
            Gizmo.Draw.LineThickness = 3f;
            Gizmo.Draw.LineCircle(anchorPos, SegmentSize * 2f, sections: 128);
        }

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
            var dir = pointPos - anchorPos;
            dir = anchorPos + Vector3.Direction(anchorPos, pointPos) * 10f;
            Gizmo.Draw.LineThickness = 3f;
            Gizmo.Draw.IgnoreDepth = true;
            Gizmo.Draw.Line(anchorPos, dir);
            Gizmo.Draw.SolidCircle(dir, 1.5f, sections: 16);
        }
    }
}