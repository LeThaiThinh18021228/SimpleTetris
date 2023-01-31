using DG.Tweening;
using PFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Block
{
    public SpriteRenderer renderers;
    public Block(Color color, SpriteRenderer spriteRenderer)
    {
        this.renderers = spriteRenderer;
        if (this.renderers)
        {
            this.renderers.color = color;
        }
    }
}
public class Tetris : MonoBehaviour
{
    public static Tetris Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        initPosShape = new Vector3(transform.position.x + (int)col / 2, transform.position.y + row + 2, 0);
    }
    public int row;
    public int col;
    public Block[,] blocks;
    public GameObject blockRenderer;
    public Vector3 initPosShape;

    void Start()
    {
        blocks = new Block[col, row] ;
        Color color = Color.white;
        color.a = 0.25f;

        for(int i=0; i< col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                //SpriteRenderer spriteRenderer = Instantiate(blockRenderer, new Vector3(i + 0.5f,j + 0.5f,0), Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                SpriteRenderer spriteRenderer = null;
                blocks[i, j] = new Block(color, spriteRenderer);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Render()
    {

    }

    public IEnumerator Collapse(List<int> collapseRow)
    {
        for (int i = collapseRow.First(); i < row; i++)
        {
            int decrease = 0;
            for (int p = 0; p < collapseRow.Count; p++)
            {
                if (i> collapseRow[p])
                {
                    decrease++;
                }
            }
            for (int j = 0; j < col; j++)
            {
                /*
                if (blocks[j, i].renderers && i >=1)
                {
                    int k = i;
                    while (k - 1 >= 0 && !blocks[j, k-1].renderers)
                    {
                        k--;
                    }
                    blocks[j, k].renderers = blocks[j, i].renderers;
                    blocks[j, i].renderers = null;
                    blocks[j, k].renderers.transform.DOMoveY(k, 1);
                }*/
                int k = i-decrease;
                blocks[j, k].renderers = blocks[j, i].renderers;
                blocks[j, i].renderers = null;
                if (blocks[j, k].renderers)
                {
                    blocks[j, k].renderers.transform.DOMoveY(k, 1);

                }
            }
        }

        yield return new WaitForSeconds(1);

        StartCoroutine(DeleteCompletedRow());
    }

    public IEnumerator DeleteCompletedRow()
    {
        List<int> rowCollapse = new List<int>();
        for (int i = 0; i < row; i++)
        {
            bool isRowCompleted = true;
            for (int j = 0; j < col; j++)
            {
                if (!blocks[j, i].renderers)
                {
                    isRowCompleted = false;
                    break;
                }
            }
            if (isRowCompleted == true)
            {
                rowCollapse.Add(i);
                for (int j = 0; j < col; j++)
                {
                    Destroy(blocks[j, i].renderers.gameObject);
                    blocks[j, i].renderers = null;
                }
            }

        }
        Debug.Log("Deleted");
        if (rowCollapse.Count>0)
        {
            yield return StartCoroutine(Collapse(rowCollapse));
        }
        else
        {
            yield return null;
        }
        Debug.Log("Collapsed");
    }
    public IEnumerator AssignShape(Shape shape)
    {
        Debug.Log("ASsing");
        for (int i= 0;i< shape.GetCurrentPos().Count ; i++)
        {
            int x= (int)shape.currentPos[i].x;
            int y= (int)shape.currentPos[i].y;
            //Debug.Log((int)shape.currentPos[i].x + "_" + (int)shape.currentPos[i].y);
            if (y<row && !blocks[x, y].renderers)
            {
                blocks[(int)shape.currentPos[i].x, (int)shape.currentPos[i].y].renderers = shape.spriteRenderers[i];
                shape.spriteRenderers[i].transform.parent = transform;

            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                Restart();
            }

        }
        shape.transform.position = new Vector3(100, 100, 0);
        yield return StartCoroutine(DeleteCompletedRow());
        Debug.Log("ASsingned");
        shape.Init();
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public bool CanMoveDownShape(Shape shape)
    {
        shape.GetCurrentPos();
        bool can = true;
        foreach (var pos in shape.GetCurrentPos())
        {
            //Debug.Log(pos.x + "_" + pos.y);
            if (pos.y >= row)
            {
                continue;
            }
            if (pos.y >= 1)
            {
                if (blocks[(int)pos.x, (int)pos.y - 1].renderers)
                {
                    can = false; break;
                }
            }
            else
            {
                can = false; break;
            }
        }
        return can;

    }
    internal bool CanRotateDirShape(Shape shape)
    {
        bool can = true;
        Vector3 initPos = shape.transform.position;
        Dir dir = (Dir)((int)(shape.dir + 1) % (int)Dir.NumberOfTypes);
        can = CanRotateDirShape(shape.GetCurrentPos(dir));
        if (!can)
        {
            shape.transform.position = initPos + Vector3.left;
            can = CanRotateDirShape(shape.GetCurrentPos(dir));
        }
        if (!can)
        {
            shape.transform.position = initPos + Vector3.right;
            can = CanRotateDirShape(shape.GetCurrentPos(dir));
        }
        if (!can)
        {
            shape.transform.position = initPos + Vector3.up;
            can = CanRotateDirShape(shape.GetCurrentPos(dir));
        }
        if (!can)
        {
            shape.transform.position = initPos + Vector3.down;
            can = CanRotateDirShape(shape.GetCurrentPos(dir));
        }
        if (!can)
        {
            Debug.Log("cant rotate");
            shape.transform.position = initPos;
        }
        return can;
    }
    internal bool CanRotateDirShape(List<Vector2> poses)
    {
        bool can = true;
        foreach (var pos in poses)
        {
            if ((int)pos.x < col && (int)pos.x >= 0)
            {
                if (pos.y >= row)
                {
                    continue;
                }
                Debug.Log((int)pos.x+"_"+ (int)pos.y);
                if (blocks[(int)pos.x, (int)pos.y].renderers)
                {
                    can = false; break;
                }
            }
            else
            {
                can = false; break;
            }

        }
        return can;
    }
    internal bool CanMoveLeftShape(Shape shape)
    {
        bool can = true;

        foreach (var pos in shape.GetCurrentPos())
        {
            if ((int)pos.x - 1 >= 0)
            {
                if (pos.y >= row)
                {
                    continue;
                }
                else
                if (blocks[(int)pos.x - 1, (int)pos.y].renderers)
                {
                    can = false; break;
                }
            }
            else
            {
                can = false; break;
            }
        }
        return can;
    }

    internal bool CanMoveRightShape(Shape shape)
    {
        bool can = true;
        foreach (var pos in shape.GetCurrentPos())
        {
            if ((int)pos.x + 1 < col)
            {
                if (pos.y >= row)
                {
                    continue;
                }
                else if (blocks[(int)pos.x + 1, (int)pos.y].renderers)
                {
                    can = false; break;
                }
            }
            else
            {
                can = false; break;
            }

        }
        return can;
    }
}
