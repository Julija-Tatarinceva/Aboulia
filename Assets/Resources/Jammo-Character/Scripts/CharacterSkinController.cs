using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinController : MonoBehaviour
{
    Animator _animator;
    Renderer[] _characterMaterials;

    public Texture2D[] albedoList;
    [ColorUsage(true,true)]
    public Color[] eyeColors;
    public enum EyePosition { Normal, Happy, Angry, Dead}
    public EyePosition eyeState;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterMaterials = GetComponentsInChildren<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //ChangeMaterialSettings(0);
            ChangeEyeOffset(EyePosition.Normal);
            ChangeAnimatorIdle("normal");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //ChangeMaterialSettings(1);
            ChangeEyeOffset(EyePosition.Angry);
            ChangeAnimatorIdle("angry");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //ChangeMaterialSettings(2);
            ChangeEyeOffset(EyePosition.Happy);
            ChangeAnimatorIdle("happy");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //ChangeMaterialSettings(3);
            ChangeEyeOffset(EyePosition.Dead);
            ChangeAnimatorIdle("dead");
        }
    }

    void ChangeAnimatorIdle(string trigger)
    {
        _animator.SetTrigger(trigger);
    }

    void ChangeMaterialSettings(int index)
    {
        for (int i = 0; i < _characterMaterials.Length; i++)
        {
            if (_characterMaterials[i].transform.CompareTag("PlayerEyes"))
                _characterMaterials[i].material.SetColor("_EmissionColor", eyeColors[index]);
            else
                _characterMaterials[i].material.SetTexture("_MainTex",albedoList[index]);
        }
    }

    void ChangeEyeOffset(EyePosition pos)
    {
        Vector2 offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.Normal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.Happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.Angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.Dead:
                offset = new Vector2(.33f, .66f);
                break;
            default:
                break;
        }

        for (int i = 0; i < _characterMaterials.Length; i++)
        {
            if (_characterMaterials[i].transform.CompareTag("PlayerEyes"))
                _characterMaterials[i].material.SetTextureOffset("_MainTex", offset);
        }
    }
}
