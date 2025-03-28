using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public GameObject explosionEffect; // Effet visuel d'explosion
    public float explosionRadius = 5f; // Rayon de l'explosion
    public float explosionForce = 10f; // Force du choc
    public float detonationTime = 3f; // Temps avant explosion

    private bool hasExploded = false;

    public void Activate()
    {
        StartCoroutine(DelayedExplosion());
    }

    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(detonationTime);
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasExploded && other.GetComponent<CarControler>() != null)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        //effet visuel
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Appliquer une force aux joueurs proches
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            CarControler car = hit.GetComponent<CarControler>();
            if (car != null)
            {
                //car.ApplyExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Détruire la bombe après explosion
        Destroy(gameObject);
    }
}
