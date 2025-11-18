using System.Collections.Generic;
using UnityEngine;

public class ItemsAndEquipmentResourcesCache : MonoBehaviour
{
    public static ItemsAndEquipmentResourcesCache Instance { get; private set; }
    public readonly Dictionary<string, Sprite> equipments = new Dictionary<string, Sprite>();
    public readonly Dictionary<string, Sprite> items = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetEquipmentSprite(string id)
    {
        if (!equipments.ContainsKey(id))
        { 
            equipments.Add(id, Resources.Load<Sprite>($"Equipments/{id}")); 
        }
        return equipments[id]; 
    }

    public Sprite GetItemSprite(string id)
    {
        if (!items.ContainsKey(id))
        {
            items.Add(id, Resources.Load<Sprite>($"Items/{id}"));
        }
        return items[id];
    }

    private void OnDestroy()
    {
        foreach(var entry in equipments)
        {
            Resources.UnloadAsset(entry.Value);
        }

        foreach (var entry in items)
        {
            Resources.UnloadAsset(entry.Value);
        }
    }
}
