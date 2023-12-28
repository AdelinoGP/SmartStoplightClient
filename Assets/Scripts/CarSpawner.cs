using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private CarBehaviour carPrefab;
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    bool spawnPoint1Used = false;

    public void SpawnCar()
    {
        var spawnedCar = Instantiate(carPrefab, transform);
        var currentSpawnPoint = spawnPoint1Used ? spawnPoint1 : spawnPoint2;
        spawnedCar.transform.SetPositionAndRotation(currentSpawnPoint.position, currentSpawnPoint.rotation);
        spawnPoint1Used = !spawnPoint1Used;
    }
}
