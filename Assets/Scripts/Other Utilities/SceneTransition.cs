using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
	//public GameObject loadingImage;
	
	public void LoadScene(string sceneName)
	{
		//loadingImage.SetActive(true);
		Application.LoadLevel(sceneName);
	}
}