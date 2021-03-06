using UnityEngine;
using System.Collections;



public class UIiOS : MonoBehaviour {

	public float fastForwardSpeed=3;
	
	public bool alwaysEnableNextButton=true;
	public string nextLevel="";
	public string mainMenu="";
	
	public GUIText generalUIText;
	public GUIText messageUIText;
	
	public GUIButton spawnButton;
	public GUIButton ffButton;
	public GUIToggleButton pauseButton;
	public GUIButton upgradeButton;
	public GUIToggleButton sellButton;
	
	public GUITexture generalBox;
	public GUIButton menuButton;
	public GUIButton restartButton;
	public GUIButton nextLvlButton;
	
	
	
	
	private UnitTower currentSelectedTower;
	
	
	void Awake(){
		if(!this.enabled){
			gameObject.SetActiveRecursively(false);
			gameObject.active=true;
		}	
	}
	
	
	// Use this for initialization
	void Start () {
		
		spawnButton.callBackFunc=this.OnSpawnButton;
		StartCoroutine(spawnButton.Update());
		
		ffButton.callBackFunc=this.OnFastForwardButton;
		StartCoroutine(ffButton.Update());
		
		pauseButton.callBackFunc=this.OnPauseButton;
		StartCoroutine(pauseButton.Update());
		
		upgradeButton.callBackFunc=this.OnUpgradeTowerButton;
		StartCoroutine(upgradeButton.Update());
		
		sellButton.callBackFunc=this.OnSellButton;
		StartCoroutine(sellButton.Update());
		
		GenerateButton();
		DisableSelectedTowerUI();
		
		DisableGeneralMenu();
		
		UIButtonRect=new Rect(0, Screen.height-90, 137, 90);
		
		
		OnUpdateGeneralUIText();
		
		//obsolete
		//BuildManager.InitiateSampleTower();
	}
	
	
	void OnEnable(){
		SpawnManager.onClearForSpawningE += OnClearForSpawning;
		SpawnManager.onWaveStartSpawnE += OnUpdateGeneralUIText;
		
		GameControl.onGameOverE += OnGameOver;
		
		GameControl.onResourceE += OnUpdateGeneralUIText;
		GameControl.onLifeE += OnUpdateGeneralUIText;
		
		UnitTower.onBuildCompleteE += OnTowerBuildComplete;
		UnitTower.onDestroyE += OnTowerDestroy;
	}
	
	void OnDisable(){
		SpawnManager.onClearForSpawningE -= OnClearForSpawning;
		SpawnManager.onWaveStartSpawnE -= OnUpdateGeneralUIText;
		
		GameControl.onGameOverE -= OnGameOver;
		
		GameControl.onResourceE -= OnUpdateGeneralUIText;
		GameControl.onLifeE -= OnUpdateGeneralUIText;
		
		UnitTower.onBuildCompleteE -= OnTowerBuildComplete;
		UnitTower.onDestroyE -= OnTowerDestroy;
	}
	
	
	void OnUpdateGeneralUIText(int val){
		OnUpdateGeneralUIText();
	}
	
	void OnUpdateGeneralUIText(){
		string info="";
		
		info+="Life: "+GameControl.GetPlayerLife()+"\n\n";
		
		Resource[] resourceList=GameControl.GetResourceList();
		foreach(Resource rsc in resourceList){
			info+=rsc.name+": "+rsc.value+"\n";
		}
		info+="\n";
		
		info+="Wave: "+SpawnManager.GetCurrentWave()+"/"+SpawnManager.GetTotalWave();
		
		generalUIText.text=info;
	}
	
	void OnTowerBuildComplete(UnitTower tower){
		if(currentSelectedTower==tower){
			if(!currentSelectedTower.IsLevelCapped())
				upgradeButton.buttonObj.enabled=true;
			
			UpdateSelectedTowerText();
		}
	}
	
	void OnTowerDestroy(UnitTower tower){
		if(currentSelectedTower==tower){
			DisableSelectedTowerUI();
		}
	}
	
	
	void OnGameOver(bool flag){
		winLostFlag=flag;
		
		generalUIText.text="";
		pauseButton.buttonObj.enabled=false;
		
		if(winLostFlag){
			GameMessage.DisplayMessage("level completed");
			messageUIText.text="level completed";
		}
		else{
			GameMessage.DisplayMessage("level failed");
			messageUIText.text="level failed";
		}
		
		EnableGeneralMenu(winLostFlag);
	}
	
	void DisableGeneralMenu(){		
		generalBox.enabled=false;
		menuButton.buttonObj.enabled=false;
		restartButton.buttonObj.enabled=false;
		nextLvlButton.buttonObj.enabled=false;
		
		generalRect=new Rect(0, 0, 0, 0);
	}
	
	void EnableGeneralMenu(bool winLostFlag){
		generalBox.enabled=true;
		menuButton.buttonObj.enabled=true;
		menuButton.callBackFunc=this.OnMenuButton;
		StartCoroutine(menuButton.Update());
		
		restartButton.buttonObj.enabled=true;
		restartButton.callBackFunc=this.OnRestartButton;
		StartCoroutine(restartButton.Update());
		
		if(winLostFlag || alwaysEnableNextButton){
			nextLvlButton.buttonObj.enabled=true;
			nextLvlButton.callBackFunc=this.OnNextLvlButton;
			StartCoroutine(nextLvlButton.Update());
		}
		
		generalRect=new Rect(Screen.width/2-70, Screen.height/2-75, 140, 150);
	}
	
	
	void DisablePauseMenu(){		
		generalBox.enabled=false;
		menuButton.buttonObj.enabled=false;
		restartButton.buttonObj.enabled=false;
		
		generalRect=new Rect(0, 0, 0, 0);
	}
	
	void EnablePauseMenu(){
		generalBox.enabled=true;
		menuButton.buttonObj.enabled=true;
		menuButton.callBackFunc=this.OnMenuButton;
		StartCoroutine(menuButton.Update());
		
		restartButton.buttonObj.enabled=true;
		restartButton.callBackFunc=this.OnRestartButton;
		StartCoroutine(restartButton.Update());
		
		generalRect=new Rect(Screen.width/2-70, Screen.height/2-75, 140, 150);
	}
	
	
	void OnMenuButton(int ID){
		if(mainMenu!="") Application.LoadLevel(mainMenu);
	}
	
	void OnNextLvlButton(int ID){
		if(nextLevel!="") Application.LoadLevel(nextLevel);
	}
	
	void OnRestartButton(int ID){
		Application.LoadLevel(Application.loadedLevelName);
	}
	
	void OnClearForSpawning(bool flag){
		spawnButton.buttonObj.enabled=flag;
	}
	
	private Rect UIButtonRect;
	private Rect towerUIRect;
	private Rect buildListRect;
	private Rect generalRect;
	private bool winLostFlag=false;
	
	public bool IsCursorOnUI(Vector3 point){
		Rect tempRect=new Rect(0, 0, 0, 0);
		
		tempRect=UIButtonRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=generalRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=buildListRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		tempRect=towerUIRect;
		tempRect.y=Screen.height-tempRect.y-tempRect.height;
		if(tempRect.Contains(point)) return true;
		
		return false;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonUp(0) && !IsCursorOnUI(Input.mousePosition) && GameControl.gameState!=_GameState.Ended && !paused){
			
			UnitTower tower=GameControl.Select(Input.mousePosition);
			
			if(tower!=null){
				OnSelectTower(tower);
				DisableBuildMenu();
				BuildManager.ClearBuildPoint();
			}
			else{
				
				if(currentSelectedTower!=null){
					OnUnselectTower();
				}
				
				if(BuildManager.CheckBuildPoint(Input.mousePosition)){
					UpdateBuildList();
					EnableBuildMenu();
				}
				else{
					if(buildMenu){
						BuildManager.ClearIndicator();
						DisableBuildMenu();
					}
				}
			}
			
		}
		else if(Input.GetMouseButtonUp(1)){
			DisableBuildMenu();
			BuildManager.ClearBuildPoint();
			if(currentSelectedTower!=null){
				CheckForTarget();
			}
		}
		
		if(Input.GetKeyUp(KeyCode.Escape)){
			TogglePause();
		}
	}
	
	void CheckForTarget(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		LayerMask mask=currentSelectedTower.GetTargetMask();
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
			Unit unit=hit.collider.gameObject.GetComponent<Unit>();
			if(unit!=null){
				currentSelectedTower.AssignTarget(unit);
			}
		}
	}
	
	public GUITexture SelectedTowerUIbox;
	public GUIText SelectedTowerUIText;
	
	void OnSelectTower(UnitTower tower){
		currentSelectedTower=tower;
		EnableSelectedTowerUI();
	}
	
	void OnUnselectTower(){
		currentSelectedTower=null;
		DisableSelectedTowerUI();
		GameControl.ClearSelection();
	}
	
	void EnableSelectedTowerUI(){
		//towerUIOn=true;
		
		SelectedTowerUIbox.enabled=true;
		SelectedTowerUIText.enabled=true;
		
		if(!currentSelectedTower.IsLevelCapped())
			upgradeButton.buttonObj.enabled=true;
		
		sellButton.buttonObj.enabled=true;
		
		UpdateSelectedTowerText();
		
		towerUIRect=new Rect(Screen.width-270, Screen.height-525, 270, 525);
	}
	
	void DisableSelectedTowerUI(){
		SelectedTowerUIbox.enabled=false;
		SelectedTowerUIText.enabled=false;
		
		upgradeButton.buttonObj.enabled=false;
		sellButton.buttonObj.enabled=false;
		
		towerUIRect=new Rect(0, 0, 0, 0);
	}
	
	void UpdateSelectedTowerText(){
		if(currentSelectedTower!=null){
			string towerInfo="";
			
			towerInfo+=currentSelectedTower.unitName+"\n";
			
			int lvl=currentSelectedTower.GetLevel();
			towerInfo+="Level: "+lvl.ToString()+"\n\n\n";
			
			_TowerType type=currentSelectedTower.type;
			
			if(type==_TowerType.ResourceTower){
				
				int[] incomes=currentSelectedTower.GetIncomes();
				string cd=currentSelectedTower.GetCooldown().ToString("f1");
				
				Resource[] resourceList=GameControl.GetResourceList();
				
				string rsc="";
				for(int i=0; i<incomes.Length; i++){
					rsc="Increase "+resourceList[i].name+" by "+incomes[i].ToString("0")+" for every "+cd+"sec\n";
				}
				
				towerInfo+=rsc;
				
			}
			else if(type==_TowerType.SupportTower){
				
				BuffStat buffInfo=currentSelectedTower.GetBuff();
				
				string buff="";
				if(buffInfo.damageBuff!=0) buff+="Buff damage by "+(buffInfo.damageBuff*100).ToString("f1")+"%\n";
				if(buffInfo.rangeBuff!=0) buff+="Buff range by "+(buffInfo.rangeBuff*100).ToString("f1")+"%\n";
				if(buffInfo.cooldownBuff!=0) buff+="Reduce CD by "+(buffInfo.cooldownBuff*100).ToString("f1")+"%\n";
				if(buffInfo.regenHP!=0) buff+="Renegerate HP by "+(buffInfo.regenHP).ToString("f1")+" per seconds\n";
			
				towerInfo+=buff;
				
			}
			else if(type==_TowerType.Mine){
				//show the basic info for mine
				if(currentSelectedTower.GetDamage()>0)
					towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
				if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
					towerInfo+="AOE Radius: "+currentSelectedTower.GetRange().ToString("f1")+"\n";
				if(currentSelectedTower.GetStunDuration()>0)
					towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
				
				//if the mine have damage over time value, display it
				Dot dot=currentSelectedTower.GetDot();
				float totalDot=dot.damage*(dot.duration/dot.interval);
				if(totalDot>0){
					string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
					
					towerInfo+=dotInfo;
				}
				
				//if the mine have slow value, display it
				Slow slow=currentSelectedTower.GetSlow();
				if(slow.duration>0){
					string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
					towerInfo+=slowInfo;
				}
			}
			else if(type==_TowerType.TurretTower || type==_TowerType.AOETower || type==_TowerType.DirectionalAOETower){
				
				if(currentSelectedTower.GetDamage()>0)
					towerInfo+="Damage: "+currentSelectedTower.GetDamage().ToString("f1")+"\n";
				if(currentSelectedTower.GetCooldown()>0)
					towerInfo+="Cooldown: "+currentSelectedTower.GetCooldown().ToString("f1")+"sec\n";
				if(type==_TowerType.TurretTower && currentSelectedTower.GetAoeRadius()>0)
					towerInfo+="AOE Radius: "+currentSelectedTower.GetAoeRadius().ToString("f1")+"\n";
				if(currentSelectedTower.GetStunDuration()>0)
					towerInfo+="Stun target for "+currentSelectedTower.GetStunDuration().ToString("f1")+"sec\n";
				
				Dot dot=currentSelectedTower.GetDot();
				float totalDot=dot.damage*(dot.duration/dot.interval);
				if(totalDot>0){
					string dotInfo="Cause "+totalDot.ToString("f1")+" damage over the next "+dot.duration+" sec\n";
					dotInfo=FormatString(dotInfo);
					
					towerInfo+=dotInfo;
				}
				
				Slow slow=currentSelectedTower.GetSlow();
				if(slow.duration>0){
					string slowInfo="Slow target by "+(slow.slowFactor*100).ToString("f1")+"% for "+slow.duration.ToString("f1")+"sec\n";
					slowInfo=FormatString(slowInfo);
					
					towerInfo+=slowInfo;
				}
			}
			
			string desp=FormatString("\n"+currentSelectedTower.GetDescription());
			
			towerInfo+=desp;
			
			
			towerInfo+="\n\n\n";
			Resource[] rscList=GameControl.GetResourceList();
			int count=0;
			
			if(!currentSelectedTower.IsLevelCapped() && currentSelectedTower.IsBuilt()){
				
				int[] cost=currentSelectedTower.GetCost();
				
				towerInfo+="Upgrade Cost:\n ";
				
				
				for(int i=0; i<cost.Length; i++){
					if(cost[i]>0){
						count+=1;
						towerInfo+="- "+cost[i].ToString()+rscList[i].name+"  ";
					}
				}
				
				towerInfo+="\n\n";
			}
			
			int[] sellValue=currentSelectedTower.GetTowerSellValue();
				
			towerInfo+="Sell Cost:\n ";
			
			count=0;
			for(int i=0; i<sellValue.Length; i++){
				if(sellValue[i]>0){
					count+=1;
					towerInfo+="+ "+sellValue[i].ToString()+rscList[i].name+"  ";
				}
			}
			
			SelectedTowerUIText.text=towerInfo;
		}
	}
	
	public string FormatString(string s) {
		char[] delimitor = new char[1] {' '};
		string[] words = s.Split(delimitor); //Split the string into seperate words
		string result = "";
		int runningLength = 0;
		foreach (string word in words) {
			if (runningLength + word.Length+1 <= 32) {
				result += " " + word;
				runningLength += word.Length+1;
			}
			else {
				result += "\n" + word;
				runningLength = word.Length;
			}
		}
		   
		return result.Remove(0,1); //Remove the first space
    }
	
	
	void OnSellButton(int id){
		if(currentSelectedTower!=null){
			if(!sellButton.isPressed) {
				
				currentSelectedTower.Sell();
				
				sellButton.buttonObj.enabled=false;
			}
		}
	}
	
	
	void OnSpawnButton(int id){
		if(GameControl.gameState!=_GameState.Ended){
			SpawnManager.Spawn();
		}
	}

	
	void OnUpgradeTowerButton(int ID){
		if(currentSelectedTower!=null){
			upgradeButton.buttonObj.enabled=false;
			currentSelectedTower.Upgrade();
			UpdateSelectedTowerText();
		}
	}
	
	void OnBuildButton(int ID){
		UnitTower[] towerList=BuildManager.GetTowerList();
		UnitTower tower=towerList[ID];
		
		if(BuildManager.BuildTowerPointNBuild(tower)==""){
			DisableBuildMenu();
			BuildManager.ClearSampleTower();
		}
		else{
			BuildManager.ClearSampleTower();
		}
	}
	
	void OnShowSampleTower(int ID, bool flag){
		if(flag) BuildManager.ShowSampleTower(ID); 
		else BuildManager.ClearSampleTower();
	}
	
	void OnFastForwardButton(int ID){
		if(Time.timeScale==1) Time.timeScale=fastForwardSpeed;
		else Time.timeScale=1;
	}
	
	private bool paused=false;
	void OnPauseButton(int ID){
		TogglePause();
	}
	
	void TogglePause(){
		paused=!paused;
		if(paused){
			Time.timeScale=0;
			messageUIText.text="Game Paused";
			spawnButton.buttonObj.enabled=false;
			ffButton.buttonObj.enabled=false;
			
			if(currentSelectedTower!=null){
				OnUnselectTower();
			}
			if(buildMenu){
				BuildManager.ClearIndicator();
				DisableBuildMenu();
			}
			
			EnablePauseMenu();
		}
		else{
			Time.timeScale=1;
			messageUIText.text="";
			spawnButton.buttonObj.enabled=true;
			ffButton.buttonObj.enabled=true;
			
			DisablePauseMenu();
		}
	}
	
	private GUIButton[] buildButtonList=new GUIButton[0];
	private int[] currentBuildList=new int[0];
	
	private bool buildMenu=false;
	
	float buildButtonSize=60;
	
	//called whevenever the build list is called up
	//compute the number of tower that can be build in this build pointer
	//store the tower that can be build in an array of number that reference to the towerlist
	//this is so these dont need to be calculated in every frame in OnGUI()
	private void UpdateBuildList(){
		//get the current buildinfo in buildmanager
		BuildableInfo currentBuildInfo=BuildManager.GetBuildInfo();
		
		//get the current tower list in buildmanager
		UnitTower[] towerList=BuildManager.GetTowerList();
		
		//construct a temporary interger array the length of the buildinfo
		int[] tempBuildList=new int[towerList.Length];
		//for(int i=0; i<currentBuildList.Length; i++) tempBuildList[i]=-1;
		
		//scan through the towerlist, if the tower matched the build type, 
		//put the tower ID in the towerlist into the interger array
		int count=0;	//a number to record how many towers that can be build
		for(int i=0; i<towerList.Length; i++){
			UnitTower tower=towerList[i];
				
			//check if this type of tower can be build on this platform
			foreach(_TowerType type in currentBuildInfo.buildableType){
				if(tower.type==type){
					tempBuildList[count]=i;
					count+=1;
					break;
				}
			}

		}
		
		//for as long as the number that can be build, copy from the temp buildList to the real buildList
		currentBuildList=new int[count];
		for(int i=0; i<currentBuildList.Length; i++) currentBuildList[i]=tempBuildList[i];
	}
	
	private void EnableBuildMenu(){
		if(buildMenu) DisableBuildMenu();
		
		buildMenu=true;
		UpdateBuildList();
		
		int count=0;
		foreach(int i in currentBuildList){
			buildButtonList[i].buttonObj.enabled=true;
			
			Vector3 pos=new Vector3((140f+count*(buildButtonSize+7))/Screen.width, 5f/Screen.height, 1);
			buildButtonList[i].buttonObj.transform.position=pos;
			
			count+=1;
		}
		
		buildListRect=new Rect(137, Screen.height-buildButtonSize-12, count*(buildButtonSize+7), buildButtonSize+12);
	}
	
	private void DisableBuildMenu(){
		foreach(GUIButton button in buildButtonList){
			button.buttonObj.enabled=false;
		}
		
		buildListRect=new Rect(0, 0, 0, 0);
		
		buildMenu=false;
		
	}
	
	void GenerateButton(){
		UnitTower[] towerList=BuildManager.GetTowerList();
		
		buildButtonList=new GUIButton[towerList.Length];
		
		for(int i=0; i<towerList.Length; i++){
			UnitTower tower=towerList[i];
			
			buildButtonList[i]=new GUIButton(tower.icon, null, OnBuildButton, OnShowSampleTower, i);
			buildButtonList[i].buttonObj.pixelInset=new Rect(0, 0, buildButtonSize, buildButtonSize);
			buildButtonList[i].buttonObj.transform.localScale=new Vector3(0, 0, 1);
			
			StartCoroutine(buildButtonList[i].Update());
		}
		
		DisableBuildMenu();
	}
	
}






