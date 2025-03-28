using UnityEngine;

public class SpeedZone : MonoBehaviour
{
    [SerializeField] private float _speedReductionFactor = 0.5f; // Réduction de moitié de base 

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Objet entré : {other.name}");
        CarControler car = other.GetComponent<CarControler>();
        if (car != null)
        {
            //Debug.Log("Voiture détectée !");
            car.ApplySpeedReduction(_speedReductionFactor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarControler car = other.GetComponent<CarControler>();
        if (car != null)
        {
            car.ResetSpeedReduction();
        }
    }
    
}