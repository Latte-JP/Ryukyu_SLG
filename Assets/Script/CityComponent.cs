using UnityEngine;

public class CityComponent : MonoBehaviour
{
    // 1. CityDataのインスタンスを保持
    [SerializeField] 
    public CityData Data;

    public void InitializeCity(CityData initialData)
    {
        Data = initialData;
        gameObject.name = "City: " + Data.cityName;
    }

    // 内政行動の実行（農業開拓）
    public void PerformAgricultureAction(int cost, int effect)
    {
        if (Data.goldStock >= cost)
        {
            Data.goldStock -= cost;
            Data.agricultureLevel += effect;
            Debug.Log($"{Data.cityName}で農業開拓が実行されました。金消費: {cost}、農業レベル: {Data.agricultureLevel}");
        }
        else
        {
            Debug.Log($"金が足りません。農業開拓を実行できませんでした。");
        }
    }
    
    // 内政行動の実行（市場開拓／商業）
    public string PerformCommerceAction(int cost, int effect)
    {
        if (Data.goldStock >= cost)
        {
            Data.goldStock -= cost;
            Data.commerceLevel += effect;
            return $"{Data.cityName}で市場開拓を実行。金消費: {cost}、商業レベル: {Data.commerceLevel}";
        }
        else
        {
            return $"金が足りません。市場開拓を実行できませんでした。";
        }
    }
    
    // 内政行動の実行（港整備／交易）
    public string PerformTradeAction(int cost, int effect)
    {
        if (Data.goldStock >= cost)
        {
            Data.goldStock -= cost;
            Data.tradeLevel += effect;
            return $"{Data.cityName}で港整備を実行。金消費: {cost}、交易レベル: {Data.tradeLevel}";
        }
        else
        {
            return $"金が足りません。港整備を実行できませんでした。";
        }
    }

    // ----------------------------------------------------
    // 兵種を指定して訓練を実行
    // ----------------------------------------------------
    public string PerformTraining(int troopIndex, int goldCost, int effect)
    {
        // 兵種特化による訓練効率ボーナスを計算
        float efficiencyBonus = 1.0f;
        if (Data.governingGeneral != null)
        {
            float bonus = Data.governingGeneral.leadership * 0.002f;
            if (troopIndex == 1 || troopIndex == 2 || troopIndex == 3)
            {
                efficiencyBonus += bonus;
            }
        }
        if (Data.goldStock < goldCost)
        {
            return "ERROR: 資金不足";
        }

        // 実際の訓練効果を計算
        int finalEffect = (int)(effect * efficiencyBonus);

        // 参照ポインタの宣言と割り当て
        ref int targetTraining = ref Data.trainingLevel1;
        ref int targetMorale = ref Data.morale1;
        string unitName = "";

        // どの兵種を対象にするか、インデックスで分岐
        switch (troopIndex)
        {
            case 1:
                targetTraining = ref Data.trainingLevel1;
                targetMorale = ref Data.morale1;
                unitName = Data.unitType1;
                break;
            case 2:
                targetTraining = ref Data.trainingLevel2;
                targetMorale = ref Data.morale2;
                unitName = Data.unitType2;
                break;
            case 3:
                targetTraining = ref Data.trainingLevel3;
                targetMorale = ref Data.morale3;
                unitName = Data.unitType3;
                break;
            default:
                return "ERROR: 不正な兵種インデックス";
        }

        Data.goldStock -= goldCost;

        // 訓練度の更新 (最大100まで)
        targetTraining = Mathf.Min(100, targetTraining + finalEffect);

        // 訓練度の上昇は士気を一時的に低下させる (トレードオフを再導入)
        targetMorale = Mathf.Max(0, targetMorale - (finalEffect / 2)); 

        return $"{unitName}の訓練を実行。訓練度: {targetTraining} (+{finalEffect})。士気は{targetMorale}に低下。";
    }

    // ----------------------------------------------------
    // 【新しいメソッド】兵種を指定して交流を実行
    // ----------------------------------------------------
    public string PerformMoraleBoost(int troopIndex, int goldCost, int effect)
    {
        // 兵種特化による士気効率ボーナスを計算 (武力に依存)
        float efficiencyBonus = 1.0f;
        if (Data.governingGeneral != null)
        {
            float bonus = Data.governingGeneral.warfare * 0.002f;
            if (troopIndex == 1 || troopIndex == 2 || troopIndex == 3)
            {
                efficiencyBonus += bonus;
            }
        }
            
        if (Data.goldStock < goldCost)
        {
            return "ERROR: 資金不足";
        }

        // 実際の士気効果を計算
        int finalEffect = (int)(effect * efficiencyBonus);

        // 参照ポインタの宣言と割り当て
        ref int targetTraining = ref Data.trainingLevel1;
        ref int targetMorale = ref Data.morale1;
        string unitName = "";

        // どの兵種を対象にするか、インデックスで分岐
        switch (troopIndex)
        {
            case 1:
                targetTraining = ref Data.trainingLevel1;
                targetMorale = ref Data.morale1;
                unitName = Data.unitType1;
                break;
            case 2:
                targetTraining = ref Data.trainingLevel2;
                targetMorale = ref Data.morale2;
                unitName = Data.unitType2;
                break;
            case 3:
                targetTraining = ref Data.trainingLevel3;
                targetMorale = ref Data.morale3;
                unitName = Data.unitType3;
                break;
            default:
                return "ERROR: 不正な兵種インデックス";
        }

        Data.goldStock -= goldCost;

        // 士気の更新 (最大100まで)
        targetMorale = Mathf.Min(100, targetMorale + finalEffect);

        // ★★★ 修正箇所: 交流の上昇は、訓練度を低下させるトレードオフを再導入 ★★★
        targetTraining = Mathf.Max(0, targetTraining - (finalEffect / 3));

        return $"{unitName}との交流を実行。士気: {targetMorale} (+{finalEffect})。訓練度は{targetTraining}に低下。";
    }
    
    // ----------------------------------------------------
    // 【新しいメソッド】兵種を指定して募兵を実行
    // ----------------------------------------------------
    public string PerformRecruitment(int troopIndex, int goldCost, int foodCost, int recruitAmount)
    {
        // 参照ポインタの宣言と割り当て
        ref int targetCount = ref Data.unitCount1;
        ref int targetMorale = ref Data.morale1;
        ref int targetTraining = ref Data.trainingLevel1;
        string unitName = "";

        // どの兵種を対象にするか、インデックスで分岐
        switch (troopIndex)
        {
            case 1:
                targetCount = ref Data.unitCount1;
                targetMorale = ref Data.morale1;
                targetTraining = ref Data.trainingLevel1;
                unitName = Data.unitType1;
                break;
            case 2:
                targetCount = ref Data.unitCount2;
                targetMorale = ref Data.morale2;
                targetTraining = ref Data.trainingLevel2;
                unitName = Data.unitType2;
                break;
            case 3:
                targetCount = ref Data.unitCount3;
                targetMorale = ref Data.morale3;
                targetTraining = ref Data.trainingLevel3;
                unitName = Data.unitType3;
                break;
            default: return "ERROR: 不正な兵種インデックス";
        }

        // 1. コストと資源のチェック
        int populationCost = recruitAmount;

        if (Data.goldStock < goldCost || Data.foodStock < foodCost || Data.population < populationCost)
        {
            string reason = "";
            if (Data.goldStock < goldCost) reason += "金不足, ";
            if (Data.foodStock < foodCost) reason += "食糧不足, ";
            if (Data.population < populationCost) reason += "人口不足";
            // ... (reasonの整形ロジック)
            return $"ERROR: 募兵に必要な資源が不足しています ({reason})。";
        }
        
        // 2. コストの消費
        Data.goldStock -= goldCost;
        Data.foodStock -= foodCost;
        Data.population -= populationCost;

        // 3. 兵数の増加
        targetCount += recruitAmount;
        
        // 訓練度への影響：募兵数が多いほど、訓練度が低下する。
        int trainingLoss = recruitAmount / 50;
        targetTraining = Mathf.Max(0, targetTraining - trainingLoss);

        // 士気への影響：募兵数が多いほど、士気が低下する。
        int moraleLoss = recruitAmount / 50;
        targetMorale = Mathf.Max(0, targetMorale - moraleLoss);
        
        return $"{unitName}の募兵を実行。兵数 +{recruitAmount}。現在兵数: {targetCount}。士気と訓練が低下しました。";
    }

    // ----------------------------------------------------
    // 【新しいメソッド】出陣する部隊を編成し、戦闘用データとして返す
    // ----------------------------------------------------
    public TroopData DeployTroop(GeneralData general, int troopIndex, int troopCount)
    {
        // 1. 資源と兵数のチェック
        int currentUnitCount;
        ref int targetCount = ref Data.unitCount1;
        ref int targetTraining = ref Data.trainingLevel1;
        ref int targetMorale = ref Data.morale1;
        string unitName = "";
        int aptitude = 5; // 初期適性はC

        // どの兵種を対象にするか、インデックスで分岐
        switch (troopIndex)
        {
            case 1:
                targetCount = ref Data.unitCount1;
                targetTraining = ref Data.trainingLevel1;
                targetMorale = ref Data.morale1;
                unitName = Data.unitType1;
                aptitude = general.aptitudeSword;
                break;
            case 2:
                targetCount = ref Data.unitCount2;
                targetTraining = ref Data.trainingLevel2;
                targetMorale = ref Data.morale2;
                unitName = Data.unitType2;
                aptitude = general.aptitudeBow;
                break;
            case 3:
                targetCount = ref Data.unitCount3;
                targetTraining = ref Data.trainingLevel3;
                targetMorale = ref Data.morale3;
                unitName = Data.unitType3;
                aptitude = general.aptitudeMarine;
                break;
            default:
                return null; // 不正な兵種インデックス
        }

        currentUnitCount = targetCount;

        if (troopCount <= 0 || troopCount > currentUnitCount)
        {
            Debug.LogError("編成可能な兵数が不足しています。");
            return null;
        }

        // 2. 都市の兵数を減らす
        targetCount -= troopCount;

        // 3. 部隊の能力値を計算し、新しいTroopDataオブジェクトを作成
        TroopData newTroop = new TroopData();
        newTroop.unitName = unitName;
        newTroop.count = troopCount;
        newTroop.training = targetTraining; // 編成時、都市の訓練度を引き継ぐ
        newTroop.morale = targetMorale;     // 編成時、都市の士気を引き継ぐ
        newTroop.general = general;         // 武将情報を引き継ぐ
        newTroop.aptitude = aptitude;       // 兵科適性

        // 4. 武将の能力値を部隊の攻防力に反映（ここは後で詳細な計算式を導入）
        newTroop.attack = (int)(general.warfare * 0.5f + general.leadership * 0.2f);
        newTroop.defense = (int)(general.leadership * 0.4f + general.warfare * 0.3f); // 統率力優先
        
        // 兵種特化（適性）による乗算ボーナス
        // S(1) = 1.2倍, A(2) = 1.1倍, B(3) = 1.0倍, C(4) = 0.9倍, D(5) = 0.8倍
        float aptitudeMultiplier = 1.0f + (3 - aptitude) * 0.1f;
        newTroop.attack = (int)(newTroop.attack * aptitudeMultiplier);
        newTroop.defense = (int)(newTroop.defense * aptitudeMultiplier);

        Debug.Log($"{unitName}を{general.generalName}が率いる部隊として{troopCount}で編成しました。");
        return newTroop;
    }
    /// <summary>
    /// 部隊が解散した際、兵を都市の備蓄に戻す。
    /// </summary>
    public void ReturnTroops(TroopData troop)
    {
        // 部隊が使用していた兵種インデックスに応じて都市の兵数を増加させる
        switch (troop.troopTypeIndex)
        {
            case 1: Data.unitCount1 += troop.unitCount; break;
            case 2: Data.unitCount2 += troop.unitCount; break;
            case 3: Data.unitCount3 += troop.unitCount; break;
            // ... 他の兵種があれば追加
        }
        
        // 兵種を戻したので、UIを更新する必要があるかもしれません (TODO)
        Debug.Log($"都市 {Data.cityName} に {troop.unitCount} の {troop.unitName} が戻されました。");
    }
}