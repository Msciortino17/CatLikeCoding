using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float GraphFunction(float _x);

public enum GraphFunctions
{
    BasicSquared,
    Sin,
    MultiSin,
    Cos,
    Tan,
}

public class Graph : MonoBehaviour
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

    private Dictionary<GraphFunctions, GraphFunction> m_Functions;

    [SerializeField]
    private GraphFunctions m_CurrentFunction;
    
    [Header("References and Prefabs")]
    [SerializeField] 
    private GameObject m_GraphCubePrefab;
    
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
        
        m_Functions = new Dictionary<GraphFunctions, GraphFunction>();
        m_Functions.Add(GraphFunctions.BasicSquared, BasicSquared);
        m_Functions.Add(GraphFunctions.Sin, Sin);
        m_Functions.Add(GraphFunctions.MultiSin, MultiSin);
        m_Functions.Add(GraphFunctions.Cos, Cos);
        m_Functions.Add(GraphFunctions.Tan, Tan);
        
        CreateGraphMarkers();
        CreateGraphCubes();
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    private void Update()
    {
        if (m_Animate)
        {
            UpdateCubePositions(Time.time);
        }
    }

    /// <summary>
    /// Create each cube we want
    /// </summary>
    private void CreateGraphCubes()
    {
        for (int i = 0; i < m_Resolution; i++)
        {
            float x = i * m_ResolutionStep - m_Range / 2;
            
            var cube = Instantiate(m_GraphCubePrefab, transform);
            cube.transform.Translate(x, Function(x), 0.0f);
            cube.transform.localScale = Vector3.one * m_ResolutionStep;
            cube.GetComponent<MeshRenderer>().material.SetFloat("GraphRange", m_Range);
            m_Cubes.Add(cube.transform);
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
        for (int i = 0; i < m_Resolution; i++)
        {
            Transform cube = m_Cubes[i];

            float x = cube.position.x;
            cube.position = new Vector3(x, Function(x, _t));
        }
    }

    /// <summary>
    /// Helper method for invoking the current graph function. Handles time.
    /// </summary>
    /// <param name="_x">X value</param>
    /// <param name="_t">Time value</param>
    /// <returns>Y value</returns>
    private float Function(float _x, float _t = 0.0f)
    {
        float x = _x + _t;
        return m_Functions[m_CurrentFunction].Invoke(x);
    }

    #region Graph Functions
    
    /// <summary>
    /// Simple squared function.
    /// </summary>
    private float BasicSquared(float _x)
    {
        return _x * _x;
    }

    private float Sin(float _x)
    {
        return Mathf.Sin(_x);
    }

    private float MultiSin(float _x)
    {
        float y = Sin(_x);
        y += Mathf.Sin(2 * _x) / 2;
        return y;
    }

    private float Cos(float _x)
    {
        return Mathf.Cos(_x);
    }

    private float Tan(float _x)
    {
        return Mathf.Tan(_x);
    }
    
    #endregion
}
