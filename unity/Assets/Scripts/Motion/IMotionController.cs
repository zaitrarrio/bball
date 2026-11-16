using BasketballSim.Core;

namespace BasketballSim.Motion
{
    /// <summary>
    /// Pluggable motion layer. Queried every frame from the current
    /// pose + intent; returns the next pose delta. The game never
    /// knows which implementation is behind this interface.
    ///
    /// Implementations, in order:
    ///  1. ProceduralMotionController (shipped here — baseline)
    ///  2. MotionMatchingController (clip database, CMU/AMASS)
    ///  3. CodebookMatchingController (AI4AnimationPy, ONNX via Sentis)
    ///  4. MotionBricksController (NVIDIA in-betweening, when bridged)
    /// </summary>
    public interface IMotionController
    {
        PoseDelta Step(PoseState current, GameIntent intent, float dt);
        void Reset(PoseState initial);
    }
}
