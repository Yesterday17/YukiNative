using System.Collections.Generic;

namespace YukiNative.utils {
  /// <summary>
  /// Migrated from https://github.com/WaniKani/WanaKana
  ///
  /// The MIT License (MIT)
  /// 
  /// Copyright (c) 2013 WaniKani Community Github
  /// 
  /// Permission is hereby granted, free of charge, to any person obtaining a copy of
  /// this software and associated documentation files (the "Software"), to deal in
  /// the Software without restriction, including without limitation the rights to
  /// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
  /// the Software, and to permit persons to whom the Software is furnished to do so,
  /// subject to the following conditions:
  /// 
  /// The above copyright notice and this permission notice shall be included in all
  /// copies or substantial portions of the Software.
  /// 
  /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
  /// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
  /// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
  /// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
  /// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  /// </summary>
  public static class WanaKana {
    private const int HiraganaStart = 0x3041;
    private const int KatakanaStart = 0x30a1;
    private const int KatakanaEnd = 0x30fc;
    private const int ProlongedSoundMark = 0x30fc;
    private const int KanaSlashDot = 0x30fb;

    private static readonly Dictionary<char, char> LongVowels = new Dictionary<char, char> {
      {'a', 'あ'}, {'i', 'い'}, {'u', 'う'}, {'e', 'え'}, {'o', 'う'},
    };

    private static readonly Dictionary<char, char> LastRomaji = new Dictionary<char, char> {
      {'あ', 'a'}, {'い', 'i'}, {'う', 'u'}, {'え', 'e'}, {'お', 'o'},
      {'か', 'a'}, {'き', 'i'}, {'く', 'u'}, {'け', 'e'}, {'こ', 'o'},
      {'さ', 'a'}, {'し', 'i'}, {'す', 'u'}, {'せ', 'e'}, {'そ', 'o'},
      {'た', 'a'}, {'ち', 'i'}, {'つ', 'u'}, {'て', 'e'}, {'と', 'o'},
      {'な', 'a'}, {'に', 'i'}, {'ぬ', 'u'}, {'ね', 'e'}, {'の', 'o'},
      {'は', 'a'}, {'ひ', 'i'}, {'ふ', 'u'}, {'へ', 'e'}, {'ほ', 'o'},
      {'ま', 'a'}, {'み', 'i'}, {'む', 'u'}, {'め', 'e'}, {'も', 'o'},
      {'ら', 'a'}, {'り', 'i'}, {'る', 'u'}, {'れ', 'e'}, {'ろ', 'o'},
      {'や', 'a'}, {'ゆ', 'u'}, {'よ', 'o'},
      {'わ', 'a'}, {'ゐ', 'i'}, {'ゑ', 'e'}, {'を', 'o'},
      {'ん', 'n'},
      {'が', 'a'}, {'ぎ', 'i'}, {'ぐ', 'u'}, {'げ', 'e'}, {'ご', 'o'},
      {'ざ', 'a'}, {'じ', 'i'}, {'ず', 'u'}, {'ぜ', 'e'}, {'ぞ', 'o'},
      {'だ', 'a'}, {'ぢ', 'i'}, {'づ', 'u'}, {'で', 'e'}, {'ど', 'o'},
      {'ば', 'a'}, {'び', 'i'}, {'ぶ', 'u'}, {'べ', 'e'}, {'ぼ', 'o'},
      {'ぱ', 'a'}, {'ぴ', 'i'}, {'ぷ', 'u'}, {'ぺ', 'e'}, {'ぽ', 'o'},
      {'ぁ', 'a'}, {'ぃ', 'i'}, {'ゔ', 'u'}, {'ぇ', 'e'}, {'ぉ', 'o'},
      {'ゃ', 'a'}, {'ゅ', 'u'}, {'ょ', 'o'},
    };

    private static bool IsCharLongDash(char ch) {
      return ch == ProlongedSoundMark;
    }


    private static bool IsCharInitialLongDash(char ch, int index) {
      return IsCharLongDash(ch) && index < 1;
    }

    private static bool IsCharInnerLongDash(char ch, int index) {
      return IsCharLongDash(ch) && index > 0;
    }

    private static bool IsCharKatakana(char ch) {
      return ch >= KatakanaStart && ch <= KatakanaEnd;
    }

    private static bool IsCharSlashDot(char ch) {
      return ch == KanaSlashDot;
    }


    private static bool IsKanaAsSymbol(char ch) {
      return ch == 'ヶ' || ch == 'ヵ';
    }


    // https://github.com/WaniKani/WanaKana/blob/34f63369368862c7f68242fc17f3ba1790d97a70/src/utils/katakanaToHiragana.js
    public static string KatakanaToHiragana(string text) {
      var result = "";

      var previousKana = '\0';
      var chars = text.ToCharArray();
      for (var index = 0; index < chars.Length; index++) {
        var c = chars[index];

        // Short circuit to avoid incorrect code shift for 'ー' and '・'
        if (IsCharSlashDot(c) || IsCharInitialLongDash(c, index) || IsKanaAsSymbol(c)) {
          result += c;
        }
        // Transform long vowels: 'オー' to 'おう'
        else if (previousKana != '\0' && IsCharInnerLongDash(c, index)) {
          // Transform previousKana back to romaji, and slice off the vowel
          result += LongVowels[LastRomaji[previousKana]];
        }
        // Shift char code.
        else if (!IsCharLongDash(c) && IsCharKatakana(c)) {
          var code = (char) (c + HiraganaStart - KatakanaStart);
          previousKana = code;
          result += code;
        }
      }

      return result;
    }
  }
}