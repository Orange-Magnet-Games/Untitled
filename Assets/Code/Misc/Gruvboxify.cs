using UnityEngine;
public enum GColor {
  BG, RED, GREEN, YELLOW, BLUE, PURPLE, AQUA, GRAY, ORANGE,
  LIGHT_RED, LIGHT_GREEN, LIGHT_YELLOW, LIGHT_BLUE, LIGHT_PURPLE, LIGHT_AQUA, DARK_GRAY, LIGHT_ORANGE,
  BG0_H, BG0_S, BG0, BG1, BG2, BG3, BG4, FG, FG0, FG1, FG2, FG3, FG4
}
// ReSharper disable UnusedType.Global
public static class GruvColor {
    // ReSharper disable UnusedMember.Global
  public static Color32 GetGColor(GColor col) {
    return col switch {
        GColor.BG => new Color32(0x28, 0x28, 0x28, 0xff),
        GColor.RED => new Color32(0xcc, 0x24, 0x1d, 0xff),
        GColor.GREEN => new Color32(0x98, 0x97, 0x1a, 0xff),
        GColor.YELLOW => new Color32(0xd7, 0x99, 0x21, 0xff),
        GColor.BLUE => new Color32(0x45, 0x85, 0x88, 0xff),
        GColor.PURPLE => new Color32(0xb1, 0x62, 0x86, 0xff),
        GColor.AQUA => new Color32(0x68, 0x9d, 0x6a, 0xff),
        GColor.GRAY => new Color32(0xa8, 0x99, 0x84, 0xff),
        GColor.ORANGE => new Color32(0xd6, 0x5d, 0x0e, 0xff),
        GColor.DARK_GRAY => new Color32(0x92, 0x83, 0x74, 0xff),
        GColor.LIGHT_ORANGE => new Color32(0xfe, 0x80, 0x19, 0xff),
        GColor.LIGHT_RED => new Color32(0xfb, 0x49, 0x34, 0xff),
        GColor.LIGHT_GREEN => new Color32(0xb8, 0xbb, 0x26, 0xff),
        GColor.LIGHT_YELLOW => new Color32(0xfa, 0xbd, 0x2f, 0xff),
        GColor.LIGHT_BLUE => new Color32(0x83, 0xa5, 0x98, 0xff),
        GColor.LIGHT_PURPLE => new Color32(0xd3, 0x86, 0x9b, 0xff),
        GColor.LIGHT_AQUA => new Color32(0x8e, 0xc0, 0x7c, 0xff),
        GColor.BG0_H => new Color32(0x1d, 0x20, 0x21, 0xff),
        GColor.BG0_S => new Color32(0x32, 0x30, 0x2f, 0xff),
        GColor.BG0 => new Color32(0x28, 0x28, 0x28, 0xff),
        GColor.BG1 => new Color32(0x3c, 0x38, 0x36, 0xff),
        GColor.BG2 => new Color32(0x50, 0x49, 0x45, 0xff),
        GColor.BG3 => new Color32(0x66, 0x5c, 0x54, 0xff),
        GColor.BG4 => new Color32(0x7c, 0x6f, 0x64, 0xff),
        GColor.FG => new Color32(0xeb, 0xdb, 0xb2, 0xff),
        GColor.FG0 => new Color32(0xfe, 0x80, 0x19, 0xff),
        GColor.FG1 => new Color32(0xfb, 0xf1, 0xc7, 0xff),
        GColor.FG2 => new Color32(0xd5, 0xc4, 0xa1, 0xff),
        GColor.FG3 => new Color32(0xbd, 0xae, 0x93, 0xff),
        GColor.FG4 => new Color32(0xa8, 0x99, 0x84, 0xff),
        _ => new Color32()
    };
  }
}
[ExecuteAlways]
internal class Gruvboxify : MonoBehaviour {
  [SerializeField] private GColor mainColor;
  [SerializeField] private GColor shadowColor;
  private Renderer mesh;
  private static readonly int BaseColor = Shader.PropertyToID("_Base_Color");
  private static readonly int ShadowColor = Shader.PropertyToID("_Shadow_Color");

  private void Awake() {
    mesh = GetComponent<Renderer>();
  }
  private void Update() {
    mesh.sharedMaterial.SetColor(BaseColor, GruvColor.GetGColor(mainColor));
    mesh.sharedMaterial.SetColor(ShadowColor, GruvColor.GetGColor(shadowColor));
  }
}