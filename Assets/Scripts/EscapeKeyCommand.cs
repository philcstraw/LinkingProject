using UnityEngine;

// simple script for exiting the game/returning to main menu when the escape key is pressed.
public class EscapeKeyCommand : MonoBehaviour
{
    public KeyCode quiteKey = KeyCode.Escape;
    public bool returnToRoot;

    void Update ()
    {
        if (Input.GetKeyUp(quiteKey))
        {
            if (!returnToRoot)
                SceneControl.instance.QuitGame();
            else
                SceneControl.instance.ReturnToRoot();
        }
    }
}
