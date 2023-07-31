using System.Diagnostics;

namespace MixTelematics.Services
{
    public class CPUOptimizer
    {
        public static void MaximiseCPU()
        {

            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
        }
    }
}
