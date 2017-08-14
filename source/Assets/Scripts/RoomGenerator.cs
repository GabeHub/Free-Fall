using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{


    public GameObject player;

    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;

    [System.Serializable]
    public struct Obstacle
    {
        public GameObject objectPrefab;
        public int xSize;
        public int ySize;
        public float xScaleCoefficient;
        public float yScaleCoefficient;

        public Obstacle(GameObject prefab, int xS, int yS, int xSC, int ySC)
        {
            objectPrefab = prefab;
            xSize = xS;
            ySize = yS;
            xScaleCoefficient = xSC;
            yScaleCoefficient = xSC;
        }
    }
    public Obstacle[] obstacles;
    
    public List<GameObject> currentObjects;

    public struct ObstacleMatrixCell
    {
        public float xPosition;
        public float yPosition;
        public bool isEmpty;

        public ObstacleMatrixCell(float xPos, float yPos, bool empty)
        {
            xPosition = xPos;
            yPosition = yPos;
            isEmpty = empty;
        }
    }
    public int xMatrix = 4;
    public int yMatrix = 10;
    private ObstacleMatrixCell[,] matrix;
    private ObstacleMatrixCell cell;

    private float height;
    private float width;
    
    private float deltaX;
    private float deltaY;

    // Use this for initialization
    void Start()
    {
        height = 2.0f * Camera.main.orthographicSize;
        width = height * Camera.main.aspect;

        deltaX = (width / xMatrix);
        deltaY = (height / yMatrix);

        SortObstacles();

        float firstRoomPosition = height * 0.5f;
        AddRoom(firstRoomPosition);
        float playerSize = width * 0.1f;
        player.transform.localScale = new Vector3(playerSize, playerSize, 1);

        float xPos = -(width * 0.5f) + deltaX * 0.5f;
        float yPos = (height * 0.5f) - deltaY * 0.5f;
        cell = new ObstacleMatrixCell(xPos, yPos, true);

        matrix = new ObstacleMatrixCell[xMatrix, yMatrix];
        MatrixUpdate(cell);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GenerateRoomIfRequired();
        GenerateObstaclesIfRequired();
    }

    void AddRoom(float farhtestRoomEndY)
    {
        int roomIndex = Random.Range(0, availableRooms.Length);
        GameObject room = Instantiate(availableRooms[roomIndex]);
        float roomHeight = room.transform.Find("leftWall").localScale.y;
        float roomCenter = farhtestRoomEndY - roomHeight * 0.5f;
        room.transform.position = new Vector2(0, roomCenter);
        currentRooms.Add(room);
    }

    void GenerateRoomIfRequired()
    {
        List<GameObject> roomsToRemove = new List<GameObject>();
        bool addRooms = true;
        float playerY = player.transform.position.y;
        float removeRoomY = playerY + height;
        float addRoomY = playerY - height;
        float farthestRoomEndY = 0;

        foreach (var room in currentRooms)
        {
            float roomHeight = room.transform.Find("leftWall").localScale.y;
            float roomStartY = room.transform.position.y + roomHeight / 2.0f;
            float roomEndY = room.transform.position.y - roomHeight / 2.0f;

            if (roomStartY < addRoomY)
                addRooms = false;

            if (roomEndY > removeRoomY)
                roomsToRemove.Add(room);

            farthestRoomEndY = Mathf.Min(farthestRoomEndY, roomEndY);
        }

        foreach (var room in roomsToRemove)
        {
            currentRooms.Remove(room);
            Destroy(room);
        }

        if (addRooms)
            AddRoom(farthestRoomEndY);
    }

    void MatrixUpdate(ObstacleMatrixCell cell)
    {
        for (int i = 0; i < yMatrix; i++)
        {
            for (int j = 0; j < xMatrix; j++)
            {
                matrix[j, i] = cell;
                cell.xPosition += deltaX;
            }
            cell.xPosition = -(width * 0.5f) + deltaX * 0.5f;
            cell.yPosition -= deltaY;
        }
    }

    void GenerateObstaclesIfRequired()
    {
        if ((player.transform.position.y - height) < matrix[xMatrix - 1, yMatrix - 1].yPosition)
        {
            cell.xPosition = matrix[0, yMatrix - 1].xPosition;
            cell.yPosition = matrix[0, yMatrix - 1].yPosition - deltaY;
            cell.isEmpty = true;
            MatrixUpdate(cell);
            MatrixFilling();
        }

        List<GameObject> obstaclesToRemove = new List<GameObject>();
        float playerY = player.transform.position.y;
        float removeObstaclesY = playerY + height;

        foreach (var obj in currentObjects)
        {
            if (obj.gameObject)
            {
                float objY = obj.transform.position.y;

                if (objY > removeObstaclesY)
                    obstaclesToRemove.Add(obj);

            }
        }

        foreach (var obj in obstaclesToRemove)
        {
            currentObjects.Remove(obj);
            Destroy(obj);
        }
    }

    void SortObstacles()
    {
        Obstacle temp = new Obstacle();
        for (int k = obstacles.Length - 1; k > 0; k--)
        {
            for (int i = 0; i < k; i++)
            {
                float sizeCurrent = obstacles[i].xSize * obstacles[i].ySize;
                float sizeNext = obstacles[i + 1].xSize * obstacles[i + 1].ySize;
                if (sizeCurrent < sizeNext)
                {
                    temp = obstacles[i];
                    obstacles[i] = obstacles[i + 1];
                    obstacles[i + 1] = temp;
                }
            }
        }
    }

    Obstacle SelectRandomObstacle(int left)
    {
        int rand = 0;
        int right = obstacles.Length;
        float sizeCurrent = obstacles[left].xSize * obstacles[left].ySize;
        float sizePrevious = obstacles[left].xSize * obstacles[left].ySize;

        if (sizeCurrent > 1)
        {
            for (int i = left + 1; i < right; i++)
            {
                sizeCurrent = obstacles[i].xSize * obstacles[i].ySize;
                if (sizeCurrent < sizePrevious)
                {
                    right = i;
                    break;
                }
                sizePrevious = sizeCurrent;
            }
        }

        rand = Random.Range(left, right);
        return obstacles[rand];
    }

    List<ObstacleMatrixCell> FindPositions(int xLength, int yLength)
    {
        bool flag = false;
        List<ObstacleMatrixCell> cells = new List<ObstacleMatrixCell>();

        for (int i = 0; i <= yMatrix - yLength; i++)
        {
            for (int j = 0; j <= xMatrix - xLength; j++)
            {
                for (int k = i; k < i + yLength; k++)
                {
                    for(int l = j; l < j + xLength; l++)
                    {
                        if (!matrix[l, k].isEmpty)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    cells.Add(matrix[j, i]);
                }
                else
                {
                    flag = false;
                }
            }
        }

        return cells;
    }

    void AddObstacle(ObstacleMatrixCell cell, Obstacle obstacle)
    {
        int xPos = 0, yPos = 0;
        for (int i = 0; i < yMatrix; i++)
        {
            for(int j = 0; j < xMatrix; j++)
            {
                if (matrix[j,i].xPosition == cell.xPosition && matrix[j,i].yPosition == cell.yPosition)
                {
                    xPos = j;
                    yPos = i;
                    break;
                }
            }
        }

        int count = 0;
        float obstaclePositionX = 0, obstaclePositionY = 0;
        for (int i = yPos; i < yPos + obstacle.ySize; i++)
        {
            for (int j = xPos; j < xPos + obstacle.xSize; j++)
            {
                matrix[j, i].isEmpty = false;
                count++;
                obstaclePositionX += matrix[j, i].xPosition;
                obstaclePositionY += matrix[j, i].yPosition;
            }
        }

        obstaclePositionX /= count;
        obstaclePositionY /= count;

        GameObject obj = Instantiate(obstacle.objectPrefab);
        obj.transform.position = new Vector2(obstaclePositionX, obstaclePositionY);
        float size = width * 0.05f;
        obj.transform.localScale = new Vector3(size * obstacle.xScaleCoefficient, size * obstacle.yScaleCoefficient, 1);
        currentObjects.Add(obj);
    }

    void MatrixFilling()
    {
        int startPosition = 0;
        for (int i = 0; i < 10; i++)
        {
            if (startPosition >= obstacles.Length)
            {
                break;
            }
            Obstacle obstacle = SelectRandomObstacle(startPosition);
            List<ObstacleMatrixCell> positions = FindPositions(obstacle.xSize, obstacle.ySize);
            if (positions.Count != 0)
            {
                int rand = Random.Range(0, positions.Count);
                ObstacleMatrixCell position = positions[rand];
                AddObstacle(position, obstacle);
            }
            else
            {
                startPosition++;
                i--;
            }
        }
    }
}
