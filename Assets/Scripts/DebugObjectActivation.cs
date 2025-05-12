using UnityEngine;

public class DebugObjectActivation : MonoBehaviour {
    void OnEnable() {
        Debug.Log("ACTIVÉ: " + gameObject.name, gameObject);
    }

    void Start() {
        Debug.Log("Start appelé sur " + gameObject.name, gameObject);
    }
}
