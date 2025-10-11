using UnityEngine;

// Unityメニューの [Assets] -> [Create] -> [Game Data] -> [General Data] からデータを作成可能にする
[CreateAssetMenu(fileName = "NewGeneral", menuName = "Game Data/General Data")]
public class GeneralData : ScriptableObject
{
    [Header("基本情報")]
    public string generalName = "新武将";
    // public string allegiance = "中山"; // ★★★ これを削除またはコメントアウト ★★★
    // ★★★ 修正箇所2：Faction 型の allegiance に置き換え ★★★
    public Faction allegiance = Faction.Chuzan; 
    
    [Header("能力値 (1〜100)")]
    // [Range]属性により、Inspectorでスライダーで値を調整可能になる
    [Range(1, 100)]
    public int leadership = 50;    // 統率力 (戦闘時の兵数)    
    [Range(1, 100)]
    public int warfare = 50;    // 武力 (戦闘時の攻撃力・防御力)
    [Range(1, 100)]
    public int politics = 50;   // 政治力 (内政行動の効率)
    [Range(1, 100)]
    public int intelligence = 50; // 知略 (交易成功率、計略成功率)
    [Range(1, 100)]
    public int charm = 50;    // 魅力 (登用の成功率)
    [Range(1, 100)]
    public int culture = 50;    // 文化力 (技術獲得確率、文化度成長)

    [Header("特殊技能")]
    public SpecialSkill skill; // スキルを保有
}

// 特殊技能の列挙型 (Enum)
public enum SpecialSkill
{
    None,
    MasterTrader,       // 交易効率・成功率アップ
    MasterStrategist,   // 計略・情報戦に特化
    LandReformer,       // 農業・内政に特化
    NavalCommander,     // 海人隊の指揮に特化
    IronGunMaster       // 鉄砲隊の訓練・戦闘に特化
}
public enum Faction
{
    None,       // 在野（浪人）
    Neutral,    // 中立（勢力に属さない中立の按司など）
    Hokuzan,    // 北山
    Chuzan,     // 中山
    Nanzan      // 南山
}