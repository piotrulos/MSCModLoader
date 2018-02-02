using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSCPatcher
{
    class DebugStuff
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

        static string monoPath = null;

        public static int checkDebugStatus()
        {
            if (Form1.mscPath != "(unknown)")
            {
                monoPath = Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll");
                switch(Form1.MD5HashFile(monoPath))
                {
                    case "971E42FED544A21FAE4448DBC3F5FE9D":
                        return 1;
                    case "62629B5CA9C1BBE3C97A3E8C91D05D3A":
                        return 2;
                    case "F190C7ECFE414FB407137C1D95AC310E":
                        return 3;
                    case "3EE5C3BD42B61AE820A028AF35DF99C0":
                        return 4;
                    default:
                        return 0;
                }
                
            }
            return 0;
        }
    }
}
