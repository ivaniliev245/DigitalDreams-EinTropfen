using UnityEngine;

public class TextureOffset : MonoBehaviour
{
    private Material material;
    public float scrollSpeed = 0.1f;
    private Vector2 currentOffset;
    public Vector2 textureTiling = new Vector2(1, 1);

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        currentOffset.x += scrollSpeed * Time.deltaTime;

        // Scroll _MainTex
        if (material.HasProperty("_MainTex"))
        {
            material.SetTextureOffset("_MainTex", currentOffset);
            material.SetTextureScale("_MainTex", textureTiling);
        }

        // Scroll _BaseMap
        if (material.HasProperty("_BaseMap"))
        {
            material.SetTextureOffset("_BaseMap", currentOffset);
            material.SetTextureScale("_BaseMap", textureTiling);
        }

        // Scroll _1st_ShadeMap
        if (material.HasProperty("_1st_ShadeMap"))
        {
            material.SetTextureOffset("_1st_ShadeMap", currentOffset);
            material.SetTextureScale("_1st_ShadeMap", textureTiling);
        }

        // Scroll _2nd_ShadeMap
        if (material.HasProperty("_2nd_ShadeMap"))
        {
            material.SetTextureOffset("_2nd_ShadeMap", currentOffset);
            material.SetTextureScale("_2nd_ShadeMap", textureTiling);
        }


    }
}
