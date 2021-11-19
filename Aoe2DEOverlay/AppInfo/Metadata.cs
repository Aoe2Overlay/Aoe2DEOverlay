﻿using System;
using System.Dynamic;
using System.Printing;

namespace Aoe2DEOverlay
{
    public enum Platfrom
    {
        x86,
        x64
    }
    public class Metadata
    {
        public static Version Version = new Version("1.0.0-alpha.6");
        public static Platfrom platform = IntPtr.Size == 4 ? Platfrom.x86 : Platfrom.x64;
        public static ISecret Secret = CreateSecret();
        public static bool HasSecret = Secret != null;



        private static ISecret CreateSecret()
        {
            var type = Type.GetType("Aoe2DEOverlay.Secret");
            if (type == null) return null;
            return Activator.CreateInstance(type) as ISecret;
        }
    }
}