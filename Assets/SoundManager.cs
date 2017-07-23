using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	public static SoundManager soundManager;

	[SerializeField] AudioClip menuMusic;
	[SerializeField] AudioClip overworldMusic;
	[SerializeField] AudioClip puzzleMusic;
	[SerializeField] List<AudioClip> SFX;

	private AudioSource audioSrc;
	private AudioSource sfxSource;
	private int currentSceneIndex;

	public void PlayPortalSound()
	{
		sfxSource.clip = SFX[0];
		sfxSource.Play();
	}

	public void PlaySolvedSound()
	{
		sfxSource.clip = SFX[1];
		sfxSource.Play();
	}

	void Awake()
	{
		if (soundManager == null)
		{
			soundManager = this;
		}
		else Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		audioSrc = GetComponent<AudioSource>();
		audioSrc.loop = true;
		audioSrc.Play();
		Transform child = transform.GetChild(0);
		sfxSource = child.GetComponent<AudioSource>();
		sfxSource.loop = false;
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(0);
		Scene scene = SceneManager.GetActiveScene();
		if (scene.buildIndex != currentSceneIndex)
		{
			audioSrc.Stop();
			if (scene.name == "Overworld")
			{
				audioSrc.clip = overworldMusic;
			}
			else if (scene.buildIndex == 0)
			{
				audioSrc.clip = menuMusic;
			}
			else
			{
				audioSrc.clip = puzzleMusic;
			}

			audioSrc.Play();
			currentSceneIndex = scene.buildIndex;
		}
	}
}
