using PFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Dir
{
    Up,
    Right,
    Down,
    Left,
    NumberOfTypes
}
public enum ShapeType
{
    _1x1,
    _2x2,
    L2,
    L3,
    L4,
    T2,
    T3,
    I2,
    I3,
    I4,
    NumberOfTypes
}
public class Shape : MonoBehaviour
{
    public List<Vector2> poses;
    public List<Vector2> currentPos;
    public List<SpriteRenderer> spriteRenderers;
    public Color color;
    public Dir dir;
    public ShapeType type;

    float currentTime;
    public float moveDownShapeTime;
    static Dictionary<Dir, Vector2> dirMatrix = new Dictionary<Dir, Vector2>()
    {
        { Dir.Up, new Vector2(1,1) },
        { Dir.Right, new Vector2(1,-1) },
        { Dir.Down, new Vector2(-1,-1) },
        { Dir.Left, new Vector2(-1,1) },
    };
    static Dictionary<ShapeType, List<Vector2>> posesMatrix = new Dictionary<ShapeType, List<Vector2>>()
    {
        { ShapeType._1x1, new List<Vector2>(){ new Vector2(0, 0)} },
        { ShapeType._2x2, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, 1) , new Vector2(1, 0) , new Vector2(1, 1) } },
        { ShapeType.L2, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, -1) } },
        { ShapeType.L3, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, -1), new Vector2(1, -1) } },
        { ShapeType.L4, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, -1), new Vector2(0, -2), new Vector2(1, -2) } },
        { ShapeType.T2, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0)} },
        { ShapeType.T3, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, -1), new Vector2(0, -2), new Vector2(1, 0), new Vector2(-1, 0) } },
        { ShapeType.I2, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, -1), } },
        { ShapeType.I3, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, -1), new Vector2(0, 1), } },
        { ShapeType.I4, new List<Vector2>(){ new Vector2(0, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(0, -2) } },

    };
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Init()
    {
        spriteRenderers = new List<SpriteRenderer>();
        currentTime = 0;
        transform.position = Tetris.Instance.initPosShape;
        color = Random.ColorHSV();
        dir = Dir.Up;
        type = (ShapeType)Random.Range(0, (int)ShapeType.NumberOfTypes);
        poses = posesMatrix[type];
        foreach (var pos in poses)
        {
            SpriteRenderer spriteRenderer = Instantiate(Tetris.Instance.blockRenderer, transform.position + pos.ToXY() ,Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            spriteRenderers.Add(spriteRenderer);
        }
        
    }
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime>moveDownShapeTime)
        {
            MoveDown();
        }
    }

    public void MoveDown()
    {
        currentTime= 0;
        if (Tetris.Instance.CanMoveDownShape(this))
        {
            transform.position += Vector3.down;
        }
        else
        {
            StartCoroutine(Tetris.Instance.AssignShape(this));
        }
        //Debug.Log(transform.position);
    }

    public void RotateDir()
    {
        if (Tetris.Instance.CanRotateDirShape(this))
        {
            dir = (Dir) ((int)(dir + 1) % (int)Dir.NumberOfTypes);
            currentPos = GetCurrentPos();
            for (int i = 0; i < currentPos.Count; i++)
            {
                spriteRenderers[i].transform.position = currentPos[i];
            }
        }
    }

    public void MoveLeft()
    {
        if (Tetris.Instance.CanMoveLeftShape(this))
        {
            transform.position += Vector3.left;
        }
    }
    
    public void MoveRight()
    {
        if (Tetris.Instance.CanMoveRightShape(this))
        {
            transform.position += Vector3.right;
        }
    }

    public List<Vector2> GetCurrentPos(Dir dir)
    {
        List<Vector2> result = new List<Vector2>(poses);
        if (dir == Dir.Right || dir == Dir.Left)
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = new Vector2(poses[i].y, poses[i].x) * dirMatrix[dir] + new Vector2(transform.position.x, transform.position.y);
            }
        }
        else
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = new Vector2(poses[i].x, poses[i].y) * dirMatrix[dir] + new Vector2(transform.position.x, transform.position.y);
            }
        }
        currentPos = result;
        return result;
    }

    public List<Vector2> GetCurrentPos()
    {
        List<Vector2> result = new List<Vector2>(poses);
        if (dir == Dir.Right || dir == Dir.Left)
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = new Vector2(poses[i].y, poses[i].x) * dirMatrix[dir] + new Vector2(transform.position.x, transform.position.y);
            }
        }
        else
        {
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = new Vector2(poses[i].x, poses[i].y) * dirMatrix[dir] + new Vector2(transform.position.x, transform.position.y);
            }
        }
        currentPos = result;
        return result;
    }
}
