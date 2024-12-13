using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Brightness")]
public class Brightness : MonoBehaviour
{

    /// Provides a shader property that is set in the inspector
    /// and a material instantiated from the shader
    public Shader shaderDerp;

    private Material _mMaterial;

    [Range(0.5f, 2f)]
    public float brightness = 1f;

    private void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        // Disable the image effect if the shader can't
        // run on the users graphics card
        if (!shaderDerp || !shaderDerp.isSupported)
            enabled = false;
    }


    private Material Material
    {
        get
        {
            if (_mMaterial == null)
            {
                _mMaterial = new Material(shaderDerp);
                _mMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _mMaterial;
        }
    }


    private void OnDisable()
    {
        if (_mMaterial)
        {
            DestroyImmediate(_mMaterial);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Material.SetFloat("_Brightness", brightness);
        Graphics.Blit(source, destination, Material);
    }
}
