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

    [Header("武将データ管理")]
    // ゲームに登場する全ての武将データ（GeneralDataアセット）を保持
    [SerializeField] public List<GeneralData> allGenerals = new List<GeneralData>();

    // ★注: このリストには、UnityエディタでGeneralDataアセットをドラッグ＆ドロップで接続する必要があります。
    [Header("軍事ユニット追跡")]
    public GameObject MilitaryUnitPrefab; // ★InspectorでMilitaryUnitPrefabを接続★
    public List<UnitController> activeUnits = new List<UnitController>();

    [Header("新しい資源とレベル")]
    public int money = 5000;  // 初期所持金
    public int food = 2000;   // 食糧資源
    public int agricultureLevel = 1;
    public int commerceLevel = 1;
    public int tradeLevel = 1;

    [Header("担当武将 (内政)")]
    // ★重要: CityUIManagerで武将を選んだら、この変数にGeneralDataのインスタンスを割り当てます
    public GeneralData currentAgricultureWarlord;
    public GeneralData currentCommerceWarlord;
    public GeneralData currentTradeWarlord;


    public void AddActiveUnit(UnitController unit)
    {
        activeUnits.Add(unit);
        Debug.Log($"アクティブ部隊数: {activeUnits.Count}");
    }

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
        // ★★★ 追記箇所: 毎秒の収入計算の開始 ★★★
        // 1秒ごとに収入計算を呼び出す
        InvokeRepeating(nameof(GenerateIncome), 1f, 1f); 
        
        // ★★★ 追記箇所: 初期武将の割り当て (デバッグ用) ★★★
        // allGeneralsリストが空でなければ、リストの最初の3人を担当に割り当てる
        if (allGenerals.Count >= 3)
        {
            currentAgricultureWarlord = allGenerals[0];
            currentCommerceWarlord = allGenerals[1];
            currentTradeWarlord = allGenerals[2];
        }
    }

    // === ターン終了処理のメソッド ===
    public void EndTurn()
    {
        // 1. 全ての城の毎月（ターン）処理を実行
        foreach (var city in allCities)
        {
            ApplyCityUpdates(city.Data);
        }
        // ★★★ ターン開始時に全アクティブユニットの移動力をリセット ★★★
        foreach (var unit in activeUnits)
        {
            unit.ResetMovement();
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

    public void GenerateIncome()
    {
        // 農業収入 (食糧) の計算
        // 知略ボーナス: 知略 / 10
        int baseFoodIncome = agricultureLevel * 10;
        int warlordFoodBonus = (currentAgricultureWarlord != null) ? (currentAgricultureWarlord.intelligence / 10) : 0; 
        food += baseFoodIncome + warlordFoodBonus;
        
        // 商業収入 (金) の計算
        // 政治力ボーナス: 政治力 / 5
        int baseMoneyFromCommerce = commerceLevel * 5;
        int warlordCommerceBonus = (currentCommerceWarlord != null) ? (currentCommerceWarlord.politics / 5) : 0; 
        
        // 交易収入 (金) の計算
        // 交易はレベルのみで計算し、武将能力は成長に影響させます
        int baseMoneyFromTrade = tradeLevel * 3;
        
        // 合計の金収入
        int totalMoneyIncome = baseMoneyFromCommerce + warlordCommerceBonus + baseMoneyFromTrade;
        money += totalMoneyIncome;

        // UIを更新 (CityUIManagerに更新を指示)
        CityUIManager uiManager = FindObjectOfType<CityUIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateCityUI(); // 既存のUI更新メソッドを呼び出す
        }
    }

    /// <summary>
    /// 交易整備時に担当武将の能力値をランダムに成長させる
    /// </summary>
    public string RandomlyImproveWarlordStat(GeneralData warlord)
    {
        if (warlord == null) return "成長担当武将が不在です。";

        int increaseAmount = Random.Range(1, 6); 
        
        // 6つの能力からランダムで1つを選択 (リフレクションを使わない簡易的な方法)
        List<string> statNames = new List<string> { "leadership", "warfare", "politics", "intelligence", "charm", "culture" };
        string selectedStat = statNames[Random.Range(0, statNames.Count)];
        
        // 反映 (switch文で実装)
        switch (selectedStat)
        {
            case "leadership": warlord.leadership = Mathf.Min(100, warlord.leadership + increaseAmount); break;
            case "warfare": warlord.warfare = Mathf.Min(100, warlord.warfare + increaseAmount); break;
            case "politics": warlord.politics = Mathf.Min(100, warlord.politics + increaseAmount); break;
            case "intelligence": warlord.intelligence = Mathf.Min(100, warlord.intelligence + increaseAmount); break;
            case "charm": warlord.charm = Mathf.Min(100, warlord.charm + increaseAmount); break;
            case "culture": warlord.culture = Mathf.Min(100, warlord.culture + increaseAmount); break;
        }

        return $"{warlord.generalName} の {selectedStat} が **+{increaseAmount}** 上昇しました！";
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
    
    /// <summary>
    /// 指定された都市に現在配置されている武将のリストを取得します。
    /// </summary>
    /// <param name="cityName">検索対象の都市名</param>
    /// <returns>その城にいる武将のリスト</returns>

    public List<GeneralData> GetGeneralsInCity(string cityName)
    {
        // 1. 検索対象のCityComponentを見つける
        CityComponent targetCity = allCities.Find(city => city.Data.cityName.Contains(cityName));

        if (targetCity == null)
        {
            Debug.LogError($"都市 '{cityName}' がallCitiesリストに見つかりません。");
            return new List<GeneralData>();
        }

        // 2. 検索対象のLocationID (Enum) を取得
        // ★このIDに基づいてフィルタリングします★
        Location targetLocationID = targetCity.Data.cityLocationID;

        // 3. 全武将リストから、LocationIDが一致する武将をフィルタリング
        // GeneralData.currentAssignedLocation (Location Enum) と targetLocationID (Location Enum) を直接比較
        return allGenerals
            .Where(g => g.currentAssignedLocation == targetLocationID)
            .ToList();
        
    }
}