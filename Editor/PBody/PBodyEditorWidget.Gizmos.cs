﻿namespace Kira.Procgen.Editor;

public partial class PBodyEditorWidget
{
    public override void OnUpdate()
    {
        if (!HasCreatedWindow || !Target.IsValid())
        {
            return;
        }


        for (var i = 0; i < Target.Descendants.Count; i++)
        {
            PNode node = Target.Descendants[i];
            if (node == null) return;

            using (Gizmo.Scope($"node{i}"))
            {
                Gizmo.Draw.Color = Gizmo.HasSelected ? Color.Cyan : Color.White;
                Gizmo.Transform = new Transform(node.Position, Rotation.Identity);

                if (Selection.Contains(node.GameObject))
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

                Gizmo.Control.Sphere("rd", node.DesiredDistance, out float radius, Color.Magenta);
                node.DesiredDistance = radius.Clamp(2f, 150f);
            }

            // Bones
            using (Gizmo.Scope("Bone"))
            {
                if (!node.HasParent)
                {
                    var capsule = new Capsule(node.LocalPos, Target.SkeletonRoot.LocalPosition, 5f);
                    Gizmo.Draw.LineCapsule(capsule);
                }
            }

            using (Gizmo.Scope("LeafBones"))
            {
                Gizmo.Draw.Color = Color.Cyan;
                for (var j = 0; j < node.Children.Count; j++)
                {
                    var cnode = node.Children[j];
                    var capsule = new Capsule(node.LocalPos, cnode.LocalPos, 1.5f);
                    Gizmo.Draw.LineCapsule(capsule);
                }
            }
        }
    }
}