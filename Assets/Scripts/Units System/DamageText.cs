// 데미지 숫자
using UnityEngine;

static class DamageText
{
    public static void Spawn(int amount, Vector3 pos)
    {
        var go = new GameObject("DMG");
        go.transform.position = pos;

        var tm = go.AddComponent<TextMesh>();
        tm.text = amount.ToString();
        tm.characterSize = 0.2f;
        tm.anchor = TextAnchor.MiddleCenter;

        go.AddComponent<Life>();
    }

    class Life : MonoBehaviour
    {
        float t;
        void Update()
        {
            t += Time.deltaTime;
            transform.position += Vector3.up * 1.2f * Time.deltaTime;
            if (Camera.main) transform.forward = Camera.main.transform.forward;
            if (t > 0.8f) Destroy(gameObject);
        }
    }
}
