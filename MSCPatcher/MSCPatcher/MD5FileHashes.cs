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
    }
}
