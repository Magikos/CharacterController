using UnityEngine;

public class EventBusRuntime : MonoBehaviour
{
    private static EventBusRuntime _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        EventBusRuntimeProxy.ProcessAll();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        var go = new GameObject("[EventBusRuntime]");
        go.AddComponent<EventBusRuntime>();
    }
}
