using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Notification : MonoBehaviour
{
    // Currently shown notifications
    public List<GameObject> Notifications = new List<GameObject>();
    // Prefab from which we create notifications
    public GameObject NotificationPrefab;
    // Parent transform to which attach notifications
    public Transform target;
    // Amount of currently shown notifications
    private int notificationCount = 0;
    // When reached we start removing old ones
    public int MaxNotificationCount = 6;

    private List<string> asyncInfoNotificationsAwaiting = new List<string>();

    public Color ErrorBackngroundColor = new Color(253, 185, 186, 198);
    public Color InfoBackngroundColor = new Color(60, 200, 120, 198);

    private static Notification instance;

    public Notification()
    {
        Notification.instance = this;
    }

    public static Notification getInstance()
    {
        if (Notification.instance == null)
        {
            Debug.LogError("Notification instance not set.");
            return null;
        }

        return Notification.instance;
    }

    private void Start()
    {
        StartCoroutine("RemoveNotifications");
        StartCoroutine("DisplayAsyncNotifications");
    }

    private GameObject CreateNotification()
    {
        GameObject newNotification =
            Instantiate<GameObject>(
                NotificationPrefab,
                target
            );

        // add to list of notifications and set position

        MoveNotificationsUp();

        newNotification.GetComponent<RectTransform>().anchoredPosition =
            new Vector3(0f, 0, 0f);
        Notifications.Add(newNotification);
        notificationCount = Notifications.Count;
        return newNotification;
    }

    // Show error notificaton
    public void Error(string message)
    {
        GameObject notification = CreateNotification();
        notification.GetComponentInChildren<Image>().color = ErrorBackngroundColor;
        notification.GetComponentInChildren<Text>().text = message;
    }

    // Show info notification
    public void Info(string message)
    {
        GameObject notification = CreateNotification();
        notification.GetComponentInChildren<Image>().color = InfoBackngroundColor;
        notification.GetComponentInChildren<Text>().text = message;
    }

    public void InfoAsync(string message)
    {
        asyncInfoNotificationsAwaiting.Add(message);
    }

    private IEnumerator RemoveNotifications()
    {
       while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = Notifications.Count - 1; i >= 0; i--)
            {
                if (Notifications[i] == null)
                    removeNotification(i);
            }
        }
    }

    private IEnumerator DisplayAsyncNotifications()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (asyncInfoNotificationsAwaiting.Count > 0)
            {
                Info(asyncInfoNotificationsAwaiting[0]);
                asyncInfoNotificationsAwaiting.RemoveAt(0);
            }
        }
    }

    private void removeNotification(int index)
    {
        if (Notifications[index] != null)
            Destroy(Notifications[index]);
        Notifications.RemoveAt(index);
        notificationCount = Notifications.Count;
    }

    private void MoveNotificationsUp()
    {
        if (notificationCount == MaxNotificationCount)
        {
            removeNotification(0);
        }
        for (int i = 0; i < Notifications.Count; i++)
        {
            if (Notifications[i] != null)
                Notifications[i].transform.position = new Vector3(
                    Notifications[i].transform.position.x,
                    Notifications[i].transform.position.y + 40, 0
                    );
        }

        
    }

}
