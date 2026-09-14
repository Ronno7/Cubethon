using System.Collections.Generic;
using UnityEngine;

namespace Cubethon
{
    public abstract class MovementCommand
    {
        public abstract void Execute(PlayerMovement player);
    }

    public sealed class MoveCommand : MovementCommand
    {
        private readonly float steering;

        public MoveCommand(float steering)
        {
            this.steering = steering;
        }

        public override void Execute(PlayerMovement player)
        {
            player.ApplyMovement(steering);
        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        public Rigidbody rb;
        public GameManager gameManager;
        public float forwardForce = 2000f;
        public float sidewaysForce = 60f;
        public float fallHeight = -2f;

        public bool IsReplaying { get; private set; }
        public bool HasRecording => commands.Count > 0;

        private readonly List<MovementCommand> commands =
            new List<MovementCommand>();

        private Vector3 startPosition;
        private Quaternion startRotation;
        private Vector3 startVelocity;
        private Vector3 startAngularVelocity;
        private float horizontal;
        private int replayIndex;

        private void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();

            if (gameManager == null)
                gameManager = FindFirstObjectByType<GameManager>();

            startPosition = rb.position;
            startRotation = rb.rotation;
            startVelocity = rb.linearVelocity;
            startAngularVelocity = rb.angularVelocity;
        }

        private void Update()
        {
            if (!IsReplaying)
                horizontal = GameInput.Horizontal;
        }

        private void FixedUpdate()
        {
            if (gameManager == null || gameManager.HasEnded)
                return;

            if (IsReplaying)
            {
                if (replayIndex < commands.Count)
                    commands[replayIndex++].Execute(this);
                else
                    gameManager.ReplayFinished();

                return;
            }

            if (rb.position.y < fallHeight)
            {
                gameManager.EndGame();
                return;
            }

            // Record every physics step, including neutral steering.
            MovementCommand command = new MoveCommand(horizontal);
            commands.Add(command);
            command.Execute(this);
        }

        public void ApplyMovement(float steering)
        {
            float step = Time.fixedDeltaTime;

            rb.AddForce(
                0f, 0f, forwardForce * step,
                ForceMode.Force);

            rb.AddForce(
                steering * sidewaysForce * step, 0f, 0f,
                ForceMode.VelocityChange);
        }

        public void BeginReplay()
        {
            replayIndex = 0;
            horizontal = 0f;
            IsReplaying = true;

            rb.isKinematic = false;
            rb.position = startPosition;
            rb.rotation = startRotation;
            transform.SetPositionAndRotation(startPosition, startRotation);

            rb.linearVelocity = startVelocity;
            rb.angularVelocity = startAngularVelocity;

            Physics.SyncTransforms();
            rb.WakeUp();
            enabled = true;
        }

        public void StopAtFinish()
        {
            enabled = false;
            IsReplaying = false;
            horizontal = 0f;

            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }
}