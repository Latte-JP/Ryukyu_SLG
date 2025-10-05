using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使うために必要

public class SceneLoaderOnClick : MonoBehaviour
{
    void Update()
    {
        // マウスの左ボタンがクリックされた瞬間を検知
        if (Input.GetMouseButtonDown(0))
        {
            // カメラからマウスカーソルの位置へRay（光線）を飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Rayが何かに当たったかどうかを判定
            if (Physics.Raycast(ray, out hit))
            {
                // 当たったオブジェクトに "Castle" というタグが付いているか確認
                if (hit.collider.tag == "Castle")
                {
                    Debug.Log("お城がクリックされました！"); // 確認用メッセージ
                    
                    // 城下町シーンをロードする
                    SceneManager.LoadScene("CastleTownScene");
                }
            }
        }
    }
}