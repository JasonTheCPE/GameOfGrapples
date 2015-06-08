using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DojoButtons : MonoBehaviour
{
	//Text Fields
	public Text[] playerSlotNames;
	public Text playerSkinName;
	public Text stageName;
	public Text timeOption;
	public Text healthOption;
	public Text ammoOption;
	public Text gamemodeOption;
	
	//Individual accessible buttons
	public Button[] playerTeamButtons;
	public Button changeSkinButton;
	public Button disconnectButton;
	
	//Host only
	public Button changeStageButton;
	public Button timeUpButton;
	public Button timeDownButton;
	public Button healthUpButton;
	public Button healthDownButton;
	public Button ammoUpButton;
	public Button ammoDownButton;
	public Button teamsOnButton;
	public Button teamsOffButton;
	public Button gamemodeRightButton;
	public Button gamemodeLeftButton;
	public Button startMatchButton;
	
	//Skin select menu buttons
	public Button skinSelectButton;
	public Button skinCancelButton;
	
	//Level select menu buttons
	public Button levelSelectButton;
	public Button levelCancelButton;
}
