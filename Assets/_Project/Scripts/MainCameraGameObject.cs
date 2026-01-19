using UnityEngine;

namespace AndrzejKebab;

public class MainCameraGameObject : MonoBehaviour
{
    public static Camera Instance;

    private void Awake()
    {
        Instance = GetComponent<Camera>();
    }
}