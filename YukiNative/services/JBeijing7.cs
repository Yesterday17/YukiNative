﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YukiNative.server;

namespace YukiNative.services {
  /// <summary>
  /// Credits
  /// https://github.com/Artikash/VNR-Core/blob/6ed038bda9dcd35696040bd45d31afa6a30e8978/py/libs/jbeijing/jbjct.py
  /// https://www.lgztx.com/?p=93
  /// </summary>
  public static class JBeijing7 {
    [DllImport("JBJCT.dll", CharSet = CharSet.Unicode)]
    private static extern int JC_Transfer_Unicode(
      int hwnd, uint fromCordPage, uint toCodePage,
      int unknown, int unknown2,
      string from, StringBuilder to, ref int toCapacity,
      StringBuilder buffer, ref int bufferCapacity);

    private const uint SimplifiedChineseCodePage = 936;
    private const uint TraditionalChineseCodePage = 950;
    private const int BufferSize = 3000;

    public static string Translate(string text, bool simplified = true) {
      var result = new StringBuilder(BufferSize);
      var buf = new StringBuilder(BufferSize);
      var toCapacity = result.Capacity;
      var bufCapacity = buf.Capacity;
      var toCodePage = simplified ? SimplifiedChineseCodePage : TraditionalChineseCodePage;
      JC_Transfer_Unicode(0, 932, toCodePage, 1, 1, text, result, ref toCapacity, buf, ref bufCapacity);
      return result.ToString();
    }

    public static async Task JBeijing7TranslateService(HttpServer server, Request request, Response response) {
      await response.WriteText(await Task.Run(() => Translate(request.Body)));
    }


    [DllImport("JBJCT.dll", CharSet = CharSet.Unicode)]
    private static extern int DJC_OpenAllUserDic_Unicode(byte[] dict, int unknown = 0);

    [DllImport("JBJCT.dll", CharSet = CharSet.Unicode)]
    private static extern int DJC_CloseAllUserDic(int unknown = 1);

    private const int UserDictSize = 0x408;
    private const int MaxUserDictCount = 3;
    private const int UserDictBufferSize = UserDictSize * MaxUserDictCount;

    public static bool OpenUserDict(params string[] paths) {
      if (paths.Length == 0) {
        return false;
      }

      var buffer = new byte[UserDictBufferSize];
      for (var i = 0; i < paths.Length; i++) {
        if (i >= MaxUserDictCount) {
          break;
        }

        var data = Encoding.Unicode.GetBytes(paths[i]);
        Array.Copy(data, 0, buffer, i * UserDictSize, data.Length);
      }

      var ret = DJC_OpenAllUserDic_Unicode(buffer);
      return ret == 1 || ret == -255;
    }

    public static bool CloseUserDict() {
      var ret = DJC_CloseAllUserDic();
      return ret == 0;
    }

    public static void JBeijing7OpenUserDictService(HttpServer server, Request request, Response response) {
      var path = request.Body.Split('\n');
      var success = OpenUserDict(path);
      if (!success) {
        response.StatusCode(400);
      }
    }
  }
}