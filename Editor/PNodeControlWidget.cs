namespace Kira.Procgen;

public class PNodeControlWidget : ControlWidget
{
    // Whether or not this control supports multi-editing (if you have multiple GameObjects selected)
    public override bool SupportsMultiEdit => false;

    public PNodeControlWidget(SerializedProperty property) : base(property)
    {
        Layout = Layout.Row();
        Layout.Spacing = 2;

        // Serialize the property as a MyClass object
        var serializedObject = property.GetValue<PNode>()?.GetSerialized();
        if (serializedObject is null)
        {
            return;
        }

        // Get the Color and Name properties from the serialized object
        serializedObject.TryGetProperty(nameof(PNode.DesiredDistance), out var dist);

        // serializedObject.TryGetProperty(nameof(SpriteAttachment.Color), out var color);
        // serializedObject.TryGetProperty(nameof(SpriteAttachment.Name), out var name);

        // Add some Controls to the Layout, both referencing their serialized properties
        Layout.Add(new FloatControlWidget(dist));
        // Layout.Add(new StringControlWidget(name) { HorizontalSizeMode = SizeMode.Default });
    }

    protected override void OnPaint()
    {
        // Overriding nothing here prevents the default background from being painted
    }
}