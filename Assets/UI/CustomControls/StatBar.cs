using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("StatBar")]
public partial class StatBar : VisualElement
{
    private readonly VisualElement _fill;
    private float _fillPercent = 1f;

    public StatBar()
    {
        AddToClassList("stat-bar");

        _fill = new VisualElement();
        _fill.name = "fill";
        _fill.AddToClassList("bar-fill");
        _fill.style.backgroundColor = StyleKeyword.Null;

        hierarchy.Add(_fill);

        UpdateFillVisual();
    }

    public void SetPercent(float percent)
    {
        _fillPercent = Mathf.Clamp01(percent);
        UpdateFillVisual();
    }

    private void UpdateFillVisual()
    {
        _fill.style.width = Length.Percent(_fillPercent * 100f);
    }
}
