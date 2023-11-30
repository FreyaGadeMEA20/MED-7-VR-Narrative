using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeController : MonoBehaviour
{
    private Animator _animator;
    private Image _image;
    public float eyeHeight = 0.9f;
    public float eyeWidth = 1.3f;
    private readonly int _eyeWidth_ID = Shader.PropertyToID("_eyeWidth");
    private readonly int _eyeHeight_ID = Shader.PropertyToID("_eyeHeight");
    private bool isClosed = false;

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _image = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        _image.materialForRendering.SetFloat(_eyeWidth_ID, eyeWidth);
        _image.materialForRendering.SetFloat(_eyeHeight_ID, eyeHeight);
    }


    //access this method to open eye
    public void EyeAnim_Open()
    {
        _animator.SetTrigger("Open");
        isClosed = false;
    }

    //access this method to close eye
    public void EyeAnim_Close()
    {
        _animator.SetTrigger("Close");
        isClosed = true;

    }
}