using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data Object", menuName = "Item Data")]
public class ItemDataSO : ScriptableObject {

    public enum QualityType { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY }
        
    public int ID;
    public string Name;
    public string Description;
    public Sprite Artwork;
    public QualityType Quality;
    public int StackSize = 1;

}