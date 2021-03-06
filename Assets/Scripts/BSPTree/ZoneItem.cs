﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZoneItem : MonoBehaviour
{
    public DungeonSet spawnSet = null;
    public bool randomizeSpawnSet;
    public bool emptySpace;

    public Color colorPicker;

    private Rect rect;
    private Color currentColor = new Color(-1, 0, 0);
    private Rect bounds;

    private Rect gizmoRect;
    private bool selected;

    public Rect Rect
    {
        get => rect;
        set
        {
            rect = value;

            Transform parentTransform = transform.parent.transform;

            float width = rect.width;
            float height = rect.height;

            float boundX = rect.x - parentTransform.localScale.x * 0.5f;
            float boundZ = rect.y - parentTransform.localScale.y * 0.5f;

            bounds = new Rect(boundX, boundZ, width, height);

            float correctedX = boundX + width * 0.5f;
            float correctedZ = boundZ + height * 0.5f;

            gizmoRect = new Rect(correctedX, correctedZ, width, height);

            float parentPosY = parentTransform.position.y;

            transform.position = new Vector3(correctedX, parentPosY, correctedZ);
            transform.localScale = new Vector3(width, height, 1f);

            AddCollider();
        }
    }

    public Rect GizmoRect => gizmoRect;

    public bool Selected { get => selected; set => selected = value; }

    private void Update()
    {
        bool alphaChange = currentColor.a != (emptySpace ? 0f : 1f);

        if (currentColor.r < 0)
        {
            UpdateCurrentColor();
            return;
        }        

        bool colorChanged = currentColor.r != colorPicker.r || currentColor.g != colorPicker.g || currentColor.b != colorPicker.b;

        if (colorChanged || alphaChange)
        {
            UpdateCurrentColor();

            ZonesGenerator itemParent = transform.parent.GetComponent<ZonesGenerator>();
            itemParent.SetSubTextureColor(rect, currentColor);
            itemParent.TextureApply();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public bool ContainsPoint(Vector3 point)
    {
        Vector2 point2D = new Vector2(point.x, point.z);

        bool containes = bounds.Contains(point2D);

        return containes;
    }

    protected void UpdateCurrentColor()
    {
        currentColor.r = colorPicker.r;
        currentColor.g = colorPicker.g;
        currentColor.b = colorPicker.b;
        currentColor.a = emptySpace ? 0f : 1f;
    }

    protected void AddCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

        float scaleX = transform.parent.localScale.x;
        float scaleY = transform.parent.localScale.y;

        boxCollider.size = new Vector3(1.0f / scaleX, 1.0f / scaleY, 1.0f);
    }

#if UNITY_EDITOR
    void OnMouseDown()
    {
        Selection.activeInstanceID = gameObject.GetInstanceID();
        Selection.activeGameObject = gameObject;

        ZonesGenerator itemParent = transform.parent.GetComponent<ZonesGenerator>();
        itemParent.UnselectAllChildren();

        Selected = !Selected;
    }
#endif
}
