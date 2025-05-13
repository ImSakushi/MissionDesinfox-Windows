using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EscapeQuitter : MonoBehaviour
{
    // Singleton pour n'avoir qu'une seule instance persistante
    private static EscapeQuitter _instance;

    void Awake()
    {
        // Si une autre instance existe déjà, on se détruit
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // À chaque frame, on vérifie si la touche Échap vient d'être pressée
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    private void QuitGame()
    {
        Debug.Log("Escape détecté : fermeture du jeu.");

        #if UNITY_EDITOR
        // Dans l'éditeur, on stoppe simplement le Play Mode
        EditorApplication.isPlaying = false;
        #else
        // En build, on ferme l'application
        Application.Quit();
        #endif
    }
}
