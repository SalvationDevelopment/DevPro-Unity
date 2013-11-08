using System;
using System.Collections.Generic;
using UnityEngine;
using DevPro.Game;
using DevPro.Game.Data;
using DevPro.Network.Data;
using Pathfinding.Serialization.JsonFx;

namespace DevPro.Network
{
	public static class BrowserMessages
	{
		public static void IsLoaded()
		{
			Application.ExternalCall("IsLoaded");	
		}
		
		public static void MessagePopUp(string message)
		{
			Application.ExternalCall("MessagePopUp", message);
		}
#region Launcher
		public static void LoginAccept(string username)
		{
			Application.ExternalCall("LoginAccept",username);	
		}
#endregion
#region Lobby
		
		public static void RoomInfo(RoomInfo info)
		{
			Application.ExternalCall("SetRoomInfo",JsonWriter.Serialize(info));
		}
		
		//This Refers the you the player and not other players
		public static void PositionUpdate(int pos)
		{
			Application.ExternalCall("PosUpdate",pos);
		}
		
		public static void PlayerEnter(string username,int pos)
		{
			Application.ExternalCall("PlayerEnter",username,pos);
		}
		
		public static void PlayerLeave(int pos)
		{
			Application.ExternalCall("PlayerLeave",pos);
		}
		
		public static void UpdatePlayer(int pos, int newpos)
		{
			Application.ExternalCall("UpdatePlayer",pos,newpos);
		}
		
		public static void PlayerReady(int pos, bool ready)
		{
			Application.ExternalCall("PlayerReady",pos,ready);
		}
		
		public static void DeckError(int card)
		{
			Application.ExternalCall("DeckError",card);
		}
		
		public static void SideError()
		{
			Application.ExternalCall("SideError");
		}
		
		public static void PlayerMessage(int player,string message)
		{
			Application.ExternalCall("PlayerMessage",player,message);
		}
		
#endregion
#region Duel
		//if the result is 2 the game is a draw
		public static void OnWin(int result)
		{
			Application.ExternalCall("OnWin",result);
		}
		
		public static void OnDuelEnd()
		{
			Application.ExternalCall("DuelEnd");
		}
		
		public static void SelectRPS()
		{
			Application.ExternalCall("SelectRPS");
		}
		public static void SelectFirstPlayer()
		{
			Application.ExternalCall("SelectFirstPlayer");
		}
		
		public static void SendStartDuel(DuelStart data)
		{
			Application.ExternalCall("StartDuel",JsonWriter.Serialize(data));
		}
		
		public static void DrawCard(int player, int count)
		{
			Application.ExternalCall("DrawCard",player,count);
		}
		
		public static void ShuffleDeck(int player)
		{
			Application.ExternalCall("ShuffleDeck",player);
		}
		
		public static void ShuffleHand(int player,IList<CardData> newHandOrder)
		{
			Application.ExternalCall("ShuffleHand",player,JsonWriter.Serialize(newHandOrder));	
		}
		
		public static void NewTurn(int player)
		{
			Application.ExternalCall("NewTurn",player);
		}
		
		public static void NewPhase(int phase)
		{
			Application.ExternalCall("NewPhase",phase);
		}
		
		public static void IdleCommands(MainPhase main)
		{
			Application.ExternalCall("IdleCommands",JsonWriter.Serialize(main));
		}
		
		public static void PlayerDamage(int player, int total)
		{
			Application.ExternalCall("DamageLifePoints",player, total);
		}
		
		public static void PlayerRecover(int player, int total)
		{
			Application.ExternalCall("RecoverLifePoints",player, total);
		}
		
		public static void UpdateLifePoints(int player, int lifepoints)
		{
			Application.ExternalCall("UpdateLifePoints",player, lifepoints);
		}
		
		public static void UpdateCard(int player,int location,int index,CardData data)
		{
			Application.ExternalCall("UpdateCard",player, location, index, JsonWriter.Serialize(data));
		}
		
		public static void UpdateCards(int player, int location,IList<CardData> data)
		{
			Application.ExternalCall("UpdateCards",player, location, JsonWriter.Serialize(data));
		}
		
		public static void MoveCard(int player,int location, int index,
			int moveplayer, int movelocation,int movezone,int moveposition)
		{
			Application.ExternalCall("MoveCard",player, location, index,
				moveplayer, movelocation, movezone, moveposition);
		}
		
		public static void ChangeCardPosition(int player, int location,int index, int newposition)
		{
			Application.ExternalCall("ChangePosition",player, location, index, newposition);
		}
		
		public static void SelectCards(IList<CardData> cards, int min, int max, int cancelable)
		{
			Application.ExternalCall("SelectCards",cards, min, max, cancelable);
		}
		
		public static void ActivateCardEffect(int cardid)
		{
			Application.ExternalCall("ActivateCardEffect",cardid);
		}
		
		public static void SelectYn(int desc)
		{
			Application.ExternalCall("SelectYn",desc);
		}
		
		public static void SelectPosition(IList<int> positions)
		{
			Application.ExternalCall("SelectPosition",JsonWriter.Serialize(positions));
		}
		
		public static void SelectOption(IList<int> options)
		{
			Application.ExternalCall("SelectOption",JsonWriter.Serialize(options));
		}
		
		public static void AnnounceCard()
		{
			Application.ExternalCall("AnnounceCard");
		}
#endregion
	}
}

