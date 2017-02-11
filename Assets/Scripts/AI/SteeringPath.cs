using UnityEngine;
using System.Collections.Generic;

public class SteeringPath : MonoBehaviour {

	public enum NavigationTypes { FollowUntilEnd = 0, LoopAround = 1, BackAndForth = 2 }
    public NavigationTypes NavigationType = NavigationTypes.FollowUntilEnd;

    private List<Vector3> _waypoints;
    private int _currentWaypointIndex = 0;

    public Vector3 CurrentWaypoint
    {
        get
        {
            return _waypoints[_currentWaypointIndex];
        }
    }

    public void InitialisePath(Vector3[] waypoints)
    {
        _waypoints = new List<Vector3>(waypoints);
        _currentWaypointIndex = 0;
    }

    public void SetNextWaypoint()
    {
        if (!AtLastWaypoint())
        {
            _currentWaypointIndex++;
        }
        else
        {
            switch (NavigationType)
            {
                case NavigationTypes.FollowUntilEnd:
                    //Do nothing
                    break;
                case NavigationTypes.LoopAround:
                    _currentWaypointIndex = 0;
                    break;
                case NavigationTypes.BackAndForth:
                     _waypoints.Reverse();
                    _currentWaypointIndex = 1;
                    break;
                default:
                    break;
            }
        }
    }

    public bool AtLastWaypoint()
    {
        return _currentWaypointIndex == _waypoints.Count - 1;
    }
}
