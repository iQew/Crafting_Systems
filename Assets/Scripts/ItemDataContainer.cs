using UnityEngine;

public struct ItemDataContainer {

    public int ID;
    public string Name;
    public int Quantity;

    public ItemDataContainer (int id, string name, int quantity) {
        ID = id;
        Name = name;
        Quantity = quantity;
    }
}
