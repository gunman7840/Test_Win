using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

    public GameObject _gameManager;

    void Awake()
    {
        if (GameManager.instance == null)
        Instantiate(_gameManager);
    }


}
