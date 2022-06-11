using FMODUnity;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public StudioEventEmitter music;
    public StudioEventEmitter musicBlackHole;
    public StudioEventEmitter musicPoet;
    public StudioEventEmitter musicColossus;
    public StudioEventEmitter blackHoleAmbience;

    public Transform player;
    public Transform colossusIsland;
    public Transform poetIsland;
    public Transform blackHole;

    public float minDistance = 400;
    public float blackHoleSize = 3562;

    private float _colossusDistance;
    private float _poetDistance;
    private float _blackHoleDistance;

    private bool _poetPlaying = false;
    private bool _colossusPlaying = false;
    private bool _blackHolePlaying = false;


    private void Start()
    {
        EventManager.Instance.Register<GameStarted>((e) =>
        {
            blackHoleAmbience.Play();
            music.Stop();
        });
        EventManager.Instance.Register<TutorialFinish>((e) =>
        {
            _blackHolePlaying = true;
            music.Stop();
            musicBlackHole.Play();
        });
        EventManager.Instance.Register<BlackHoleEffectActive>((e) =>
        {
            if (!((BlackHoleEffectActive) e).Active)
            {
                blackHoleAmbience.Stop();
            }
        });
        
    }

    private void Update()
    {
        _blackHoleDistance = Vector3.Distance(player.position, blackHole.position) - blackHoleSize;
        _poetDistance = Vector3.Distance(player.position, poetIsland.position);
        _colossusDistance = Vector3.Distance(player.position, colossusIsland.position);
        if (_blackHolePlaying)
        {
            musicBlackHole.SetParameter("Black Hole Proximity",_blackHoleDistance);
        }

        if (_poetDistance < minDistance && !_poetPlaying)
        {
            _poetPlaying = true;
            music.Stop();
            musicBlackHole.Stop();
            musicPoet.Play();
        }
        else
        {
            if (_poetDistance > minDistance && _poetPlaying)
            {
                _poetPlaying = false;
                musicPoet.Stop();
            }
        }

        if (_colossusDistance < minDistance && !_colossusPlaying)
        {
            _colossusPlaying = true;
            music.Stop();
            musicBlackHole.Stop();
            musicColossus.Play();
        }
        else
        {
            if (_colossusDistance > minDistance && _colossusPlaying)
            {
                _colossusPlaying = false;
                musicColossus.Stop();
            }
        }

        if (_poetPlaying)
        {
            musicPoet.SetParameter("Main Track Weight", _poetDistance / minDistance);
        }

        if (_colossusPlaying)
        {
            musicColossus.SetParameter("Main Track Weight", _colossusDistance / minDistance);
        }
    }
}