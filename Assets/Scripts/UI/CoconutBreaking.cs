using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoconutBreaking : MonoBehaviour
{
    [SerializeField] private Button Bail;
    [SerializeField] private Color32 text_color;
    [SerializeField] private TMP_Text text;
    [SerializeField]
    private GameObject Breaking;
    [SerializeField] private ImageAnimation imageAnimation;
    [SerializeField] private BonusController _bonusManager;
    [SerializeField] private SocketIOManager SocketManager;
    [SerializeField] private UIManager uiManager;

    [SerializeField]
    internal bool isOpen;
    [SerializeField] private int CaseIndex;


    void Start()
    {
        if (Bail) Bail.onClick.RemoveAllListeners();
        // if (Bail) Bail.onClick.AddListener(OpenCase);
        if (Bail) Bail.onClick.AddListener(() => StartCoroutine(TapBonus()));
    }

    internal void ResetCase()
    {
        isOpen = false;
        Breaking.SetActive(false);
        text.gameObject.SetActive(false);
        Bail.gameObject.SetActive(true);
    }

    private IEnumerator TapBonus()
    {
        _bonusManager.BonusPanel.gameObject.SetActive(true);
        SocketManager.AccumulateTapBonusResult(CaseIndex);
        yield return new WaitUntil(() => SocketManager.isResultdone);
         _bonusManager.BonusPanel.gameObject.SetActive(false);
        // if (SocketManager.BonusData.winAmount > 0)
        // {
        //     text.text = SocketManager.BonusData.winAmount.ToString("f2");
        // }
        // else
        // {
        //     text.text = "GAME OVER";
        // }
        OpenCase();
    }
    void OpenCase()
    {

        if (isOpen)
            return;
        _bonusManager.enableRayCastPanel(true);
        PopulateCase();
        imageAnimation.StartAnimation();
        Breaking.SetActive(true);
        Bail.gameObject.SetActive(false);
        StartCoroutine(setCase());
    }

    void PopulateCase()
    {
        double value =  SocketManager.BonusData.payload.winAmount;
        double payout = SocketManager.BonusData.payload.payout;
        if (payout > 0)
        {
            _bonusManager.Totalwinamount += value;
            text.text = value.ToString("f2");
        }

        else
        {
            text.text = "GAME OVER";
        }
    }

    IEnumerator setCase()
    {
        yield return new WaitUntil(() => !imageAnimation.isplaying);
        yield return new WaitForSeconds(0.8f);
        _bonusManager.UpdateWinText();
        text.gameObject.SetActive(true);
        text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, text_color);
        isOpen = true;
        if (text.text == "GAME OVER")
        {
            _bonusManager.PlayLoseSound();
            _bonusManager.enableRayCastPanel(true);
            yield return new WaitForSeconds(2f);
            _bonusManager.GameOver();
        }
        else
        {
            _bonusManager.PlayWinSound();
            _bonusManager.enableRayCastPanel(false);
        }
    }

}
