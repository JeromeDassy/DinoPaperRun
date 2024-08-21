using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private Text hudScore;
    [SerializeField] private Text hudHighScore;

    private float time = 0;

    private int playTime = 0;
    private int highestPlayTime = 0;

    void LateUpdate()
    {
        if (GameManager.Instance.IsPlaying && !GameManager.Instance.ParallaxManager.IsCountingDown)
        {
            time += Time.deltaTime;
        }

        if (hudScore != null)
        {
            highestPlayTime = GetHighScore();
            playTime = (int)Mathf.Round(time*10);
            hudScore.text = playTime.ToString();

            if (playTime > highestPlayTime)
            {
                highestPlayTime = playTime;
                hudHighScore.text = highestPlayTime.ToString();
                SetHighScore(highestPlayTime);
            }
        }
    }

    private int GetHighScore()
    {
        return FindObjectOfType<GameManager>().gameData.HighScore;
    }

    private void SetHighScore(int score)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.DisplayHighScore(score.ToString());
    }

    public void RestartScore()
    {
        hudScore.text = "0";
        time = 0;
    }
}
