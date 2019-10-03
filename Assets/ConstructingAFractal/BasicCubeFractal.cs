using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCubeFractal : MonoBehaviour
{
    private float m_RotationSpeed_X;
    private float m_RotationSpeed_Y;
    private float m_RotationSpeed_Z;

    public float m_RotationSpeed;

    [SerializeField]
    private float m_BreakTimer;

    [SerializeField]
    private bool m_Broke = false;
    
    public Mesh mesh;

    public Material material;

    public float childScale;

    public int maxDepth;
    
    public int depth;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        if (depth < maxDepth)
        {
            Bud(Vector3.up);
            Bud(Vector3.right);
            Bud(Vector3.left);
            Bud(Vector3.down);
            Bud(Vector3.forward);
            Bud(Vector3.back);
        }

        m_RotationSpeed_X = Random.Range(-m_RotationSpeed, m_RotationSpeed);
        m_RotationSpeed_Y = Random.Range(-m_RotationSpeed, m_RotationSpeed);
        m_RotationSpeed_Z = Random.Range(-m_RotationSpeed, m_RotationSpeed);
    }

    /// <summary>
    /// Sets up variables based off of the given parent object
    /// </summary>
    /// <param name="_parent"></param>
    public void Initialize(BasicCubeFractal _parent, Vector3 _direction)
    {
        // Copy over info
        depth = _parent.depth + 1;
        maxDepth = _parent.maxDepth;
        mesh = _parent.mesh;
        material = new Material(_parent.material);// _parent.material;
        material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        childScale = _parent.childScale;
        m_RotationSpeed = _parent.m_RotationSpeed;
        
        // Setup size and position
        transform.parent = _parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = _direction * (0.5f + 0.5f * childScale);

        m_BreakTimer = (maxDepth + 3 - depth) * Random.Range(2.0f, 10.0f);
    }

    private void Bud(Vector3 _direction)
    {
        GameObject child = new GameObject("Cube " + depth);
        child.AddComponent<BasicCubeFractal>().Initialize(this, _direction);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_Broke)
        {
            if (m_BreakTimer <= 0.0f && depth != 0)
            {
                transform.parent = null;
                Vector3 forceVector = new Vector3(m_RotationSpeed_X, m_RotationSpeed_Y, m_RotationSpeed_Z) / 30.0f;
                gameObject.AddComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
//                gameObject.AddComponent<BoxCollider>();
                gameObject.AddComponent<SphereCollider>();
                m_Broke = true;
            }
            else
            {
                m_BreakTimer -= Time.deltaTime;
                transform.Rotate(Time.deltaTime * m_RotationSpeed_X, Time.deltaTime * m_RotationSpeed_Y, Time.deltaTime * m_RotationSpeed_Z);
            }
        }
    }
}
