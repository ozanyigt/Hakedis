using netDxf;
using NetDxfVersion = netDxf.Header.DxfVersion;

namespace Infrastructure.Metraj;

internal static class DxfLegacyLoader
{
  public static bool IsLegacyVersion(string filePath)
  {
    NetDxfVersion version = DxfDocument.CheckDxfFileVersion(filePath);
    return version < NetDxfVersion.AutoCad2000;
  }
}
