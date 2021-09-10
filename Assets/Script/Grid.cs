using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public enum PieceType
    {
        NORMAL,
        COUNT,
    };
    [System.Serializable]
    public struct piecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }

    public int xDim;
    public int yDim;
    public piecePrefab[] piecePrefabs;
    public GameObject BackGourndPrefabs;
    private Dictionary<PieceType,GameObject> piecePrefabDict;
    // Start is called before the first frame update
    void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type)){
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    GameObject backgroud = (GameObject)Instantiate(BackGourndPrefabs, new Vector3(x, y, 0), Quaternion.identity);
                    backgroud.transform.parent = transform;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
