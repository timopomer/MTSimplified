using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTSimplified
{
    public class AlreadyStartedException : Exception { public AlreadyStartedException() { } }
    public class NotStartedException : Exception { public NotStartedException(){ } }

    class ThreadOrganizer
    {

        private List<Thread> threadList;
        private int maxThreads,creationInterval,finishedThreads,currentThreadIndex;
        private bool keepCreating,started;
        private CancellationTokenSource cts;

        /* entire pool */
        public event EventHandler onDone;
        public event EventHandler onStopped;

        /* per thread */
        public event EventHandler onThreadCreate;
        public event EventHandler onThreadFinish;

        public ThreadOrganizer(int maxThreads, int creationInterval)
        {
            this.maxThreads = maxThreads;
            this.creationInterval = creationInterval;
            this.threadList = new List<Thread>();
            this.finishedThreads = 0;
            this.currentThreadIndex = 0;
            this.cts = new CancellationTokenSource();
            this.keepCreating = true;
            this.started = false;
        }

        public void addTask(Action function)
        {
            if (started)
                throw new AlreadyStartedException();
            
            threadList.Add(new Thread(delegate ()
            {
                functionWrapper(function,cts.Token);
            }));
        }

        public void startAll()
        {
            if (started)
                throw new AlreadyStartedException();

            this.started = true;
            int targetFinish = threadList.Count;
            Task.Run(() => {
                while (finishedThreads != targetFinish && currentThreadIndex != targetFinish && keepCreating)
                {
                    if (threadList.Count(t => t.IsAlive) < maxThreads)
                    {
                        startThread();
                        Thread.Sleep(creationInterval);
                    }
                }

                if (!keepCreating)
                    return;

                while (threadList.Count(t => t.IsAlive) != 0) ;

                onDone?.Invoke(this, EventArgs.Empty);


            });
        }

        public void stopAll()
        {
            if (!started)
                throw new NotStartedException();

            this.cts.Cancel();
            this.keepCreating = false;


            while (threadList.Count(t => t.IsAlive) != 0) ;

            onStopped?.Invoke(this, EventArgs.Empty);
        }

        /* private methods */
        private void functionWrapper(Action function, CancellationToken ct)
        {
            onThreadCreate?.Invoke(this, EventArgs.Empty);
            function.Invoke();
            this.finishedThreads++;
            onThreadFinish?.Invoke(this, EventArgs.Empty);
        }

        private void startThread()
        {
            threadList[currentThreadIndex].Start();
            currentThreadIndex++;

            
        }
        
    }
}
