using UnityEngine;


public class InfoUrlButtonBehavior : MonoBehaviour
{
	public string targetUrl = "https://bit.ly/3qNdfpK";

	public void Click()
	{
		Application.OpenURL(targetUrl);
	}
}