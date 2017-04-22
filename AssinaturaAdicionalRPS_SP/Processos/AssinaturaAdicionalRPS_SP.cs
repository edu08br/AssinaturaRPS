using MXM.Assinatura.Domain.Interface;
using MXM.Infraestrutura;
using System;
using System.Runtime.InteropServices;

namespace MXM.Assinatura.Processos
{
    [ClassInterface(ClassInterfaceType.None), Guid("C7C6CBA0-5677-4E1F-9CBC-F35E35FA564F")]
    public class AssinaturaAdicionalRPS_SP : IAssinaturaRPS
    {
        public string AssinaLoteRPS_Salvador(string sNumeroSerieCert, string sXML)
        {
            return String.Empty;
        }

        public string AssinaRPS_SP(String sNumeroSerieCert, String sAssinatura)
        {
            return new AssinaturaRPS_SP(sAssinatura).Assinar(sNumeroSerieCert);
        }
    }
}