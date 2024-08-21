[System.Serializable]
public class GameData
{
    //int as bool
    private int _isFirstStart;

    public int IsFirstStart
    {
        get { return _isFirstStart; }
        set { _isFirstStart = value; }
    }

    private int _highScore;

    public int HighScore
    {
        get { return _highScore; }
        set { _highScore = value; }
    }
}