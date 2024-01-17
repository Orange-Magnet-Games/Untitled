using System;
using UnityEngine;
[ExecuteAlways]
public class Crosshair : MonoBehaviour {
    [Serializable] public struct CrosshairValues {
        public float centerSize, centerOutlineThickness, lineOutlineThickness, lineLength, lineWidth, lineDistance, movementMultiplier;
        public Color centerColor, centerOutlineColor, lineColor, lineOutlineColor;
        public bool Equals(CrosshairValues other) {
            return centerSize.Equals(other.centerSize) && lineLength.Equals(other.lineLength) && 
                   lineWidth.Equals(other.lineWidth) && lineDistance.Equals(other.lineDistance) && 
                   movementMultiplier.Equals(other.movementMultiplier) && centerColor.Equals(other.centerColor) && 
                   lineColor.Equals(other.lineColor) && centerOutlineColor.Equals(other.centerOutlineColor) && 
                   lineOutlineColor.Equals(other.lineOutlineColor) && centerOutlineThickness.Equals(other.centerOutlineThickness) && 
                   lineOutlineThickness.Equals(other.lineOutlineThickness);
        }

        public override bool Equals(object obj) {
            return obj is CrosshairValues other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(centerSize, lineLength, lineWidth, lineDistance, movementMultiplier, centerColor, lineColor);
        }
        
        public static bool operator ==(CrosshairValues a, CrosshairValues b) => a.Equals(b);
        public static bool operator !=(CrosshairValues a, CrosshairValues b) => !a.Equals(b);
    }
    
    [SerializeField, Header("Crosshair")] private CrosshairElement center;
    [SerializeField] private CrosshairElement up, down, left, right;
    [SerializeField] private float lineMovement;

    public float LineMovement {
        get => lineMovement;
        set {
            if (lineMovement.Equals(value)) return;
            lineMovement = value;
            OnMovement();
        }
    }
    
    [SerializeField, Header("Sizes")] private CrosshairValues values;

    private CrosshairValues Values {
        get => values;
        set {
            if (values == value) return;
            values = value;
            OnValueChanged();
        }
    }

    
#if UNITY_EDITOR
    private void Update() {
        Values = values;
        LineMovement = lineMovement;
        OnValueChanged();
        OnMovement();
    }
#endif
    
    private void OnValueChanged() {
        //Center Dot
        center.image.color = Values.centerColor;
        center.rt.sizeDelta = new Vector2(Values.centerSize, Values.centerSize);
        center.Thickness = Values.centerOutlineThickness;
        center.OnValueChanged();
        
        //Lines
        up.rt.sizeDelta = new Vector2(Values.lineWidth, Values.lineLength);
        up.image.color = Values.lineColor;
        up.Thickness = Values.lineOutlineThickness;
        up.OnValueChanged();
        
        down.rt.sizeDelta = new Vector2(Values.lineWidth, Values.lineLength);
        down.image.color = Values.lineColor;
        down.Thickness = Values.lineOutlineThickness;
        down.OnValueChanged();
        
        left.rt.sizeDelta = new Vector2(Values.lineLength, Values.lineWidth);
        left.image.color = Values.lineColor;
        left.Thickness = Values.lineOutlineThickness;
        left.OnValueChanged();
        
        right.rt.sizeDelta = new Vector2(Values.lineLength, Values.lineWidth);
        right.image.color = Values.lineColor;
        right.Thickness = Values.lineOutlineThickness;
        right.OnValueChanged();
    }

    private void OnMovement() {
        up.transform.localPosition = new Vector2(0, Values.lineDistance + (Values.centerSize + Values.lineLength) / 2 + Values.centerOutlineThickness + Values.lineOutlineThickness + (LineMovement <= 0 ? 0 : LineMovement) * Values.movementMultiplier);
        down.transform.localPosition = new Vector2(0, -(Values.lineDistance + (Values.centerSize + Values.lineLength) / 2 + Values.centerOutlineThickness + Values.lineOutlineThickness + (LineMovement <= 0 ? 0 : LineMovement) * Values.movementMultiplier));
        left.transform.localPosition = new Vector2(-(Values.lineDistance + (Values.centerSize + Values.lineLength) / 2 + Values.centerOutlineThickness + Values.lineOutlineThickness + (LineMovement <= 0 ? 0 : LineMovement) * Values.movementMultiplier), 0);
        right.transform.localPosition = new Vector2(Values.lineDistance + (Values.centerSize + Values.lineLength) / 2 + Values.centerOutlineThickness + Values.lineOutlineThickness + (LineMovement <= 0 ? 0 : LineMovement) * Values.movementMultiplier, 0);
        
        center.OnValueChanged();
        up.OnValueChanged();
        down.OnValueChanged();
        left.OnValueChanged();
        right.OnValueChanged();
    }
}
