using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float ShapeFunction(float _x, float _z, float _t);

public enum ShapeFunctions
{
    BasicSquared,
    Sin,
    MultiSin,
    Sin2D,
    MultiSin2D,
    Cos,
    Tan,
    Ripple,
}

public class ShapeGraph : MonoBehaviour
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

    private Dictionary<ShapeFunctions, ShapeFunction> m_Functions;

    [SerializeField]
    private ShapeFunctions m_CurrentFunction;
    
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
        
        m_Functions = new Dictionary<ShapeFunctions, ShapeFunction>();
        m_Functions.Add(ShapeFunctions.BasicSquared, BasicSquared);
        m_Functions.Add(ShapeFunctions.Sin, Sin);
        m_Functions.Add(ShapeFunctions.MultiSin, MultiSin);
        m_Functions.Add(ShapeFunctions.Sin2D, Sin2D);
        m_Functions.Add(ShapeFunctions.MultiSin2D, MultiSin2D);
        m_Functions.Add(ShapeFunctions.Cos, Cos);
        m_Functions.Add(ShapeFunctions.Tan, Tan);
        m_Functions.Add(ShapeFunctions.Ripple, Ripple);
        
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
            float x = i * m_ResolutionStep - halfRange;
            for (int j = 0; j < m_Resolution; j++)
            {
                float z = j * m_ResolutionStep - halfRange;
            
                var cube = Instantiate(m_ShapeCubePrefab, transform);
                cube.transform.Translate(x, Function(x, z), z);
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
        for (int i = 0; i < m_Cubes.Count; i++)
        {
            Transform cube = m_Cubes[i];

            float x = cube.position.x;
            float z = cube.position.z;
            cube.position = new Vector3(x, Function(x, z, _t), z);
        }
    }

    /// <summary>
    /// Helper method for invoking the current graph function. Handles time.
    /// </summary>
    /// <param name="_x">X value</param>
    /// <param name="_t">Time value</param>
    /// <returns>Y value</returns>
    private float Function(float _x, float _z, float _t = 0.0f)
    {
        return m_Functions[m_CurrentFunction].Invoke(_x, _z, _t);
    }

    #region Graph Functions
    
    /// <summary>
    /// Simple squared function.
    /// </summary>
    private float BasicSquared(float _x, float _z, float _t)
    {
        return _x * _x;
    }

    private float Sin(float _x, float _z, float _t)
    {
        return Mathf.Sin(_x + _z + _t);
    }

    private float MultiSin(float _x, float _z, float _t)
    {
        float y = Sin(_x, _z, _t);
        y += Mathf.Sin(2 * _x) / 2;
        return y;
    }

    private float Sin2D(float _x, float _z, float _t)
    {
        float x = Mathf.Sin(_x);
        float z = Mathf.Sin(_z);
        return (x + z) * 0.5f;
    }

    private float MultiSin2D(float _x, float _z, float _t)
    {
        float a = Sin2D(_x * 3, _z, _t);
        float b = Sin2D(_x, _z * 4, _t);
        float c = 2 * Sin2D(_x, _z, _t);
        float d = Sin2D(_x * 1.5f, _z * 2.5f, _t);
        return (a + b + c + d) * 0.25f;
    }

    private float Cos(float _x, float _z, float _t)
    {
        return Mathf.Cos(_x);
    }

    private float Tan(float _x, float _z, float _t)
    {
        return Mathf.Tan(_x);
    }

    private float Ripple(float _x, float _z, float _t)
    {
        float d = Mathf.Sqrt(_x * _x + _z * + _z);
        float y = Mathf.Sin(6 * d - _t);
        y /= 1f + 2f * d;
        return y;
    }
    
    #endregion
}
