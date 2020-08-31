using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NMeCab.Specialized;
using YukiNative.server;
using YukiNative.utils;

namespace YukiNative.services {
  public static class Mecab {
    private static readonly MeCabIpaDicTagger Tagger = MeCabIpaDicTagger.Create();

    private static readonly Dictionary<string, string> KanjiToAbbrMap = new Dictionary<string, string> {
      {"人名", "m"}, {"地名", "mp"}, {"名詞", "n"}, {"数詞", "num"},
      {"代名詞", "pn"}, {"動詞", "v"}, {"形状詞", "a"}, {"連体詞", "adn"},
      {"形容詞", "adj"}, {"副詞", "adv"}, {"助詞", "p"}, {"助動詞", "aux"},
      {"接尾辞", "suf"}, {"接尾詞", "suf"}, {"接頭辞", "pref"}, {"接頭詞", "pref"},
      {"感動詞", "int"}, {"接続詞", "conj"}, {"補助記号", "punct"}, {"記号", "w"},
    };

    private static string KanjiToAbbr(string abbr) {
      try {
        return KanjiToAbbrMap[abbr];
      }
      catch (Exception e) {
        return "unknown";
      }
    }

    private static string MecabTag(string text) {
      var result = "";

      var nodes = Tagger.Parse(text);
      foreach (var node in nodes) {
        var kana = "";
        var abbr = KanjiToAbbr(node.PartsOfSpeech);

        if (abbr.Equals("w")) {
          continue;
        }

        kana = WanaKana.KatakanaToHiragana(node.Pronounciation);
        if (kana.Equals(node.Surface)) {
          kana = "";
        }

        result += $"|{node.Surface},{abbr},{kana}";
      }

      return result.Substring(1);
    }

    public static async void MecabService(HttpServer server, Request request, HttpListenerResponse response) {
      var data = Encoding.UTF8.GetBytes(MecabTag(request.Body));
      await response.OutputStream.WriteAsync(data, 0, data.Length);
    }
  }
}