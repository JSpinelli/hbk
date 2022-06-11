using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Global
    public static WaveManager Instance;

    [HeaderAttribute("Distortion Texture")]
    public Transform sailboat;

    public float distortionFactor;
    public RenderTexture distortionTexture;
    private Texture2D _distortionTexture2D;

    // Properties
    [HeaderAttribute("X Axis")] public float amplitudeX = 1f;
    public float lengthX = 2f;
    public float speedX = 1f;

    [HideInInspector] public float offsetX;

    [HeaderAttribute("Z Axis")] public float amplitudeZ = 1f;
    public float lengthZ = 2f;
    public float speedZ = 1f;

    [HideInInspector] public float offsetZ;

    [HeaderAttribute("Lerp Speed")] public float lerpSpeed = 0.2f;

    [HeaderAttribute("Materials to update")]
    public List<Material> oceanMaterials;

    // Constants
    private static readonly int AmplitudeX = Shader.PropertyToID("_AmplitudeX");
    private static readonly int AmplitudeZ = Shader.PropertyToID("_AmplitudeZ");
    private static readonly int LengthX = Shader.PropertyToID("_LengthX");
    private static readonly int LengthZ = Shader.PropertyToID("_LengthZ");
    private static readonly int OffsetX = Shader.PropertyToID("_OffsetX");
    private static readonly int OffsetZ = Shader.PropertyToID("_OffsetZ");
    private static readonly int SailboatPosition = Shader.PropertyToID("_SailboatPosition");
    private static readonly int DistortionFactor = Shader.PropertyToID("_DistortionScale");
    private const float Tolerance = 0.01f;

    // State
    private float _targetAmpX;
    private float _targetAmpZ;
    private float _targetLenghtX;
    private float _targetLenghtZ;
    private int _textureOffsetX;
    private int _textureOffsetY;
    private Vector3 _sailboatPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Should not be another class");
            Destroy(this);
        }

        _distortionTexture2D = distortionTexture.ToTexture2D();
    }

    private void Start()
    {
        _targetAmpX = amplitudeX;
        _targetAmpZ = amplitudeZ;
        _targetLenghtX = lengthX;
        _targetLenghtZ = lengthZ;

        _textureOffsetX = distortionTexture.width / 2;
        _textureOffsetY = distortionTexture.height / 2;
    }

    private void Update()
    {
        _distortionTexture2D = distortionTexture.ToTexture2D();
        _sailboatPos = sailboat.position;
        offsetX += Time.deltaTime * speedX;
        offsetZ += Time.deltaTime * speedZ;
        if (Math.Abs(_targetAmpX - amplitudeX) > Tolerance)
        {
            amplitudeX = Mathf.Lerp(amplitudeX, _targetAmpX, Time.deltaTime * lerpSpeed);
        }

        if (Math.Abs(_targetAmpZ - amplitudeZ) > Tolerance)
        {
            amplitudeZ = Mathf.Lerp(amplitudeZ, _targetAmpZ, Time.deltaTime * lerpSpeed);
        }

        if (Math.Abs(_targetLenghtX - lengthX) > Tolerance)
        {
            lengthX = Mathf.Lerp(lengthX, _targetLenghtX, Time.deltaTime * lerpSpeed);
        }

        if (Math.Abs(_targetLenghtZ - lengthZ) > Tolerance)
        {
            lengthZ = Mathf.Lerp(lengthZ, _targetLenghtZ, Time.deltaTime * lerpSpeed);
        }

        foreach (var mat in oceanMaterials)
        {
            mat.SetFloat(AmplitudeX, Instance.amplitudeX);
            mat.SetFloat(AmplitudeZ, Instance.amplitudeZ);
            mat.SetFloat(LengthX, Instance.lengthX);
            mat.SetFloat(LengthZ, Instance.lengthZ);
            mat.SetFloat(OffsetX, Instance.offsetX);
            mat.SetFloat(OffsetZ, Instance.offsetZ);
            mat.SetFloat(DistortionFactor, distortionFactor);
            mat.SetVector(SailboatPosition, _sailboatPos);
        }
    }

    public float GetWaveHeight(float x, float z)
    {
        var pixelVal = _distortionTexture2D.GetPixel(
            (int) (_sailboatPos.x - x + _textureOffsetX),
            (int) (_sailboatPos.z - z + _textureOffsetY));
        
        float textureOffset = (pixelVal.r - pixelVal.g) * distortionFactor;
        var zVal = amplitudeZ * Mathf.Sin((z / lengthZ) + offsetZ);
        var xVal = amplitudeX * Mathf.Sin((x / lengthX) + offsetX);
        return zVal + xVal + textureOffset;
    }

    public void ChangeWaveValues(float ampX, float lenX, float ampZ, float lenZ)
    {
        _targetAmpX = ampX;
        _targetAmpZ = ampZ;
        _targetLenghtX = lenX;
        _targetLenghtZ = lenZ;
    }
}