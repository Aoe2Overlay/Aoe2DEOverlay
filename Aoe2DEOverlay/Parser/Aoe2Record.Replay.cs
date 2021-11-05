using System;
using System.IO;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        private void ReadReplay(BinaryReader reader)
        {
            var oldTime = reader.ReadUInt32();
            var worldTime = reader.ReadUInt32();
            var oldWorldTime = reader.ReadUInt32();
            var gameSpeedId = reader.ReadUInt32(); //  world_time_delta
            var worldTimeDeltaSeconds = reader.ReadUInt32();
            var timer = reader.ReadSingle();
            var gameSpeedFloat = Math.Round(reader.ReadSingle(), 1);
            var tempPause = reader.ReadByte();
            var nextObjectId = reader.ReadUInt32();
            var nextReusableObjectId = reader.ReadInt32();
            var randomSeed = reader.ReadUInt32();
            var randomSeed2 = reader.ReadUInt32();
            var recPlayer = reader.ReadUInt32();
            var numPlayers = reader.ReadByte(); // including gaia
            var gameMode = reader.ReadUInt16(); // MP or SP?
            var campaign = reader.ReadUInt32();
            var campaignPlayer = reader.ReadUInt32();
            var campaignScenario = reader.ReadUInt32();
            var kingCampaign = reader.ReadUInt32();
            var kingCampaignPlayer = reader.ReadByte();
            var kingCampaignScenario = reader.ReadByte();
            var playerTurn = reader.ReadUInt32();
            var playerTimeDelta = ArrayUInt32(reader, 9);
            Padding(reader, 8);
        }
    }
}