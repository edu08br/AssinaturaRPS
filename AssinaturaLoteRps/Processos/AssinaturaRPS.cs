using MXM.Assinaturas.Infraestrutura.Prefeituras;
using RGiesecke.DllExport;
using System.Runtime.InteropServices;

namespace MXM.Assinaturas.Processos
{
    public class AssinaturaRPS
    {
        [DllExport("AssinaLoteRPS_Salvador", CallingConvention.StdCall)]
        public static void AssinaLoteRPS_Salvador([MarshalAs(UnmanagedType.BStr)] string sNumeroSerieCert,
                                                  [MarshalAs(UnmanagedType.BStr)] string sXML,
                                                  [MarshalAs(UnmanagedType.BStr)] ref string sRetorno)
        {
            sRetorno = new AssinaturaRPS_Salvador(sXML).Assinar(sNumeroSerieCert);
        }

        [DllExport("AssinaRPS_SP", CallingConvention.StdCall)]
        public static void AssinaRPS_SP([MarshalAs(UnmanagedType.BStr)] string sNumeroSerieCert,
                                        [MarshalAs(UnmanagedType.BStr)] string sAssinatura,
                                        [MarshalAs(UnmanagedType.BStr)] ref string sRetorno)
        {
            sRetorno = new AssinaturaRPS_SP(sAssinatura).Assinar(sNumeroSerieCert);
        }
    }
}