using UnityEngine;

public class WebButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
    gameObject.SetActive(false);
#else
    gameObject.SetActive(true);
#endif
    }
}
