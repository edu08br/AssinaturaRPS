using System;
using System.Runtime.InteropServices;

namespace MXM.Assinatura.Domain.Interface
{
    [InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("25293B56-1238-4FF3-BCB2-E69430C37165")]
    public interface IAssinaturaRPS
    {
        string AssinaRPS_SP(String sNumeroSerieCert, String sAssinatura);

        string AssinaLoteRPS_Salvador(String sNumeroSerieCert, String sXML);
    }
}