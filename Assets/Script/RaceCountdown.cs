using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private float _countdownDuration = 3f;
    [SerializeField] private float _boostThreshold = 0.2f;
    [SerializeField] private float _boostMultiplier = 1.5f;
    [SerializeField] private float _boostDuration = 2f;

    private bool _isCounting = true;
    private bool _canGetBoost = false;
    private CarControler[] _allCars;

    private void Start()
    {
        _allCars = FindObjectsByType<CarControler>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        FreezeAllCars(true);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
    _countdownText.text = "3";
    yield return new WaitForSeconds(1f);
    
    _countdownText.text = "2";
    yield return new WaitForSeconds(1f);

    //Activation boost possible
    _countdownText.text = "1";
    _canGetBoost = true;
    yield return new WaitForSeconds(1f);

    //Déblocage des voitures
    _countdownText.text = "GO!";
    FreezeAllCars(false); // Débloque toutes les voitures
    _isCounting = false;
    _canGetBoost = false;
    
    yield return new WaitForSeconds(0.5f);
    _countdownText.gameObject.SetActive(false);
    }

    private void FreezeAllCars(bool freeze)
    {
        foreach (CarControler car in _allCars)
    {
        car.SetCanMove(!freeze);
        if (freeze) 
        {
            car.FullReset(); // Reset complet 
        }
        else
        {
            //petite impulsion au départ
            car.GetComponent<Rigidbody>().AddForce(car.transform.forward * 5f, ForceMode.Impulse);
        }
    }
    }

    public void TryGetBoost(string accelerateInputName)
    {
        if (!_canGetBoost || !_isCounting) return;

    float timingAccuracy = Mathf.Abs(Time.time - (_countdownDuration - 1f));
    
    if (timingAccuracy <= _boostThreshold && Input.GetButtonDown(accelerateInputName))
    {
        CarControler playerCar = GetComponent<CarControler>();
        if (playerCar != null)
        {
            playerCar.ActivateTurbo(_boostMultiplier, _boostDuration);
            //Debug.Log("Good start");
        }
    }
    }
}