using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private int msTillUpdate = 20;

    private NetworkManager instance;

    public int MsTillUpdate { get => msTillUpdate;}

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);


    }
}
