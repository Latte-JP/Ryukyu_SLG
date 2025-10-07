using UnityEngine;
using System.Collections.Generic;

public class CityManager : MonoBehaviour
{
    // Inspectorから設定できるように、城のアイコンプレファブを公開
    public GameObject cityPrefab;
    // 【POINT】: 希望のスケールをここで定義しておくと便利
    private readonly Vector3 ICON_SCALE = new Vector3(0.3f, 0.3f, 0.3f);

    // 城の名前とマップ上の座標を定義 (沖縄本島の主要な場所を仮で設定)
    private readonly Dictionary<string, Vector3> cityLocations = new Dictionary<string, Vector3>()
    {
        // 座標はTerrainの中心を(0, 0, 0)として、SLGの縮尺に合わせて調整してください
        {"北山 (今帰仁)", new Vector3(-0.34f, 0.1f, 2.34f)}, // 仮の座標
        {"中山 (浦添/首里)", new Vector3(-3.26f, 0.1f, -2.6f)},     // 仮の座標
        {"南山 (島尻)", new Vector3(-3.75f, 0.1f, -3.83f)}      // 仮の座標
    };

    // 城の初期データを定義
    private readonly Dictionary<string, CityData> initialCityData = new Dictionary<string, CityData>()
    {
        // (名前, 勢力ID, 人口, 食糧, 金)
        {"北山 (今帰仁)", new CityData("今帰仁", 1, 15000, 5000, 3000)},
        {"中山 (首里)",   new CityData("首里",   2, 30000, 8000, 5000)},
        {"南山 (島尻)",   new CityData("大里",   3, 10000, 4000, 2500)}
    };

    void Start()
    {
        InstantiateCities();
    }

    void InstantiateCities()
    {
        // 定義された全ての座標に城アイコンを配置
        foreach (var location in cityLocations)
        {
            // 1. アイコンのプレファブを生成
            GameObject cityIcon = Instantiate(cityPrefab, location.Value, Quaternion.identity);
            
            // 2. 親オブジェクトを設定（シーンを整理するため）
            cityIcon.transform.SetParent(transform);

            // 【⭐ 修正点】: 親子設定後、ローカルスケールを強制的にリセットする
            // これにより、親のスケールに関係なく、アイコンのサイズがこの値に固定されます。
            cityIcon.transform.localScale = ICON_SCALE; // 1, 1, 1 に設定

            // 3. オブジェクトに城の名前を設定（デバッグやUI接続のため）
            cityIcon.name = location.Key;

            // *追加* 将来的にこのオブジェクトに城のステータス（人口、金など）を管理する
            // 独自のコンポーネントを追加します。
        }
        foreach (var dataEntry in initialCityData)
        {
            // 1. アイコンのプレファブを生成
            GameObject cityIcon = Instantiate(cityPrefab, new Vector3(
                // X 座標 (float)
                dataEntry.Value.cityName switch {
                    "今帰仁" => -0.34f,
                    "首里" => -3.26f,
                    "大里" => -3.75f,
                    _ => 0f
                }, 
                // Y 座標 (float)
                0.1f, 
                // Z 座標 (float) <- ここで new Vector3(...) を削除する
                dataEntry.Value.cityName switch {
                    "今帰仁" => 2.34f,
                    "首里" => -2.6f,
                    "大里" => -3.83f,
                    _ => 0f
                }
            ), Quaternion.identity); // <- ここが Vector3 の閉じ括弧
            
            // 2. CityComponentを取得し、データを初期化
            CityComponent cityComp = cityIcon.AddComponent<CityComponent>();
            cityComp.InitializeCity(dataEntry.Value);
        }
    }
}