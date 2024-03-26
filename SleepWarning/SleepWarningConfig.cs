using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepWarning
{
    internal class SleepWarningConfig
    {
        public int FirstWarnTime { get; set; } = 2300;
        public int SecondWarnTime { get; set; } = 2400;
        public int ThirdWarnTime { get; set; } = 2500;
        public string WarningSound = "crystal";
    }
}
