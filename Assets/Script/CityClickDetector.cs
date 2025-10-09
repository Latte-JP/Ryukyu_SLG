using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理のために追加

public class CityClickDetector : MonoBehaviour
{
    private CityComponent cityComponent;

    void Start()
    {
        cityComponent = GetComponent<CityComponent>();
    }

    // マウスがオブジェクト上をクリックしたときに呼び出される
    void OnMouseDown()
    {
        if (cityComponent != null)
        {
            // ★★★ 修正箇所 ★★★
            // 1. GameManagerがロードされているか確認
            if (GameManager.Instance == null)
            {
                Debug.LogError("FATAL ERROR: GameManager.InstanceがNULLです。マップシーンの起動順序やGameManagerのAwakeを確認してください。");
                return; // GameManagerがない場合、即座に処理を中断
            }
            // ★修正: CityComponentのData.cityNameではなく、cityIconの名前を直接使うなど、
            // 確実に正規化されたキー（"今帰仁"、"首里"、"大里"）を渡すようにする。
    
            // cityComponent.Data.cityName は初期データと同じ "今帰仁" のはずですが、
            // 念のため、GameManagerに渡す値が「今帰仁」「首里」「大里」のどれかであることを確認してください。

            string selectedKeyName = cityComponent.Data.cityName; // "今帰仁"

            GameManager.Instance.SetSelectedCity(selectedKeyName); 
            
            // 3.CitySceneをロード
            SceneManager.LoadScene("CityScene"); 
            // 4. ログ出力（シーン遷移後にコンソールがクリアされる可能性あり）
            Debug.Log($"城 {cityComponent.Data.cityName} が選択され、CitySceneに遷移します。");
        }
    }
}