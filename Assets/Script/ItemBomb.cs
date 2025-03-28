using UnityEngine;

[CreateAssetMenu(fileName = "ItemBomb", menuName = "Scriptable Objects/ItemBomb")]
public class ItemBomb : Item
{
    public GameObject bombPrefab;

    public override void Activation(PlayerItemManager player)
{
    if (player.NumberOfItemUse > 0) 
    {
        Vector3 spawnPosition = player.transform.position - player.transform.forward * 2;

        GameObject bomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
        bomb.GetComponent<Bomb>().Activate();

        
    }
}

}

