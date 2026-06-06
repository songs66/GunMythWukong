using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    public static CrosshairUI Instance { get; private set; }

    [Header("准星开关")]
    public bool showCrosshair = true;

    [Header("普通十字准星")]
    public Color crosshairColor = new Color(1f, 1f, 1f, 0.9f);
    public int lineLength = 12;
    public int lineThickness = 2;
    public int centerGap = 6;

    [Header("命中反馈准星")]
    public Color hitMarkerColor = new Color(1f, 0.85f, 0.1f, 1f);
    public int hitDiagonalLength = 16;
    public float hitMarkerDuration = 0.12f;

    private Texture2D whiteTexture;
    private float hitMarkerTimer = 0f;

    void Awake()
    {
        Instance = this;

        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    void Update()
    {
        if (hitMarkerTimer > 0f)
        {
            hitMarkerTimer -= Time.unscaledDeltaTime;
        }
    }

    public void ShowHitMarker()
    {
        hitMarkerTimer = hitMarkerDuration;
    }

    void OnGUI()
    {
        if (!showCrosshair)
        {
            return;
        }

        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
        {
            return;
        }

        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        bool isHitMarkerActive = hitMarkerTimer > 0f;

        GUI.color = isHitMarkerActive ? hitMarkerColor : crosshairColor;

        DrawNormalCrosshair(centerX, centerY);

        if (isHitMarkerActive)
        {
            DrawHitMarkerDiagonal(centerX, centerY);
        }

        GUI.color = Color.white;
    }

    void DrawNormalCrosshair(float centerX, float centerY)
    {
        // 上
        GUI.DrawTexture(
            new Rect(
                centerX - lineThickness / 2f,
                centerY - centerGap - lineLength,
                lineThickness,
                lineLength
            ),
            whiteTexture
        );

        // 下
        GUI.DrawTexture(
            new Rect(
                centerX - lineThickness / 2f,
                centerY + centerGap,
                lineThickness,
                lineLength
            ),
            whiteTexture
        );

        // 左
        GUI.DrawTexture(
            new Rect(
                centerX - centerGap - lineLength,
                centerY - lineThickness / 2f,
                lineLength,
                lineThickness
            ),
            whiteTexture
        );

        // 右
        GUI.DrawTexture(
            new Rect(
                centerX + centerGap,
                centerY - lineThickness / 2f,
                lineLength,
                lineThickness
            ),
            whiteTexture
        );
    }

    void DrawHitMarkerDiagonal(float centerX, float centerY)
    {
        // 命中时额外画两条斜线，让十字准星变成米字准星
        DrawRotatedLine(centerX, centerY, hitDiagonalLength, lineThickness, 45f);
        DrawRotatedLine(centerX, centerY, hitDiagonalLength, lineThickness, -45f);
    }

    void DrawRotatedLine(float centerX, float centerY, float length, float thickness, float angle)
    {
        Matrix4x4 oldMatrix = GUI.matrix;

        GUIUtility.RotateAroundPivot(angle, new Vector2(centerX, centerY));

        GUI.DrawTexture(
            new Rect(
                centerX - length / 2f,
                centerY - thickness / 2f,
                length,
                thickness
            ),
            whiteTexture
        );

        GUI.matrix = oldMatrix;
    }
}