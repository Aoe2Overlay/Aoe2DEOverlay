using System;
using System.IO;
using System.Linq;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        private uint _numHeaderData;
        private void ReadInitial(BinaryReader reader)
        {
            // NOT IMPLEMENTED 
            throw new Exception("ERROR: initial reader not implemented!");
            
            var restoreTime = reader.ReadUInt32(); // '0' for non-restored
            var numParticles = reader.ReadUInt32();
            var particles = reader.ReadBytes((int)numParticles * 27);
            var identifier = reader.ReadUInt32();
            for (int i = 0; i < NumberOfPlayers; i++)
            {
                var type = reader.ReadByte();
                var unk = reader.ReadByte();
                ReadInitialPlayer(reader);
                
                // start of objects
                Find(reader, new byte[]{ 0x0b,0x00,0x00,0x00,0x02,0x00,0x00});
                
                /*
                if (restoreTime == 0)
                {
                    Find(reader, new byte[] {0x00}, -1);
                    ExistingObject.Read(reader);
                    Const(reader, new byte[] { 0x00, 0x0b });
                    // Skip Gaia tree for performance reasons
                    if (type != 2)
                    {
                        var sSize = reader.ReadUInt32();
                        var sGrow = reader.ReadUInt32();
                        Find(reader, new byte[] {0x00}, -1);
                        ExistingObject.Read(reader);
                        Const(reader, new byte[] { 0x00, 0x0b });
                        var dSize = reader.ReadUInt32();
                        var dGrow = reader.ReadUInt32();
                        Find(reader, new byte[] {0x00}, -1);
                        ExistingObject.Read(reader); // doppleganger objects
                        Const(reader, new byte[] { 0x00, 0x0b });
                    }
                }
                */
                
                //GotoObjectsEnd(reader);
                
            }
            Padding(reader, 21);
        }

        public void GotoObjectsEnd(BinaryReader reader)
        {
            // num_players: NumberOfPlayers
            // marker_num: _numHeaderData
            // save_version: SaveVersion
            
            var start = reader.BaseStream.Position;

            // Try to find the first marker, a portion of the next player structure
            // The byte that changes is the number of player stats fields
            var markerNumber = BitConverter.GetBytes((int)_numHeaderData).Reverse().ToArray();
            var markerPattern = new byte[] {0x16}.Concat(markerNumber).Concat(new byte[] {0x21}).ToArray();
            Find(reader, markerPattern);
            // If it exists, we're not on the last player yet
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // Backtrack through the player name
                var count = 0;
                var marker = 0;
                reader.BaseStream.Seek(-2, SeekOrigin.Current);
                while (reader.ReadUInt16() != count)
                {
                    marker -= 1;
                    count += 1;
                    reader.BaseStream.Seek(marker-2, SeekOrigin.Current);
                } 
                // Backtrack through the rest of the next player structure
                //backtrack = 43 + num_players

            }
            // Otherwise, this is the last player
            else 
            {
                
            }
            /*
            # If it exists, we're not on the last player yet
            if marker > 0:
                # Backtrack through the player name
                count = 0
                while struct.unpack("<H", read_bytes[marker-2:marker])[0] != count:
                    marker -= 1
                    count += 1
                # Backtrack through the rest of the next player structure
                backtrack = 43 + num_players
            # Otherwise, this is the last player
            else:
                # Search for the scenario header
                # TODO: make this section more reliable
                marker_aok = read_bytes.find(b"\x9a\x99\x99\x3f")
                marker_up = read_bytes.find(b"\xf6\x28\x9c\x3f")
                marker_hd = read_bytes.find(b"\xae\x47\xa1\x3f")
                if save_version >= 25.01:
                    marker_de = read_bytes.find(b"\x3d\x0a\xb7\x3f")
                elif save_version >= 20.16:
                    marker_de = read_bytes.find(b"\x8f\xc2\xb5\x3f")
                elif save_version >= 20.06:
                    marker_de = read_bytes.find(b"\xe1\x7a\xb4\x3f")
                elif save_version >= 13.34:
                    marker_de = read_bytes.find(b"\x33\x33\xb3\x3f")
                elif save_version >= 13.07:
                    marker_de = read_bytes.find(b"\x29\x5c\xaf\x3f")
                else:
                    marker_de = read_bytes.find(b"\x7b\x14\xae\x3f")
                new_marker = -1
                if marker_up > 0 and marker_de < 0 and marker_hd < 0: # aok marker can appear in up
                    new_marker = marker_up
                elif marker_de > 0 and marker_up < 0 and marker_aok < 0 and marker_hd < 0:
                    new_marker = marker_de
                elif marker_aok > 0 and marker_up < 0 and marker_de < 0 and marker_hd < 0:
                    new_marker = marker_aok
                elif marker_hd > 0 and marker_up < 0 and marker_de < 0 and marker_aok < 0:
                    new_marker = marker_hd
                if new_marker == -1:
                    raise RuntimeError("could not find scenario marker")
                marker = new_marker
                # Backtrack through the achievements and initial structure footer
                backtrack = ((1817 * (num_players - 1)) + 4 + 19)
            # Seek to the position we found
            end = start + marker - backtrack - 2
            stream.seek(end)
            return end
             */
        }

        private void ReadInitialPlayer(BinaryReader reader)
        {
            // Player attributes
            var theirDiplomacyId = ArrayByte(reader, (int)NumberOfPlayers);
            var myDiplomacy = ArrayInt32(reader, 9);
            var alliedLos = reader.ReadUInt32();
            var alliedVictory = reader.ReadBoolean();
            var playerNameLength = reader.ReadUInt16();
            var playerName = reader.ReadBytes(playerNameLength - 1);
            Padding(reader, 1); // 0x00
            Padding(reader, 1); // 0x16
            _numHeaderData = reader.ReadUInt32();
            Padding(reader, 1); // 0x21
            
            // Player states
            var food = reader.ReadSingle();
            var wood = reader.ReadSingle();
            var stone = reader.ReadSingle();
            var gold = reader.ReadSingle();
            var headroom = reader.ReadSingle();
            var conversionRange = reader.ReadSingle();
            var currentAge = reader.ReadSingle();
            var numRelics = reader.ReadSingle();
            var tradeBonus = reader.ReadSingle();
            var tradeGoods = reader.ReadSingle();
            var tradeProduction = reader.ReadSingle();
            var popCurrent = reader.ReadSingle();
            var decay = reader.ReadSingle();
            var discovery = reader.ReadSingle();
            var ruins = reader.ReadSingle();
            var meat = reader.ReadSingle();
            var berries = reader.ReadSingle();
            var fish = reader.ReadSingle();
            var u10 = reader.ReadSingle();
            var totalUnitsCreated = reader.ReadSingle();
            var numKills = reader.ReadSingle();
            var numItemsReserched = reader.ReadSingle();
            var precentMapExplored = reader.ReadSingle();
            var castleAge = reader.ReadSingle();
            var imperialAge = reader.ReadSingle();
            var feudalAge = reader.ReadSingle();
            var u14 = reader.ReadSingle();
            var convertPriests = reader.ReadSingle();
            var convertBuildings = reader.ReadSingle();
            var u17 = reader.ReadSingle();
            var buildingLimit = reader.ReadSingle();
            var foodLimit = reader.ReadSingle();
            var popMax = reader.ReadSingle();
            var maintenance = reader.ReadSingle();
            var faith = reader.ReadSingle();
            var faithRechargeRate = reader.ReadSingle();
            var farmFoodAmount = reader.ReadSingle();
            var civilianPop = reader.ReadSingle();
            var u23 = reader.ReadSingle();
            var allTechsAchieved = reader.ReadSingle();
            var militaryPop = reader.ReadSingle();
            var conversions = reader.ReadSingle();
            var numWonders = reader.ReadSingle();
            var razings = reader.ReadSingle();
            var killRatio = reader.ReadSingle();
            var playerKilled = reader.ReadSingle();
            var tributeInefficiency = reader.ReadSingle();
            var goldBonusMiningProductivity = reader.ReadSingle();
            var townCenterUnavailable = reader.ReadSingle();
            var totalGoldGathered2 = reader.ReadSingle();
            var hasCartography = reader.ReadSingle();
            var housesCount = reader.ReadSingle();
            var monasteries = reader.ReadSingle();
            var totalResourcesTributed = reader.ReadSingle();
            var holdRuins = reader.ReadSingle();
            var holdRelics = reader.ReadSingle();
            var ore = reader.ReadSingle();
            var capturedUnit = reader.ReadSingle();
            var darkAge = reader.ReadSingle();
            var tradeGoodQuality = reader.ReadSingle();
            var tradeMarketLevel = reader.ReadSingle();
            var formations = reader.ReadSingle();
            var buildingHousingRate = reader.ReadSingle();
            var gatherTaxRate = reader.ReadSingle();
            var gatherAccumulator = reader.ReadSingle();
            var decayRate = reader.ReadSingle();
            var allowFormations = reader.ReadSingle();
            var canConvert = reader.ReadSingle();
            var hitOointsKilled = reader.ReadSingle();
            var p1Kills = reader.ReadSingle();
            var p2Kills = reader.ReadSingle();
            var p3Kills = reader.ReadSingle();
            var p4Kills = reader.ReadSingle();
            var p5Kills = reader.ReadSingle();
            var p6Kills = reader.ReadSingle();
            var p7Kills = reader.ReadSingle();
            var p8Kills = reader.ReadSingle();
            var convertResistance = reader.ReadSingle();
            var tradeFee = reader.ReadSingle();
            var stoneMiningProductivity = reader.ReadSingle();
            var numUnitsQueued = reader.ReadSingle();
            var numUnitsMaking = reader.ReadSingle();
            var raider = reader.ReadSingle();
            var boardingRechargeRate = reader.ReadSingle();
            var startingVillagers = reader.ReadSingle();
            var researchCostMod = reader.ReadSingle();
            var researchTimeMod = reader.ReadSingle();
            var convertBoats = reader.ReadSingle();
            var fishTrapFood = reader.ReadSingle();
            var healRateModifier = reader.ReadSingle();
            var healRange = reader.ReadSingle();
            var foodBonus = reader.ReadSingle();
            var woodBonus = reader.ReadSingle();
            var stoneBonus = reader.ReadSingle();
            var goldBonus = reader.ReadSingle();
            var raiderAbility = reader.ReadSingle();
            var berserkerHealTimer = reader.ReadSingle();
            var dominantSheepControl = reader.ReadSingle();
            var objectCostSummation = reader.ReadSingle();
            var researchCostSummation = reader.ReadSingle();
            var relicIncomeSummation = reader.ReadSingle();
            var tradeIncomeSummation = reader.ReadSingle();
            var p1TributedTo = reader.ReadSingle();
            var p2TributedTo = reader.ReadSingle();
            var p3TributedTo = reader.ReadSingle();
            var p4TributedTo = reader.ReadSingle();
            var p5TributedTo = reader.ReadSingle();
            var p6TributedTo = reader.ReadSingle();
            var p7TributedTo = reader.ReadSingle();
            var p8TributedTo = reader.ReadSingle();
            var p1UnitKillWorth = reader.ReadSingle();
            var p2UnitKillWorth = reader.ReadSingle();
            var p3UnitKillWorth = reader.ReadSingle();
            var p4UnitKillWorth = reader.ReadSingle();
            var p5UnitKillWorth = reader.ReadSingle();
            var p6UnitKillWorth = reader.ReadSingle();
            var p7UnitKillWorth = reader.ReadSingle();
            var p8UnitKillWorth = reader.ReadSingle();
            var p1NumRazes = reader.ReadSingle();
            var p2NumRazes = reader.ReadSingle();
            var p3NumRazes = reader.ReadSingle();
            var p4NumRazes = reader.ReadSingle();
            var p5NumRazes = reader.ReadSingle();
            var p6NumRazes = reader.ReadSingle();
            var p7NumRazes = reader.ReadSingle();
            var p8NumRazes = reader.ReadSingle();
            var p1RazeWorth = reader.ReadSingle();
            var p2RazeWorth = reader.ReadSingle();
            var p3RazeWorth = reader.ReadSingle();
            var p4RazeWorth = reader.ReadSingle();
            var p5RazeWorth = reader.ReadSingle();
            var p6RazeWorth = reader.ReadSingle();
            var p7RazeWorth = reader.ReadSingle();
            var p8RazeWorth = reader.ReadSingle();
            var numCastles = reader.ReadSingle();
            var numWonders2 = reader.ReadSingle();
            var killsByP1 = reader.ReadSingle();
            var killsByP2 = reader.ReadSingle();
            var killsByP3 = reader.ReadSingle();
            var killsByP4 = reader.ReadSingle();
            var killsByP5 = reader.ReadSingle();
            var killsByP6 = reader.ReadSingle();
            var killsByP7 = reader.ReadSingle();
            var killsByP8 = reader.ReadSingle();
            var razingsByP1 = reader.ReadSingle();
            var razingsByP2 = reader.ReadSingle();
            var razingsByP3 = reader.ReadSingle();
            var razingsByP4 = reader.ReadSingle();
            var razingsByP5 = reader.ReadSingle();
            var razingsByP6 = reader.ReadSingle();
            var razingsByP7 = reader.ReadSingle();
            var razingsByP8 = reader.ReadSingle();
            var valueKilledByOthers = reader.ReadSingle();
            var valueRazedByOthers = reader.ReadSingle();
            var kills = reader.ReadSingle();
            var losses = reader.ReadSingle();
            var p1TributeRecvd = reader.ReadSingle();
            var p2TributeRecvd = reader.ReadSingle();
            var p3TributeRecvd = reader.ReadSingle();
            var p4TributeRecvd = reader.ReadSingle();
            var p5TributeRecvd = reader.ReadSingle();
            var p6TributeRecvd = reader.ReadSingle();
            var p7TributeRecvd = reader.ReadSingle();
            var p8TributeRecvd = reader.ReadSingle();
            var currentUnitWorth = reader.ReadSingle();
            var currentBuildingWorth = reader.ReadSingle();
            var totalFoodGathered = reader.ReadSingle();
            var totalWoodGathered = reader.ReadSingle();
            var totalStoneGathered = reader.ReadSingle();
            var totalGoldGathered = reader.ReadSingle();
            var totalKillAndRazeWorth = reader.ReadSingle();
            var totalTributeRecvd = reader.ReadSingle();
            var totalValueOfRazings = reader.ReadSingle();
            var castleHigh = reader.ReadSingle();
            var wonderHigh = reader.ReadSingle();
            var totalTributeSent = reader.ReadSingle();
            var convertMinAdj = reader.ReadSingle();
            var convertMaxAdj = reader.ReadSingle();
            var convertResistMinAdj = reader.ReadSingle();
            var convertResistMaxAdj = reader.ReadSingle();
            var convertBuildingMin = reader.ReadSingle();
            var convertBuildingMax = reader.ReadSingle();
            var convertBuildingChance = reader.ReadSingle();
            var spies = reader.ReadSingle();
            var valueWondersCastles = reader.ReadSingle();
            var foodScore = reader.ReadSingle();
            var woodScore = reader.ReadSingle();
            var stoneScore = reader.ReadSingle();
            var goldScore = reader.ReadSingle();
            var woodBonus0 = reader.ReadSingle();
            var foodBonus0 = reader.ReadSingle();
            var relicRate = reader.ReadSingle();
            var heresy = reader.ReadSingle();
            var theocracy = reader.ReadSingle();
            var crenellations = reader.ReadSingle();
            var constructionRateMod = reader.ReadSingle();
            var hunWonderBonus = reader.ReadSingle();
            var spiesDiscount = reader.ReadSingle();
            ArrayFloat32(reader, (int) _numHeaderData - 198);
            Padding(reader, 1);
            var cameraX = reader.ReadSingle();
            var cameraY = reader.ReadSingle();
            //var endOfCamera = reader.BaseStream.Position;
            var spawnLocationX = reader.ReadUInt16();
            var spawnLocationY = reader.ReadUInt16();
            var culture = reader.ReadByte();
            var civilization = reader.ReadByte();
            var gameStatus = reader.ReadByte();
            var resigned = reader.ReadBoolean();
            Padding(reader, 1);
            var playerColor = reader.ReadBoolean();
            Padding(reader, 1);
            //var endOfAttr = reader.BaseStream.Position;
        }

        private static class ExistingObject
        {
            public static void Read(BinaryReader reader)
            {
                var type = reader.ReadByte();
                var playerId = reader.ReadByte();

                if (type == 10) ReadStatic(reader);
                if (type == 20) ReadAnimated(reader);
                if (type == 25) ReadAnimated(reader);
                if (type == 30) ReadMoving(reader);
                if (type == 40) ReadAction(reader);
                if (type == 50) ReadBaseCombat(reader);
                if (type == 60) ReadMissile(reader);
                if (type == 70) ReadCombat(reader);
                if (type == 80) ReadBuilding(reader);
                if (type == 90) ReadStatic(reader);
                // else  Pass
            }
            private static void ReadStatic(BinaryReader reader)
            {
            }
            
            private static void ReadAnimated(BinaryReader reader)
            {
            } 
            private static void ReadMoving(BinaryReader reader)
            {
            }
            private static void ReadAction(BinaryReader reader)
            {
            }
            private static void ReadBaseCombat(BinaryReader reader)
            {
            }
            private static void ReadMissile(BinaryReader reader)
            {
            }
            private static void ReadCombat(BinaryReader reader)
            {
            }
            private static void ReadBuilding(BinaryReader reader)
            {
            }
        }
    }
}