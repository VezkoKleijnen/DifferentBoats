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
                float doneDamage = Mathf.Max(0, attack - otherBoat.defense);
                float gainedPoints = Mathf.Min(doneDamage, otherBoat.points);
                points += gainedPoints;
                if (doneDamage > otherBoat.points)
                {
                    otherBoat.points = -1;
                    SaveBoatStats.Instance.AddStats(otherBoat);
                    Destroy(otherBoat.gameObject);
                    points++;
                }
                else
                {
                    otherBoat.points -= gainedPoints;
                }
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