using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door3DOpening : MonoBehaviour
{
    public Collider2D doorCollider;
    public LevelManager levelManager;
    public Animator doorFragAnim;
    // public Animator doorScreenAnim;
    bool _playerNear;

    void Update()
    {
        if(levelManager.switchesPressed == 2) {
            // doorFragAnim.Play("3DDoorUnlockedKeyboard");
        }
        if(_playerNear && Input.GetButtonDown("Interact"))
        {
            // levelManager.LevelFinished();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (levelManager.switchesPressed == 2 && coll.CompareTag("Player"))
        {
            Debug.Log("You are gay");
            _playerNear = true;
            doorFragAnim.Play("3DDoorOpen");
            //upperDoorFragAnim.SetBool("isOpen", true);
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        if(coll.CompareTag("Player"))
        {
            Debug.Log("You are not gay");
            _playerNear = false;
            doorFragAnim.Play("3DDoorClose");
        }
    }
}
