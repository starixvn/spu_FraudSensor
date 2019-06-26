namespace Fanex.NAP.FS.ExecutingService.Schedules
{
    using Quartz;
    public class Schedule : ISchedule
    {
        private IScheduler scheduler;
        public virtual ISchedule Create()
        {
            return this;
        }

        public virtual ISchedule Pause()
        {
            scheduler.PauseAll();

            return this;
        }

        public virtual ISchedule Shutdown()
        {
            scheduler.Shutdown(true);

            return this;
        }

        public virtual ISchedule Start()
        {
            scheduler.Start();

            return this;
        }
    }
}
