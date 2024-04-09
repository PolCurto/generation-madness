using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [SerializeField] private RectTransform _progress;
    [SerializeField] private float _rotateSpeed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        _progress.Rotate(0f, 0f, -_rotateSpeed * Time.deltaTime);
    }
}
