using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource engineSound;
    public AudioClip bombExplosion;
    
    void Start()
    {
        if (backgroundMusic != null)
            backgroundMusic.Play();
            
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.volume = 0;
            engineSound.Play();
        }
    }
    
    void Update()
    {
        if (engineSound != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || 
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                engineSound.volume = Mathf.Lerp(engineSound.volume, 0.1f, Time.deltaTime * 3f);
                engineSound.pitch = 1.1f;
            }
            else
            {
                engineSound.volume = Mathf.Lerp(engineSound.volume, 0.02f, Time.deltaTime * 5f);
                engineSound.pitch = 0.9f;
            }
        }
    }
    
    public void PlayExplosionSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(bombExplosion, position);
    }
}