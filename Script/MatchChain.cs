using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchChain : MonoBehaviour
{
    // Start is called before the first frame update
    public ArrayLayout boarLayout;


    [Header("Prefabs")]
    public GameObject NodePices;
    [Header("Ui Elements")]
    public RectTransform gameBoar;
    public Sprite[] pieces;
    int width = 8;
    int height = 8;
    Node[,] boar;
    System.Random random;
    void Start()
    {
        StartGame();
    }
    void StartGame()
    {

        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        InitializeBoar();
        VerifyBoar();
        InstantiateBoard();
    }
    // Update is called once per frame
    void Update()
    {

    }
    void InitializeBoar()
    {
        boar = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                boar[x, y] = new Node(boarLayout.rows[y].row[x] ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }
    string getRandomSeed()
    {
        string seed = "";
        string acceptTableCharacter = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()";
        for (int i = 0 - 1; i <= 20; i++)
        {
            seed += acceptTableCharacter[Random.Range(0, acceptTableCharacter.Length)];
        }
        return getRandomSeed();
    }
    void VerifyBoar()
    {
        List<int> remove;
        boar = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;
                remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {
                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    setvalureAtPoint(p, newValue(ref remove));
                }
            }
        }
    }
    void InstantiateBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int val = boar[x, y].value;
                if (val <= 0) continue;
                GameObject p = Instantiate(NodePices, gameBoar);
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (54 * y));
            }
        }
        List<Point> isConnected(Point p, bool main)
        {
            List<Point> connected = new List<Point>();
            int val = getValueAtPoint(p);

            Point[] directions =
            {
        Point.up,
        Point.down,
        Point.right,
        Point.left
        };
            foreach (Point dir in directions) // Checking 2 or more same Shapes in the directions 0x000
            {
                List<Point> line = new List<Point>();
                int same = 0;
                for (int i = 0; i < 3; i++)
                {
                    Point check = Point.Add(p, Point.mult(dir, i));
                    if (getValueAtPoint(check) == val)
                    {
                        line.Add(check);
                        same++;
                    }
                }
                if (same > 1)// if there more than 1 of the same shape in the direction then we know it is a match
                {
                    AddPoint(ref connected, line);// Add these points to the overaching connected list
                }
            }

            for (int i = 0; i < 2; i++) // Checking if we are in the middle of two of the same shapes
            {
                List<Point> line = new List<Point>();
                int same = 0;
                Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[i + 2]) };
                foreach (Point next in check)// Check bothSides of the piece, if they are same value,add them to the list
                    if (getValueAtPoint(next) == val)
                    {
                        line.Add(next);
                        same++;
                    }

                if (same > 1)
                {
                    AddPoint(ref connected, line);
                }
            }
            for (int i = 0; i < 4; i++) // check for 2x2
            {
                List<Point> square = new List<Point>();
                int same = 0;
                int next = i + 1;
                if (next >= 4)
                    next -= 4;
                Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[next]), Point.Add(p, Point.Add(directions[i], directions[next])) };
                foreach (Point point in check)// Check all sides of the piece, if they are same value,add them to the list
                    if (getValueAtPoint(point) == val)
                    {
                        square.Add(point);
                        same++;
                    }
                if (same > 2)
                    AddPoint(ref connected, square);
            }
            if (main) // check for other matches along the current match
            {
                for (int i = 0; i < connected.Count; i++)
                {
                    AddPoint(ref connected, isConnected(connected[i], false));
                }
            }
            if (connected.Count > 0)
                connected.Add(p);
            return connected;
        }
        void AddPoint(ref List<Point> points, List<Point> add)
        {
            foreach (Point p in add)
            {
                bool doadd = true;

                for (int i = 0; i < add.Count; i++)
                {
                    if (add[i].Equals(p))
                    {
                        doadd = false;
                        break;
                    }
                }
                if (doadd) points.Add(p);

            }
        }
        int fillPiece()
        {
            int val = 0;
            val = (random.Next(0, 100) / (100 / pieces.Length) + 1);
            return val;
        }
        int getValueAtPoint(Point p)
        {
            if (p.x < 0 || p.x >= width || p.y < 0 || p.y > height) return -1;
            return boar[p.x, p.y].value;
        }
        int newValue(ref List<int> remove)
        {
            List<int> availabe = new List<int>();
            for (int i = 0; i < pieces.Length; i++)
            {
                availabe.Add(i + 1);
            }
            foreach (int i in remove)
                availabe.Remove(i);
            if (availabe.Count <= 0) return 0;
            return availabe[random.Next(0, availabe.Count)];
        }
        void setvalureAtPoint(Point p, int v)
        {
            boar[p.x, p.y].value = v;
        }
    }

    
}
[System.Serializable]
public class Node
{
    public int value;
    public Point index;
    public Node(int v, Point i)
    {
        value = v;
        index = i;

    }
}
