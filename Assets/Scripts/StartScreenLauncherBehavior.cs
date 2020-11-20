using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenLauncherBehavior : MonoBehaviour
{
    public float speed = 0.0f;
    public float loadBoarderX = 100.0f;

    void Update()
    {
        if(isFlying)
		{
            velocity += speed * Time.deltaTime;
            transform.Translate(new Vector3(0.0f, velocity, 0.0f), Space.Self); 

            if(transform.position.x > loadBoarderX)
			{
                SceneManager.LoadScene("GameScene");
			}
		}

    }

    public void SetFlying()
	{
        isFlying = true;
	}

    private bool isFlying = false;
    private float velocity = 0.0f;
}
