using UnityEngine;

[CreateAssetMenu(fileName = "Research Item", menuName = "Data/Research Item")]
public class ResearchItemData : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public Sprite sprite;
}