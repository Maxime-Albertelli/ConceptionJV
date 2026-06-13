using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Planet : MonoBehaviour
{

    [Range(2, 64)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;


    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;
    
    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private GameObject teleporteur;

    private GameObject spawnedTeleporterInstance;

    [SerializeField] private float spawnHeightOffset = 1.5f;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    [SerializeField, HideInInspector]
    MeshCollider[] meshColliders;
    TerrainFace[] terrainFaces;

    private void Start()
    {
        GeneratePlanet();
    }
    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        if (meshColliders == null || meshColliders.Length == 0)
        {
            meshColliders = new MeshCollider[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();

                meshColliders[i] = meshObj.AddComponent<MeshCollider>();
            }

            else if (meshColliders[i] == null)
            {
                meshColliders[i] = meshFilters[i].gameObject.GetComponent<MeshCollider>();
                if (meshColliders[i] == null)
                {
                    meshColliders[i] = meshFilters[i].gameObject.AddComponent<MeshCollider>();
                }
            }

            if (meshFilters[i].sharedMesh == null)
            {
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
        ClearAllFaces();
        SpawnObjectsOnFaces();
        SpawnTeleporteur();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                // 1. Calcul de la géométrie de la face
                terrainFaces[i].ConstructMesh();

                // 2. Transmission de la géométrie au moteur physique
                meshColliders[i].sharedMesh = meshFilters[i].sharedMesh;
            }
        }
        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours()
    {
        colourGenerator.UpdateColours();
    }
    private void ClearAllFaces()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] != null && meshFilters[i].gameObject.activeSelf)
            {
                // Destruction de tous les anciens objets sur toutes les faces
                for (int j = meshFilters[i].transform.childCount - 1; j >= 0; j--)
                {
                    DestroyImmediate(meshFilters[i].transform.GetChild(j).gameObject);
                }
            }
        }
    }

    private void SpawnObjectsOnFaces()
    {
        if (prefabToSpawn == null) return;

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] != null && meshFilters[i].gameObject.activeSelf)
            {
                // LA BOUCLE DE NETTOYAGE A ÉTÉ RETIRÉE ICI

                Mesh faceMesh = meshFilters[i].sharedMesh;
                if (faceMesh == null || faceMesh.vertices.Length == 0) continue;

                Vector3[] vertices = faceMesh.vertices;
                int[] triangles = faceMesh.triangles;
                int randomTriangleIndex = Random.Range(0, triangles.Length / 3) * 3;

                Vector3 vertexA = vertices[triangles[randomTriangleIndex]];
                Vector3 vertexB = vertices[triangles[randomTriangleIndex + 1]];
                Vector3 vertexC = vertices[triangles[randomTriangleIndex + 2]];

                float r1 = Random.value;
                float r2 = Random.value;

                if (r1 + r2 > 1f)
                {
                    r1 = 1f - r1;
                    r2 = 1f - r2;
                }

                Vector3 randomLocalPoint = vertexA + r1 * (vertexB - vertexA) + r2 * (vertexC - vertexA);
                Vector3 spawnPosition = meshFilters[i].transform.TransformPoint(randomLocalPoint);
                Vector3 outwardDirection = (spawnPosition - transform.position).normalized;
                spawnPosition += outwardDirection * spawnHeightOffset;
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, outwardDirection);

                Instantiate(prefabToSpawn, spawnPosition, spawnRotation, meshFilters[i].transform);
            }
        }
    }

    private void SpawnTeleporteur()
    {
        if (teleporteur == null) return;

        int meshnumber = Random.Range(0, 6);
        if (meshFilters[meshnumber] != null && meshFilters[meshnumber].gameObject.activeSelf)
        {
            // LA BOUCLE DE NETTOYAGE A ÉTÉ RETIRÉE ICI AUSSI

            Mesh faceMesh = meshFilters[meshnumber].sharedMesh;
            if (faceMesh == null || faceMesh.vertices.Length == 0) return;

            Vector3[] vertices = faceMesh.vertices;
            int[] triangles = faceMesh.triangles;
            int randomTriangleIndex = Random.Range(0, triangles.Length / 3) * 3;

            Vector3 vertexA = vertices[triangles[randomTriangleIndex]];
            Vector3 vertexB = vertices[triangles[randomTriangleIndex + 1]];
            Vector3 vertexC = vertices[triangles[randomTriangleIndex + 2]];

            float r1 = Random.value;
            float r2 = Random.value;

            if (r1 + r2 > 1f)
            {
                r1 = 1f - r1;
                r2 = 1f - r2;
            }

            Vector3 randomLocalPoint = vertexA + r1 * (vertexB - vertexA) + r2 * (vertexC - vertexA);
            Vector3 spawnPosition = meshFilters[meshnumber].transform.TransformPoint(randomLocalPoint);
            Vector3 outwardDirection = (spawnPosition - transform.position).normalized;
            spawnPosition += outwardDirection * spawnHeightOffset;
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, outwardDirection);

            spawnedTeleporterInstance = Instantiate(teleporteur, spawnPosition, spawnRotation, meshFilters[meshnumber].transform);
            spawnedTeleporterInstance.SetActive(false);
        }
    }

    public void SetTeleporterActive(bool isActive)
    {
        if (spawnedTeleporterInstance != null)
        {
            spawnedTeleporterInstance.SetActive(isActive);
        }
    }

    public Transform GetTeleporterTransform()
    {
        if (spawnedTeleporterInstance != null)
        {
            return spawnedTeleporterInstance.transform;
        }
        return null;
    }
}