namespace Blink.Plugin
{
    public class PluginDuo
    {
        public IBlink Plugin { get; internal set; }
        public PluginDetail Detail { get; internal set; }

        public override string ToString()
        {
            return Detail.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is PluginDuo r)
            {
                return string.Equals(r.Detail.Id, Detail.Id);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashcode = Detail.Id?.GetHashCode() ?? 0;
            return hashcode;
        }
    }
}
