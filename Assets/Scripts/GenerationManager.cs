using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationManager : MonoBehaviour
{
    [Header("Generators")] [SerializeField]
    private GenerateObjectsInArea[] boxGenerators;

    [SerializeField] private GenerateObjectsInArea neutralBoatGenerator;
    [SerializeField] private GenerateObjectsInArea aggressiveBoatGenerator;
    [SerializeField] private GenerateObjectsInArea passiveBoatGenerator;
    //[SerializeField] private GenerateObjectsInArea pirateGenerator;

    [Space(10)] [Header("Parenting and Mutation")] [SerializeField]
    private float mutationFactor;

    [SerializeField] private float mutationChance;
    [SerializeField] private int boatParentSize;

    [Space(10)] [Header("Simulation Controls")] [SerializeField, Tooltip("Time per simulation (in seconds).")]
    private float simulationTimer;

    [SerializeField, Tooltip("Current time spent on this simulation.")]
    private float simulationCount;

    [SerializeField, Tooltip("Automatically starts the simulation on Play.")]
    private bool runOnStart;

    [SerializeField, Tooltip("Initial count for the simulation. Used for the Prefabs naming.")]
    private int generationCount;

    [Space(10)] [Header("Prefab Saving")] [SerializeField]
    private string savePrefabsAt;

    /// <summary>
    /// Those variables are used mostly for debugging in the inspector.
    /// </summary>
    [Header("Former winners")]
    [SerializeField] private AgentData lastNeutralBoatWinnerData;
    [SerializeField] private AgentData lastAggressiveBoatWinnerData;
    [SerializeField] private AgentData lastPassiveBoatWinnerData;

    private bool _runningSimulation;
    private List<BoatLogic> _activeNeutralBoats;
    private List<BoatLogic> _activeAggressiveBoats;
    private List<BoatLogic> _activePassiveBoats;

    private BoatLogic[] _boatNeutralParents;
    private BoatLogic[] _boatAggressiveParents;
    private BoatLogic[] _boatPassiveParents;

    private void Awake()
    {
        Random.InitState(6);
    }

    private void Start()
    {
        if (runOnStart)
        {
            StartSimulation();
        }
    }

    private void Update()
    {
        if (!_runningSimulation) return;
        //Creates a new generation.
        if (simulationCount >= simulationTimer)
        {
            ++generationCount;
            MakeNewGeneration();
            simulationCount = -Time.deltaTime;
        }

        simulationCount += Time.deltaTime;
    }


    /// <summary>
    /// Generates the boxes on all box areas.
    /// </summary>
    public void GenerateBoxes()
    {
        foreach (var generateObjectsInArea in boxGenerators)
        {
            generateObjectsInArea.RegenerateObjects();
        }
    }

    /// <summary>
    /// Generates boats and pirates using the parents list.
    /// If no parents are used, then they are ignored and the boats/pirates are generated using the default prefab
    /// specified in their areas.
    /// </summary>
    /// <param name="boatNeutralParents"></param>
    /// <param name="pirateParents"></param>
    public void GenerateObjects(BoatLogic[] boatNeutralParents = null, BoatLogic[] boatAggressiveParents = null, BoatLogic[] boatPassiveParents = null)
    {
        if (neutralBoatGenerator != null) GenerateBoats(ref _activeNeutralBoats, neutralBoatGenerator, boatNeutralParents);
        if (aggressiveBoatGenerator != null) GenerateBoats(ref _activeAggressiveBoats, aggressiveBoatGenerator, boatAggressiveParents);
        if (passiveBoatGenerator != null) GenerateBoats(ref _activePassiveBoats, passiveBoatGenerator, boatPassiveParents);
        //GeneratePirates(pirateParents);
    }

    /// <summary>
    /// Generates the list of pirates using the parents list. The parent list can be null and, if so, it will be ignored.
    /// Newly created pirates will go under mutation (MutationChances and MutationFactor will be applied).
    /// Newly create agents will be Awaken (calling AwakeUp()).
    /// </summary>
    /// <param name="pirateParents"></param>
/*    private void GeneratePirates(PirateLogic[] pirateParents)
    {
        _activePirates = new List<PirateLogic>();
        var objects = pirateGenerator.RegenerateObjects();
        foreach (var pirate in objects.Select(obj => obj.GetComponent<PirateLogic>()).Where(pirate => pirate != null))
        {
            _activePirates.Add(pirate);
            if (pirateParents != null)
            {
                var pirateParent = pirateParents[Random.Range(0, pirateParents.Length)];
                pirate.Birth(pirateParent.GetData());
            }

            pirate.Mutate(mutationFactor, mutationChance);
            pirate.AwakeUp();
        }
    }*/

    /// <summary>
    /// Generates the list of boats using the parents list. The parent list can be null and, if so, it will be ignored.
    /// Newly created boats will go under mutation (MutationChances and MutationFactor will be applied).
    /// /// Newly create agents will be Awaken (calling AwakeUp()).
    /// </summary>
    /// <param name="boatParents"></param>
    private void GenerateBoats(ref List<BoatLogic> _activeBoats, GenerateObjectsInArea boatGenerator, BoatLogic[] boatParents)
    {
        _activeBoats = new List<BoatLogic>();
        var objects = boatGenerator.RegenerateObjects();
        int i = 0; //for evenly spread out stats for the winners
        foreach (var boat in objects.Select(obj => obj.GetComponent<BoatLogic>()).Where(boat => boat != null))
        {
            _activeBoats.Add(boat);
            if (boatParents != null)
            {
                var boatParent = boatParents[i % (boatParents.Length)]; // i % boatParents.Length could also work
                boat.Birth(boatParent.GetData());
            }

            boat.Mutate(mutationFactor, mutationChance);
            boat.AwakeUp();
            i++;
        }
    }

    /// <summary>
    /// Creates a new generation by using GenerateBoxes and GenerateBoats/Pirates.
    /// Previous generations will be removed and the best parents will be selected and used to create the new generation.
    /// The best parents (top 1) of the generation will be stored as a Prefab in the [savePrefabsAt] folder. Their name
    /// will use the [generationCount] as an identifier.
    /// </summary>
    private void MakeNewGeneration()
    {
        Random.InitState(6);

        if (neutralBoatGenerator != null) StartNewBoats(ref _activeNeutralBoats, neutralBoatGenerator, ref _boatNeutralParents, ref lastNeutralBoatWinnerData);
        if (aggressiveBoatGenerator != null) StartNewBoats(ref _activeAggressiveBoats, aggressiveBoatGenerator, ref _boatAggressiveParents, ref lastAggressiveBoatWinnerData);
        if (passiveBoatGenerator != null) StartNewBoats(ref _activePassiveBoats, passiveBoatGenerator, ref _boatPassiveParents, ref lastPassiveBoatWinnerData);

        GenerateObjects(_boatNeutralParents, _boatAggressiveParents, _boatPassiveParents);
        GenerateBoxes();

    }

    private void StartNewBoats(ref List<BoatLogic> _activeBoats, GenerateObjectsInArea boatGenerator, ref BoatLogic[] _boatParents, ref AgentData lastBoatWinnerData)
    {
        //Fetch parents
        _activeBoats.RemoveAll(item => item == null);
        _activeBoats.Sort();
        if (_activeBoats.Count == 0)
        {
            GenerateBoats(ref _activeBoats, boatGenerator, _boatParents);
        }

        _boatParents = new BoatLogic[boatParentSize];
        for (var i = 0; i < boatParentSize; i++)
        {
            _boatParents[i] = _activeBoats[i];
        }
        var lastBoatWinner = _activeBoats[0];
        lastBoatWinner.name += "Gen-" + generationCount;
        lastBoatWinnerData = lastBoatWinner.GetData();
        PrefabUtility.SaveAsPrefabAsset(lastBoatWinner.gameObject, savePrefabsAt + lastBoatWinner.name + ".prefab");
    }

    /// <summary>
    /// Starts a new simulation. It does not call MakeNewGeneration. It calls both GenerateBoxes and GenerateObjects and
    /// then sets the _runningSimulation flag to true.
    /// </summary>
    public void StartSimulation()
    {
        Random.InitState(6);

        GenerateBoxes();
        GenerateObjects();
        _runningSimulation = true;
    }

    /// <summary>
    /// Continues the simulation. It calls MakeNewGeneration to use the previous state of the simulation and continue it.
    /// It sets the _runningSimulation flag to true.
    /// </summary>
    public void ContinueSimulation()
    {
        MakeNewGeneration();
        _runningSimulation = true;
    }

    /// <summary>
    /// Stops the count for the simulation. It also removes null (Destroyed) boats from the _activeBoats list and sets
    /// all boats and pirates to Sleep.
    /// </summary>
    public void StopSimulation()
    {
        _runningSimulation = false;
        _activeNeutralBoats.RemoveAll(item => item == null);
        _activeNeutralBoats.ForEach(boat => boat.Sleep());
        //_activePirates.ForEach(pirate => pirate.Sleep());
    }
}