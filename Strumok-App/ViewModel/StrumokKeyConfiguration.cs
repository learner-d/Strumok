using System;

namespace StrumokApp.ViewModel
{
    [Serializable]
    public class StrumokKeyConfiguration
    {
        public StrumokKeyConfiguration() { }
        public StrumokKeyConfiguration(bool strumok512, StrumokKeyConfiguration oldKeyConfiguration = null)
        {
            Strumok512 = strumok512;
            IV = new ulong[4];
            Key = new ulong[strumok512 ? 8 : 4];
            if (oldKeyConfiguration == null)
                return;
            if (oldKeyConfiguration.Key != null)
                Array.Copy(oldKeyConfiguration.Key, Key, Math.Min(oldKeyConfiguration.Key.Length, Key.Length));
            if (oldKeyConfiguration.IV != null)
                Array.Copy(oldKeyConfiguration.IV, IV, Math.Min(oldKeyConfiguration.IV.Length, IV.Length));
        }
        public bool Strumok512 { get; set; }
        public ulong[] Key { get; set; }
        public ulong[] IV { get; set; }
    }

    public static class StrumokKeyConfigurationExt
    {
        public static ulong GetValueOrDefault(ulong[] array, int index, ulong @default)
        {
            if (!(index < array?.Length))
                return @default;
            return array[index];
        }
        public static ulong GetKeyPartOrDefault(this StrumokKeyConfiguration keyConfiguration, int index, ulong @default)
        {
            return GetValueOrDefault(keyConfiguration?.Key, index, @default);
        }
        public static ulong GetIVPartOrDefault(this StrumokKeyConfiguration keyConfiguration, int index, ulong @default)
        {
            return GetValueOrDefault(keyConfiguration?.IV, index, @default);
        }
        public static void TrySetValue(ulong[] array, int index, ulong value)
        {
            if (!(index < array?.Length))
                return;
            array[index] = value;
        }
        public static void TrySetKeyPart(this StrumokKeyConfiguration keyConfiguration, int index, ulong value)
        {
            TrySetValue(keyConfiguration?.Key, index, value);
        }
        public static void TrySetIVPart(this StrumokKeyConfiguration keyConfiguration, int index, ulong value)
        {
            TrySetValue(keyConfiguration?.IV, index, value);
        }
    }
}
