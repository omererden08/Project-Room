using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource audioSource;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>(); // ⬅️ BURAYA TAŞI
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}

