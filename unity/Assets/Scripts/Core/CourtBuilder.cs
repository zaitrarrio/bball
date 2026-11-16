using UnityEngine;

namespace BasketballSim.Core
{
    /// <summary>
    /// Builds a half-court from primitives for Editor testing, so the
    /// prototype runs with zero authored art. Replace with a real
    /// court model later; nothing in the game logic depends on this.
    /// Court: 15m wide, half-court line at z=0, basket at z=-6.4,
    /// rim height 3.05m, three-point arc radius 6.75m.
    /// </summary>
    public static class CourtBuilder
    {
        public static void Build(Transform parent)
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "CourtFloor";
            floor.transform.SetParent(parent);
            floor.transform.localScale = new Vector3(1.5f, 1f, 1.4f);
            floor.GetComponent<Renderer>().material.color = new Color(0.72f, 0.45f, 0.25f);

            var backboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backboard.name = "Backboard";
            backboard.transform.SetParent(parent);
            backboard.transform.position = new Vector3(0f, 3.45f, -6.7f);
            backboard.transform.localScale = new Vector3(1.8f, 1.05f, 0.1f);

            // Rim: a thin cylinder ring as a visual placeholder.
            // A real rim model replaces this later.
            var rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "Rim";
            rim.transform.SetParent(parent);
            rim.transform.position = new Vector3(0f, 3.05f, -6.4f);
            rim.transform.localScale = new Vector3(0.45f, 0.02f, 0.45f);
        }
    }
}
