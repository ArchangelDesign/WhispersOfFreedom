using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private ApiClient apiClient = ApiClient.getInstance();
    private WofUdpClient udpClient = WofUdpClient.GetInstance();
    private Text notificationText;

    public float notificationShowTime = 2;
    public float notificationFadeStep = 0.05f;
    public float notificationTimerDelay = 0.02f;

    void Start()
    {
        notificationText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Notification(string message)
    {
        notificationText.text = message;
        StartCoroutine("ShowNotification");
    }

    private IEnumerator ShowNotification()
    {
        yield return FadeNotificationIn();
        yield return new WaitForSeconds(notificationShowTime);
        yield return FadeNotificationOut();
    }

    private IEnumerator FadeNotificationIn()
    {
        while (notificationText.color.a < 1.0f)
        {
            notificationText.color = new Color(
                notificationText.color.r,
                notificationText.color.g,
                notificationText.color.b,
                notificationText.color.a + notificationFadeStep);
            yield return new WaitForSeconds(notificationTimerDelay);
        }
    }

    private IEnumerator FadeNotificationOut()
    {
        while (notificationText.color.a > 0f)
        {
            notificationText.color = new Color(
                notificationText.color.r,
                notificationText.color.g,
                notificationText.color.b,
                notificationText.color.a - notificationFadeStep);
            yield return new WaitForSeconds(notificationTimerDelay);
        }
    }

    private void OnDestroy()
    {
        //apiClient.OnDestroy();
        //udpClient.OnDestroy();
    }


}
