using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZonesGenerator : MonoBehaviour
{
    public int minZoneSize;
    public int maxZoneSize;

    public int zoneHeight = 10;

    public bool clamp;

    public bool RandomizeSpawnSets;    

    public Button genButton;

    public ZoneItem zonePrefab;

    private List<ZoneItem> spawnZones = new List<ZoneItem>();
    private SubZone rootZone;
    private Texture2D texture;    

    public List<ZoneItem> SpawnZones => spawnZones;

    void Start()
    {
        Generate();

        if (genButton != null)
        {
            genButton.onClick.AddListener(Generate);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }

    public void UnselectAllChildren()
    {
        int length = SpawnZones.Count;
        for (int i = 0; i < length; i++)
        {
            ZoneItem zone = SpawnZones[i];
            zone.Selected = false;
        }
    }

    public void Generate()
    {
        Rect rect = new Rect(0, 0, transform.localScale.x, transform.localScale.y);

        int length = SpawnZones.Count;
        for (int i = 0; i < length; i++)
        {
            SpawnZones[i].Die();
        }

        spawnZones = new List<ZoneItem>();

        texture = new Texture2D((int)rect.width, (int)rect.height)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        rootZone = new SubZone(rect);
        CreateBSP(rootZone);

        TextureApply();

        GetComponent<Renderer>().material.mainTexture = texture;
    }

    public void CreateBSP(SubZone subDungeon)
    {
        if (subDungeon.IAmLeaf())
        {
            //partition if the sub-dungeon is too large
            if (subDungeon.rect.width > maxZoneSize || subDungeon.rect.height > maxZoneSize)
            {                
                if (subDungeon.Split(minZoneSize, maxZoneSize))
                {
                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
                else
                {
                    Color color = Random.ColorHSV();
                    
                    SetSubTextureColor(subDungeon.rect, color);

                    ZoneItem zone = Instantiate(zonePrefab, transform);

                    zone.colorPicker = color;
                    zone.Rect = subDungeon.rect;
                    zone.name = "Zone " + (SpawnZones.Count + 1) + " - " + zone.transform.position;

                    zone.randomizeSpawnSet = RandomizeSpawnSets;

                    SpawnZones.Add(zone);
                }
            }
        }
    }

    public void SetSubTextureColor(Rect rect, Color color)
    {        
        int width = (int)rect.width;
        for (int i = 0; i < width; i++)
        {
            int height = (int)rect.height;
            for (int j = 0; j < height; j++)
            {
                texture.SetPixel((int)rect.x + i, (int)rect.y + j, color);                
            }
        }
    }

    public void TextureApply()
    {
        texture.Apply();
    }

    void OnDrawGizmos()
    {
        if (rootZone == null) return;

        Gizmos.color = Color.cyan;

        int length = SpawnZones.Count;
        for (int i = 0; i < length; i++)
        {
            ZoneItem zone = SpawnZones[i];

            if (zone.Selected) continue;

            Rect rect = zone.GizmoRect;

            Vector3 size = new Vector3(rect.width, zoneHeight, rect.height);
            Vector3 pos = new Vector3(rect.x, 0f, rect.y);

            Gizmos.DrawWireCube(pos, size);
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < length; i++)
        {
            ZoneItem zone = SpawnZones[i];

            if (!zone.Selected) continue;

            Rect bounds = zone.GizmoRect;

            Vector3 size = new Vector3(bounds.width, zoneHeight, bounds.height);
            Vector3 pos = new Vector3(bounds.x, 0f, bounds.y);

            Gizmos.DrawWireCube(pos, size);
        }
    }
}