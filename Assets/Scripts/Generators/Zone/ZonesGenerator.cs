using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Generators.Zone
{
    public class ZonesGenerator : MonoBehaviour
    {
        private static readonly string NAME = "Zone";
        public int MinZoneSize;
        public int MaxZoneSize;
        public int ZoneHeight = 10;
        public bool Clamp;
        public bool RandomizeSpawnSets;
        public Button GenButton;
        public ZoneItem ZonePrefab;

        private List<ZoneItem> spawnZones = new();
        private SubZone rootZone;
        private Texture2D texture;

        public List<ZoneItem> SpawnZones => spawnZones;

        void Start()
        {
            Generate();

            if (GenButton != null)
            {
                GenButton.onClick.AddListener(Generate);
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
            spawnZones.ForEach(zone => zone.Selected = false);
        }

        public void Generate()
        {
            Rect rect = new(0, 0, transform.localScale.x, transform.localScale.y);

            SpawnZones.ForEach(zone => zone.Die());

            spawnZones = new List<ZoneItem>();

            texture = new Texture2D((int)rect.width, (int)rect.height)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };

            rootZone = new SubZone(rect);

            SplitZone(rootZone);

            TextureApply();

            GetComponent<Renderer>().material.mainTexture = texture;
        }

        public void SplitZone(SubZone subDungeon)
        {
            if (subDungeon.IsLeaf() && subDungeon.rect.width > MaxZoneSize || subDungeon.rect.height > MaxZoneSize)
            {
                if (subDungeon.Split(MinZoneSize, MaxZoneSize))
                {
                    SplitZone(subDungeon.left);
                    SplitZone(subDungeon.right);
                }
                else
                {
                    Color color = Random.ColorHSV();

                    SetSubTextureColor(subDungeon.rect, color);

                    ZoneItem zone = Instantiate(ZonePrefab, transform);

                    zone.colorPicker = color;
                    zone.Rect = subDungeon.rect;
                    zone.name = NAME + (SpawnZones.Count + 1) + " - " + zone.transform.position;

                    zone.randomizeSpawnSet = RandomizeSpawnSets;

                    SpawnZones.Add(zone);
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

            foreach (ZoneItem zone in SpawnZones)
            {
                if (zone.Selected) continue;

                Rect rect = zone.GizmoRect;

                Vector3 size = new(rect.width, ZoneHeight, rect.height);
                Vector3 pos = new(rect.x, 0f, rect.y);

                Gizmos.DrawWireCube(pos, size);
            }

            Gizmos.color = Color.yellow;

            foreach (ZoneItem zone in SpawnZones)
            {
                if (!zone.Selected) continue;

                Rect bounds = zone.GizmoRect;

                Vector3 size = new(bounds.width, ZoneHeight, bounds.height);
                Vector3 pos = new(bounds.x, 0f, bounds.y);

                Gizmos.DrawWireCube(pos, size);
            }
        }
    }
}