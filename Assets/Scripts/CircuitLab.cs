using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
//using UnityEngine.XR.Interaction.Toolkit;

public class CircuitLab : MonoBehaviour
{
    // public members found in Unity inspector
    public GameObject pegTemplate = null;
    public float pegInterval = 0.1f;
    public float pegHeight = 0.45f;
    public Vector3 pegScale;
    public bool isWorldFixed;

    Board board;
    const int numRows = 9;
    const int numCols = 9;

    //List<IDynamic> dynamicComponents = new List<IDynamic>();
    int numActiveCircuits = 0;

    // Start is called before the first frame update
    void Start()
    {
        board = new Board(numRows,numCols);
        CreatePegs();
        PreloadSimulator();
    }

    void PreloadSimulator()
    {
        // Build a simple circuit and run an analysis to preload the SpiceSharp simulator. 
        // This avoids a multi-second lag when connecting our first circuit on the breadboard.

        /*
        var ckt = new Circuit(
            new VoltageSource("V1", "in", "0", 1.0),
            new Resistor("R1", "in", "out", 1.0e4),
            new Resistor("R2", "out", "0", 2.0e4)
            );
        var dc = new OP("DC 1");
        dc.Run(ckt);*/

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePegs(){

        // Creates a matrix of pegs
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < numCols; j++){
                CreatePeg(i,j);
            }
        }
    }

    private void CreatePeg(int row, int col){

        // create peg name
        string name = "Peg_" + row.ToString() + "_" + col.ToString();

        // find bounds of breadboard
        var boardObject = GameObject.Find("Breadboard").gameObject;
        var mesh = boardObject.GetComponent<MeshFilter>().mesh;
        var size = mesh.bounds.size;
        var boardWidth = size.x * boardObject.transform.localScale.x;
        var boardHeight = size.z * boardObject.transform.localScale.z;
        var boardXPos = boardObject.transform.position.x;
        var boardZPos = boardObject.transform.position.z;

        // Create a new peg
        var position = new Vector3(-(boardWidth / 2.0f) + ((col + 1) * pegInterval)+(boardWidth/2)-0.5f, pegHeight, -(boardHeight / 2.0f) + ((row + 1) * pegInterval)+(boardHeight/2)-0.5f);
        var rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        var peg = Instantiate(pegTemplate, position, rotation) as GameObject;
        peg.transform.parent = boardObject.transform;
        peg.transform.localPosition = position;
        peg.transform.localRotation = rotation;
        peg.transform.localScale = pegScale;

        peg.name = name;

        Point coords = new Point(col, row);
        board.SetPegGameObject(coords, peg);
    }


    public void RemoveComponent(GameObject component, Point start)
    {
        Peg pegA = board.GetPeg(start);
        if (pegA != null)
        {
            PlacedComponent found = pegA.Components.Find(x => x.GameObject == component);
            if (found != null)
            {
                Peg pegB = board.GetPeg(found.End);
                if (pegB != null)
                {
                    // Remove it from each of the pegs it's attached to
                    if (!pegA.Components.Remove(found))
                        Debug.Log("Failed to remove component from Peg A!");
                    if (!pegB.Components.Remove(found))
                        Debug.Log("Failed to remove component from Peg B!");

                    // Remove it from the master list as well
                    board.Components.Remove(found);

                    // Unblock/unhide intermediate pegs
                    BlockPegs(found.Start, found.End, false);
                }
            }
        }

        // Deactivate the component
        var script = component.GetComponent<CircuitComponent>();
        if (script != null)
        {
            script.SetActive(false, false);
        }

        // Run a new circuit simulation so that any circuits we've just broken get deactivated
        SimulateCircuit();
    }

    public void BlockPegs(Point start, Point end, bool block)
    {
        // Hide all pegs between start and end
        List<Point> points = new List<Point>();
        if (start.x != end.x)
        {
            int xStart = (start.x < end.x ? start.x : end.x);
            int xEnd = (start.x < end.x ? end.x : start.x);
            for (int x = xStart + 1; x < xEnd; x++)
            {
                Point coords = new Point(x, start.y);
                board.BlockPeg(coords, block);
            }
        }
        if (start.y != end.y)
        {
            int yStart = (start.y < end.y ? start.y : end.y);
            int yEnd = (start.y < end.y ? end.y : start.y);
            for (int y = yStart + 1; y < yEnd; y++)
            {
                Point coords = new Point(start.x, y);
                board.BlockPeg(coords, block);
            }
        }
    }

    public void SimulateCircuit()
    {
        
        
        
        Debug.Log("SIMULATE START");
        
        
        numActiveCircuits = 0;

        // Bump the generation number for this circuit simulation
        int gen = ++board.Generation;

        /*
        // First, search for all batteries (and solar panels) so we know where to start
        List<PlacedComponent> batteries = new List<PlacedComponent>();
        foreach (PlacedComponent component in board.Components)
        {
            if (component.Component is IBattery || component.Component is ISolar)
            {
                batteries.Add(component);
            }
        }

        foreach (PlacedComponent battery in batteries)
        {
            // Skip batteries that have already been included in a circuit
            if (battery.Generation == gen)
            {
                continue;
            }

            List<PlacedComponent> circuit = new List<PlacedComponent>();
            List<PlacedComponent> components = new List<PlacedComponent>();
            List<SpiceSharp.Entities.Entity> entities = new List<SpiceSharp.Entities.Entity>();

            // Add the battery as the first component
            circuit.Add(battery);
            components.Add(battery);

            int resistors = 0;

            // Leaving the positive terminal of the battery (to follow current flow), do a depth-first 
            // search to find each circuit that eventually leads to the negative terminal of the battery
            Point currPosition = battery.End;
            Peg peg = board.GetPeg(currPosition);

            foreach (PlacedComponent component in peg.Components)
            {
                if (component != battery)
                {
                    FindCircuit(circuit, entities, components, component, currPosition, resistors, gen);
                }
            }

            // Case 1: There's a short circuit on this battery
            if (battery.ShortCircuitGeneration == gen)
            {
                foreach (PlacedComponent component in components)
                {
                    // Highlight each component involved in the short circuit, and deactivate all 
                    // other connected components.
                    if (component.ShortCircuitGeneration == gen)
                    {
                        component.Component.SetShortCircuit(true, component.ShortCircuitForward);
                    }
                    else
                    {
                        component.Component.SetActive(false, false);
                    }
                }

                if (!battery.ActiveShort)
                {
                    battery.ActiveShort = true;
                    battery.ActiveCircuit = false;
                    StartCoroutine(PlaySound(shortSound1, shortSound1StartTime));
                    StartCoroutine(PlaySound(shortSound2, shortSound2StartTime));
                }
            }
            // Case 2: A normal circuit has been completed on this battery
            else if (battery.Generation == gen)
            {
                numActiveCircuits++;
                var ssCircuit = new Circuit(entities);

                // Create an Operating Point Analysis for the circuit
                var op = new OP("DC 1");

                // Create exports so we can access component properties
                foreach (PlacedComponent component in components)
                {
                    bool isBattery = (component.Component is IBattery);

                    if (component.Generation == gen)
                    {
                        component.VoltageExport = new RealVoltageExport(op, isBattery ? component.End.ToString() : component.Start.ToString());
                        component.CurrentExport = new RealPropertyExport(op, "V" + component.GameObject.name, "i");
                    }
                }

                // Catch exported data
                op.ExportSimulationData += (sender, args) =>
                {
                    var input = args.GetVoltage(battery.End.ToString());
                    var output = args.GetVoltage(battery.Start.ToString());

                    // Loop through the components and find the lowest voltage, so we can normalize the entire
                    // circuit to start at 0V.
                    double minVoltage = 0f;
                    foreach (PlacedComponent component in components)
                    {
                        if (component.Generation == gen)
                        {
                            if (component.VoltageExport.Value < minVoltage)
                                minVoltage = component.VoltageExport.Value;
                        }
                    }

                    // Now loop through again and tell each component what its voltage and current values are
                    foreach (PlacedComponent component in components)
                    {
                        if (component.Generation == gen)
                        {
                            // Update the voltage value
                            var voltage = component.VoltageExport.Value - minVoltage;
                            component.Component.SetVoltage(voltage);

                            // Update the current value
                            var current = component.CurrentExport.Value;
                            component.Component.SetCurrent(current);
                        }
                    }
                };

                // Run the simulation
                try
                {
                    op.Run(ssCircuit);

                    if (!battery.ActiveCircuit)
                    {
                        battery.ActiveCircuit = true;
                        battery.ActiveShort = false;
                        StartCoroutine(PlaySound(circuitSound, circuitSoundStartTime));
                    }
                }
                catch (ValidationFailedException exception)
                {
                    Debug.Log("Simulation Error! Caught exception: " + exception);
                    foreach (var rule in exception.Rules)
                    {
                        Debug.Log("  Rule: " + rule);
                        Debug.Log("  ViolationCount: " + rule.ViolationCount);
                        foreach (var violation in rule.Violations)
                        {
                            Debug.Log("    Violation: " + violation);
                            Debug.Log("    Subject: " + violation.Subject);
                        }
                    }
                    Debug.Log("Inner exception: " + exception.InnerException);

                    // Treat a simulation error as a short circuit
                    battery.ActiveShort = true;
                    battery.ActiveCircuit = false;
                    StartCoroutine(PlaySound(shortSound1, shortSound1StartTime));
                    StartCoroutine(PlaySound(shortSound2, shortSound2StartTime));

                    // Deactivate all components in this circuit
                    foreach (PlacedComponent component in components)
                    {
                        component.Component.SetActive(false, false);
                    }
                }
            }
            // Case 3: No complete circuit
            else
            {
                battery.ActiveShort = false;
                battery.ActiveCircuit = false;
            }
        }

        // Deactivate any components that didn't get included in any complete circuits this time
        foreach (PlacedComponent component in board.Components)
        {
            if (component.Generation != gen)
            {
                component.Component.SetActive(false, false);
            }
        }

        // Clear any components that didn't get included in any short circuits this time
        foreach (PlacedComponent component in board.Components)
        {
            if (component.ShortCircuitGeneration != gen)
            {
                component.Component.SetShortCircuit(false, false);
            }
        }
        */
        Debug.Log("SIMULATE END");
    }
}
