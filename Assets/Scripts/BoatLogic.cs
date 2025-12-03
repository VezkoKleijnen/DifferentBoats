using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BoatLogic : AgentLogic
{
    #region Static Variables

    private static readonly float _boxPoints = 2.0f;

    #endregion

    public int boatType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Box")) return;
        points += _boxPoints;
        Destroy(other.gameObject);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag.Equals("Boat"))
        {
            BoatLogic otherBoat = other.transform.GetComponent<BoatLogic>();
            if (otherBoat != null)
            {
                float gainedPoints = attack - otherBoat.defense;
                points += gainedPoints;
                otherBoat.points -= gainedPoints;
                return;
            }
        }
    }
    private void LateUpdate()
    {
        if (points < 0)
        {
            Destroy(gameObject);
        }
    }
}