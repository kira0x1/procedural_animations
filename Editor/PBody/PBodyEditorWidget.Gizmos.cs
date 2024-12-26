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
            var selection = Selection.FirstOrDefault();
            GameObject s = (GameObject)selection;
            if (selection != null && s.Name == Target.GameObject.Name)
            {
                Gizmo.Transform = new Transform(Target.WorldPosition);
                Gizmo.Control.Position("rootPos", Target.LocalPosition, out Vector3 rootPos);
                Target.WorldPosition = rootPos;
            }
        }

        for (var i = 0; i < Target.Descendants.Count; i++)
        {
            PNode node = Target.Descendants[i];
            if (node == null) return;

            using (Gizmo.Scope($"node{i}"))
            {
                node.DrawGizmos();
                Gizmo.Transform = node.GameObject.LocalTransform;
                Gizmo.Draw.Color = Gizmo.HasSelected ? Color.Cyan : Color.White;

                if (drawHandlesCheckbox.Value || Selection.Contains(node.GameObject))
                {
                    Gizmo.Control.Position("pos", node.LocalPos, out Vector3 newPos, squareSize: 2.5f);

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


            using (Gizmo.Scope($"DistanceControl{i}"))
            {
                var t = node.GameObject.LocalTransform;
                Gizmo.Transform = t;
                if (ShowDistanceRadius.State == CheckState.On)
                {
                    Gizmo.Draw.LineCircle(Vector3.Zero, Vector3.Left, node.DesiredDistance, sections: 64);
                }

                if (ShowDistanceControl.State == CheckState.On && IsNodeSelected && nodeSelected.Id == node.Id)
                {
                    var dx = Gizmo.Camera.ToWorld(t.Position);
                    var offsetArrow = dx.Length / 8f;
                    offsetArrow += node.DesiredDistance / 1.2f;
                    Gizmo.Control.Arrow("rd", Vector3.Forward, out float dist, 8f, 6f, offsetArrow);

                    node.DesiredDistance += dist;
                    node.DesiredDistance = node.DesiredDistance.Clamp(1f, 12f);
                }
            }


            // Bones
            using (Gizmo.Scope("Bone"))
            {
                if (!node.HasParent)
                {
                    if (boneDisplayMode == BoneDisplayMode.Capsule)
                    {
                        var capsule = new Capsule(node.GameObject.LocalPosition, Target.SkeletonRoot.WorldPosition, 1f);
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