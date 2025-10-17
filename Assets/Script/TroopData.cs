using UnityEngine;
using System; // Serializableのために必要

// 戦闘時に使用する部隊データ構造体
[System.Serializable] // ★重複しないように、クラスの直前で一度だけ宣言★
public class TroopData
{
    // リーダー情報のためにGeneralDataを参照できるようにします
    public GeneralData general;  // 部隊を率いる武将

    // 基本情報
    public string unitName;     // 兵種名
    public int count;           // 兵数
    public int training;        // 訓練度 (0-100)
    public int morale;          // 士気 (0-100)

    // 戦闘能力値
    public int attack;          // 攻撃力
    public int defense;         // 防御力
    
    // 兵科適性
    public int aptitude;        // 兵科適性 (1:S, 2:A, 3:B, 4:C, 5:D, 6:E)

    // TroopData コンストラクタ（CityData内で new する際に必要であれば残す）
    public TroopData()
    {
        // 初期化ロジック
    }
}