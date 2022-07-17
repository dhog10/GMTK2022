using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{
    [SerializeField]
    private float _delay = 1f;

    private void Start()
    {
        StartCoroutine(this.Delay(_delay, () =>
        {
            GameObject.Destroy(this.gameObject);
        }));
    }

    private IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}
