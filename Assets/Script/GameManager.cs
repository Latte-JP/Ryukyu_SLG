using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Listの操作に必要

public class GameManager : MonoBehaviour
{
    // シングルトン化のための静的変数
    public static GameManager Instance { get; private set; }
    public string selectedCityName; // 選択された城の名前を保持

    [Header("ゲーム情報")]
    public int currentTurn = 1;
    public int currentYear = 1400; // 例: 三山時代の開始年
    public int turnPerYear = 12; // 1年を12ターン（月）とする
    
    // 全ての城コンポーネントを管理するためのリスト
    public List<CityComponent> allCities = new List<CityComponent>();

    void Awake()
    {
        // シングルトン設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初期化時に全てのCityComponentをリストに登録
        //allCities = FindObjectsOfType<CityComponent>().ToList();
        Debug.Log($"ゲーム開始。管理対象の城: {allCities.Count} 箇所");
    }

    // === ターン終了処理のメソッド ===
    public void EndTurn()
    {
        // 1. 全ての城の毎月（ターン）処理を実行
        foreach (var city in allCities)
        {
            ApplyCityUpdates(city.Data);
        }

        // 2. ターンを進行
        currentTurn++;
        if (currentTurn > turnPerYear)
        {
            currentTurn = 1;
            currentYear++;
            Debug.Log($"新しい年 {currentYear} が始まりました！");
        }

        Debug.Log($"ターン {currentTurn} が終了しました。");

        // 3. UIの更新通知（後述のUIシステムで利用）
        // UIManager.Instance.UpdateUI(); 
    }

    // 城のステータス更新ロジック
    private void ApplyCityUpdates(CityData data)
    {
        // 統率力特化による維持ボーナス
        float maintenanceBonus = 0f;
        if (data.governingGeneral != null)
        {
            // 政治力と文化力を利用したボーナスを計算
            maintenanceBonus = (data.governingGeneral.politics * 0.0005f) + (data.governingGeneral.culture * 0.0002f);
        }
        // 資源の自動増減
        data.foodStock += data.agricultureLevel * 10 - data.population / 100; // 農業レベルと人口消費
        data.goldStock += data.commerceLevel * 5 - data.unitCount1 / 50 - data.unitCount2 / 50 - data.unitCount3 / 50; // 商業レベルと兵の維持費

        // 人口増加
        data.population = Mathf.RoundToInt(data.population * (1f + data.populationGrowthRate));

        // TODO: 士気低下、文化度変動、疫病・イベント判定などをここに追加
        // 訓練度と士気の自然な低下（ボーナスで軽減）
        data.trainingLevel1 = Mathf.Max(0, data.trainingLevel1 - (int)(2 * (1f - maintenanceBonus)));
        data.morale1 = Mathf.Max(0, data.morale1 - (int)(1 * (1f - maintenanceBonus)));
        data.trainingLevel2 = Mathf.Max(0, data.trainingLevel2 - (int)(2 * (1f - maintenanceBonus)));
        data.morale2 = Mathf.Max(0, data.morale2 - (int)(1 * (1f - maintenanceBonus)));
        data.trainingLevel3 = Mathf.Max(0, data.trainingLevel3 - (int)(2 * (1f - maintenanceBonus)));
        data.morale3 = Mathf.Max(0, data.morale3 - (int)(1 * (1f - maintenanceBonus)));
        // 効果：政治力100の武将がいると、低下率が大幅に抑制されるため、毎ターンの訓練/交流の負担が軽減される。

        // 交易リスクイベント判定
    if (data.tradeRiskFactor > 0.1f)
    {
        float baseRisk = data.tradeRiskFactor * 10f; // リスクを確率に変換 (例: factor 0.3 = 3%リスク)
    
        // ★★★ 修正箇所：水軍によるリスク軽減 ★★★
        // 水軍兵数が多ければリスクを軽減（例: 海人隊1000でリスク-1.0%）
        float mitigation = data.unitType3 == "海人隊" ? data.unitCount3 / 1000f : 0f;
    
        float finalRisk = Mathf.Max(0f, baseRisk - mitigation); 

        if (Random.Range(0f, 100f) < finalRisk)
        {
            // 海賊イベント発生
            int loss = data.goldStock / 10;
            data.goldStock -= loss;
            data.tradeRiskFactor = 0f; // リスクをリセット
        
            Debug.Log($"海賊襲撃！{data.cityName} の交易路が襲われ、金 {loss} を失いました。");
        }
    }
    }
    public void SetSelectedCity(string cityName)
    {
    selectedCityName = cityName;
    }

    public CityComponent GetSelectedCityComponent()
    {
    // selectedCityNameを使用して、allCitiesリストから該当するCityComponentを検索して返す
    
    // ★修正：完全一致ではなく、部分一致または Contains を使用してロバスト化 ★
    // 例: "中山 (首里)" などの複雑な名前の場合でも、"首里"というデータを持っていれば検索できる
    return allCities.Find(city => city.Data.cityName.Contains(selectedCityName));

    // もし selectedCityName が "首里" なら、cityNameに "首里"を含むオブジェクトを探す
    }
}