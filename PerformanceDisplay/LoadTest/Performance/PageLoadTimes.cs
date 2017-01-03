using System.Collections.Generic;

namespace LoadTest.Performance
{
    public class PageLoadTimes
    {
        public long PageFullyLoadedTime { get; set; }

        public long PageFetchTime { get; set; }

        public long PageDomCompleteTime { get; set; }

        public long PageConnectTime { get; set; }
    }
}
