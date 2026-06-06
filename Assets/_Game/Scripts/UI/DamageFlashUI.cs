using UnityEngine;

public class DamageFlashUI : MonoBehaviour
{
    public static DamageFlashUI Instance { get; private set; }

    [Header("ÊÜ»÷ÉÁÆÁ²ÎÊý")]
    public Color flashColor = new Color(0.65f, 0f, 0f, 0.35f);
    public float fadeSpeed = 2.8f;
    public float maxAlpha = 0.35f;

    private Texture2D redTexture;
    private float currentAlpha = 0f;

    void Awake()
    {
        Instance = this;

        redTexture = new Texture2D(1, 1);
        redTexture.SetPixel(0, 0, Color.white);
        redTexture.Apply();
    }

    void Update()
    {
        if (currentAlpha > 0f)
        {
            currentAlpha -= fadeSpeed * Time.unscaledDeltaTime;

            if (currentAlpha < 0f)
            {
                currentAlpha = 0f;
            }
        }
    }

    public void ShowDamageFlash()
    {
        currentAlpha = maxAlpha;
    }

    void OnGUI()
    {
        if (currentAlpha <= 0f)
        {
            return;
        }

        Color drawColor = flashColor;
        drawColor.a = currentAlpha;

        GUI.color = drawColor;

        GUI.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height),
            redTexture
        );

        GUI.color = Color.white;
    }
}