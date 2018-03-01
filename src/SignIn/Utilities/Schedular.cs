using System;
using System.Timers;
using Starcounter;

namespace SignIn
{
    public class Schedular
    {
        public void SetupSessionCleanupTimer()
        {
            var timer = new Timer();
            timer.Interval = (1000 * 60 * 60); // 1 hour
            timer.Elapsed += CleanUpSessions;
            timer.AutoReset = true;
            timer.Start();
        }

        private void CleanUpSessions(object sender, ElapsedEventArgs e)
        {
            //ensure that it is running between 20:00 - 24:00 only.
            if (DateTime.Now.Hour >= 20 && DateTime.Now.Hour <= 23)
            {
                Scheduling.RunTask(() =>
                {
                    //remove all the expired sessions
                    Db.SQL($"DELETE FROM {typeof(SystemUserSession)} WHERE ExpiresAt < ?", DateTime.Now);
                });
            }
        }
    }
}
