using ReadAoe2Recrod;

namespace Aoe2DEOverlay
{
    public class WatchRecordMessage : Message
    {
        public Match Match;

        public WatchRecordMessage(Match match)
        {
            Match = match;
        }
    }
}