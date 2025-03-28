using UnityEngine;

public class Oil : MonoBehaviour
{
    
    [SerializeField] private float _driftForce = 2f;
    [SerializeField] private ParticleSystem _oilSplashEffect;

    private void OnTriggerEnter(Collider other)
    {
        CarControler car = other.GetComponent<CarControler>();
        if (car != null)
        {
            // Force le drift
            car.ForceDrift(_driftForce);
            
            // Effet visuel
            if (_oilSplashEffect != null)
            {
                ParticleSystem splash = Instantiate(_oilSplashEffect, 
                    other.transform.position, 
                    Quaternion.identity);
                Destroy(splash.gameObject, 2f);
            }
        }
    }
}
