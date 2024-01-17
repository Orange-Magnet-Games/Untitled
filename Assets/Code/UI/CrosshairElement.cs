using UnityEngine;
using UnityEngine.UI;

public class CrosshairElement : MonoBehaviour
{
    
    [SerializeField] private RectTransform up, down, left, right;
    private float thickness;

    public float Thickness {
        get => thickness;
        set {
            if (thickness.Equals(value)) return;
            thickness = value;
            OnValueChanged();
        }
    }
    
    public Image image;
    public RectTransform rt;
    
#if UNITY_EDITOR
    private void Update() {
        Thickness = thickness;
        OnValueChanged();
    }
#endif

    public void OnValueChanged() {
        
        Rect rect = rt.rect;
        
        up.localPosition = new Vector2(0, (rect.height + Thickness) / 2);
        up.sizeDelta = new Vector2(rect.width, Thickness);
        
        down.localPosition = new Vector2(0, (-rect.height - Thickness) / 2);
        down.sizeDelta = new Vector2(rect.width, Thickness);
        
        left.localPosition = new Vector2((-rect.width - Thickness) / 2, 0);
        left.sizeDelta = new Vector2(Thickness, rect.height + Thickness * 2);
        
        right.localPosition = new Vector2((rect.width + Thickness) / 2, 0);
        right.sizeDelta = new Vector2(Thickness, rect.height + Thickness * 2);
    }
}
