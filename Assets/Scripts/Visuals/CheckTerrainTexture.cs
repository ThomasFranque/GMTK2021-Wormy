using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTerrainTexture : MonoBehaviour
{
    public Terrain t;

    public int posX;
    public int posZ;
    public float[] textureValues;

    private void Awake()
    {
        t = Terrain.activeTerrain;
        UniversalGameData.WalkingColor = Color.white;
    }

    private void Update()
    {
        if (GarryController.Disabled || t == null) return;

        GetTerrainTexture();
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / t.terrainData.size.x, 0,
        terrainPosition.z / t.terrainData.size.z);

        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        textureValues = new float[aMap.GetLength(2)];
        for (int i = 0; i < aMap.GetLength(2); i++)
        {
            textureValues[i] = aMap[0, 0, i];
        }

        float max = 0f;
        int index = 0;
        Color color;
        for (int i = 0; i < textureValues.Length; i++)
        {
            if (textureValues[i] > max)
            {
                max = textureValues[i];
                index = i;
            }
        }

        color = GetColor(t.terrainData.terrainLayers[index].diffuseTexture,
             t.terrainData.terrainLayers[index].tileSize);
        UniversalGameData.WalkingColor = color;
    }

    private Color GetColor(Texture2D tex, Vector2 tiling)
    {
        Color color = Color.white;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20f, ~LayerMask.GetMask("Player")))
        {
            Vector2 coord = hit.textureCoord;
            coord.x *= tex.width;
            coord.y *= tex.height;

            Vector2Int pixel = new Vector2Int(
                Mathf.FloorToInt(coord.x * tiling.x), 
                Mathf.FloorToInt(coord.y * tiling.y));

            Debug.Log(hit.textureCoord + " for pixel: " + pixel);
            color = tex.GetPixel(pixel.x, pixel.y);
            UniversalGameData.WalkingColor = color;
        }

        // // Brighten it up a bit
        // float h;
        // float s;
        // float v;
        // Color.RGBToHSV(color, out h, out s, out v);
        // v = Mathf.Clamp(v + 0.25f, 0, 1);
        // color = Color.HSVToRGB(h, s, v);

        return color;
    }

    public void GetTerrainTexture()
    {
        ConvertPosition(transform.position);
        CheckTexture();
    }
}
