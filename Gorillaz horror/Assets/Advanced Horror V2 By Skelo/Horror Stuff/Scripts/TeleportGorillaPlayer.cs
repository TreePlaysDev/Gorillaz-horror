using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGorillaPlayer : MonoBehaviour
{
    public Rigidbody GorillaPlayer;
    public GameObject[] ObjectsToDisable;
    public Transform TeleportLocation;
    public float WaitTime;
    public GameObject TeleportOverlay;
    public AudioSource TeleportSound;

    private LayerMask defaultLayers;

    void Start()
    {
        defaultLayers = GorillaLocomotion.Player.Instance.locomotionEnabledLayers;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("jumpscare"))
        {
            TeleportOverlay.SetActive(true);
            TeleportSound.Play();

            foreach (GameObject obj in ObjectsToDisable)
            {
                obj.SetActive(false);
            }

            StartCoroutine(TPWD());
        }
    }

    IEnumerator TPWD()
    {
        yield return new WaitForSeconds(WaitTime);

        // disable collisions
        GorillaLocomotion.Player.Instance.locomotionEnabledLayers = 0;
        GorillaLocomotion.Player.Instance.headCollider.enabled = false;
        GorillaLocomotion.Player.Instance.bodyCollider.enabled = false;

        // teleport
        GorillaPlayer.position = TeleportLocation.position;
        GorillaPlayer.velocity = Vector3.zero;

        yield return new WaitForSeconds(WaitTime);

        // re-enable collisions
        GorillaLocomotion.Player.Instance.locomotionEnabledLayers = defaultLayers;
        GorillaLocomotion.Player.Instance.headCollider.enabled = true;
        GorillaLocomotion.Player.Instance.bodyCollider.enabled = true;

        foreach (GameObject obj in ObjectsToDisable)
        {
            obj.SetActive(true);
        }

        TeleportOverlay.SetActive(false);
    }
}
