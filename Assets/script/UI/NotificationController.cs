using System;
using UnityEngine;
using System.Collections;

public class NotificationController : MonoBehaviour
{
	void Start()
	{
		// Start coroutine to fade it out after awhile
		StartCoroutine("AutoDestroy");
        // TODO: fade in
	}

    private IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(2);
        // TODO: fade out
		Destroy(gameObject);
	}
}
