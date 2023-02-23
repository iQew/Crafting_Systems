using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory Item")]
public class InventoryItem : ScriptableObject {

    public int ID;
    public new string name;
    public string description;
    public Sprite artwork;
}