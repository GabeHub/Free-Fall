using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class RoomGenerator : MonoBehaviour
{
    public GameObject player;

    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;

    public Obstacle[] obstacles;
    
    public List<GameObject> currentObjects;

    public struct ObstacleMatrixCell
    {
        public float xPosition;
        public float yPosition;
        public bool isEmpty;
        public string obstacleName;
        public Direction gearDirection;
        public int rotation;
        public GameObject obstacle;
        public int obstacleSizeX;
        public int obstacleSizeY;
    }

    public static int raw = 19;
    public static int column = 5;

    private ObstacleMatrixCell[,] matrix;
    public static ObstacleMatrixCell[,] playerMatrix;
    private ObstacleMatrixCell cell;

    private float height;
    private float width;
    private float borders = 0.55f;

    private float deltaX;
    public static float deltaY;

    public static string levelPath;
    public static int startLevelNumber;
    private int levelNumber;

    // Use this for initialization
    void Start()
    {
        height = 2.0f * Camera.main.orthographicSize;
        width = height * Camera.main.aspect - borders;

        deltaX = (width / column);
        deltaY = (2.0f * height / raw);

        //SortObstacles();

        float firstRoomPosition = 2.0f * height * 0.5f;
        AddRoom(firstRoomPosition);
        //float playerSize = width * 0.1f;
        //player.transform.localScale = new Vector3(playerSize, playerSize, 1);

        float xPos = -(width * 0.5f) + deltaX * 0.5f;
        float yPos = (2.0f * height * 0.5f) - deltaY * 0.5f;
        cell = new ObstacleMatrixCell()
        {
            xPosition = xPos,
            yPosition = yPos - height,
            isEmpty = true
        };

        matrix = new ObstacleMatrixCell[raw, column];
        playerMatrix = new ObstacleMatrixCell[raw, column];
        MatrixUpdate(cell);
        levelNumber = startLevelNumber;
        InitMatrix(levelNumber);
        LevelUpdate();
        MatrixFilling();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player)
        {
            InitPlayerMatrix();
            GenerateRoomIfRequired();
            GenerateObstaclesIfRequired();
        }
        else return;
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
        for (int i = 0; i < raw; i++)
        {
            for (int j = 0; j < column; j++)
            {
                matrix[i, j] = cell;
                cell.xPosition += deltaX;
            }
            cell.xPosition = -(width * 0.5f) + deltaX * 0.5f;
            cell.yPosition -= deltaY;
        }
    }

    void GenerateObstaclesIfRequired()
    {
        if ((player.transform.position.y - height) < matrix[raw - 1, column - 1].yPosition)
        {
            cell.xPosition = matrix[raw - 1, 0].xPosition;
            cell.yPosition = matrix[raw - 1, 0].yPosition - deltaY;
            cell.isEmpty = true;
            MatrixUpdate(cell);
            InitMatrix(levelNumber);
            LevelUpdate();
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

    void AddObstacle(ObstacleMatrixCell cell, Obstacle obstacle)
    {
        int xPos = 0, yPos = 0;
        for (int i = 0; i < raw; i++)
        {
            for(int j = 0; j < column; j++)
            {
                if (matrix[i, j].xPosition == cell.xPosition && matrix[i, j].yPosition == cell.yPosition)
                {
                    xPos = i;
                    yPos = j;
                    break;
                }
            }
        }

        int count = 0;
        float obstaclePositionX = 0, obstaclePositionY = 0;
        for (int i = xPos; i < xPos + cell.obstacleSizeY; i++)
        {
            for (int j = yPos; j < yPos + cell.obstacleSizeX; j++)
            {
                matrix[i, j].isEmpty = false;
                count++;
                obstaclePositionX += matrix[i, j].xPosition;
                obstaclePositionY += matrix[i, j].yPosition;
            }
        }
        obstaclePositionX /= count;
        obstaclePositionY /= count;

        GameObject obj = Instantiate(obstacle.objectPrefab);
        obj.transform.position = new Vector2(obstaclePositionX, obstaclePositionY);
        if (cell.rotation != 0)
        {
            obj.transform.Rotate(Vector3.forward * cell.rotation);
        }
        if (cell.gearDirection != 0)
        {
            obj.GetComponentInChildren<Rotation>().direction = cell.gearDirection;
        }
        //float size = width * 0.05f;
        //obj.transform.localScale = new Vector3(size, size, 1);
        currentObjects.Add(obj);

        for (int i = xPos; i < xPos + cell.obstacleSizeY; i++)
        {
            for (int j = yPos; j < yPos + cell.obstacleSizeX; j++)
            {
                matrix[i, j].isEmpty = false;
                matrix[i, j].obstacle = obj;
                matrix[i, j].obstacleName = obj.name;
            }
        }
    }

    void MatrixFilling()
    {
        Obstacle obstacle = new Obstacle();
        for (int i = 0; i < raw; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (!matrix[i,j].isEmpty && !matrix[i,j].obstacle)
                {
                    foreach(var obs in obstacles)
                    {
                        if(matrix[i,j].obstacleName == obs.objectPrefab.name)
                        {
                            obstacle = obs;
                        }
                    }
                    AddObstacle(matrix[i, j], obstacle);
                }
            }
        }
    }

    void InitPlayerMatrix()
    {
        if(player.transform.position.y <= matrix[0, 0].yPosition && matrix[0, 0].yPosition < playerMatrix[0, 0].yPosition)
        {
            for (int i = 0; i < raw; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    playerMatrix[i, j] = matrix[i, j];
                }
            }
        }
    }

    void InitMatrix(int number)
    {
        if (File.Exists(levelPath + number + ".xml"))
        {
            XmlTextReader xtr;

            for (int i = 0; i < raw; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    xtr = new XmlTextReader(levelPath + number + ".xml");
                    while (xtr.Read())
                    {
                        if (xtr.IsStartElement("cell") && !xtr.IsEmptyElement && int.Parse(xtr.GetAttribute("x").ToString()) == i && int.Parse(xtr.GetAttribute("y").ToString()) == j)
                        {
                            matrix[i, j].isEmpty = false;
                            matrix[i, j].obstacleSizeX = int.Parse(xtr.GetAttribute("xSize").ToString());
                            matrix[i, j].obstacleSizeY = int.Parse(xtr.GetAttribute("ySize").ToString());
                            if (xtr.GetAttribute("Type").ToString() == "gear")
                            {
                                if (xtr.GetAttribute("Direction").ToString() == "left")
                                    matrix[i, j].gearDirection = Direction.left;
                                else matrix[i, j].gearDirection = Direction.right;
                            }
                            if (xtr.GetAttribute("Type").ToString() == "rotatable")
                            {
                                matrix[i, j].rotation = int.Parse(xtr.GetAttribute("Rotation").ToString());
                            }
                            matrix[i, j].obstacleName = xtr.ReadString();
                        }
                    }
                }
            }
        }
        else Debug.Log("There is no such file: " + levelPath + number + ".xml");
    }

    void LevelUpdate()
    {
        levelNumber++;
        if (levelNumber > 30)
        {
            levelNumber = 1;
        }
    }
}
