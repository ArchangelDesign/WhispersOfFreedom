using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BattleListController : MonoBehaviour
{
    public Button buttonPrefab;
    public Text textPrefab;
    public Transform content;

    private RectTransform rectTransform;
    private int totalBattles = 0;

    Thread refreshThread;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = (RectTransform)content;
        RefreshBattleList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshBattleList()
    {
        AddBattle("Test battle", 1, "BattleId1");
        AddBattle("Test battle 2", 2, "secondId");
    }

    void AddBattle(string battleName, int clients, string battleId)
    {

        float joinButtonWidth = 60;
        float margin = 15;
        float contentWidth = rectTransform.rect.width;
        float topPosition = (-20 - margin) * (totalBattles + 1);
        float rowHeight = 35;

        Button joinButton = Instantiate<Button>(buttonPrefab, content);
        joinButton.GetComponentInChildren<Text>().text = "Join";
        joinButton.GetComponent<RectTransform>().sizeDelta = new Vector2(joinButtonWidth, 20);
        joinButton.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        joinButton.GetComponent<Transform>().localPosition = new Vector2(contentWidth - joinButtonWidth - margin, topPosition);
        joinButton.onClick.AddListener(() => OnJoinBattleClicked(battleId));

        Text battleNameText = Instantiate(textPrefab, content);
        battleNameText.text = battleName;
        battleNameText.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        battleNameText.GetComponent<Transform>().localPosition = new Vector2(0 + margin, topPosition);

        Text clientsText = Instantiate(textPrefab, content);
        clientsText.text = clients.ToString();
        clientsText.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        clientsText.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 18);
        clientsText.GetComponent<Transform>().localPosition = new Vector2(contentWidth - 140, topPosition);

        totalBattles++;
    }

    void OnJoinBattleClicked(string battleId)
    {
        Debug.Log("Joining battle " + battleId);
    }
}
