using UnityEngine;
using System.Collections.Generic;

public class SteeringObstacleManager : MonoBehaviour {

    public static SteeringObstacleManager Instance { get; private set; }

    private SteeringObstacle[] _steeringObstacles;
    public SteeringObstacle[] SteeringObstacles
    {
        get { return _steeringObstacles; }
    }

    private SteeringWall[] _steeringWalls;
    public SteeringWall[] SteeringWalls
    {
        get { return _steeringWalls; }
    }

    private const string STEERING_OBSTACLE_TAG_NAME = "SteeringObstacle";
    private const string STEERING_WALL_TAG_NAME = "SteeringWall";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _steeringObstacles = GetSteeringObstaclesFromScene();
        _steeringWalls = GetSteeringWallsFromScene();
    }

    private SteeringObstacle[] GetSteeringObstaclesFromScene()
    {
        GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag(STEERING_OBSTACLE_TAG_NAME);
        List<SteeringObstacle> validObstacles = new List<SteeringObstacle>();

        SteeringObstacle attachedObstacle;
        for (int i = 0; i < taggedGameObjects.Length; i++)
        {
            attachedObstacle = taggedGameObjects[i].GetComponent<SteeringObstacle>();
            if(attachedObstacle != null)
            {
                validObstacles.Add(attachedObstacle);
            }
            else
            {
                Debug.LogError("GameObject " + taggedGameObjects[i] + " is tagged as a " + STEERING_OBSTACLE_TAG_NAME + " but doesn't have a SteeringObstacle attached!");
            }
        }

        return validObstacles.ToArray();
    }

    public SteeringWall[] GetSteeringWallsFromScene()
    {
        GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag(STEERING_WALL_TAG_NAME);
        List<SteeringWall> validWalls = new List<SteeringWall>();

        SteeringWall attachedWall;
        for (int i = 0; i < taggedGameObjects.Length; i++)
        {
            attachedWall = taggedGameObjects[i].GetComponent<SteeringWall>();
            if (attachedWall != null)
            {
                validWalls.Add(attachedWall);
            }
            else
            {
                Debug.LogError("GameObject " + taggedGameObjects[i] + " is tagged as a " + STEERING_WALL_TAG_NAME + " but doesn't have a SteeringWall attached!");
            }
        }

        return validWalls.ToArray();
    }

    public void TagObstaclesWithinRange(Vehicle vehicle, float boxLength)
    {
        for (int i = 0; i < _steeringObstacles.Length; i++)
        {
            if(Vector3.SqrMagnitude(vehicle.Position - _steeringObstacles[i].Position) <= boxLength * boxLength)
            {
                _steeringObstacles[i].TaggedForAvoidance = true;
            }
            else
            {
                _steeringObstacles[i].TaggedForAvoidance = false;
            }
        }
    }
}
