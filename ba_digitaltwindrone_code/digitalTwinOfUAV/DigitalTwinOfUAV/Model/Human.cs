using System;
using Mars.Components.Environments;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using NetTopologySuite.Geometries;
using Position = Mars.Interfaces.Environments.Position;

namespace GeoVectorBlueprint.Model;

/// <summary>
///     A simple agent stub that has an Init() method for initialization and a
///     Tick() method for acting in every tick of the simulation.
/// </summary>
public class Human : IAgent<GraphLayer>, ISpatialGraphEntity
{
    #region Properties and Fields

    /// <summary>
    ///     The current position of the agent in the environment.
    /// </summary>
    public Position Position { get; set; }

    /// <summary>
    ///     The current route of the agent.
    /// </summary>
    private Route Route { get; set; }

    /// <summary>
    ///     A counter that tracks how many routes the agent has completed.
    /// </summary>
    public int RouteCounter { get; set; }

    /// <summary>
    ///     The name of the POI that the agent is currently navigating to.
    /// </summary>
    public string TargetPoiName { get; set; }

    /// <summary>
    ///     The category of the POI that the agent is currently navigating to.
    /// </summary>
    public string TargetPoiCategory { get; set; }

    /// <summary>
    ///     The layer through which the agent can access the travel network on which it can move.
    /// </summary>
    public GraphLayer GraphLayer { get; private set; }

    /// <summary>
    ///     The layer through which the agent can access POI locations and resources.
    /// </summary>
    public PoiLayer PoiLayer { get; set; }

    /// <summary>
    ///     The length of this agent.
    /// </summary>
    public double Length { get; }

    /// <summary>
    ///     The edge on which the agent currently is.
    /// </summary>
    public ISpatialEdge CurrentEdge { get; set; }

    /// <summary>
    ///     The position of the edge on which the agent currently is.
    /// </summary>
    public double PositionOnCurrentEdge { get; set; }

    /// <summary>
    ///     The lane of the edge that is currently occupied by the agent.
    /// </summary>
    public int LaneOnCurrentEdge { get; set; }

    /// <summary>
    ///     The current modality type of the agent.
    /// </summary>
    public SpatialModalityType ModalityType { get; }

    /// <summary>
    ///     A flag that states if the agent can be collided with.
    /// </summary>
    public bool IsCollidingEntity { get; }

    /// <summary>
    ///     A unique identifier of the agent.
    /// </summary>
    public Guid ID { get; set; } // identifies the agent

    #endregion

    #region Initialization

    public void Init(GraphLayer layer)
    {
        // Store the given layer in a property and set the route counter to an initial value.
        GraphLayer = layer;
        RouteCounter = 0;

        // Insert the agent into the environment on a random node of the travel network (graph)
        var startNode = layer.Environment.GetRandomNode();
        Position = startNode.Position;
        layer.Environment.Insert(this, startNode);

        // Find a route from the current position to some destination.
        Route = CreateNewRoute();
    }

    #endregion

    #region Tick

    /// <summary>
    ///     The agent's tick routine. The agent completes routes between POIs. If the current route has been completed,
    ///     a new route is created.
    /// </summary>
    public void Tick()
    {
        if (Route.GoalReached)
        {
            // Goal of current route has been reached, so find a new route from to some destination.
            Console.WriteLine("Goal reached. Start new route.");
            Route = CreateNewRoute();
        }
        else
        {
            // Goal of current route has not yet been reached, so move towards goal.
            // The agent moves 2 meters per tick (i.e., 2m/s since 1 tick = 1 second). 2m/s = 7.2km/h.
            // If the agent is close to its route destination, it might be < 2m. In this case, the Move() call would
            // overshoot the destination position, resulting in no movement. The Min() call checks if this is the case,
            // and its result is used to move the agent in the current tick.
            var movementDistance = Math.Min(Route.RemainingRouteDistanceToGoal, 2);

            GraphLayer.Environment.Move(this, Route, movementDistance);
            Position = this.CalculateNewPositionFor(Route, out var bearing);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    ///     This subroutine create a new route for the agent, using its current position as that source of the route and
    ///     the position of a randomly chosen POI as the destination of the route.
    /// </summary>
    /// <returns>The created route</returns>
    private Route CreateNewRoute()
    {
        // Increment counter to track how many routes the agent completed.
        RouteCounter += 1;

        // Find a random POI with the given OSM category (in this case, "restaurant").
        // See the Prepare POIs Notebook for more categories.
        var poi = PoiLayer.GetRandomPoiForCategory("restaurant");

        // Log the category and name of the new destination POI in the CSV output.
        TargetPoiCategory = (string)poi.VectorStructured.Attributes["fclass"];
        TargetPoiName = (string)poi.VectorStructured.Attributes["name"];

        // Get the Position of the POI.
        var point = (Point)poi.VectorStructured.Geometry;
        var targetPosition = new Position(point.X, point.Y);

        // Find the nodes of the graph corresponding to the agent's current position and the chosen target position.
        var startNode = GraphLayer.Environment.NearestNode(Position);
        var goalNode = GraphLayer.Environment.NearestNode(targetPosition);

        // Create and return a route between the two nodes.
        return GraphLayer.Environment.FindShortestRoute(startNode, goalNode);
    }

    #endregion
}