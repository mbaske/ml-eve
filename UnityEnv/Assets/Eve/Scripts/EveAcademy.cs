using UnityEngine;
using MLAgents;

public class EveAcademy : Academy
{
    public override void InitializeAcademy()
    {
        // Monitor.verticalOffset = 1f;
        // Time.fixedDeltaTime = 0.01333f; // (75fps). default is .2 (60fps)
        // Time.maximumDeltaTime = .15f; // Default is .33
        // Physics.defaultSolverIterations = 12;
        // Physics.defaultSolverVelocityIterations = 2;
    }

    public override void AcademyReset()
    {
    }

    public override void AcademyStep()
    {
    }
}
