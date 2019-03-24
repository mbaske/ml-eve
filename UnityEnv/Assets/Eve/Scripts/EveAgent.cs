using UnityEngine;
using MLAgents;

public class EveAgent : Agent
{
    [SerializeField]
    private Ball ball;

    private EvePhysics eve;
    private EveAnimation anim;
    private Resetter defaults;
    
    public override void InitializeAgent()
    {
        eve = GetComponent<EvePhysics>();
        eve.Initialize();
        anim = GetComponent<EveAnimation>();
        anim.Initialize();
        defaults = new Resetter(transform);

        ball.RaiseCollisionEvent += OnBallCollision;
        ball.Initialize();
    }

    public override void AgentReset()
    {
        defaults.Reset();
        anim.ReSet();
        ball.ReSet();
    }

    public override void CollectObservations()
    {
        // NOTE: Training took well over 10M steps.
        // Might need to add a reward for staying close to the ball to speed things up.
        AddReward(ball.Velocity.sqrMagnitude / 10000f);
        AddReward(Mathf.Min(0f, ball.LocalPosition.y / 10f));

        AddVectorObs(Util.Sigmoid((ball.Position - eve.Position) / 10f)); // 3
        AddVectorObs(Util.Sigmoid(LocalizeDir(ball.Velocity) / 20f)); // 3
        AddVectorObs(Util.Sigmoid(LocalizeDir(ball.AngularVelocity) / 10f)); // 3

        AddVectorObs(LocalizeDir(eve.UpAxis)); // 3
        AddVectorObs(LocalizeDir(eve.ForwardAxis)); // 3
        AddVectorObs(Util.Sigmoid(LocalizeDir(eve.Velocity) / 5f)); // 3
        AddVectorObs(Util.Sigmoid(LocalizeDir(eve.AngularVelocity) / 2.5f)); // 3

        AddVectorObs(eve.GetNormalizedArmOffsets()); // 2

        AddVectorObs(eve.LeftArm.GetNormalizedRotation()); // 3
        AddVectorObs(eve.LeftArm.GetNormalizedDriveForce()); // 1
        AddVectorObs(Util.Sigmoid(LocalizeDir(eve.LeftArm.Velocity) / 20f)); // 3
        AddVectorObs(Util.Sigmoid(LocalizeDir(eve.LeftArm.AngularVelocity) / 20f)); // 3

        AddVectorObs(eve.RightArm.GetNormalizedRotation()); // 3
        AddVectorObs(eve.RightArm.GetNormalizedDriveForce()); // 1
        AddVectorObs(Util.Sigmoid(LocalizeDir(eve.RightArm.Velocity) / 20f)); // 3
        AddVectorObs(Util.Sigmoid(LocalizeDir(eve.RightArm.AngularVelocity) / 20f)); // 3
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int i = 0;
        eve.Move(vectorAction[i++], vectorAction[i++]);
        eve.Turn(vectorAction[i++]);
        eve.LeftArm.Rotate(new Vector3(vectorAction[i++], vectorAction[i++],
            vectorAction[i++]), vectorAction[i++]);
        eve.LeftArm.Move(vectorAction[i++]);
        eve.RightArm.Rotate(new Vector3(vectorAction[i++], vectorAction[i++],
            vectorAction[i++]), vectorAction[i++]);
        eve.RightArm.Move(vectorAction[i++]);
        eve.Hover(); // auto
    }

    private Vector3 LocalizeDir(Vector3 dir)
    {
        return transform.InverseTransformDirection(dir);
    }

    private void OnBallCollision(object sender, CollisionArgs e)
    {
        if (e.CompareTag(Tags.GROUND))
        {
            if (ball.Velocity.magnitude < 7f)
            {
                // TBD
                Done(); 
            }
        }
        else if (e.CompareTag(Tags.OUT))
        {
            Done();
        }
    }

    private void Update()
    {
        if (anim.Enabled)
        {
            anim.LookAt(ball.LocalPosition);

            if (Random.value > 0.97f)
            {
                anim.Blink();
            }
        }
    }
}
