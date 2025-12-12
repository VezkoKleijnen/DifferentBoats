using UnityEngine;

public class TableBehavior : MonoBehaviour
{
    Vector3 destination;
    float moveSpeed = 5;
    [SerializeField] float attack;
    [SerializeField] float defense;
    [SerializeField] float speed;
    [SerializeField] float points;
    private void Start()
    {
        transform.rotation = Quaternion.identity;
    }
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }
    public void SetStats(float attack, float defense, float speed, float points)
    {
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.points = points;
    }
    private void Update()
    {
        // Convert local -> world
        Vector3 worldDestination;
        if (transform.parent != null)
        {
            worldDestination = transform.parent.TransformPoint(destination);
        }
        else
        {
            worldDestination = destination;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            worldDestination,
            Time.deltaTime * moveSpeed
        );
        //transform.position = worldDestination;
        var attackDireciton = new Vector3(1, 0, -1) * attack;
        var defenseDirection = new Vector3(0, 0, 1) * defense;
        var speedDirection = new Vector3(-1, 0, -1) * speed;
        var totalDirection = speedDirection + defenseDirection + attackDireciton;
        totalDirection.Normalize();
        transform.forward = Vector3.Slerp(transform.forward, totalDirection, Time.deltaTime * moveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, Mathf.Max(0, points), transform.localScale.z), Time.deltaTime * moveSpeed);
        //transform.localScale = new Vector3(transform.localScale.x, points, transform.localScale.z);
    }
    private void OnMouseDown()
    {
        //show stats somehow
    }
}
