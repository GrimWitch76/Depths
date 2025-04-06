using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType type;
    public Sprite[] _upgradeSprites;
    public int level = 0;
    public int maxLevel = 5;
    public int[] costPerLevel;
    public float[] valuePerLevel;
}
