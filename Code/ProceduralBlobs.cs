[Category("Kira")]
public sealed class ProceduralBlobs : Component
{
    [Property, Range(1, 20)]
    private int Resolution { get; set; } = 4;

    [Property, Range(0f, 100f)]
    private List<float> SegmentSize { get; set; }

    [Property, Range(0, 50f)]
    private float Offset { get; set; } = 5f;
    private Vector3 anchorPos = Vector3.Zero;

    protected override void OnValidate()
    {
        var length = SegmentSize.Count;
        if (length > Resolution)
        {
            var toDel = length - Resolution;
            var start = length - toDel;
            SegmentSize.RemoveRange(start, toDel);
        }
    }

    protected override void OnUpdate()
    {
        var pointPos = new Vector3(0, -60f, 30f);

        using (Gizmo.Scope("anchor"))
        {
            Gizmo.Draw.Color = new Color(130f, 20f, 100f);
            Gizmo.Draw.LineThickness = 6f;
            Gizmo.Draw.LineCircle(anchorPos, 10f, sections: 128);
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


        var mouse = Mouse.Position;
        var camera = Scene.Camera;
        var ray = camera.ScreenPixelToRay(mouse);

        var tr = Scene.Trace.Ray(ray, 300f).Run();

        Log.Info(tr.EndPosition);
        anchorPos = tr.EndPosition;

        // anchorPos -= Vector3.Right * 5 * Time.Delta;
        // anchorPos += Vector3.Down * 15 * Time.Delta;

        // for (int i = 0; i < Resolution; i++)
        // {
        //     var pos = GameObject.LocalPosition;
        //     pos.y += Offset * i;
        //
        //     var innerCircle = pos;
        //     innerCircle -= float.Cos(10f) + float.Sin(10f);
        //
        //     Gizmo.Draw.LineCircle(pos, 5f);
        //     Gizmo.Draw.LineCircle(innerCircle, 1f);
        // }
    }
}