// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("9RMGni8cdS8jZf+zpb614F1Hb9bZJgXU8TUXGTNewrEptoFmzjzDA4r0BTntV6JYmf6HP7dhpeEkZ/aFtgSHpLaLgI+sAM4AcYuHh4eDhoXgn+mR+mj3N76whNwnzL08wUnqbkaslat+3AES/Cpw6zK3TwKTTHfrYweoqIHyIsxpZ1EqV98z590N3trTCiA38uOYldzSzXHWw9z0IM44eXJh5yro6xNYlz4HZuNZev8CG4pqBIeJhrYEh4yEBIeHhi0i2C3TulJua6j8OTO9jXpNwK6jAKEFhPduTevsSVNMt2KcP/LiQrccp+Tc39EXXAx/wBWamVudFUaAAMruLjvHaDnx0w34NWYci281f/ZPu8j8ctNmd65iYvbM7nEMS4SFh4aH");
        private static int[] order = new int[] { 9,13,8,9,10,9,11,9,8,13,11,12,13,13,14 };
        private static int key = 134;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
