using UnityEngine;

namespace CatWar
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music Config")]
        [SerializeField] private AudioClip battleThemeBgm;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Auto setup sources if unassigned
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.playOnAwake = false;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            bgmSource.loop = true;

            // Start playing battle theme
            if (battleThemeBgm != null)
            {
                PlayBGM(battleThemeBgm);
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || bgmSource.clip == clip) return;
            
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip);
        }

        public void SetVolumeBGM(float volume)
        {
            bgmSource.volume = Mathf.Clamp01(volume);
        }

        public void SetVolumeSFX(float volume)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}
