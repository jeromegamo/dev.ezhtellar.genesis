using Shapes;
using UnityEngine;

public class PanningEdgeGuide : ImmediateModeCanvas
{
    private float m_thresholdSize = 8f;
    // this is called automatically by the base class, in an existing Draw.Command context
    public override void DrawCanvasShapes(ImCanvasContext ctx) =>
        Draw.RectangleBorder(
            ctx.canvasRect,
            m_thresholdSize,
            cornerRadius: 0,
            new Color(
                Color.green.r,
                Color.green.g,
                Color.green.b,
                0.4f
            )
        );

    public void SetThresholdSize(float thresholdSize) => m_thresholdSize = thresholdSize;
}
