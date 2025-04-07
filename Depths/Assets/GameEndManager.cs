using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance { get; private set; }

    [SerializeField] PlayerController playerController;
    [SerializeField] DrillShip drillShip;
    [SerializeField] DrillShipAnimation drillShipAnimations;
    [SerializeField] Image _fadeImage;
    [SerializeField] Color _fadeInColour, _fadeInTextColour;
    [SerializeField] TextMeshProUGUI _fadeTextOne, _fadeTextTwo, _fadeTextThree;
    [SerializeField] string _textOne, _textTwo, _textThree;
    [SerializeField] Button _returnToMenuButton;

    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip[] _typingClips;
    private bool _gameOverTriggered = false;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void TriggerGameEndSequence()
    {
        if (_gameOverTriggered) return;

        _gameOverTriggered = true;
        playerController.GameOver();
        drillShip.GameOver();
        drillShipAnimations.GameOver();

        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        float timer = 0;
        Color color = _fadeImage.color;
        while (timer <= 1f)
        {
            timer += Time.deltaTime;
            _fadeImage.color = Color.Lerp(color, _fadeInColour, timer/1);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(TypeText(_fadeTextOne, _textOne, 0.05f));

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(TypeText(_fadeTextTwo, _textTwo, 0.05f));

        yield return new WaitForSeconds(3f);


        timer = 0;
        color = _fadeTextTwo.color;
        while (timer <= 1f)
        {
            timer += Time.deltaTime;
            _fadeTextTwo.color = Color.Lerp(color, _fadeInColour, timer / 1f);
            _fadeTextOne.color = Color.Lerp(color, _fadeInColour, timer / 1f);
            yield return null;
        }

        yield return StartCoroutine(TypeText(_fadeTextThree, _textThree, 0.1f));
        _returnToMenuButton.gameObject.SetActive(true);
        _returnToMenuButton.interactable = true;

        // _fadeTextOne.color = _fadeInColour;
        // yield return new WaitForSeconds(1f);

        // timer = 0;
        // color = _fadeTextTwo.color;
        // while (timer <= 2f)
        // {
        //     timer += Time.deltaTime;
        //     _fadeTextTwo.color = Color.Lerp(color, _fadeInColour, timer / 1);
        //     yield return null;
        // }

        // _fadeTextTwo.color = _fadeInColour;
        //yield return new WaitForSeconds(10f);

        // timer = 0;
        // color = _fadeTextThree.color;
        // while (timer <= 3f)
        // {
        //     timer += Time.deltaTime;

        //     _fadeTextOne.color = Color.Lerp(_fadeInColour, color, timer / 3);
        //     _fadeTextTwo.color = Color.Lerp(_fadeInColour, color, timer / 3);
        //     _fadeTextThree.color = Color.Lerp(color, _fadeInColour, timer / 3);
        //     yield return null;
        // }

        // _fadeTextThree.color = _fadeInColour;
        // yield return new WaitForSeconds(0.25f);


        // _returnToMenuButton.gameObject.SetActive(true);
        // _returnToMenuButton.interactable = true;
    }


    public IEnumerator TypeText(TMPro.TMP_Text textComponent, string fullText, float charDelay = 0.05f)
    {
        textComponent.text = ""; // Clear existing
        for (int i = 0; i < fullText.Length; i++)
        {
            if (i % 3 == 0)
            {
                _source.pitch = Random.Range(0.9f, 1.1f);
                _source.Play();
            }

            textComponent.text += fullText[i];
            yield return new WaitForSeconds(charDelay);
        }
    }
}
