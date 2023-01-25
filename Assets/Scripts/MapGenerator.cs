using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    //[SerializeField] TextAsset mapText;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject goal;
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform map2D;
    [SerializeField] Sprite wallSprite,groundSprite;
    //[SerializeField] WallsArr[] wallsArr;
    MazeCreator maze;
    int[,] mazeDatas;
    Camera cam;
    public enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1

    }

    
    MAP_TYPE[,] mapTable;
    
    MapChip[,] mapChip;
    List<Vector2Int> tracePlayer = new List<Vector2Int>();
    float mapSize;
    Vector2 centerPos,goalPos;
    public int w, h;
    int goalRandomPos;

    public Player player;


    private void Awake()
    {
        GameManager.I.map = this;
    }

    public void InitMap()
    {
        cam = Camera.main;
        w = 7 + GameManager.I.w;
        h = 7 + GameManager.I.h;
        goalRandomPos = Random.Range(0, 101);

        if (w % 2 == 0) w++;
        if (h % 2 == 0) h++;
        _loadMapData();
        _createMap();
        _setPlayer();
        _setGoal();
    }

    void _loadMapData()
    {
        maze = new MazeCreator(w, h);
        mazeDatas = new int[w, h];
        mazeDatas = maze.CreateMaze();
        int row = mazeDatas.GetLength(1);
        int col = mazeDatas.GetLength(0);
        mapTable = new MAP_TYPE[col, row];
        mapChip = new MapChip[col, row];
        
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                if(x == col/2 && y == row / 2)
                {
                    mapTable[x, y] = MAP_TYPE.GROUND;
                }
                else
                {
                    mapTable[x, y] = (MAP_TYPE)mazeDatas[x, y];
                }
            }
        }

    }

    void _createMap()
    {
        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;



        if (mapTable.GetLength(0) % 2 == 0)
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize;
        }

        if (mapTable.GetLength(1) % 2 == 0)
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize;
        }

        

        for (int y = 0; y < mapTable.GetLength(1); y++)
        {
            for (int x = 0; x < mapTable.GetLength(0); x++)
            {
                
                Vector2Int pos = new Vector2Int(x, y);
                GameObject _wall = Instantiate(prefabs[(int)MAP_TYPE.WALL], map2D);
                GameObject _map = Instantiate(prefabs[(int)mapTable[x, y]], map2D);
                _wall.transform.position = ScreenPos(pos);
                _map.transform.position = ScreenPos(pos);
                mapChip[x, y] = _map.GetComponent<MapChip>();

            }
        }
    }

    private void _setPlayer()
    {
        int harf = mapTable.GetLength(0) / 2;
        
        Vector2Int pos = new Vector2Int(harf, harf);
        
        
        player = Instantiate(playerPrefab, map2D);
        player.transform.position = ScreenPos(pos);
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0, -2f, -10f);
        player.GetComponent<Player>().currentPos = pos;
        player.GetComponent<Player>().mapGenerator = this;
    }



    private void _setGoal()
    {
        if (goalRandomPos >= 0 && goalRandomPos < 25)
        {
            Vector2Int pos = new Vector2Int(w - 2, h - 2);
            PutGoal(pos);
        }
        else if (goalRandomPos >= 25 && goalRandomPos < 50)
        {
            Vector2Int pos = new Vector2Int(1, h - 2);
            PutGoal(pos);
        }
        else if(goalRandomPos >= 50 && goalRandomPos < 75)
        {
            Vector2Int pos = new Vector2Int(w -2, 1);
            PutGoal(pos);
        }
        else
        {
            Vector2Int pos = new Vector2Int(w - 2, 1);
            PutGoal(pos);
        }
    }

    private void PutGoal(Vector2Int _pos)
    {
        GameObject _goal = Instantiate(goal, map2D);
        _goal.transform.position = ScreenPos(_pos);
        goalPos = _pos;
    }


    public Vector2 ScreenPos(Vector2Int _pos)
    {
        return new Vector2(
            _pos.x * mapSize - centerPos.x,
            -(_pos.y * mapSize - centerPos.y ));
    }

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        return mapTable[_pos.x, _pos.y];
    }

    public void PutGround(Vector2Int _pos)
    {
        mapTable[_pos.x, _pos.y] = MAP_TYPE.GROUND;
        GameObject _map = Instantiate(prefabs[0], map2D);
        _map.transform.position = ScreenPos(_pos);
        mapChip[_pos.x, _pos.y] = _map.GetComponent<MapChip>();
    }

    public void TracePlayer(Vector2Int _pos)
    {
        mapTable[_pos.x, _pos.y] = MAP_TYPE.WALL;
        mapChip[_pos.x, _pos.y].LostAnim();
        //mapChip[_pos.x, _pos.y].ChangeSprite(wallSprite);
        tracePlayer.Add(_pos);
    }

    public void CheckGoal(Vector2Int _pos)
    {
        if(_pos == goalPos)
        {
            SoundManager.I.PlaySE(SESoundData.SE.CLEAR);
            GameManager.I.StageClear();
        }
    }

    public void InTheWall()
    {
        if (GameManager.I.life == 0)
        {
            GameManager.I.GameOver();
            return;
        } 
        GameManager.I.Miss();
        StartCoroutine(_resetStage());
        
    }



    IEnumerator _resetStage()
    {
        yield return new WaitForSeconds(0.1f);
        tracePlayer.Reverse();
        foreach (Vector2Int _pos in tracePlayer)
        {
            yield return new WaitForSeconds(0.02f);
            mapTable[_pos.x, _pos.y] = MAP_TYPE.GROUND;
            mapChip[_pos.x, _pos.y].SetDefault();

        }
        tracePlayer.Clear();
        player.ResetPlayer();
        GameManager.I.Restart();
    }
}
