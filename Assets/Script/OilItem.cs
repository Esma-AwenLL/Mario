using UnityEngine;

[CreateAssetMenu(fileName = "ItemOil", menuName = "Scriptable Objects/ItemOil")]
public class ItemOil : Item
{
    [Header("Oil Settings")]
    public GameObject oilPatchPrefab;
    public float duration = 5f;
    public float spawnDistance = 3f;

    public override void Activation(PlayerItemManager player)
    {
        Vector3 spawnPosition = player.transform.position - player.transform.forward * spawnDistance;
        Quaternion spawnRotation = Quaternion.Euler(0, player.transform.eulerAngles.y + 180f, 0);
        
        GameObject oilPatch = Instantiate(oilPatchPrefab, spawnPosition, spawnRotation);
        Destroy(oilPatch, duration);
    }
}
