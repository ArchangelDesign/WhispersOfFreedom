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
    public int MaxNotificationCount = 5;


    private void Start()
    {
        StartCoroutine("RemoveNotifications");
    }

    private GameObject CreateNotification()
    {
        GameObject newNotification =
            Instantiate<GameObject>(
                NotificationPrefab,
                target
            );

        // add to list of notifications and set position

        newNotification.GetComponent<RectTransform>().anchoredPosition =
            new Vector3(0f, 40 * notificationCount, 0f);
        Notifications.Add(newNotification);
        notificationCount = Notifications.Count;
        return newNotification;
    }

    // Show error notificaton
    public void Error(string message)
    {
        GameObject notification = CreateNotification();
        notification.GetComponentInChildren<Text>().color = Color.red;
        notification.GetComponentInChildren<Text>().text = message;
    }

    // Show info notification
    public void Info(string message)
    {
        
    }

    private IEnumerator RemoveNotifications()
    {
       while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (int i = Notifications.Count - 1; i >= 0; i--)
            {
                if (Notifications[i] == null)
                    Notifications.RemoveAt(i);
            }
        }

    }

}
