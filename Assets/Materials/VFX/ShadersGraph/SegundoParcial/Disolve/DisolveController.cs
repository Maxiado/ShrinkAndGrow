using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveController : MonoBehaviour
{
    public float duration;
    public Material material;
    public Collider objCollider;

    void Start()
    {
        var rend = GetComponent<Renderer>();
        if (rend)
        {
            material = rend.material;
        }

        if (material)
        {
            material.SetFloat("_Amount", -1f);
        }
    }

    public void ActivateDisolve()
    {
        StartCoroutine(DisolverRoutine());
    }

    private IEnumerator DisolverRoutine()
    {
        if (objCollider)
        {
            objCollider.enabled = false;
        }

        var timeSpent = 0f;
        var initialValue = -1f;
        var finalValue = 1f;

        while (timeSpent < duration)
        {
            timeSpent += Time.deltaTime;

            float valorActual = Mathf.Lerp(initialValue, finalValue, timeSpent / duration);

            if (material)
            {
                material.SetFloat("_Amount", valorActual);
            }

            yield return null;
        }

        if (material)
        {
            material.SetFloat("_Amount", finalValue);
        }

        gameObject.SetActive(false);
    }
}