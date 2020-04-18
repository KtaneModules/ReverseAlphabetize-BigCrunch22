using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class ReverseAlphabetizeScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
	
	public AudioClip[] SFX;

    public KMSelectable LeftQuad;
    public KMSelectable RightQuad;
	
	public TextMesh TheLetter;
	public TextMesh TheFiber;
	public TextMesh TheTetra;
	public TextMesh TheHex;
	
	private bool Playable = false;
	
	private int Computer = 0;

    private int TheCopperValue = 0;
	private int TheFoil = 25;
	private int Supper = 0;
	
	private int Style = 0;

	private int[] SilverLine;
	
	private string[] Alphabreak = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};

	
	private string[][] TheSequence = new string[16][]{
		new string[26] {"N", "Z", "Y", "I", "F", "S", "U", "J", "W", "B", "D", "G", "V", "C", "A", "H", "M", "X", "T", "K", "L", "Q", "E", "P", "O", "R"},
		new string[26] {"A", "O", "X", "B", "R", "Y", "G", "H", "W", "F", "N", "L", "D", "M", "J", "Q", "V", "Z", "S", "K", "C", "T", "U", "P", "E", "I"},
		new string[26] {"Z", "P", "D", "Y", "V", "K", "A", "U", "Q", "W", "M", "C", "T", "L", "X", "J", "N", "H", "S", "G", "O", "F", "E", "I", "R", "B"},
		new string[26] {"R", "Y", "C", "B", "E", "N", "F", "Z", "V", "Q", "T", "S", "L", "W", "P", "X", "M", "K", "A", "G", "I", "H", "J", "U", "D", "O"},
		new string[26] {"A", "L", "D", "N", "U", "B", "S", "T", "V", "R", "X", "Z", "O", "W", "F", "C", "I", "H", "E", "J", "G", "P", "Q", "Y", "K", "M"},
		new string[26] {"O", "M", "R", "S", "N", "C", "G", "T", "Z", "Y", "D", "F", "Q", "A", "V", "P", "I", "B", "X", "H", "E", "L", "K", "U", "J", "W"},
		new string[26] {"U", "H", "K", "T", "L", "E", "P", "Q", "N", "J", "M", "I", "Z", "O", "C", "D", "R", "W", "V", "S", "X", "F", "B", "A", "Y", "G"},
		new string[26] {"E", "B", "U", "Y", "Z", "L", "R", "C", "D", "X", "W", "O", "K", "Q", "I", "G", "T", "A", "M", "S", "N", "P", "H", "V", "F", "J"},
		new string[26] {"Y", "Q", "M", "G", "R", "P", "F", "H", "S", "U", "N", "C", "E", "Z", "T", "A", "B", "V", "W", "K", "L", "D", "J", "I", "O", "X"},
		new string[26] {"X", "B", "O", "J", "N", "Y", "Q", "U", "Z", "F", "V", "A", "L", "T", "K", "P", "G", "C", "W", "E", "S", "R", "H", "I", "M", "D"},
		new string[26] {"T", "W", "G", "C", "Y", "N", "B", "X", "Q", "K", "A", "U", "D", "Z", "E", "J", "I", "M", "R", "O", "S", "L", "H", "F", "V", "P"},
		new string[26] {"K", "T", "P", "Q", "B", "J", "C", "E", "I", "S", "A", "Y", "Z", "N", "O", "U", "X", "G", "M", "R", "D", "W", "L", "H", "V", "F"},
		new string[26] {"D", "X", "U", "A", "G", "E", "H", "M", "C", "J", "T", "O", "Q", "S", "L", "R", "W", "P", "F", "V", "Z", "B", "I", "N", "K", "Y"},
		new string[26] {"C", "B", "D", "J", "U", "H", "O", "V", "L", "F", "I", "K", "S", "X", "P", "Z", "R", "W", "Q", "G", "E", "T", "Y", "A", "M", "N"},
		new string[26] {"J", "I", "Y", "E", "P", "U", "C", "A", "F", "K", "G", "N", "O", "Q", "B", "W", "Z", "D", "V", "L", "X", "M", "R", "T", "S", "H"},
		new string[26] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}
		};

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        LeftQuad.OnInteract += delegate () { PressLeftQuad(); return false; };
        RightQuad.OnInteract += delegate () { PressRightQuad(); return false; };
    }

    void Start()
    {
        Module.OnActivate += ActivateModule;
		Module.OnActivate += Randomizer;
		Playable = true;
    }

    void ActivateModule()
    {
		TheFiber.text = "";
        List<int> NumericalValue = new List<int>();
        for (int i = 0; i < 1; i++) 
		{
			if ((Bomb.GetBatteryCount() == 3) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(0);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 1 is false.", moduleId);
				}
			}

			if ((Bomb.GetPortCount() == 2) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(1);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 2 is false.", moduleId);
				}
			}

			if ((Bomb.IsIndicatorPresent("BOB")) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(2);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 3 is false.", moduleId);
				}
			}

			if ((Bomb.GetStrikes() % 2 == 1) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(3);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 4 is false.", moduleId);
				}
			}

			if (((Bomb.GetIndicators().Count()) % 2 == 1) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(4);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 5 is false.", moduleId);
				}
			}

			if ((Bomb.IsIndicatorPresent("FRK")) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(5);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 6 is false.", moduleId);
				}
			}

			if (Bomb.GetSerialNumberNumbers().First() % 2 == 0) 
			{
				if (i < 1)
				{
					NumericalValue.Add(6);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 7 is false.", moduleId);
				}
			}

			if ((Bomb.GetIndicators().Count() == 2) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(7);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 8 is false.", moduleId);
				}
			}

			if ((Bomb.GetStrikes() % 2 == 0) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(8);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 9 is false.", moduleId);
				}
			}

			if ((Bomb.GetSerialNumberLetters().Count() >= 3) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(9);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 10 is false.", moduleId);
				}
			}

			if ((Bomb.GetIndicators().Count() < 2) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(10);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 11 is false.", moduleId);
				}
			}

			if ((Bomb.GetSolvedModuleNames().Count() < 5) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(11);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 12 is false.", moduleId);
				}
			}

			if (Bomb.GetSerialNumber().Contains("0") || Bomb.GetSerialNumber().Contains("2") || Bomb.GetSerialNumber().Contains("4") || Bomb.GetSerialNumber().Contains("6") || Bomb.GetSerialNumber().Contains("8"))
			{
				if (i < 1)
				{
					NumericalValue.Add(12);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 13 is false.", moduleId);
				}
			}

			if ((Bomb.GetModuleNames().Count() > 30) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(13);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 14 is false.", moduleId);
				}
			}

			if ((Bomb.GetBatteryHolderCount() < 3) != true)
			{
				if (i < 1)
				{
					NumericalValue.Add(14);
					Debug.LogFormat("[Reverse Alphabetize #{0}] Table 15 is false.", moduleId);
				}
			}
		}
		
		SilverLine = NumericalValue.Distinct().ToArray();
		
		if ((SilverLine.Count() % 2) == 0)
		{
			TheCopperValue = 15;
			Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
			Debug.LogFormat("[Reverse Alphabetize #{0}] The string used is the English Alphabet backwards.", moduleId);
			Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
		}
		
		else if ((SilverLine.Count() % 2) == 1)
		{
			if (SilverLine.Count() == 1)
			{
				TheCopperValue = 0;
			}
			
			else if (SilverLine.Count() == 3)
			{
				TheCopperValue = 1;
			}
			
			else if (SilverLine.Count() == 5)
			{
				TheCopperValue = 2;
			}
			
			else if (SilverLine.Count() == 7)
			{
				TheCopperValue = 3;
			}
			
			else if (SilverLine.Count() == 9)
			{
				TheCopperValue = 4;
			}
			
			else if (SilverLine.Count() == 11)
			{
				TheCopperValue = 5;
			}
			
			else if (SilverLine.Count() == 13)
			{
				TheCopperValue = 6;
			}
			
			else if (SilverLine.Count() == 15)
			{
				TheCopperValue = 7;
			}
			
			Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
			Debug.LogFormat("[Reverse Alphabetize #{0}] The string used is in Table " + (SilverLine[TheCopperValue] + 1).ToString(), moduleId);
			Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
		}
	}
	
	void Randomizer()
	{
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		if (TheFoil == -1)
		{
			StartCoroutine(TheSolved());
		}
		
		else if (TheFoil != -1)
		{
			int Choco = 0;
			Choco = UnityEngine.Random.Range(0,3);
			
			if (Choco != 0)
			{
				Supper = UnityEngine.Random.Range(0, Alphabreak.Count());
				TheLetter.text = Alphabreak[Supper];
			}
			
			else if (Choco == 0)
			{
				if (TheCopperValue != 15)
				{
					TheLetter.text = TheSequence[SilverLine[TheCopperValue]][TheFoil];
				}
				
				else if (TheCopperValue == 15)
				{
					TheLetter.text = TheSequence[TheCopperValue][TheFoil];
				}
			}
			
			if ((TheCopperValue != 15 && TheLetter.text == TheSequence[SilverLine[TheCopperValue]][TheFoil]) || (TheCopperValue == 15 && TheLetter.text == TheSequence[TheCopperValue][TheFoil]))
			{
				Debug.LogFormat("[Reverse Alphabetize #{0}] Letter " + TheLetter.text.ToString() + " matches the letter in the current stage.", moduleId);
			}
			
			else
			{
				Debug.LogFormat("[Reverse Alphabetize #{0}] Letter " + TheLetter.text.ToString() + " does not match the letter in the current stage.", moduleId);
			}
		}
	}

	void PressLeftQuad()
	{
		if (Computer == 0 && Playable == true)
		{
			LeftQuad.AddInteractionPunch(0.2f);
			if (TheCopperValue != 15)
			{
				if (TheSequence[SilverLine[TheCopperValue]][TheFoil] == TheLetter.text)
				{
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed left. Correct.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil - 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed left. Incorrect.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
			
			else if (TheCopperValue == 15)
			{
				if (TheSequence[TheCopperValue][TheFoil] == TheLetter.text)
				{
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed left. Correct.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil - 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed left. Incorrect.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
		}
	}

	void PressRightQuad()
	{
		if (Computer == 0 && Playable == true)
		{
			RightQuad.AddInteractionPunch(0.2f);
			if (TheCopperValue != 15)
			{
				if (TheSequence[SilverLine[TheCopperValue]][TheFoil] != TheLetter.text)
				{
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed right. Correct.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil - 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed right. Incorrect.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
			
			else if (TheCopperValue == 15)
			{
				if (TheSequence[TheCopperValue][TheFoil] != TheLetter.text)
				{
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed right. Correct.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
					TheFoil = TheFoil - 1;
					Randomizer();
				}
				
				else
				{
					StartCoroutine(Again());
					Debug.LogFormat("[Reverse Alphabetize #{0}] You pressed right. Incorrect.", moduleId);
					Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
				}
			}
		}
	}
	
	IEnumerator TheSolved()
	{
		StrikingBool = true;
		Style = UnityEngine.Random.Range(0,5);
		Computer = 1;
		TheLetter.text = "";
		TheTetra.text = "D";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "DO";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		
		if (Style != 0)
		{
			yield return new WaitForSeconds(0.2f);
			TheTetra.text = "DON";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheTetra.text = "DONE";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheTetra.text = "DONE!";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.5f);
			Module.HandlePass();
			Audio.PlaySoundAtTransform(SFX[0].name, transform);
			Debug.LogFormat("[Reverse Alphabetize #{0}] Module is done.", moduleId);
		}
		
		else if (Style == 0)
		{
			yield return new WaitForSeconds(1.5f);
			TheTetra.text = "D";
			yield return new WaitForSeconds(0.2f);
			TheTetra.text = "";
			yield return new WaitForSeconds(0.2f);
			TheHex.text = "D";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheHex.text = "DO";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheHex.text = "DO";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheHex.text = "DON";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheHex.text = "DONE";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.2f);
			TheHex.text = "DONE!";
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			yield return new WaitForSeconds(0.5f);
			Module.HandlePass();
			Audio.PlaySoundAtTransform(SFX[0].name, transform);
			Debug.LogFormat("[Reverse Alphabetize #{0}] Module is done.", moduleId);
		}
	}
	
	IEnumerator Again()
	{
		StrikingBool = true;
		Computer = 1;
		TheLetter.text = "";
		TheTetra.text = "N";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NO";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NOP";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NOPE";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(0.2f);
		TheTetra.text = "NOPE!";
		Audio.PlaySoundAtTransform(SFX[1].name, transform);
		yield return new WaitForSeconds(1f);
		TheTetra.text = "";
		Module.HandleStrike();
		Reset();
		ActivateModule();
		Randomizer();
		Computer = 0;
		StrikingBool = false;
	}
	
	void Reset()
	{
		TheCopperValue = 0;
		TheFoil = 25;
		Supper = 0;
	}
	
	IEnumerator UpdateVariables()
	{
		StrikingBool = true;
		TheLetter.text = "U";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "P";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "D";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "A";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "T";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "E";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "D";
		yield return new WaitForSeconds(0.1f);
		TheLetter.text = "!";
		yield return new WaitForSeconds(0.5f);
		Reset();
		Debug.LogFormat("[Reverse Alphabetize #{0}] The module is updated due to a command", moduleId);
		Debug.LogFormat("[Reverse Alphabetize #{0}] -----------------------------", moduleId);
		ActivateModule();
		Randomizer();
		StrikingBool = false;
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use the command !{0} left/right to press the corresponding side on the module. You can use the command !{0} update to update the rule list being applied in the module (This however resets all inputs in the module)";
    #pragma warning restore 414
	
	bool StrikingBool = false;
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		if (Regex.IsMatch(command, @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*r\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (StrikingBool == true)
			{
				yield return "sendtochaterror The module is not showing a letter. The command was not processed.";
				yield break;
			}
			RightQuad.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*l\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (StrikingBool == true)
			{
				yield return "sendtochaterror The module is not showing a letter. The command was not processed.";
				yield break;
			}
			LeftQuad.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*update\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (StrikingBool == true)
			{
				yield return "sendtochaterror The module is not showing a letter. The command was not processed.";
				yield break;
			}
			StartCoroutine(UpdateVariables());
			yield return "sendtochaterror The rules are now updated.";
		}
	}
}
