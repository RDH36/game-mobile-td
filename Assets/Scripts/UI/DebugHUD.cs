using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    private EnemySpawner _spawner;
    private EnemyCounterAttack _counterAttack;

    void Start()
    {
        _spawner = FindFirstObjectByType<EnemySpawner>();
        _counterAttack = FindFirstObjectByType<EnemyCounterAttack>();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold
        };

        float y = Screen.height - 50f;

        // Game State
        if (GameManager.Instance != null)
        {
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(10, y, 400, 30), $"State: {GameManager.Instance.CurrentState}", style);
            y -= 30f;
        }

        // Enemies + counter-attack
        if (_spawner != null && _spawner.AliveCount > 0)
        {
            string counterTxt = _counterAttack != null && _counterAttack.IsCounterAttacking ? " [COUNTER]" : "";
            style.normal.textColor = new Color(1f, 0.5f, 0f);
            GUI.Label(new Rect(10, y, 400, 30), $"Enemies: {_spawner.AliveCount}{counterTxt}", style);
        }
    }
}
