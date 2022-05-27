using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIManager : ObjectSingleton<GameUIManager>
{
    [SerializeField]
    private Slider _pointGaugeSlider;
    [SerializeField]
    private TextSlider _timerSlider;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private UI_GameResult _gameResult;

    public void SetPointGauge(int curPoint, int maxPoint)
    {
        _pointGaugeSlider.value = (float)curPoint / (float)maxPoint;
    }

    public void SetTimerValue(float curTime, float maxTime)
    {
        _timerSlider.value = curTime / maxTime;
        _timerSlider.SetText(Mathf.CeilToInt(curTime).ToString());
    }

    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void OpenGameResult(int score, int record)
    {
        _gameResult.gameObject.SetActive(true);
        _gameResult.SetResult(score, record);
    }
}
