using DG.Tweening;
using UnityEngine;
using System.Collections;
using Entities;

[RequireComponent(typeof(Animal))]
public class FoxMovement : MonoBehaviour
{
    private void OnDestroy()
    {
        if (DOTween.IsTweening(transform))
            DOTween.Kill(transform);
    }
}
