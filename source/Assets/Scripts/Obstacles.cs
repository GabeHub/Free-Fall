using UnityEngine;
[System.Serializable]
public class Obstacle
{
    public GameObject objectPrefab;
    //public int xSize;
    //public int ySize;
}

public class ObstacleGear : Obstacle
{
    public Direction direction;
}

public class ObstacleSpike : Obstacle
{
    public float rotation;
}

public enum Direction
{
    left = -1,
    right = 1
}
