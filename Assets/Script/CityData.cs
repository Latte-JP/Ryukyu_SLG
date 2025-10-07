using UnityEngine;
using System; // Enumを使うために必要

// Unityのインスペクターに表示可能にするための属性
[System.Serializable]
public class CityData
{
    // === 識別情報 ===
    public string cityName;
    public int ownerID; // 統治勢力のID (例: 1=北山, 2=中山, 3=南山, 0=中立)
    
    // === 経済・資源ステータス ===
    public int population;       // 人口
    public float populationGrowthRate; // 人口増加率 (例: 0.05 = 5%増加)
    public int foodStock;        // 保有する食糧
    public int goldStock;        // 保有する金
    
    // === 統治・文化ステータス (内政システムの核) ===
    [Range(0, 100)] // 0から100の範囲でスライダー表示
    public int cultureValue;     // 文化・伝統度 (旧: 民からの信頼)
    public int agricultureLevel; // 農業レベル (食糧収入に影響)
    public int commerceLevel;    // 商業レベル (金収入に影響)
    public int roadNetworkLevel; // 海路/陸路整備レベル (移動速度、交易リスクに影響)
    
    // === 防衛ステータス ===
    public int fortressDurability; // 城の耐久度 (旧: 城の耐久度)

    // === 軍事ステータス ===
    // 兵種ごとのデータを保持するための構造体を定義
    public TroopData swordTroops;
    public TroopData spearTroops;
    public TroopData archerTroops;
    public TroopData navyTroops; // 海人隊

    // コンストラクタ（初期値を設定するメソッド）
    public CityData(string name, int owner, int pop, int food, int gold)
    {
        cityName = name;
        ownerID = owner;
        population = pop;
        foodStock = food;
        goldStock = gold;
        
        // 初期値の設定
        cultureValue = 50;
        agricultureLevel = 1;
        commerceLevel = 1;
        roadNetworkLevel = 0;
        fortressDurability = 100;
        
        // 兵種データの初期化
        swordTroops = new TroopData();
        spearTroops = new TroopData();
        archerTroops = new TroopData();
        navyTroops = new TroopData();
    }

    // 毎ターンの処理などをここに追加する（例: FoodStock += agricultureLevel * 10）
    public void UpdateCityStatus()
    {
        // ...
    }
}

// 兵種ごとの詳細データを保持する構造体
[System.Serializable]
public class TroopData
{
    public int count;    // 兵数
    [Range(0, 100)]
    public int morale;   // 士気
    [Range(0, 100)]
    public int training; // 訓練度

    public TroopData()
    {
        count = 0;
        morale = 50;
        training = 0;
    }
}