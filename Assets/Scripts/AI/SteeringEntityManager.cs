using UnityEngine;
using System.Collections.Generic;

public class SteeringEntityManager : MonoBehaviour {

    public static SteeringEntityManager Instance { get; private set; }

    public SteeringObstacle[] SteeringObstacles;

    public SteeringWall[] SteeringWalls;

    public Vehicle[] VehiclesInWorld;

    private const string STEERING_OBSTACLE_TAG_NAME = "SteeringObstacle";
    private const string STEERING_WALL_TAG_NAME = "SteeringWall";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        SteeringObstacles = GetSteeringObstaclesFromScene();
        SteeringWalls = GetSteeringWallsFromScene();
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

    private SteeringWall[] GetSteeringWallsFromScene()
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
        for (int i = 0; i < SteeringObstacles.Length; i++)
        {
            if(Vector3.SqrMagnitude(vehicle.Position - SteeringObstacles[i].Position) <= boxLength * boxLength)
            {
                SteeringObstacles[i].TaggedForAvoidance = true;
            }
            else
            {
                SteeringObstacles[i].TaggedForAvoidance = false;
            }
        }
    }
    
    public void TagVehiclesWithinNeighbourRadius(Vehicle vehicle, float neighbourRadius)
    {
        Vector3 to;
        float range;
        for (int i = 0; i < VehiclesInWorld.Length; i++)
        {
            VehiclesInWorld[i].TaggedForGroupBehaviours = false;

            if (VehiclesInWorld[i] != vehicle)
            {
                to = VehiclesInWorld[i].Position - vehicle.Position;
                range = neighbourRadius + VehiclesInWorld[i].Radius;

                if(to.sqrMagnitude < range * range)
                {
                    VehiclesInWorld[i].TaggedForGroupBehaviours = true;
                }
            }
        }
    }
}
