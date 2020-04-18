using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entities
{
    public class TreeObstacle : MonoBehaviour
    {
        [SerializeField]
        private Transform _trunk = null;
        [SerializeField]
        private Transform _crown = null;

        private Vector3 _trunkDefaultScale;
        private Vector3 _crownDefaultScale;

        private void Awake()
        {
            _crownDefaultScale = _crown.localScale;
            float crownScaleFactor = Random.Range(0.8f, 1.2f);
            _crown.localScale = crownScaleFactor * _crownDefaultScale;

            float crownYPos = Random.Range(-0.7f, 0.7f);
            _crown.localPosition += Vector3.up * crownYPos;
        }
    }
}