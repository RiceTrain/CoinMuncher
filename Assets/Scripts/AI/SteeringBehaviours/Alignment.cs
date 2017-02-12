using UnityEngine;
using System.Collections;

public class Alignment {

    private Vehicle _vehicle;

    public Alignment(Vehicle vehicleToAffect)
    {
        _vehicle = vehicleToAffect;
    }

    private Vector3 _averageHeading;
    private int _neighbourCount;
    public Vector3 Calculate(ref Vehicle[] neighbours)
    {
        _averageHeading = Vector3.zero;
        _neighbourCount = 0;

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != _vehicle && neighbours[i].TaggedForGroupBehaviours)
            {
                _averageHeading += neighbours[i].Heading;
                _neighbourCount++;
            }
        }

        if (_neighbourCount > 0)
        {
            _averageHeading /= (float)_neighbourCount;
            _averageHeading -= _vehicle.Heading;
        }

        return _averageHeading;
    }
}
