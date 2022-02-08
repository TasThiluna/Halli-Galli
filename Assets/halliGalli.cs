using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using rnd = UnityEngine.Random;

public class halliGalli : MonoBehaviour
{
    public new KMAudio audio;
    public KMBombInfo bomb;
    public KMBombModule module;

    public KMSelectable bell;
    public GameObject[] cardParents;
    private Transform[] cardTransforms;
    private Renderer[] cardBacks;
    public Texture[] cardBackTextures;
    public Texture[] fruitTextures;
    public Renderer[] bellComponents;
    public Color goldColor;
    public TextMesh colorblindText;

    private int[] displayedFruits = new[] { -1, -1, -1 };
    private int[] displayedCounts = new[] { -1, -1, -1 };
    private int blankFruit;
    private bool bellGold;
    private int stage;
    private int solutionDigit;

    private static readonly string[] fruitNames = new[] { "strawberry", "melon", "lemon", "raspberry", "banana" };
    private static readonly string[] fruitNamesPlural = new[] { "strawberries", "melons", "lemons", "raspberries", "bananas" };
    private static readonly string[] positionNames = new[] { "left", "middle", "right" };
    private static readonly int[] table = new[] { 0, 4, 2, 3, 1, 4, 2, 0, 1, 3, 3, 0, 1, 4, 2, 1, 3, 4, 2, 0, 2, 1, 3, 0, 4 };
    private enum cardState { notMoving, flippingUp, flippingDown };
    private cardState[] cardStates = new cardState[3];
    private Coroutine cycleCoroutine;

    private static int moduleIdCounter = 1;
    private int moduleId;
    private bool moduleSolved;
    private bool activated;
#pragma warning disable 0649
    private bool TwitchPlaysActive;
#pragma warning restore 0649

    private void Awake()
    {
        moduleId = moduleIdCounter++;
        cardTransforms = cardParents.Select(card => card.transform).ToArray();
        cardBacks = cardTransforms.Select(card => card.Find("back").GetComponent<Renderer>()).ToArray();
        bell.OnInteract += delegate () { RingBell(); return false; };
        module.OnActivate += delegate () { activated = true; };
    }

    private void Start()
    {
        blankFruit = rnd.Range(0, 5);
        var cardIx = bomb.GetSerialNumberNumbers().Last() % 3;
        Debug.LogFormat("[Halli Galli #{0}] The last digit of the serial number is {1}, modulo 3 is {2}, so look at the {3} card. Blank cards represent {4}.", moduleId, bomb.GetSerialNumberNumbers().Last(), cardIx, positionNames[cardIx], fruitNamesPlural[blankFruit]);
        Texture relevantBackTexture;
        switch (blankFruit)
        {
            case 0:
                relevantBackTexture = cardBackTextures.Where(tex => tex.name.StartsWith("pink ")).PickRandom();
                break;
            case 1:
                relevantBackTexture = cardBackTextures.Where(tex => tex.name.EndsWith(" stars") && !tex.name.StartsWith("pink ") && !tex.name.StartsWith("black ")).PickRandom();
                break;
            case 2:
                relevantBackTexture = cardBackTextures.Where(tex => tex.name.StartsWith("black ")).PickRandom();
                break;
            case 3:
                relevantBackTexture = cardBackTextures.Where(tex => tex.name.EndsWith(" diamonds") && !tex.name.StartsWith("pink ") && !tex.name.StartsWith("black ")).PickRandom();
                break;
            case 4:
                relevantBackTexture = cardBackTextures.Where(tex => tex.name.EndsWith(" stripes") && !tex.name.StartsWith("pink ") && !tex.name.StartsWith("black ")).PickRandom();
                break;
            default:
                throw new Exception("blankFruit has an invalid value (expected 0-4).");
        }
        for (int i = 0; i < 3; i++)
            cardBacks[i].material.mainTexture = i == cardIx ? relevantBackTexture : cardBackTextures.PickRandom();
        bellGold = rnd.Range(0, 2) == 0;
        if (bellGold)
            foreach (Renderer component in bellComponents)
                component.material.color = goldColor;
        var colorNames = new[] { "black", "blue", "green", "pink", "red" };
        if (GetComponent<KMColorblindMode>().ColorblindModeActive)
        {
            var str = "";
            for (int i = 0; i < 3; i++)
                str += "KBGIR"[Array.IndexOf(colorNames, colorNames.First(name => cardBacks[i].material.mainTexture.name.StartsWith(name)))].ToString() + "\n";
            colorblindText.text = str;
        }
    }

    private void RingBell()
    {
        bell.AddInteractionPunch(1f);
        audio.PlaySoundAtTransform("bell", bell.transform);
        if (moduleSolved || !activated)
            return;
        if (stage == 0)
        {
            stage++;
            Debug.LogFormat("[Halli Galli #{0}] Cycle started!", moduleId);
            cycleCoroutine = StartCoroutine(Cycle());
        }
        else if (stage == 1)
        {
            if (displayedCounts.Contains(-1) || displayedCounts.Contains(-1))
            {
                Debug.LogFormat("[Halli Galli #{0}] You hit the bell when a card was face down. Strike!", moduleId);
                module.HandleStrike();
                return;
            }
            var sums = new int[5];
            for (int i = 0; i < 5; i++)
                sums[i] = Enumerable.Range(0, 3).Where(x => displayedFruits[x] == i).Select(x => displayedCounts[x]).Sum();
            if (sums.Count(x => x == 5) != 1)
            {
                Debug.LogFormat("[Halli Galli #{0}] You hit the bell when there wasn't exactly one fruit totaling to 5. Strike!", moduleId);
                module.HandleStrike();
            }
            else
            {
                var fruitWithFive = Array.IndexOf(sums, 5);
                Debug.LogFormat("[Halli Galli #{0}] You rang the bell when there were 5 {1}. Continuing the module...", moduleId, fruitNamesPlural[fruitWithFive]);
                stage++;
                var usedFruits = displayedFruits.ToArray();
                var usedCounts = displayedCounts.ToArray();
                StartCoroutine(FlipCardsBack());
                var row = 0;
                var column = 0;
                if (usedFruits.Count(x => x == fruitWithFive) == 2)
                {
                    row = usedFruits.First(x => x != fruitWithFive);
                    Debug.LogFormat("[Halli Galli #{0}] 2 cards contributed to the total of 5, so use the other displayed fruit as the row. This was a {1}.", moduleId, fruitNames[row]);
                }
                else
                {
                    row = fruitWithFive;
                    Debug.LogFormat("[Halli Galli #{0}] {1} card{2} contributed to the total of 5, so use {3} as the row.", moduleId, usedFruits.Count(x => x == fruitWithFive), usedFruits.Count(x => x == fruitWithFive) == 1 ? "" : "s", fruitNames[row]);
                }
                var contributingCounts = new List<int>();
                for (int i = 0; i < 3; i++)
                    if (usedFruits[i] == fruitWithFive && usedCounts[i] != 0)
                        contributingCounts.Add(usedCounts[i]);
                contributingCounts.Sort();
                var possibleArrangements = new[] { "5", "14", "23", "113", "122" };
                column = Array.IndexOf(possibleArrangements, contributingCounts.Join(""));
                Debug.LogFormat("[Halli Galli #{0}] The column to use is {1}.", moduleId, contributingCounts.Join(", "));
                solutionDigit = table[row * 5 + column];
                Debug.LogFormat("[Halli Galli #{0}] The digit from the table is {1}.", moduleId, solutionDigit);
                if (bellGold)
                {
                    solutionDigit = 4 - solutionDigit;
                    Debug.LogFormat("[Halli Galli #{0}] However, the bell is gold, so subtracted from 4, the new digit is {1}.", moduleId, solutionDigit);
                }
                Debug.LogFormat("[Halli Galli #{0}] Ring the bell when the last digit of the timer is {1} or {2} to solve the module.", moduleId, solutionDigit, 5 + solutionDigit);
            }
        }
        else
        {
            var submittedTime = ((int)bomb.GetTime()) % 10;
            Debug.LogFormat("[Halli Galli #{0}] Submitted when the last digit was a {1}.", moduleId, submittedTime);
            if (submittedTime % 5 == solutionDigit)
            {
                Debug.LogFormat("[Halli Galli #{0}] That is a valid digit. Module solved!", moduleId);
                module.HandlePass();
                moduleSolved = true;
                colorblindText.text = "";
            }
            else
            {
                Debug.LogFormat("[Halli Galli #{0}] That was not a valid digit. Strike!", moduleId);
                module.HandleStrike();
                Debug.LogFormat("[Halli Galli #{0}] Resetting...", moduleId);
                stage = 0;
                for (int i = 0; i < 3; i++)
                {
                    displayedFruits[i] = -1;
                    displayedCounts[i] = -1;
                }
            }
        }
    }

    private IEnumerator Cycle()
    {
        var initialOrder = Enumerable.Range(0, 3).ToList().Shuffle().ToArray();
        foreach (int ix in initialOrder)
        {
            displayedFruits[ix] = rnd.Range(0, 5);
            displayedCounts[ix] = rnd.Range(0, 6);
            if (displayedCounts[ix] == 0)
                displayedFruits[ix] = blankFruit;
            SetCardDisplay(ix);
            StartCoroutine(FlipCard(cardTransforms[ix], false));
            yield return new WaitUntil(() => cardStates[ix] == cardState.notMoving);
            yield return new WaitForSeconds(TwitchPlaysActive ? 5f : 1f);
        }
    flipAnotherCard:
        var nextCard = rnd.Range(0, 3);
        StartCoroutine(FlipCard(cardTransforms[nextCard], true));
        yield return new WaitUntil(() => cardStates[nextCard] == cardState.notMoving);
        if (stage != 1)
            yield break;
        displayedFruits[nextCard] = -1;
        displayedCounts[nextCard] = -1;
        yield return new WaitForSeconds(.3f);
        displayedFruits[nextCard] = rnd.Range(0, 5);
        displayedCounts[nextCard] = rnd.Range(0, 6);
        if (displayedCounts[nextCard] == 0)
            displayedFruits[nextCard] = blankFruit;
        SetCardDisplay(nextCard);
        StartCoroutine(FlipCard(cardTransforms[nextCard], false));
        yield return new WaitUntil(() => cardStates[nextCard] == cardState.notMoving);
        yield return new WaitForSeconds(TwitchPlaysActive ? 5f : 1f);
        if (stage == 1)
            goto flipAnotherCard;
    }

    private void SetCardDisplay(int ix)
    {
        const float cardZ = -.0011f;
        var count = displayedCounts[ix];
        var front = cardTransforms[ix].Find("front");
        for (int i = 0; i < front.childCount; i++)
            if (i != 0)
                Destroy(front.GetChild(i).gameObject);
        var originalSymbol = front.Find("fruit").gameObject;
        originalSymbol.transform.localPosition = new Vector3(0f, 0f, cardZ);
        originalSymbol.SetActive(true);
        var fruitSymbols = new List<GameObject>() { originalSymbol };
        if (count == 0)
            fruitSymbols[0].SetActive(false);
        else if (count != 1)
        {
            for (int i = 1; i < count; i++)
                fruitSymbols.Add(Instantiate(originalSymbol, front));
            switch (count)
            {
                case 2:
                    fruitSymbols[0].transform.localPosition = new Vector3(-.015f, -.025f, cardZ);
                    fruitSymbols[1].transform.localPosition = new Vector3(.015f, .025f, cardZ);
                    break;
                case 3:
                    fruitSymbols[1].transform.localPosition = new Vector3(-.015f, -.05f, cardZ);
                    fruitSymbols[2].transform.localPosition = new Vector3(.015f, .05f, cardZ);
                    break;
                case 4:
                    fruitSymbols[0].transform.localPosition = new Vector3(-.025f, .05f, cardZ);
                    fruitSymbols[1].transform.localPosition = new Vector3(.025f, .05f, cardZ);
                    fruitSymbols[2].transform.localPosition = new Vector3(-.025f, -.05f, cardZ);
                    fruitSymbols[3].transform.localPosition = new Vector3(.025f, -.05f, cardZ);
                    break;
                case 5:
                    fruitSymbols[1].transform.localPosition = new Vector3(-.025f, .05f, cardZ);
                    fruitSymbols[2].transform.localPosition = new Vector3(.025f, .05f, cardZ);
                    fruitSymbols[3].transform.localPosition = new Vector3(-.025f, -.05f, cardZ);
                    fruitSymbols[4].transform.localPosition = new Vector3(.025f, -.05f, cardZ);
                    break;
                default:
                    throw new Exception("A card had an invalid count (expected 0-5).");
            }
        }
        foreach (Renderer fruit in fruitSymbols.Select(f => f.GetComponent<Renderer>()))
            fruit.material.mainTexture = fruitTextures[displayedFruits[ix]];
    }

    private IEnumerator FlipCard(Transform card, bool flipDown)
    {
        cardStates[Array.IndexOf(cardTransforms, card)] = flipDown ? cardState.flippingDown : cardState.flippingUp;
        var elapsed = 0f;
        var duration = .1f;
        var startVector = card.localPosition;
        var end = .0382f;
        while (elapsed < duration)
        {
            card.localPosition = new Vector3(startVector.x, Easing.OutSine(elapsed, startVector.y, end, duration), startVector.z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        card.localPosition = new Vector3(startVector.x, end, startVector.z);
        elapsed = 0f;
        duration = .25f;
        var startRotation = Quaternion.Euler(new Vector3(flipDown ? 90f : -90f, -270f, 90f));
        var endRotation = Quaternion.Euler(new Vector3(flipDown ? -90f : 90f, -270f, 90f));
        while (elapsed < duration)
        {
            card.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            yield return null;
            elapsed += Time.deltaTime;
        }
        card.localRotation = endRotation;
        elapsed = 0f;
        duration = .1f;
        startVector = card.localPosition;
        end = .0157f;
        while (elapsed < duration)
        {
            card.localPosition = new Vector3(startVector.x, Easing.OutSine(elapsed, startVector.y, end, duration), startVector.z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        card.localPosition = new Vector3(startVector.x, end, startVector.z);
        audio.PlaySoundAtTransform("card", card);
        cardStates[Array.IndexOf(cardTransforms, card)] = cardState.notMoving;
    }

    private IEnumerator FlipCardsBack()
    {
        var cardsToFlipAfter = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            if (cardStates[i] == cardState.flippingUp)
            {
                cardsToFlipAfter.Add(i);
                continue;
            }
            if (displayedFruits[i] == -1 || displayedCounts[i] == -1 || cardStates[i] == cardState.flippingDown)
                continue;
            displayedFruits[i] = -1;
            displayedCounts[i] = -1;
            StartCoroutine(FlipCard(cardTransforms[i], true));
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitUntil(() => cardStates.All(state => state == cardState.notMoving));
        StopCoroutine(cycleCoroutine);
        cycleCoroutine = null;
        for (int i = 0; i < 3; i++)
        {
            if (!cardsToFlipAfter.Contains(i))
                continue;
            displayedFruits[i] = -1;
            displayedCounts[i] = -1;
            StartCoroutine(FlipCard(cardTransforms[i], true));
            yield return new WaitForSeconds(.2f);
        }
    }

    // Twitch Plays
#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} ring [Rings the bell at any time] !{0} ring <#> [Rings the bell when the last digit of the timer is #] NOTE: The amount of time between cards flipping over is increased from 1 to 5 seconds.";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string input)
    {
        input = input.Trim().ToLowerInvariant();
        var inputArray = input.Split(' ');
        var x = 0;
        if (input == "ring")
        {
            yield return null;
            bell.OnInteract();
        }
        else if (inputArray.Length == 2 && inputArray[0] == "ring" && int.TryParse(inputArray[1], out x))
        {
            if (x < 0 || x > 9)
            {
                yield return "sendtochaterror That's not a single digit.";
                yield break;
            }
            yield return null;
            while (((int)bomb.GetTime()) % 10 != x)
            {
                yield return null;
                yield return "trycancel";
            }
            bell.OnInteract();
        }
        else
            yield break;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        if (stage == 0)
        {
            yield return null;
            bell.OnInteract();
        }
        if (stage == 1)
        {
            var sums = new int[5];
            while (sums.Count(x => x == 5) != 1 || displayedCounts.Contains(-1))
            {
                for (int i = 0; i < 5; i++)
                    sums[i] = Enumerable.Range(0, 3).Where(x => displayedFruits[x] == i).Select(x => displayedCounts[x]).Sum();
                yield return true;
                yield return null;
            }
            bell.OnInteract();
        }
        if (stage == 2)
        {
            while (((int)bomb.GetTime()) % 5 != solutionDigit)
            {
                yield return true;
                yield return null;
            }
            yield return null;
            bell.OnInteract();
        }
    }
}
