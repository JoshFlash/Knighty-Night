using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubWorldLevelPortal : MonoBehaviour
{
	public int levelToLoadIndex;
	public float FOVWidenAmount = 16f;
	public GameObject lowCloud, middleCloud;
	private float cameraInitialFOV;
	private float extraCameraFOV;


	Transform child;
	bool levelCanBeLoaded;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			levelCanBeLoaded = true;
			StopAllCoroutines();
			StartCoroutine(AnimateClouds());
			StartCoroutine(FOVKick(true));
			SoundManager.soundManager.PlayPortalSound();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			levelCanBeLoaded = false;
			StopAllCoroutines();
			StartCoroutine(DeactivateClouds());
			StartCoroutine(FOVKick(false));
		}
	}

	void Start ()
	{
		child = transform.GetChild(0);
		child.gameObject.SetActive(false);
		cameraInitialFOV = Camera.main.fieldOfView;
		extraCameraFOV = cameraInitialFOV + FOVWidenAmount;
	}

	private void Update()
	{
		if (levelCanBeLoaded)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				LevelGrid grid = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<LevelGrid>();
				PlayerController plyControl = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<PlayerController>();
				grid.level.playerStart = PlayerController.GetCurrentCell().coordinates;
				SceneManager.LoadScene(levelToLoadIndex);
			}
		}
	}
	private IEnumerator AnimateClouds()
	{
		WaitForSeconds wait = new WaitForSeconds(0.24f);
		lowCloud.SetActive(true);
		yield return wait;
		middleCloud.SetActive(true);
		yield return wait;
		child.gameObject.SetActive(true);
	}
	private IEnumerator DeactivateClouds()
	{
		WaitForSeconds wait = new WaitForSeconds(0.12f);
		child.gameObject.SetActive(false);
		yield return wait;
		middleCloud.SetActive(false);
		yield return wait;
		lowCloud.SetActive(false);
	}

	private IEnumerator FOVKick(bool zoomOut)
	{
		float currentCameraFOV = Camera.main.fieldOfView;
		float journeyTime = 1f;
		float startTime = Time.time;
		bool isLerping = true;
		while (isLerping)
		{
			float fracComplete = ( Time.time - startTime ) / journeyTime;
			if (zoomOut) Camera.main.fieldOfView = Mathf.Lerp(currentCameraFOV, extraCameraFOV, fracComplete);
			else Camera.main.fieldOfView = Mathf.Lerp(currentCameraFOV, cameraInitialFOV, fracComplete);
			if (fracComplete > 1.0f)
			{
				isLerping = false;
			}
			yield return new WaitForEndOfFrame();
		}
	}

}
