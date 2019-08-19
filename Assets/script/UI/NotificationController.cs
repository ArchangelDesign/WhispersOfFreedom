using System;
using UnityEngine;
using System.Collections;

public class NotificationController : MonoBehaviour
{
    public int ttl = 4;

	void Start()
	{
		// Start coroutine to fade it out after awhile
		StartCoroutine("AutoDestroy");
        // TODO: fade in
	}

    private IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(ttl);
        // TODO: fade out
		Destroy(gameObject);
	}
}
