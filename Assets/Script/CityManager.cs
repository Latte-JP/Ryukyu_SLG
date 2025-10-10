using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Dictionary操作のために必要
using UnityEngine.SceneManagement;

public class CityManager : MonoBehaviour
{
    public GameObject cityPrefab;
    private readonly Vector3 ICON_SCALE = new Vector3(0.3f, 0.3f, 0.3f);

    // ★修正点1: 位置情報とデータをDictionaryで結合する新しい構造★
    // データを渡すためのシンプルな構造体
    private struct CitySetup
    {
        public Vector3 location;
        public CityData data;
    }

    // 城の初期データと座標を一つの構造で管理
    private readonly Dictionary<string, CitySetup> cityConfigurations = new Dictionary<string, CitySetup>()
    {
        // キー: 内部名, 値: (座標, CityData)
        {"今帰仁", new CitySetup{ location = new Vector3(-0.34f, 0.1f, 2.34f), 
                                  data = new CityData("今帰仁", 1, 15000, 5000, 3000)
                                 { backgroundSceneID = "IMAKIJIN_GUSUKU" }} },
        {"首里", new CitySetup{ location = new Vector3(-3.26f, 0.1f, -2.6f), 
                                 data = new CityData("首里", 2, 30000, 8000, 5000)
                                 { backgroundSceneID = "SHURI_CASTLE" } } },
        {"大里", new CitySetup{ location = new Vector3(-3.75f, 0.1f, -3.83f), 
                                 data = new CityData("大里", 3, 10000, 4000, 2500)
                                 { backgroundSceneID = "OSATO_CASTLE" } } },
    };

    void Start()
    {
        InstantiateCities();
    }

    void InstantiateCities()
    {
        if (GameManager.Instance != null)
        {
            // ★修正点2: GameManagerのリストを最初にクリア★
            GameManager.Instance.allCities.Clear(); 
        }

        // ★修正点3: 一つのループで生成、データ割り当て、登録を完了★
        foreach (var config in cityConfigurations)
        {
            // 1. アイコンのプレファブを生成 (config.Value.locationを使用)
            GameObject cityIcon = Instantiate(cityPrefab, config.Value.location, Quaternion.identity);
            
            // 2. 親オブジェクトを設定
            cityIcon.transform.SetParent(transform);
            cityIcon.transform.localScale = ICON_SCALE;

            // 3. オブジェクトに名前を設定
            cityIcon.name = "City: " + config.Key; // 例: "City: 今帰仁"

            // 4. CityComponentを取得し、データを初期化 (config.Value.dataを使用)
            CityComponent cityComp = cityIcon.AddComponent<CityComponent>();
            cityComp.InitializeCity(config.Value.data);

            // 5. GameManagerに登録
            if (GameManager.Instance != null)
            {
                GameManager.Instance.allCities.Add(cityComp);
            }
        }
        
        Debug.Log($"[CityManager] 全{cityConfigurations.Count}個の城の生成と登録が完了しました。");

        // ★修正点4: 不要なFindObjectsOfTypeを削除 (上記で全て登録済みのため)
    }
}