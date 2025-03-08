using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FootstepHandler : MonoBehaviour
{
    public Tilemap tilemap;
    public AudioSource footstepSource;
    public float footstepInterval = 0.5f;
    public float pitchVariation = 0.1f;

    [System.Serializable]
    public class FootstepData
    {
        public string tileTag;
        public AudioClip[] walkSounds;
        public AudioClip[] runSounds;
    }

    public List<FootstepData> footstepDataList;
    
    private bool isRunning;
    private bool isMoving;
    private Dictionary<string, FootstepData> footstepDictionary = new Dictionary<string, FootstepData>();

    private void Start()
    {
        foreach (var data in footstepDataList)
        {
            footstepDictionary[data.tileTag] = data;
        }
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    public void StartFootsteps()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(FootstepRoutine());
        }
    }

    public void StopFootsteps()
    {
        isMoving = false;
    }

    private IEnumerator FootstepRoutine()
    {
        while (isMoving)
        {
            PlayFootstep();
            yield return new WaitForSeconds(isRunning ? footstepInterval / 2 : footstepInterval);
        }
    }

    private void PlayFootstep()
    {
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        TileBase tile = tilemap.GetTile(tilePos);

        if (tile != null && footstepDictionary.TryGetValue(tile.name, out FootstepData data))
        {
            AudioClip clip = isRunning ? GetRandomClip(data.runSounds) : GetRandomClip(data.walkSounds);
            footstepSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            footstepSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
