using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour, IResourceReceiver {

    public List<ResourceNode> ResourceNodes;
    public PlayerData PlayerData;

    private void Start() {
        for (int i = 0; i < ResourceNodes.Count; i++) {
            ResourceNodes[i].ResourceReceiver = this;
        }
    }

    public void OnResourceDestruction(ItemDataContainer droppedItem) {
        Debug.Log("KB: Adding " + droppedItem.Quantity + " " + droppedItem.Name + " to PlayerData");
        PlayerData.AddItem(droppedItem);
    }
}
