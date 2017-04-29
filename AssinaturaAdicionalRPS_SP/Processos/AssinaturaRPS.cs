﻿using MXM.Assinatura.Domain.Interface;
using MXM.Assinatura.Infraestrutura.Prefeituras;
using System;
using System.Runtime.InteropServices;

namespace MXM.Assinatura.Processos
{
    [ClassInterface(ClassInterfaceType.None), Guid("C7C6CBA0-5677-4E1F-9CBC-F35E35FA564F")]
    public class AssinaturaRPS : IAssinaturaRPS
    {
        public string AssinaLoteRPS_Salvador(string sNumeroSerieCert, string sXML)
        {
            return new AssinaturaRPS_Salvador(sXML).Assinar(sNumeroSerieCert);
        }

        public string AssinaRPS_SP(String sNumeroSerieCert, String sAssinatura)
        {
            return new AssinaturaRPS_SP(sAssinatura).Assinar(sNumeroSerieCert);
        }
    }
}