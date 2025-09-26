using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Best.SocketIO;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private GameObject Bonus_Object;
    [SerializeField]
    private SlotBehaviour slotManager;
    [SerializeField] private SocketIOManager socketManager;
    [SerializeField]
    private GameObject raycastPanel;
    [SerializeField]
    private List<CoconutBreaking> BonusCases;
    [SerializeField]
    private AudioController _audioManager;
    [SerializeField]
    private TMP_Text Win_Text;

    [SerializeField]
    private List<int> CaseValues;

    int index = 0;
    double winAmount = 0;
    public double Totalwinamount=0;
    public Image BonusPanel;

    internal void GetBailCaseList()
    {
        index = 0;
        CaseValues.Clear();
        CaseValues.TrimExcess();
        if (Win_Text) Win_Text.text = "0";
        winAmount = 0;
        foreach (CoconutBreaking cases in BonusCases)
        {
            cases.ResetCase();
        }

        if (raycastPanel) raycastPanel.SetActive(false);
        StartBonus();
    }

    internal void enableRayCastPanel(bool choice)
    {
        if (raycastPanel) raycastPanel.SetActive(choice);
    }

    internal void GameOver()
    {
        slotManager.CheckPopups = false;
        if (Bonus_Object) Bonus_Object.SetActive(false);
        if (_audioManager) _audioManager.SwitchBGSound(false);
        slotManager.updateBalance(socketManager.BonusData.player.balance, socketManager.BonusData.payload.winAmount);
        socketManager.resultData.payload.winAmount = socketManager.BonusData.payload.winAmount;
        Totalwinamount = 0;
        BonusPanel.gameObject.SetActive(false);
    }

    internal double GetValue()
    {
        int value = 0;

        value = CaseValues[index];

        winAmount += value * slotManager.currentBet;

        index++;

        if (_audioManager) _audioManager.PlayBonusAudio("coconut");

        return value * slotManager.currentBet;
    }

    public void StartBonus()
    {
        if (Win_Text) Win_Text.text = "0";
        winAmount = 0;
        foreach (CoconutBreaking cases in BonusCases)
        {
            cases.ResetCase();
        }
        if (raycastPanel) raycastPanel.SetActive(false);
        if (_audioManager) _audioManager.SwitchBGSound(true);
        if (Bonus_Object) Bonus_Object.SetActive(true);
    }

    internal void PlayWinSound()
    {
        if (_audioManager) _audioManager.PlayBonusAudio("win");
    }

    internal void PlayLoseSound()
    {
        if (_audioManager) _audioManager.PlayBonusAudio("lose");
    }

    internal void UpdateWinText()
    {
        if (Win_Text) Win_Text.text = Totalwinamount.ToString("f2");
    }
}
