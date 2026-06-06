using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    public float lifeTime = 0.35f;
    public float startScale = 0.3f;
    public float endScale = 1.6f;

    private float timer = 0f;
    private Renderer effectRenderer;
    private Material effectMaterial;

    void Start()
    {
        effectRenderer = GetComponent<Renderer>();

        if (effectRenderer != null)
        {
            effectMaterial = new Material(Shader.Find("Standard"));
            effectMaterial.color = new Color(1f, 0.25f, 0f, 0.7f);
            effectMaterial.EnableKeyword("_EMISSION");
            effectMaterial.SetColor("_EmissionColor", new Color(1f, 0.2f, 0f) * 2.5f);
            effectRenderer.material = effectMaterial;
        }

        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float progress = timer / lifeTime;

        transform.localScale = Vector3.Lerp(
            Vector3.one * startScale,
            Vector3.one * endScale,
            progress
        );

        if (effectMaterial != null)
        {
            Color c = effectMaterial.color;
            c.a = Mathf.Lerp(0.7f, 0f, progress);
            effectMaterial.color = c;
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}