using System.Collections.Generic;

namespace Cubethon
{
    // Strategy: selects the next command without applying physics itself.
    public interface IMovementStrategy
    {
        bool TryGetCommand(float steering, out MovementCommand command);
    }

    public sealed class LiveMovementStrategy : IMovementStrategy
    {
        private readonly List<MovementCommand> recording;

        public LiveMovementStrategy(List<MovementCommand> recording)
        {
            this.recording = recording;
        }

        public bool TryGetCommand(float steering, out MovementCommand command)
        {
            command = new MoveCommand(steering);
            recording.Add(command);
            return true;
        }
    }

    public sealed class ReplayMovementStrategy : IMovementStrategy
    {
        private readonly IReadOnlyList<MovementCommand> recording;
        private int nextIndex;

        public ReplayMovementStrategy(IReadOnlyList<MovementCommand> recording)
        {
            this.recording = recording;
        }

        public bool TryGetCommand(float steering, out MovementCommand command)
        {
            // Playback ignores live input and never appends to the recording.
            if (nextIndex >= recording.Count)
            {
                command = null;
                return false;
            }

            command = recording[nextIndex++];
            return true;
        }
    }
}
