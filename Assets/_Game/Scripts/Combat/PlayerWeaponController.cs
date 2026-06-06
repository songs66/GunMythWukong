using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("摄像机引用")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    [Header("枪口引用")]
    public Transform muzzlePoint;

    [Header("武器参数")]
    public float damage = 25f;
    public float range = 100f;
    public float fireInterval = 0.15f;

    [Header("射击层级")]
    public LayerMask hitLayers = ~0;

    [Header("子弹轨迹显示")]
    public bool showBulletTracer = true;
    public float tracerDuration = 0.06f;
    public float tracerWidth = 0.045f;

    [Header("枪口火光")]
    public bool showMuzzleFlash = true;
    public float muzzleFlashDuration = 0.05f;
    public float muzzleFlashSize = 0.18f;
    public float muzzleFlashLightIntensity = 5f;
    public float muzzleFlashLightRange = 3f;

    [Header("开枪音效")]
    public bool enableGunSound = true;
    public float gunSoundVolume = 0.6f;

    private AudioSource audioSource;
    private AudioClip generatedGunShotClip;

    private float nextFireTime = 0f;

    private LineRenderer tracerLine;
    private GameObject muzzleFlashObject;
    private Light muzzleFlashLight;

    void Start()
    {
        CreateTracerLine();
        CreateMuzzleFlash();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        generatedGunShotClip = GenerateGunShotClip();
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            TryFire();
        }
    }

    void TryFire()
    {
        if (Time.time < nextFireTime)
        {
            return;
        }

        nextFireTime = Time.time + fireInterval;
        Fire();
    }

    void Fire()
    {
        Camera activeCamera = GetActiveCamera();

        if (activeCamera == null)
        {
            Debug.LogWarning("没有找到当前启用的摄像机，无法射击。");
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogWarning("没有设置 MuzzlePoint，无法从枪口发射。");
            return;
        }

        /*
         * 第一步：从摄像机中心发射一条瞄准射线。
         * 目的：确定准星真正瞄准的位置。
         */
        Ray cameraRay = activeCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint;

        RaycastHit? cameraHit = FindFirstValidHit(cameraRay);

        if (cameraHit.HasValue)
        {
            aimPoint = cameraHit.Value.point;
        }
        else
        {
            aimPoint = cameraRay.origin + cameraRay.direction * range;
        }

        /*
         * 第二步：真正的子弹从枪口发出，方向指向准星瞄准点。
         */
        Vector3 shootOrigin = muzzlePoint.position;
        Vector3 shootDirection = (aimPoint - shootOrigin).normalized;

        Ray muzzleRay = new Ray(shootOrigin, shootDirection);

        RaycastHit? finalHit = FindFirstValidHit(muzzleRay);

        Vector3 endPoint;

        if (finalHit.HasValue)
        {
            RaycastHit hit = finalHit.Value;
            endPoint = hit.point;

            Debug.DrawLine(shootOrigin, hit.point, Color.red, 1f);

            Debug.Log("射击命中：" + hit.collider.name);

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);

                // 只有命中敌人时，才触发准星命中反馈
                EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null && CrosshairUI.Instance != null)
                {
                    CrosshairUI.Instance.ShowHitMarker();
                }
            }
        }
        else
        {
            endPoint = shootOrigin + shootDirection * range;

            Debug.DrawRay(shootOrigin, shootDirection * range, Color.yellow, 1f);

            Debug.Log("射击未命中");
        }

        if (showBulletTracer)
        {
            ShowTracer(shootOrigin, endPoint);
        }

        if (showMuzzleFlash)
        {
            ShowMuzzleFlash();
        }

        PlayGunSound();
    }

    RaycastHit? FindFirstValidHit(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            range,
            hitLayers,
            QueryTriggerInteraction.Ignore
        );

        if (hits.Length == 0)
        {
            return null;
        }

        hits = hits.OrderBy(h => h.distance).ToArray();

        foreach (RaycastHit hit in hits)
        {
            // 跳过 Player 自己以及 Player 的子物体
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hit.collider.transform.root == transform)
            {
                continue;
            }

            return hit;
        }

        return null;
    }

    Camera GetActiveCamera()
    {
        if (firstPersonCamera != null && firstPersonCamera.gameObject.activeInHierarchy)
        {
            return firstPersonCamera;
        }

        if (thirdPersonCamera != null && thirdPersonCamera.gameObject.activeInHierarchy)
        {
            return thirdPersonCamera;
        }

        return null;
    }

    void CreateTracerLine()
    {
        GameObject tracerObject = new GameObject("BulletTracer");
        tracerObject.transform.SetParent(transform);

        tracerLine = tracerObject.AddComponent<LineRenderer>();

        tracerLine.positionCount = 2;
        tracerLine.startWidth = tracerWidth;
        tracerLine.endWidth = tracerWidth * 0.35f;
        tracerLine.useWorldSpace = true;
        tracerLine.textureMode = LineTextureMode.Stretch;

        /*
         * 使用红黄色渐变：
         * 起点靠近枪口：亮黄色
         * 终点靠近命中点：橙红色
         */
        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1f, 0.95f, 0.1f), 0f),   // 亮黄
                new GradientColorKey(new Color(1f, 0.45f, 0.0f), 0.45f), // 橙色
                new GradientColorKey(new Color(1f, 0.05f, 0.0f), 1f)     // 红色
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.9f, 0.5f),
                new GradientAlphaKey(0.1f, 1f)
            }
        );

        tracerLine.colorGradient = gradient;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        tracerLine.material = mat;

        tracerLine.enabled = false;
    }

    void CreateMuzzleFlash()
    {
        if (muzzlePoint == null)
        {
            return;
        }

        /*
         * 创建一个小球作为简易枪口火焰。
         * 它不会参与碰撞，只用于短暂显示火光。
         */
        muzzleFlashObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        muzzleFlashObject.name = "MuzzleFlash";
        muzzleFlashObject.transform.SetParent(muzzlePoint);
        muzzleFlashObject.transform.localPosition = Vector3.zero;
        muzzleFlashObject.transform.localRotation = Quaternion.identity;
        muzzleFlashObject.transform.localScale = Vector3.one * muzzleFlashSize;

        Collider col = muzzleFlashObject.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        Renderer renderer = muzzleFlashObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material flashMat = new Material(Shader.Find("Standard"));
            flashMat.color = new Color(1f, 0.55f, 0.05f);
            flashMat.EnableKeyword("_EMISSION");
            flashMat.SetColor("_EmissionColor", new Color(1f, 0.35f, 0.02f) * 3f);
            renderer.material = flashMat;
        }

        muzzleFlashLight = muzzleFlashObject.AddComponent<Light>();
        muzzleFlashLight.type = LightType.Point;
        muzzleFlashLight.color = new Color(1f, 0.45f, 0.05f);
        muzzleFlashLight.intensity = muzzleFlashLightIntensity;
        muzzleFlashLight.range = muzzleFlashLightRange;

        muzzleFlashObject.SetActive(false);
    }

    void ShowTracer(Vector3 startPoint, Vector3 endPoint)
    {
        if (tracerLine == null)
        {
            return;
        }

        StopCoroutineIfRunning();
        StartCoroutine(ShowTracerCoroutine(startPoint, endPoint));
    }

    IEnumerator ShowTracerCoroutine(Vector3 startPoint, Vector3 endPoint)
    {
        tracerLine.startWidth = tracerWidth;
        tracerLine.endWidth = tracerWidth * 0.35f;

        tracerLine.SetPosition(0, startPoint);
        tracerLine.SetPosition(1, endPoint);

        tracerLine.enabled = true;

        yield return new WaitForSeconds(tracerDuration);

        tracerLine.enabled = false;
    }

    void ShowMuzzleFlash()
    {
        if (muzzleFlashObject == null)
        {
            CreateMuzzleFlash();
        }

        if (muzzleFlashObject == null)
        {
            return;
        }

        StartCoroutine(ShowMuzzleFlashCoroutine());
    }

    IEnumerator ShowMuzzleFlashCoroutine()
    {
        /*
         * 每次开枪随机改变一点大小，让火光看起来不那么机械。
         */
        float randomScale = Random.Range(0.8f, 1.3f);
        muzzleFlashObject.transform.localScale = Vector3.one * muzzleFlashSize * randomScale;

        muzzleFlashObject.SetActive(true);

        yield return new WaitForSeconds(muzzleFlashDuration);

        muzzleFlashObject.SetActive(false);
    }

    void StopCoroutineIfRunning()
    {
        /*
         * 这里为了简单，停止本脚本上所有协程。
         * 当前脚本只有子弹轨迹和枪口火光两个短协程，影响不大。
         */
        StopAllCoroutines();

        if (tracerLine != null)
        {
            tracerLine.enabled = false;
        }

        if (muzzleFlashObject != null)
        {
            muzzleFlashObject.SetActive(false);
        }
    }

    void PlayGunSound()
    {
        if (!enableGunSound)
        {
            return;
        }

        if (audioSource != null && generatedGunShotClip != null)
        {
            audioSource.PlayOneShot(generatedGunShotClip, gunSoundVolume);
        }
    }

    AudioClip GenerateGunShotClip()
    {
        int sampleRate = 44100;
        float duration = 0.12f;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);

        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;

            // 快速衰减，让声音像短促枪响
            float envelope = Mathf.Exp(-35f * t);

            // 噪声 + 低频冲击混合
            float noise = Random.Range(-1f, 1f);
            float punch = Mathf.Sin(2f * Mathf.PI * 90f * t);

            samples[i] = (noise * 0.75f + punch * 0.25f) * envelope;
        }

        AudioClip clip = AudioClip.Create(
            "GeneratedGunShot",
            sampleCount,
            1,
            sampleRate,
            false
        );

        clip.SetData(samples, 0);

        return clip;
    }
}