//File for constants

namespace MSCPatcher
{
    public static class MD5FileHashes
    {
        /*
        * mono.dll MD5 hashes:
        * 
        * 971E42FED544A21FAE4448DBC3F5FE9D (32-bit, normal)
        * 62629B5CA9C1BBE3C97A3E8C91D05D3A (32-bit, debug)
        * F190C7ECFE414FB407137C1D95AC310E (64-bit, normal)
        * 3EE5C3BD42B61AE820A028AF35DF99C0 (64-bit, debug)
        * 
        */
        public const string mono32normal = "971E42FED544A21FAE4448DBC3F5FE9D";
        public const string mono32debug = "62629B5CA9C1BBE3C97A3E8C91D05D3A";
        public const string mono64normal = "F190C7ECFE414FB407137C1D95AC310E";
        public const string mono64debug = "3EE5C3BD42B61AE820A028AF35DF99C0";

        /*
         * mysummercar.exe
         * 
         * E9F1D7E11359DA995881B9280E1726B3 (32-bit)
         * 0B782168216B6B614D6C63485DAE20B0 (64-bit unity default)
         * 3C3F1460A074993E7F483F08318A2015 (64-bit from beta branch)
         * 
         * CSteamworks.dll
         * 
         * 4802608A59A9D268EF94A5C0727EC777 (32-bit)
         * B7F58E5AD108BFEDC1F90CD3525AD29A (64-bit)
         */

        public const string exe32 = "E9F1D7E11359DA995881B9280E1726B3";
        public const string exe64 = "0B782168216B6B614D6C63485DAE20B0";
        public const string exe64o = "3C3F1460A074993E7F483F08318A2015";
        public const string steam32 = "4802608A59A9D268EF94A5C0727EC777";
        public const string steam64 = "B7F58E5AD108BFEDC1F90CD3525AD29A";


    }
}
