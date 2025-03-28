using UnityEngine;

[CreateAssetMenu(fileName = "ItemBoost", menuName = "Scriptable Objects/ItemBoost")]
public class ItemBoost : Item
{
    public float turboMultiplier = 2f; // Multiplicateur de vitesse
    public float duration = 3f; // Durée du turbo

    public override void Activation(PlayerItemManager player)
    {
        player.carControler.ActivateTurbo(turboMultiplier, duration);
    }
}
