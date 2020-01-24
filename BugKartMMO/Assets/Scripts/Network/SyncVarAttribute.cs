using AttributeTargets = System.AttributeTargets;

namespace Network
{
    [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SyncVar : System.Attribute
    {
        public string Hook { get; private set; }

        public SyncVar(string _hook)
        {
            Hook = _hook;
        }

        public SyncVar()
        {
            Hook = null;
        }
    }
}