using System;
using UnityEngine;

public class WorldBillboard : MonoBehaviour
{
    [SerializeField] private Camera m_targetCamera;

    private void Awake()
    {
        if (m_targetCamera == null)
        {
            m_targetCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (m_targetCamera == null)
            return;

        transform.rotation = m_targetCamera.transform.rotation;
    }
}