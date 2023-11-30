using System.Runtime.InteropServices;

namespace BomberMatch.Bombers.C
{
    public sealed class CBomber : IBomber
    {
        public CBomber(string fileName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            pLib  = LoadLibrary(path);
            if (pLib == IntPtr.Zero)
                throw new InvalidOperationException($"Load library failed: {Marshal.GetLastWin32Error()}\n{fileName}");

            var pName = GetProcAddress(pLib, nameof(Name));
            var pSetRules = GetProcAddress(pLib, nameof(SetRules));
            var pGo = GetProcAddress(pLib, nameof(Go));

            name = (NameDelegate)Marshal.GetDelegateForFunctionPointer(pName, typeof(NameDelegate));
            setRules = (SetRulesDelegate)Marshal.GetDelegateForFunctionPointer(pSetRules, typeof(SetRulesDelegate));
            go = (GoDelegate)Marshal.GetDelegateForFunctionPointer(pGo, typeof(GoDelegate));
        }

        #region Unmanaged

        private readonly IntPtr pLib;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void NameDelegate([Out] char[] nameBot, uint size);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void SetRulesDelegate(int matchActionsNumber, int detonationRadius, int timeToDetonate);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        private delegate int GoDelegate(int[] arena, uint sizeArena, uint sizeArenaCol, int[] bombers, uint sizeBombers, int[] availableMoves, uint sizeAvailableMoves);

        private readonly NameDelegate name;

        private readonly SetRulesDelegate setRules;

        private readonly GoDelegate go;

        #endregion

        #region Kernel32

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        #endregion

        #region IBomber

        public string Name
        {
            get
            {
                var chars = new char[255];
                name(chars, 255);

                return new string(chars).Replace("\0", "");
            }           
        }

        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            setRules(matchActionsNumber, detonationRadius, timeToDetonate);
        }

        public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
        {
            var arena1d = Copy(arena);
            var bombers1d = Copy(bombers);
            var sizeArenaCol = arena.GetLength(1);

            return go(arena1d, (uint)arena1d.Length, (uint)sizeArenaCol, bombers1d, (uint)bombers1d.Length, availableMoves, (uint)availableMoves.Length);

            int[] Copy(int[,] array2d)
            {
                var result = new int[array2d.Length];
                var bytes = Buffer.ByteLength(array2d);
                Buffer.BlockCopy(array2d, 0, result, 0, bytes);
                return result;
            }
        }
        
        #endregion
    }
}
