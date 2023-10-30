using System.Reflection;

namespace BomberMatch.Bombers.Assembly
{
    public sealed class AssemblyBomber : IBomber
    {
        private System.Reflection.Assembly ass;
        private PropertyInfo name;
        private MethodInfo setRules;
        private MethodInfo go;
        private object instance;

        public AssemblyBomber(string fileName)
        {
            ass = System.Reflection.Assembly.LoadFrom(fileName);

            var allTypes = ass.GetExportedTypes();
            if (allTypes.Length == 0)
            {
                throw new Exception($"No public types in assembly {fileName}");
            }

            foreach (var type in allTypes)
            {
                name = type.GetProperty("Name", typeof(string));
                setRules = type.GetMethod("SetRules", new[] { typeof(int), typeof(int), typeof(int) });
                go = type.GetMethod("Go", new[] { typeof(int[,]), typeof(int[,]), typeof(int[]) });

                if (name == null || !name.CanRead
                    || setRules == null || setRules.ReturnType != typeof(void)
                    || go == null || go.ReturnType != typeof(int))
                {
                    continue;
                }

                instance = Activator.CreateInstance(type);
                break;
            }
            if (instance == null)
            {
                throw new Exception("Can't find proper type");
            }
        }

        public string Name => (string)name.GetValue(instance);

        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            setRules.Invoke(instance, new object[] { matchActionsNumber, detonationRadius, timeToDetonate });
        }

        public int Go(int[,] arena, int[,] bombers, int[] availableActions)
        {
            return (int)go.Invoke(instance, new object[] { arena, bombers, availableActions });
        }
    }
}
