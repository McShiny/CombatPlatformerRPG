using UnityEngine;
using UnityEngine.UI;

public class PlayerCooldownsUI : MonoBehaviour
{

    [SerializeField] private Image dashCooldownImage;

    private void Start() {
        Player.Instance.OnPlayerDashed += Player_OnPlayerDashed;

        dashCooldownImage.fillAmount = 1f;
    }

    private void Player_OnPlayerDashed(object sender, Player.OnPlayerDashedEventArgs e) {
        dashCooldownImage.fillAmount = e.progressNormalized;
    }
}
