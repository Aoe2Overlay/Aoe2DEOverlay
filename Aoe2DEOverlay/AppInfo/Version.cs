using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Aoe2DEOverlay
{
    public class Version
    {
        public int Major = 0;
        public int Minor = 0;
        public int Patch = 0; 
        public Stage StageLabel = Stage.Alpha;
        public int StageNumber = 0;

        public int StageLabelNumber => (int) StageLabel;

        public enum Stage
        {
            Alpha = 0,
            Beta = 1,
            ReleaseCandidate = 2,
            Release = 3
        }

        public Version(int major = 0, int minor = 0, int patch = 0, Stage stage = Stage.Release, int number = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            StageLabel = stage;
            if(stage != Stage.Release) StageNumber = number;
        }
        
        public bool IsValid(string version)
        {
            Regex regex = new Regex("^(\\d)(\\.\\d)(\\.\\d)(|((\\-alpha|\\-beta|\\-rc)(|\\.\\d)))$");
            return regex.Match(version).Success;
        }
        public Version(string version)
        {
            if (!IsValid(version)) return;
            var splited = version.Split("-");

            var versionNumber = splited[0].Split(".");
            Major = Int32.Parse(versionNumber[0]);
            Minor = Int32.Parse(versionNumber[1]);
            Patch = Int32.Parse(versionNumber[2]);

            StageNumber = 0;
            StageLabel = Stage.Release;
            if (splited.Length > 1)
            {
                var versionStage = splited[1].Split(".");
                if (versionStage[0] == "alpha") StageLabel = Stage.Alpha;
                if (versionStage[0] == "beta") StageLabel = Stage.Beta;
                if (versionStage[0] == "rc") StageLabel = Stage.ReleaseCandidate;
                if (versionStage.Length > 1) StageNumber = Int32.Parse(versionStage[1]);
            }
        }
        
        
        
        public static bool operator ==(Version a, Version b) => Equals(a, b);
        public static bool operator !=(Version a, Version b) => !Equals(a, b);
        public static bool operator >(Version a, Version b) => GreaterThan(a, b);
        public static bool operator <(Version a, Version b) => LessThan(a, b);
        public static bool operator >=(Version a, Version b) => GreaterEqualThan(a, b);
        public static bool operator <=(Version a, Version b) => LessEqualThan(a, b);

        protected bool Equals(Version other)
        {
            return  Version.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            var version = obj as Version;
            if (version == null) return false;
            return Version.Equals(this, version);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, StageLabelNumber, StageNumber);
        }

        private static bool Equals(Version a, Version b)
        {
            return a.Major == b.Major &&
                   a.Minor == b.Minor &&
                   a.Patch == b.Patch &&
                   a.StageLabelNumber == b.StageLabelNumber &&
                   a.StageNumber == b.StageNumber;
        }
        
        private static bool GreaterThan(Version a, Version b)
        {
            if (a.Major > b.Major) return true;
            if (a.Major < b.Major) return false;
            if (a.Minor > b.Minor) return true;
            if (a.Minor < b.Minor) return false;
            if (a.Patch > b.Patch) return true;
            if (a.Patch < b.Patch) return false;
            if (a.StageLabelNumber > b.StageLabelNumber) return true;
            if (a.StageLabelNumber < b.StageLabelNumber) return false;
            if (a.StageNumber > b.StageNumber) return true;
            if (a.StageNumber < b.StageNumber) return false;
            return false;
        }
        
        private static bool GreaterEqualThan(Version a, Version b)
        {
            return GreaterThan(a, b) || Equals(a, b);
        }
        
        public static bool LessThan(Version a, Version b)
        {
            if (a.Major < b.Major) return true;
            if (a.Major > b.Major) return false;
            if (a.Minor < b.Minor) return true;
            if (a.Minor > b.Minor) return false;
            if (a.Patch < b.Patch) return true;
            if (a.Patch > b.Patch) return false;
            if (a.StageLabelNumber < b.StageLabelNumber) return true;
            if (a.StageLabelNumber > b.StageLabelNumber) return false;
            if (a.StageNumber < b.StageNumber) return true;
            if (a.StageNumber > b.StageNumber) return false;
            return false;
        }
        
        public static bool LessEqualThan(Version a, Version b)
        {
            return LessThan(a, b) || Equals(a, b);
        }

        public override string ToString()
        {
            var version = $"{Major}.{Minor}.{Patch}";
            if(StageLabel == Stage.Release) return version;
            var stage = new string[] { "alpha", "beta", "rc", ""};
            version += $"-{stage[StageLabelNumber]}";
            if (StageNumber > 0) version += $".{StageNumber}"; 
            return version;
        }
    }
}