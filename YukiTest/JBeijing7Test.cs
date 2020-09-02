using System.IO;
using NUnit.Framework;
using YukiNative.services;
using YukiNative.utils;

namespace YukiTest {
  [TestFixture]
  public class JBeijing7Test {
    private const string JBeijing7Path = @"D:\Game\Gal\Tools\JBeijing7";

    private const string TestSentence = "これからも、アイドル！！！";
    private const string TranslatedSimplifiedChinese = "今后，偶像!!!";
    private const string TranslatedTraditionalChinese = "今後，偶像!!!";

    private const string TestWord = "アイマス";
    private const string CorrectWordTranslation = "偶像大师";
    private const string WrongWordTranslation = "蓼蓝集体";

    static JBeijing7Test() {
      Library.SetDllDirectory(JBeijing7Path);
    }

    [Test]
    public void TestJcTransferUnicode() {
      // Simplified Chinese
      Assert.True(JBeijing7.Translate(TestSentence).Equals(TranslatedSimplifiedChinese));

      // Traditional Chinese
      Assert.True(JBeijing7.Translate(TestSentence, false).Equals(TranslatedTraditionalChinese));
    }

    [Test]
    public void TestJcUserDict() {
      // With user dict
      Assert.True(JBeijing7.OpenUserDict(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\JcUserdic\Jcuser")));
      Assert.True(JBeijing7.Translate(TestWord).Equals(CorrectWordTranslation));

      // Without user dict
      Assert.True(JBeijing7.CloseUserDict());
      Assert.True(JBeijing7.Translate(TestWord).Equals(WrongWordTranslation));
    }
  }
}