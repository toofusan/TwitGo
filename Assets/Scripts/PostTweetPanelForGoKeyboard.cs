using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using OculusGo;

public class PostTweetPanelForGoKeyboard : MonoBehaviour {

    [SerializeField] private GameObject charPanelPrefab;

    private PostTweetPanelForGo postTweetPanel;
	private Transform CharPanels;
	private Transform ChangeTypePanels;
    private KanaType currentKanaType;

    public void SetKeyboard()
    {
		postTweetPanel = GetComponentInParent<PostTweetPanelForGo>();
		CharPanels = transform.Find("CharPanels");
		ChangeTypePanels = transform.Find("ChangeTypePanels");

        SetCharPanel(KanaType.Hiragana);
		SetKanaTypePanel();
    }
    private string[,] hiraganaForKeyboard = new string[10, 5] {
        {"あ", "い", "う", "え", "お"},
        {"か", "き", "く", "け", "こ"},
        {"さ", "し", "す", "せ", "そ"},
        {"た", "ち", "つ", "て", "と"},
        {"な", "に", "ぬ", "ね", "の"},
        {"は", "ひ", "ふ", "へ", "ほ"},
        {"ま", "み", "む", "め", "も"},
        {"や", "ゆ", "よ", "、", "。"},
        {"ら", "り", "る", "れ", "ろ"},
        {"わ", "を", "ん", "ー", "change"}
    };

    private string[,] katakanaForKeyboard = new string[10, 5] {
        {"ア", "イ", "ウ", "エ", "オ"},
        {"カ", "キ", "ク", "ケ", "コ"},
        {"サ", "シ", "ス", "セ", "ソ"},
        {"タ", "チ", "ツ", "テ", "ト"},
        {"ナ", "ニ", "ヌ", "ネ", "ノ"},
        {"ハ", "ヒ", "フ", "ヘ", "ホ"},
        {"マ", "ミ", "ム", "メ", "モ"},
        {"ヤ", "ユ", "ヨ", "、", "。"},
        {"ラ", "リ", "ル", "レ", "ロ"},
        {"ワ", "ヲ", "ン", "ー", "change"}
    };

    private string[,] lowerCaseForKeyboard = new string[10, 5] {
        {"j", "t", "", "", ""},
        {"i", "s", "", "", ""},
        {"h", "r", "", "", ""},
        {"g", "q", "", "", ""},
        {"f", "p", "z", "", ""},
        {"e", "o", "y", "", ""},
        {"d", "n", "x", "", ""},
        {"c", "m", "w", "", ""},
        {"b", "l", "v", "", ""},
        {"a", "k", "u", " ", ""}
    };

    private string[,] UpperCaseForKeyboard = new string[10, 5] {
        {"J", "T", "", "", ""},
        {"I", "S", "", "", ""},
        {"H", "R", "", "", ""},
        {"G", "Q", "", "", ""},
        {"F", "P", "Z", "", ""},
        {"E", "O", "Y", "", ""},
        {"D", "N", "X", "", ""},
        {"C", "M", "W", "", ""},
        {"B", "L", "V", "", ""},
        {"A", "K", "U", " ", ""}
    };

    private Dictionary<string, string> kanaMarks = new Dictionary<string, string>() {
        {"か", "が"}, {"き", "ぎ"}, {"く", "ぐ"}, {"け", "げ"}, {"こ", "ご"},
        {"さ", "ざ"}, {"し", "じ"}, {"す", "ず"}, {"せ", "ぜ"}, {"そ", "ぞ"},
        {"た", "だ"}, {"ち", "ぢ"}, {"つ", "づ"}, {"て", "で"}, {"と", "ど"},
        {"は", "ば"}, {"ひ", "び"}, {"ふ", "ぶ"}, {"へ", "べ"}, {"ほ", "ぼ"},
        {"ば", "ぱ"}, {"び", "ぴ"}, {"ぶ", "ぷ"}, {"べ", "ぺ"}, {"ぼ", "ぽ"},
        {"カ", "ガ"}, {"キ", "ギ"}, {"ク", "グ"}, {"ケ", "ゲ"}, {"コ", "ゴ"},
        {"サ", "ザ"}, {"シ", "ジ"}, {"ス", "ズ"}, {"セ", "ゼ"}, {"ソ", "ゾ"},
        {"タ", "ダ"}, {"チ", "ヂ"}, {"ツ", "ヅ"}, {"テ", "デ"}, {"ト", "ド"},
        {"ハ", "バ"}, {"ヒ", "ビ"}, {"フ", "ブ"}, {"ヘ", "べ"}, {"ホ", "ボ"},
        {"バ", "パ"}, {"ビ", "ピ"}, {"ブ", "プ"}, {"ベ", "ペ"}, {"ボ", "ポ"},
        {"あ", "ぁ"}, {"い", "ぃ"}, {"う", "ぅ"}, {"え", "ぇ"}, {"お", "ぉ"},
        {"や", "ゃ"}, {"ゆ", "ゅ"}, {"よ", "ょ"}, {"づ", "っ"},
        {"ア", "ァ"}, {"イ", "ィ"}, {"ウ", "ゥ"}, {"エ", "ェ"}, {"オ", "ォ"},
        {"ヤ", "ャ"}, {"ユ", "ュ"}, {"ヨ", "ョ"}, {"ヅ", "ッ"}
    };

    private Dictionary<string, string> hiraganaSonantMarks = new Dictionary<string, string>() {
        {"か", "が"}, {"き", "ぎ"}, {"く", "ぐ"}, {"け", "げ"}, {"こ", "ご"},
        {"さ", "ざ"}, {"し", "じ"}, {"す", "ず"}, {"せ", "ぜ"}, {"そ", "ぞ"},
        {"た", "だ"}, {"ち", "ぢ"}, {"つ", "づ"}, {"て", "で"}, {"と", "ど"},
        {"は", "ば"}, {"ひ", "び"}, {"ふ", "ぶ"}, {"へ", "べ"}, {"ほ", "ぼ"}
    };

    private Dictionary<string, string> katakanaSonantMarks = new Dictionary<string, string>() {
        {"カ", "ガ"}, {"キ", "ギ"}, {"ク", "グ"}, {"ケ", "ゲ"}, {"コ", "ゴ"},
        {"サ", "ザ"}, {"シ", "ジ"}, {"ス", "ズ"}, {"セ", "ゼ"}, {"ソ", "ゾ"},
        {"タ", "ダ"}, {"チ", "ヂ"}, {"ツ", "ヅ"}, {"テ", "デ"}, {"ト", "ド"},
        {"ハ", "バ"}, {"ヒ", "ビ"}, {"ふ", "ぶ"}, {"へ", "べ"}, {"ホ", "ボ"}
    };

    private Dictionary<string, string> hiraganaPSoundMarks = new Dictionary<string, string>() {
        {"は", "ぱ"}, {"ひ", "ぴ"}, {"ふ", "ぷ"}, {"へ", "ぺ"}, {"ほ", "ぽ"}
    };
    private Dictionary<string, string> katakanaPSoundMarks = new Dictionary<string, string>() {
        {"ハ", "パ"}, {"ヒ", "ピ"}, {"フ", "プ"}, {"ヘ", "ペ"}, {"ホ", "ポ"}
    };

    private Dictionary<string, string> hiraganaSmallChars = new Dictionary<string, string>() {
        {"あ", "ぁ"}, {"い", "ぃ"}, {"う", "ぅ"}, {"え", "ぇ"}, {"お", "ぉ"},
        {"や", "ゃ"}, {"ゆ", "ゅ"}, {"よ", "ょ"}, {"つ", "っ"}
    };

    private Dictionary<string, string> katakanaSmallChars = new Dictionary<string, string>() {
        {"ア", "ァ"}, {"イ", "ィ"}, {"ウ", "ゥ"}, {"エ", "ェ"}, {"オ", "ォ"},
        {"ヤ", "ャ"}, {"ユ", "ュ"}, {"ヨ", "ョ"}, {"ツ", "ッ"}
    };

    private Vector2 positionOfA = new Vector2(0f, 0f);
    private float x = 0f;
    private float y = 0f;
    
    private float intervalOfPanel = -50f;
    private void SetCharPanel(KanaType type)
    {

        string[,] keyboardList = new string[10, 5];

        switch (type)
        {
            case KanaType.Hiragana:
                keyboardList = hiraganaForKeyboard;
                break;
            case KanaType.Katakana:
                keyboardList = katakanaForKeyboard;
                break;
            case KanaType.lowerCase:
                keyboardList = lowerCaseForKeyboard;
                break;
            case KanaType.UpperCase:
                keyboardList = UpperCaseForKeyboard;
                break;
        }

        for (int i = 0; i < keyboardList.GetLength(0); i++)
        {
            for (int j = 0; j < keyboardList.GetLength(1); j++)
            {
                if (keyboardList[i, j] == "") continue;

                GameObject charPanel = (GameObject)Instantiate(charPanelPrefab, transform.position, Quaternion.identity);
                charPanel.transform.SetParent(CharPanels, false);
                charPanel.GetComponent<PostTweetPanelForGoCharPanel>().Init(x, y, keyboardList[i, j]);

                y += intervalOfPanel;
            }
            y = 0f;
            x += intervalOfPanel;
        }
        x = 0f;
        y = 0f;
        currentKanaType = type;
    }
    public void ClearCharPanel()
    {
        foreach (Transform child in CharPanels)
        {
            StartCoroutine(DestroyCharPanel(child));
        }

    }

    private IEnumerator DestroyCharPanel(Transform child)
    {
        yield return new WaitForEndOfFrame();
        Destroy(child.gameObject);
    }
    private void SetKanaTypePanel()
    {
        GameObject hiraganaPanel = (GameObject)Instantiate(charPanelPrefab, transform.position, Quaternion.identity);
        hiraganaPanel.transform.SetParent(ChangeTypePanels, false);
        hiraganaPanel.GetComponent<PostTweetPanelForGoCharPanel>().Init(0f, 0f, "かな");

        GameObject katakanaPanel = (GameObject)Instantiate(charPanelPrefab, transform.position, Quaternion.identity);
        katakanaPanel.transform.SetParent(ChangeTypePanels, false);
        katakanaPanel.GetComponent<PostTweetPanelForGoCharPanel>().Init(0f, -50f, "カナ");

        GameObject alphabetPanel = (GameObject)Instantiate(charPanelPrefab, transform.position, Quaternion.identity);
        alphabetPanel.transform.SetParent(ChangeTypePanels, false);
        alphabetPanel.GetComponent<PostTweetPanelForGoCharPanel>().Init(0f, -100f, "aA");
    }

    private void ChangeKanaType(KanaType type) {
        if (type == currentKanaType) return;
        ClearCharPanel();
        SetCharPanel(type);
    }

    public void ActionFromKeyboard(string s)
    {
        if (s == "change")
        {
            ChangeCharacter(postTweetPanel.StatusTextString.Substring(postTweetPanel.StatusTextString.Length - 1));
        }
        else if(s == "かな") {
            ChangeKanaType(KanaType.Hiragana);
        }
        else if (s == "カナ")
        {
            ChangeKanaType(KanaType.Katakana);
        }
        else if (s == "aA")
        {
            if (currentKanaType == KanaType.lowerCase) {
                ChangeKanaType(KanaType.UpperCase);
            } else {
                ChangeKanaType(KanaType.lowerCase);
            }
            
        }
        else
        {
            postTweetPanel.AddCharacter(s);
        }
    }

    private void ChangeCharacter(string s)
    {
        if (s == "" || s == null) return;

        if (kanaMarks.ContainsKey(s))
        {
            postTweetPanel.ChangeCharacter(kanaMarks[s]);
        }
        else
        {
            var key = kanaMarks.First(x => x.Value == s).Key;
            if (key == "" || key == null) return;
            postTweetPanel.ChangeCharacter(key);

        }

    }
}

public enum KanaType {
    Hiragana,
    Katakana,
    lowerCase,
    UpperCase,
    Number
}
