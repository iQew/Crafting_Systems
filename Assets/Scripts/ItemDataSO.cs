using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data Object", menuName = "Item Data")]
public class ItemDataSO : ScriptableObject {

    public int ID;
    public string Name;
    public string Description;
    public Sprite Artwork;
    public int StackSize = 1;

}