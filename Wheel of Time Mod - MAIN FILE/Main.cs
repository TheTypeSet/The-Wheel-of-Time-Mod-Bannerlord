﻿using HarmonyLib;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;

using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using WoT_Main.Behaviours;
using WoT_Main.Support;
using TaleWorlds.Library;
using System.Collections.Generic;
using System.Reflection;
using WoT_Main.CampaignEventSystem;
using System.IO;
using TaleWorlds.Engine.Screens;
using SandBox.View;
using SandBox;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using WoT_Main.Cheats;
using WoT_Main.Behaviours.Campaign;

namespace WoT_Main
{
    public class Main : MBSubModuleBase
    {
        
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {

            base.OnBeforeInitialModuleScreenSetAsRoot();

            campaignSupport.displayMessageInChat("WoT Core loaded", Color.White);

        }

        protected override void OnSubModuleLoad()
        {

            base.OnSubModuleLoad();
            TextObject coreContentDisabledReason = new TextObject("Disabled during installation.", null);
            //Removes all the Menu Options (Initial State Options the mod doesn't need), Battle because it is broken, new Campaign because
            startScreenSupport.removeInitialStateOption("CustomBattle");
            startScreenSupport.removeInitialStateOption("StoryModeNewGame");
            startScreenSupport.removeInitialStateOption("Credits");
            startScreenSupport.removeInitialStateOption("SandBoxNewGame");
            


           
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("SandBoxNewGame", new TextObject("New WoT Campaign", null), 3, delegate ()
            {
                MBGameManager.StartNewGame(new WoTCampaignManager());
            }, () => new ValueTuple<bool, TextObject>(TaleWorlds.MountAndBlade.Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));

            InitialStateOption initialStateOption = new InitialStateOption("Donate", new TextObject("Donate"), 1233, Donate, menuFunc);
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(initialStateOption);

            Harmony harmony = new Harmony("WoT_Main.HarmonyPatches");
            harmony.PatchAll();

        }
        

        public override void OnMissionBehaviorInitialize(Mission mission)
        {

            base.OnMissionBehaviorInitialize(mission);

            mission.AddMissionBehavior(new NoFriendlyFire());

        }
      
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {

            base.OnGameStart(game, gameStarterObject);

            if(game.GameType is Campaign)
            {

                CampaignGameStarter starter = gameStarterObject as CampaignGameStarter;

                
                ConstantWars constantWars = new ConstantWars();

                starter.AddBehavior(constantWars);
                starter.AddBehavior(new ShayolGhulCaptureMechanic());
                starter.AddBehavior(new RandomEvents(constantWars));
                starter.AddBehavior(new ShadowAlwaysAtWar(constantWars));
                starter.AddBehavior(new PartyMapStuckFix());

                
            }
        }  
        


        private void Donate()
        {
            System.Diagnostics.Process.Start("https://www.patreon.com/WoT_Mod");
        }
        
        private (bool, TextObject) menuFunc()
        {
            return (false, null);
        }

       

      
      
    }
}