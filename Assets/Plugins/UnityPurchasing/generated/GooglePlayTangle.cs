#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("cW/om7bi2gKI7rkxBJNsaSsK1UAaXPC5XbXPLx4EhlCwdM0I3xv+He17H17PNfsAAQ3eLE27y5+iCsL6cfSJVbg5z8YAMuLhL5tSBI8vKff3gMQOZzsZGzNHpjfM5FzEJNyFT84ZAqFyHgzb5sgZ/bq/753u55ZEXt5GEGPhqLizDF4K8Ktv2PcqFAyAMrGSgL22uZo2+DZHvbGxsbWws0mSlSO5pSC7sdYbAvjzMR443aM+BK/3x8D2IDcK1D1sWZrB5xUNPDGMZDuWtcqGMsoDcv3d6bZ4Acn/hzKxv7CAMrG6sjKxsbAM1FMvw9V25ESRrRpT06pUD+sTuUyTXNQBGYcHCosBtKugtTAMVnhVt+G4XT649O8WtIJnGrVCL7KzsbCx");
        private static int[] order = new int[] { 13,10,12,7,4,7,8,13,12,12,12,12,12,13,14 };
        private static int key = 176;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
