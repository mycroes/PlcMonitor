using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace PlcMonitor.UI.Models.Plcs
{
    public abstract class Plc : IPlc, IDisposable
    {
        private readonly BehaviorSubject<Connection?> _connection;
        private readonly Subject<Job> _jobs;
        private readonly IDisposable _disposable;

        public IObservable<Connection> Connection { get; }

        protected Plc()
        {
            _connection = new BehaviorSubject<Connection?>(default);
            _jobs = new Subject<Job>();
            Connection = _connection.Where(c => c is {}).Select(c => c!);

            _disposable = new CompositeDisposable(
                _jobs.ObserveOn(Scheduler.Default).SelectMany(ExecuteJob).Subscribe(),
                Disposable.Create(() => {
                     if (_connection.Value?.State.Value > ConnectionState.New && _connection.Value?.State.Value < ConnectionState.Closing)
                        _connection.Value.Close();
                })
            );;
        }

        public async Task Schedule(Func<IPlcConnection, Task> fn)
        {
             var job = new Job(fn);
             _jobs.OnNext(job);
             await job;
        }

        public abstract IPlcConnection CreateConnection();

        public void Dispose()
        {
            _disposable.Dispose();
        }

        protected abstract bool BreaksConnection(Exception exception);

        private async Task<Unit> ExecuteJob(Job job)
        {
            var conn = _connection.Value;
            if (conn == null || conn.State.Value == ConnectionState.Closed)
            {
                conn = new Connection(CreateConnection());
                _connection.OnNext(conn);
            }

            if (conn.State.Value == ConnectionState.New)
            {
                try
                {
                    await conn.Open();
                }
                catch (Exception e)
                {
                    job.Error(e);
                    return Unit.Default;
                }
            }

            try
            {
                await job.Execute(conn.PlcConnection);
            }
            catch (Exception e)
            {
                if (BreaksConnection(e))
                {
                    conn.Close();
                }

                job.Error(e);
            }

            return Unit.Default;
        }
    }
}