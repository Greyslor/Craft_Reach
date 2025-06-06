using Mirror;
using UnityEngine;

public class sceneScript : NetworkBehaviour
{
    [Header("Referencias UI")]
    public TMPro.TextMeshProUGUI canvasStatusText;
    public TMPro.TextMeshProUGUI canvasBulletsText;

    // Referencia al jugador local
    public NetworkPlayer plMove;

    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string statusText;

    void OnStatusTextChanged(string _old, string _new)
    {
        canvasStatusText.text = _new;
    }

    // Mostrar balas restantes
    public void ammoUI(float ammo)
    {
        canvasBulletsText.text = "Balas restantes: " + ammo;
    }

    // Llamado por boton para testear si esta enlazado
    public void ButtonSendMessage()
    {
        if (plMove != null)
        {
            // Aqui puedes agregar algun mensaje o comando personalizado
            Debug.Log("Botón presionado desde UI");
        }
    }
}
