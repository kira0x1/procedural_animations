namespace Kira.Procgen.Editor;

using System;

public enum BoneDisplayMode
{
    Capsule,
    Line
}

public partial class PBodyEditorWidget
{
    public BoneDisplayMode boneDisplayMode { get; set; } = BoneDisplayMode.Line;

    public override void OnUpdate()
    {
        Gizmo.Settings.GizmosEnabled = hideOtherGizmosCheckbox.State != CheckState.On;

        if (!HasCreatedWindow || !Target.IsValid())
        {
            return;
        }

        using (Gizmo.Scope("RootControl"))
        {
            Gizmo.Transform = new Transform(Target.WorldPosition);
            Gizmo.Control.Position("rootPos", Target.LocalPosition, out Vector3 rootPos);
            Target.WorldPosition = rootPos;
        }

        for (var i = 0; i < Target.Descendants.Count; i++)
        {
            PNode node = Target.Descendants[i];
            if (node == null) return;

            using (Gizmo.Scope($"node{i}"))
            {
                node.DrawGizmos();
                Gizmo.Transform = new Transform(node.Position, Rotation.Identity);
                Gizmo.Draw.Color = Gizmo.HasSelected ? Color.Cyan : Color.White;

                if (drawHandlesCheckbox.Value || Selection.Contains(node.GameObject))
                {
                    Gizmo.Control.Position("pos", node.Position, out Vector3 newPos, squareSize: 2.5f);

                    // this is so that we can still make changes to gameobjects position in the inspector
                    if (newPos != node.Position)
                    {
                        node.SetPosition(newPos);
                    }
                    else
                    {
                        node.Position = node.GameObject.LocalPosition;
                    }
                }

                // Gizmo.Control.DragSquare("rd", new Vector2(1, node.DesiredDistance * 0.5f), Rotation.From(0, 90, 0), out Vector3 mov);
            }


            using (Gizmo.Scope("DistanceControl"))
            {
                var t = node.GameObject.LocalTransform;
                Gizmo.Transform = t;

                Gizmo.Draw.LineCircle(Vector3.Zero, Vector3.Left, node.DesiredDistance, sections: 64);

                t.Position += Vector3.Forward * (4f + node.DesiredDistance + 2f);
                Gizmo.Transform = t;

                // Gizmo.Settings.GizmoScale = 1f;
                // node.DesiredDistance += mov.z;
                // Gizmo.Control.Sphere("rd", node.DesiredDistance, out float radius, Color.Magenta);

                Gizmo.Control.Arrow("rd", Vector3.Forward, out float dist, 12f, 6f, 5f);

                node.DesiredDistance += dist;
                node.DesiredDistance = node.DesiredDistance.Clamp(2f, 150f);
            }

            // Bones
            using (Gizmo.Scope("Bone"))
            {
                if (!node.HasParent)
                {
                    if (boneDisplayMode == BoneDisplayMode.Capsule)
                    {
                        var capsule = new Capsule(node.LocalPos, Target.SkeletonRoot.WorldPosition, 1f);
                        Gizmo.Draw.LineCapsule(capsule);
                    }
                    else if (boneDisplayMode == BoneDisplayMode.Line)
                    {
                        Gizmo.Draw.Line(node.LocalPos, Target.SkeletonRoot.WorldPosition);
                    }
                }
            }

            using (Gizmo.Scope("LeafBones"))
            {
                Gizmo.Draw.Color = Color.Cyan;

                for (var j = 0; j < node.Children.Count; j++)
                {
                    var cnode = node.Children[j];

                    if (boneDisplayMode == BoneDisplayMode.Capsule)
                    {
                        var capsule = new Capsule(node.LocalPos, cnode.LocalPos, 1.5f);
                        Gizmo.Draw.LineCapsule(capsule);
                    }
                    else if (boneDisplayMode == BoneDisplayMode.Line)
                    {
                        // var boneLines = new Capsule(node.LocalPos, cnode.LocalPos, 1.5f);
                        Gizmo.Draw.Line(node.LocalPos, cnode.LocalPos);
                    }
                }
            }
        }
    }
}