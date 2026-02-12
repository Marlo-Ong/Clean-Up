using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TintErasable : MonoBehaviour
{
    [Header("Erase Settings")]
    public float eraseRadiusWorld = 0.2f;   // Radius in world units
    [Range(0f, 1f)]
    public float eraseStrength = 0.15f;     // Alpha removed per pass
    public bool softEdges = true;

    SpriteRenderer sr;
    Texture2D runtimeTexture;
    Color[] pixels;
    int texWidth;
    int texHeight;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Clone the sprite texture so we can edit it safely
        Texture2D source = sr.sprite.texture;

        runtimeTexture = new Texture2D(
            source.width,
            source.height,
            TextureFormat.RGBA32,
            false
        );

        runtimeTexture.filterMode = FilterMode.Bilinear;
        runtimeTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] srcPixels = source.GetPixels();
        runtimeTexture.SetPixels(srcPixels);
        runtimeTexture.Apply(false);


        pixels = runtimeTexture.GetPixels();
        texWidth = runtimeTexture.width;
        texHeight = runtimeTexture.height;

        // Create a new sprite using the runtime texture
        sr.sprite = Sprite.Create(
            runtimeTexture,
            sr.sprite.rect,
            sr.sprite.pivot / sr.sprite.rect.size,
            sr.sprite.pixelsPerUnit
        );
    }

    void Update()
    {
        this.EraseAtWorldPosition(this.transform.position);
    }

    public void EraseAtWorldPosition(Vector2 worldPos)
    {
        if (!WorldToPixel(worldPos, out int cx, out int cy))
            return;

        float pixelsPerUnit = sr.sprite.pixelsPerUnit;
        int radiusPx = Mathf.RoundToInt(eraseRadiusWorld * pixelsPerUnit);

        for (int y = -radiusPx; y <= radiusPx; y++)
        {
            int py = cy + y;
            if (py < 0 || py >= texHeight) continue;

            for (int x = -radiusPx; x <= radiusPx; x++)
            {
                int px = cx + x;
                if (px < 0 || px >= texWidth) continue;

                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist > radiusPx) continue;

                float falloff = softEdges
                    ? 1f - (dist / radiusPx)
                    : 1f;

                int index = py * texWidth + px;
                Color c = pixels[index];

                c.a = Mathf.Clamp01(c.a - eraseStrength * falloff);
                pixels[index] = c;
            }
        }

        runtimeTexture.SetPixels(pixels);
        runtimeTexture.Apply(false);
    }

    bool WorldToPixel(Vector2 worldPos, out int px, out int py)
    {
        Vector2 local = transform.InverseTransformPoint(worldPos);

        Bounds b = sr.sprite.bounds;
        Vector2 normalized = new Vector2(
            (local.x + b.extents.x) / b.size.x,
            (local.y + b.extents.y) / b.size.y
        );

        if (normalized.x < 0 || normalized.x > 1 ||
            normalized.y < 0 || normalized.y > 1)
        {
            px = py = 0;
            return false;
        }

        px = Mathf.RoundToInt(normalized.x * texWidth);
        py = Mathf.RoundToInt(normalized.y * texHeight);
        return true;
    }
}
