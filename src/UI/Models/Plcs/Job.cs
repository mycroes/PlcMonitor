using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PlcMonitor.UI.Models.Plcs
{
    public class Job : INotifyCompletion
    {
        private readonly Func<IPlcConnection, Task> _fn;

        private Action? _continuation;
        private Exception? _error;

        public Job(Func<IPlcConnection, Task> fn)
        {
            _fn = fn;
        }

        public Job GetAwaiter() => this;

        public bool IsCompleted { get; private set; }

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public void Error(Exception error)
        {
            _error = error;
            IsCompleted = true;
            _continuation?.Invoke();
        }

        public async Task Execute(IPlcConnection connection)
        {
            await _fn.Invoke(connection);

            IsCompleted = true;
            _continuation?.Invoke();
        }

        public void GetResult()
        {
            if (_error is {}) throw _error;
        }
    }
}