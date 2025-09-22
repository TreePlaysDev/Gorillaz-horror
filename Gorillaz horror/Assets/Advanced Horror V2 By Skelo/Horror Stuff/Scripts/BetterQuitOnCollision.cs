using UnityEngine;

public class BetterQuitOnCollision : MonoBehaviour
{
    [Header("Created By Eternal, Better QuitOnCollision. If you delete this header your a skid.")]
    public GameObject jumpscareObject;
    public float delayBeforeQuit = 2.0f;
    public string targetPlayerName = "GorillaPlayer";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            if (collision.gameObject.name == targetPlayerName)
            {
                EnableJumpscare();
                Invoke("QuitGame", delayBeforeQuit);
            }
        }
    }

    private void EnableJumpscare()
    {
        if (jumpscareObject != null)
        {
            jumpscareObject.SetActive(true);
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}