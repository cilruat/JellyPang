using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UI_GameResult : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _bestRecordText;
    [SerializeField]
    private Button _okButton;

    public void SetResult(int score, int record)
    {
        _bestRecordText.text = string.Format("BEST RECORD\n{0}", record.ToString("N0"));
        _scoreText.DOText(score.ToString("N0"), 2f, false, ScrambleMode.Numerals)
            .OnComplete(()=> 
            {
                _okButton.gameObject.SetActive(true);
            });
    }

    //테스트용
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}