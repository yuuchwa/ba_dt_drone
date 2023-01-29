using System;
using System.Linq;
using Mars.Components.Layers;
using Mars.Interfaces.Layers;

namespace GeoVectorBlueprint.Model;

/// <summary>
///     The PoiLayer holds point geometries that represent points of interest in the environment with which agents can
///     interact and to which they can navigate.
/// </summary>
public class PoiLayer : VectorLayer
{
    /// <summary>
    ///     Obtains a random POI that is of the given category (e.g., "restaurant").
    /// </summary>
    /// <param name="category">The given category</param>
    /// <returns>A POI of the given category, if any exist</returns>
    /// <exception cref="ArgumentException">Thrown if no POI of the given category exists</exception>
    public IVectorFeature GetRandomPoiForCategory(string category)
    {
        // Shuffle all available features randomly so each POI has a chance of being selected.
        var shuffledFeatures = Features.OrderBy(_ => new Random().Next()).ToList();

        foreach (var feature in shuffledFeatures)
        {
            if ((string)feature.VectorStructured.Attributes["fclass"] == category)
            {
                return feature;
            }
        }

        // If we reach this code, no POI with this category is available, so abort the simulation.
        throw new ArgumentException($"No POIs found with category '{category}");
    }
}