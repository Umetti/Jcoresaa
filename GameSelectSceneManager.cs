using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

//enum FIELD { Attention, WorkMemory, VerbalMemory, ProcessingSpeed, ExecutiveFunction, Fluency, Comprehensive };

// ゲーム選択画面用SceneManager
public class GameSelectSceneManager : MonoBehaviour {
	GameObject 	fade;			// フェード用Image UIを格納
	float 		startTime;		// フェード処理の開始時間を格納
	public int 	fadeTime;		// フェード処理にかける時間（s)
	string 		fadeMode;		// フェードモード（インの場合「FadeIn」、アウトの場合「FadeOut」を格納
	Color 		alpha;			// フェードImageのα値
	GameObject sample_image;	// 選択したゲームのサンプルイメージ画像を格納
	GameObject abstract_text;	// 選択したゲームについての概要説明を格納
	GameObject[] field_button = new GameObject[7];	// 分野を選択するためのボタン
	GameObject[] game_button = new GameObject[4];	// ゲームを選択するためのボタン
	GameObject[] board = new GameObject[4];			// game_buttonの下に表示するボード
	int select_field, select_game;					// 選択した分野とゲームを保持する変数
	// 分野に関する文字列を保存する配列変数
	string[] FIELD = new string[] { 
		"Attention", "WorkMemory", "VerbalMemory", "ProcessingSpeed", "ExecutiveFunction", "Fluency", "Comprehensive" 
	};
	// 分野別のゲームに関する文字列を保存する配列変数
	string[,] GAME = new string[,] {
		{"Pickup"		, "Compare"				, "ReactScreen"		, "SignSearch"		},
		{"MoneyBox"		, "Concentration"		, "TextSort"		, "RemembersReading"},
		{"ShoppingList"	, "AuditoryStimulation"	, "SightStimulus"	, "MNM"				},
		{"FallBall"		, "WhackAMole"			, "FlagUp"			, "Typing"			},
		{"TraiaMaking"	, "Labyrinth"			, "ChangeConcept"	, "CaveEscape"		},
		{"Shiritori"	, "YamanoteLineGame"	, "WordSearch"		, ""				},
		{"Shopping"		, "Working"				, ""				, ""				}
	};


	// Use this for initialization
	void Start () {
		// フェードインの処理を開始
		fade = GameObject.Find ("FadeImage");
		startTime = Time.time;
		fadeMode = "FadeIn";

		// 配置されたオブジェクトを取得
		sample_image = GameObject.Find ("sampleImage");
		abstract_text = GameObject.Find ("abstractText"); 
		field_button [0] = GameObject.Find ("attentionButton");
		field_button [1] = GameObject.Find ("workmemoryButton");
		field_button [2] = GameObject.Find ("verbalmemoryButton");
		field_button [3] = GameObject.Find ("processingspeedButton");
		field_button [4] = GameObject.Find ("executivefunctionButton");
		field_button [5] = GameObject.Find ("fluencyButton");
		field_button [6] = GameObject.Find ("comprehensiveButton");
		game_button [0] = GameObject.Find ("game1Button");
		game_button [1] = GameObject.Find ("game2Button");
		game_button [2] = GameObject.Find ("game3Button");
		game_button [3] = GameObject.Find ("game4Button");
		board [0] = GameObject.Find ("board_A");	// この変数は実際には使用されていない
		board [1] = GameObject.Find ("board_B");	// この変数は実際には使用されていない
		board [2] = GameObject.Find ("board_C");
		board [3] = GameObject.Find ("board_D");

		// 初期状態として、「総合」が押されていることにする
		ComprehensiveButtonDown ();
	}
	
	// Update is called once per frame
	void Update () {
		// fedeModeの値から、フェードイン、フェードアウトの処理を行う
		switch (fadeMode) {
		case "FadeIn":
			alpha.a = 1.0f - (Time.time - startTime) / fadeTime;
			fade.GetComponent<Image> ().color = new Color (0, 0, 0, alpha.a);
			//Fade用Imageのα値が０（フェードインが完了）になったら、Imageを非アクティブにする
			if (alpha.a <= 0.0f) {
				fadeMode = "";
				fade.SetActive (false);
			}
			break;
		case "FadeOut":
			alpha.a = (Time.time - startTime) / fadeTime;
			fade.GetComponent<Image> ().color = new Color (0, 0, 0, alpha.a);
			if (alpha.a >= 1.0f) {
				fadeMode = "";
			}
			break;
		}		
	}

	/*（以下の７つの関数について、同じ内容となる）
	 * 関数名	：（分野を示す文字列）ButtonDown
	 * 引数		：なし
	 * 内容		：各分野のボタン押下時に呼び出される関数。
	 * 			：押下に応じた処理（GameBoardDisplay関数）の実施を行う
	 */
	public void AttentionButtonDown(){
		GameBoardDisplay (0);
	}

	public void WorkmemoryButtonDown(){
		GameBoardDisplay (1);
	}

	public void VerbalmemoryButtonDown(){
		GameBoardDisplay (2);
	}

	public void ProcessingspeedButtonDown(){
		GameBoardDisplay (3);
	}

	public void ExecutivefunctionButtonDown(){
		GameBoardDisplay (4);
	}

	public void FluencyButtonDown(){
		GameBoardDisplay (5);
	}

	public void ComprehensiveButtonDown(){
		GameBoardDisplay (6);
	}

	/*
	 * 関数名	：GameBoardDisplay
	 * 引数		：num　→　分野に応じた数値（０から６）が渡される整数型変数
	 * 内容		：以下の処理を行う
	 * 			：　(1) 押下された分野のボタンに色を付ける
	 * 			：　(2) 分野にあるゲーム一覧をボードとして表示する
	 */
	void GameBoardDisplay( int num ){
		string field, game, path;

		// 押下した分野のボタンの色を変える
		field_button[select_field].GetComponentInChildren<Image> ().color = new Color (255, 255, 255, 255); // 前に押されたボタンをもとの色に戻す
		select_field = num;		// 選択したボタンが何かを判別できるようにするため、値を格納
		field_button[select_field].GetComponentInChildren<Image> ().color = new Color (255, 255, 0, 255); // 押されたボタンの背景色を黄色にする

		// ボードの初期化処理。流暢性、総合を選択した場合、３つ目と４つ目のボードが非アクティブになるため、元に戻している
		game_button [2].SetActive (true);
		game_button [3].SetActive (true);
		board [2].SetActive (true);
		board [3].SetActive (true);

		// ボードの表示処理。
		field = FIELD [select_field];
		for (int i = 0; i < 4; i++) {
			path = "Assets/Resource/GameSelect/" + field + "/";
			game = GAME [select_field, i];
			if (game != "") {
				game_button [i].GetComponent<Image> ().sprite = AssetDatabase.LoadAssetAtPath<Sprite> (path + game + ".bmp");
			} else {
				game_button [i].SetActive (false);
				board [i].SetActive (false);
			}
		}

		// 初期状態として、ボード上の一番上が押されていることにする
		Game1ButtonDown ();
	}

	/*（以下の４つの関数について、同じ内容となる）
	 * 関数名	：Game（ボード番号）ButtonDown
	 * 引数		：なし
	 * 内容		：分野のゲームのボタン押下時に呼び出される関数。
	 * 			：押下に応じた処理（GameSampleImageDisplay関数）の実施を行う
	 */
	public void Game1ButtonDown(){
		GameSampleImageDisplay (0);
	}

	public void Game2ButtonDown(){
		GameSampleImageDisplay (1);
	}

	public void Game3ButtonDown(){
		GameSampleImageDisplay (2);
	}

	public void Game4ButtonDown(){
		GameSampleImageDisplay (3);
	}

	/*
	 * 関数名	：GameSampleImageDisplay
	 * 引数		：num　→　押されたボード位置を示す数値（０から３）が渡される整数型変数
	 * 内容		：以下の処理を行う
	 * 			：　(1) 押下されたゲーム（ボード）のボタンに色を付ける
	 * 			：　(2) 押下されたゲームに関するサンプル画像と概要説明を表示する
	 */
	void GameSampleImageDisplay( int num ){
		string field, game, path;

		// 押下したゲーム（ボード）のボタンの色を変える
		game_button [select_game].GetComponentInChildren<Image> ().color = new Color (255, 255, 255, 255); // 前に押されたボタンをもとの色に戻す
		select_game = num;	// 選択したボタン（ボード）が何かを判別できるようにするため、値を格納
		game_button [select_game].GetComponentInChildren<Image> ().color = new Color (255, 255, 0, 255); // 押されたボタンの背景色を黄色にする

		// ゲームに関するサンプル画像と概要説明の表示処理
		field = FIELD [select_field];
		game = GAME [select_field, select_game];
		path = "Assets/Resource/GameSelect/" + field + "/" + game + "/";
		sample_image.GetComponentInChildren<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite> (path + game + "_image.bmp");
//		sample_image.GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite> (path + game + "_image.bmp");
		abstract_text.GetComponent<Text> ().text = AssetDatabase.LoadAssetAtPath<TextAsset>(path + game + "_abst.txt").text;
	}

	/*
	 * 関数名	：GameStartButtonDown
	 * 引数		：なし
	 * 内容		：「ゲーム」ボタン押下時に呼び出される関数。
	 * 			：フェードアウトの処理開始とゲーム選択画面への遷移の手続きを行う
	 */
	public void GameStartButtonDown(){
		// フェードイン完了で非アクティブ化されたFade用Imageをアクティブに戻し、フェードアウト処理の手続きを行う。
		fade.SetActive (true);
		startTime = Time.time;
		fadeMode = "FadeOut";
		// （fadeTime）秒後にSceneLoad関数を実行する
		Invoke ("SceneLoad", fadeTime);
	}

	/*
	 * 関数名	：SceneLoad
	 * 引数		：なし
	 * 内容		：選択した分野およびゲーム（ボード）により、各ゲームのタイトルシーンへ遷移を実行する関数。
	 */
	public void SceneLoad(){
		string game;

		game = GAME [select_field, select_game];
		SceneManager.LoadScene ( game + "Title" );
	}

	/*
	 * 関数名	：BackTitleButtonDown
	 * 引数		：なし
	 * 内容		：「タイトルへ戻る」ボタン押下時に呼び出される関数。
	 * 			：メインタイトル画面へ遷移を行う
	 */
	public void BackTitleButtonDown(){
		SceneManager.LoadScene ( "MainTitle" );
	}
}
