using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BoatLogic : AgentLogic
{
    #region Static Variables

    private static readonly float _boxPoints = 2.0f;
    private static readonly float _piratePoints = -100.0f;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Box")) return;
        points += _boxPoints;
        Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Boat"))
        {
            BoatLogic otherBoat = other.transform.GetComponent<BoatLogic>();
            if (otherBoat != null) //add checksystem if they've engaged before
            {
                int thisBoatScore = attack - otherBoat.defense;
                int otherBoatScore = otherBoat.attack - defense;
                if (thisBoatScore > otherBoatScore)
                {
                    points += 7;
                }
                else if (otherBoatScore > thisBoatScore)
                {
                    points -= 10;
                }
                //add to the checked boats list I guess
                return;
            }
        }
    }
}