using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Vector3 ShapeFunctionUV(float _u, float _v, float _t);

public enum ShapeFunctionsUV
{
    Cylinder,
    Sphere,
    PulsingSphere,
    SuperSphere,
    Torus,
}

public class ShapeGraphUV : MonoBehaviour
{

    [Header("Main Data")]
    [SerializeField] 
    [Range(10, 400)]
    private int m_Resolution;

    [SerializeField]
    [Range(2, 8)]
    private float m_Range;

    private float m_ResolutionStep;

    [SerializeField]
    private bool m_Animate;

    [SerializeField]
    private bool m_Cycle;

    [SerializeField]
    private float m_CycleTimer;

    private int m_CycleDirection = 1;

    [SerializeField]
    [Range(1, 8)]
    private float m_CycleRange;

    private Dictionary<ShapeFunctionsUV, ShapeFunctionUV> m_Functions;

    [SerializeField]
    private ShapeFunctionsUV m_CurrentFunction;
    
    [Header("References and Prefabs")]
    [SerializeField] 
    private GameObject m_ShapeCubePrefab;
    
    [SerializeField] 
    private GameObject m_GraphMarkerPrefab;

    private List<Transform> m_Cubes;

    /// <summary>
    /// Called at startup
    /// </summary>
    void Awake()
    {
        m_ResolutionStep = m_Range / m_Resolution;
        m_Cubes = new List<Transform>();
        
        m_Functions = new Dictionary<ShapeFunctionsUV, ShapeFunctionUV>();
        m_Functions.Add(ShapeFunctionsUV.Cylinder, Cylinder);
        m_Functions.Add(ShapeFunctionsUV.Sphere, Sphere);
        m_Functions.Add(ShapeFunctionsUV.PulsingSphere, PulsingSphere);
        m_Functions.Add(ShapeFunctionsUV.SuperSphere, SuperSphere);
        m_Functions.Add(ShapeFunctionsUV.Torus, Torus);
        
//        CreateGraphMarkers();
        CreateGraphCubes();
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    private void Update()
    {
        if (m_Animate)
        {
            if (m_Cycle)
            {
                m_CycleTimer += Time.deltaTime * m_CycleDirection;
                if (m_CycleTimer > m_CycleRange)
                {
                    m_CycleDirection = -1;
                }
                else if (m_CycleTimer < -m_CycleRange)
                {
                    m_CycleDirection = 1;
                }
                
                UpdateCubePositions(m_CycleTimer);
            }
            else
            {
                UpdateCubePositions(Time.time);
            }
        }
    }

    /// <summary>
    /// Create each cube we want
    /// </summary>
    private void CreateGraphCubes()
    {
        float halfRange = m_Range / 2.0f;
        for (int i = 0; i < m_Resolution; i++)
        {
            float u = i * m_ResolutionStep - halfRange;
            for (int j = 0; j < m_Resolution; j++)
            {
                float v = j * m_ResolutionStep - halfRange;
            
                var cube = Instantiate(m_ShapeCubePrefab, transform);
                cube.transform.position = Function(u, v);
                cube.transform.localScale = Vector3.one * m_ResolutionStep;
                cube.GetComponent<MeshRenderer>().material.SetFloat("GraphRange", m_Range);
                m_Cubes.Add(cube.transform);
            }
        }
    }

    /// <summary>
    /// Create each graph marker
    /// </summary>
    private void CreateGraphMarkers()
    {
        int numMarkers = 100;
        // X markers
        for (int i = 0; i < numMarkers; i++)
        {
            var marker = Instantiate(m_GraphMarkerPrefab, transform);
            marker.transform.localScale = new Vector3(0.01f, (i % 10 == 0) ? 0.2f : 0.1f, 1f);
            marker.transform.position = new Vector3((i - numMarkers / 2) * 0.1f, 0.0f, 0.0f);
        }
        
        // Y markers
        for (int i = 0; i < numMarkers; i++)
        {
            var marker = Instantiate(m_GraphMarkerPrefab, transform);
            marker.transform.localScale = new Vector3((i % 10 == 0) ? 0.2f : 0.1f, 0.01f, 1f);
            marker.transform.position = new Vector3(0.0f, (i - numMarkers / 2) * 0.1f, 0.0f);
        }
    }

    /// <summary>
    /// Update each cubes position, factoring in _dT into x
    /// </summary>
    /// <param name="_t"></param>
    private void UpdateCubePositions(float _t)
    {
        float halfRange = m_Range / 2.0f;
        for (int i = 0; i < m_Resolution; i++)
        {
            float u = i * m_ResolutionStep - halfRange;
            for (int j = 0; j < m_Resolution; j++)
            {
                float v = j * m_ResolutionStep - halfRange;
            
                Transform cube = m_Cubes[i * m_Resolution + j];
                cube.position = Function(u, v, _t);
            }
        }
    }

    /// <summary>
    /// Helper method for invoking the current graph function. Handles time.
    /// </summary>
    /// <param name="_u">X value</param>
    /// <param name="_v">Y value</param>
    /// <param name="_t">Time value</param>
    /// <returns>Y value</returns>
    private Vector3 Function(float _u, float _v, float _t = 0.0f)
    {
        return m_Functions[m_CurrentFunction].Invoke(_u, _v, _t);
    }

    #region Graph Functions

    private Vector3 Cylinder(float _u, float _v, float _t)
    {
        float r = 0.8f + Mathf.Sin(Mathf.PI * (6f * _u + 2f * _v + _t)) * 0.2f;
        Vector3 v = new Vector3();
        v.x = r * Mathf.Sin(Mathf.PI * _u);
        v.y = _v;
        v.z = r * Mathf.Cos(Mathf.PI * _u);
        return v;
    }

    private Vector3 Sphere(float _u, float _v, float _t)
    {
        float r = Mathf.Cos(Mathf.PI * 0.5f * _v);
        Vector3 v = new Vector3();
        v.x = r * Mathf.Sin(Mathf.PI * _u);
        v.y = Mathf.Sin(Mathf.PI * 0.5f * _v);;
        v.z = r * Mathf.Cos(Mathf.PI * _u);
        return v;
    }

    private Vector3 PulsingSphere(float _u, float _v, float _t)
    {
        float r = 0.8f + Mathf.Sin(Mathf.PI * (6f * _u + _t)) * 0.1f;
        r += Mathf.Sin(Mathf.PI * (4f * _v + _t)) * 0.1f;
        float s = r * Mathf.Cos(Mathf.PI * 0.5f * _v);
        
        Vector3 v = new Vector3();
        v.x = s * Mathf.Sin(Mathf.PI * _u);
        v.y = r * Mathf.Sin(Mathf.PI * 0.5f * _v);;
        v.z = s * Mathf.Cos(Mathf.PI * _u);
        return v;
    }

    private Vector3 SuperSphere(float _u, float _v, float _t)
    {
        float r = 0.8f + Mathf.Sin(Mathf.PI * (6f * _u + _t)) * 0.1f;
        r += Mathf.Sin(Mathf.PI * (4f * _v + _t)) * 0.1f;
        float s = r * Mathf.Cos(Mathf.PI * 0.5f * _v);
        
        Vector3 v = new Vector3();
        v.x = s * Mathf.Sin(Mathf.PI * _u);
        v.y = r * Mathf.Tan(Mathf.PI * 0.5f * _v);;
        v.z = s * Mathf.Cos(Mathf.PI * _u);
        return v;
    }

    private Vector3 Torus(float _u, float _v, float _t)
    {
        float r1 = 0.65f + Mathf.Sin(Mathf.PI * (6f * _u + _t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(Mathf.PI * (4f * _v + _t)) * 0.05f;
        float s = r2 * Mathf.Cos(Mathf.PI * _v) + r1;
        
        Vector3 v = new Vector3();
        v.x = s * Mathf.Sin(Mathf.PI * _u);
        v.y = r2 * Mathf.Sin(Mathf.PI * _v);;
        v.z = s * Mathf.Cos(Mathf.PI * _u);
        return v;
    }
    
    #endregion
}
