﻿using System;
using System.Reflection;
using System.Windows.Forms;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using SandBox.GauntletUI;
using SandBox.View.Map;

using HarmonyLib;
using CharacterCreation.Models;

namespace CharacterCreation
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleFolderName = "zzCharacterCreation";
        public static readonly string strings = "strings";

        // Main
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                var harmony = new Harmony("mod.bannerlord.popowanobi.dcc");
                harmony.PatchAll();

                TaleWorlds.Core.FaceGen.ShowDebugValues = true; // Developer facegen
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing Detailed Character Creation:\n{ex.Message} \n\n{ex.InnerException?.Message}");
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (!this._isLoaded)
            {
                InformationManager.DisplayMessage(new InformationMessage("Loaded Detailed Character Creation.", ColorManager.Orange));
                this._isLoaded = true;
            }
        }

        // Load our XML files
        private void LoadXMLFiles(CampaignGameStarter gameInitializer)
        {
            // Load our additional strings
            gameInitializer.LoadGameTexts(BasePath.Name + "Modules/" + ModuleFolderName + "/ModuleData/" + strings + ".xml");
        }

        // Called when loading save game
        public override void OnGameLoaded(Game game, object initializerObject)
        {
            CampaignGameStarter gameInitializer = (CampaignGameStarter)initializerObject;
            this.LoadXMLFiles(gameInitializer);
            TaleWorlds.Core.FaceGen.ShowDebugValues = true;
        }

        // Called when starting new campaign
        public override void OnNewGameCreated(Game game, object initializerObject)
        {
            CampaignGameStarter gameInitializer = (CampaignGameStarter)initializerObject;
            this.LoadXMLFiles(gameInitializer);
            TaleWorlds.Core.FaceGen.ShowDebugValues = false; // Disable until after game started.
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            if (!(game.GameType is Campaign))
            {
                return;
            }
            AddModels(gameStarterObject as CampaignGameStarter);

            game.EventManager.RegisterEvent(delegate (EncyclopediaPageChangedEvent e)
            {
                EncyclopediaData.EncyclopediaPages newPage = e.NewPage;
                if ((int)newPage != 12)
                {
                    this.selectedHeroPage = null;
                    this.selectedHero = null;
                    if (this.gauntletLayerTopScreen != null && this.gauntletLayer != null)
                    {
                        this.gauntletLayerTopScreen.RemoveLayer(this.gauntletLayer);
                        if (this.gauntletMovie != null)
                        {
                            this.gauntletLayer.ReleaseMovie(this.gauntletMovie);
                        }
                        this.gauntletLayerTopScreen = null;
                        this.gauntletMovie = null;
                    }
                    return;
                }
                GauntletEncyclopediaScreenManager gauntletEncyclopediaScreenManager = MapScreen.Instance.EncyclopediaScreenManager as GauntletEncyclopediaScreenManager;
                if (gauntletEncyclopediaScreenManager == null)
                {
                    return;
                }

                FieldInfo field = typeof(GauntletEncyclopediaScreenManager).GetField("_encyclopediaData", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field2 = typeof(EncyclopediaData).GetField("_activeDatasource", BindingFlags.Instance | BindingFlags.NonPublic);
                EncyclopediaData encyclopediaData = (EncyclopediaData)field.GetValue(gauntletEncyclopediaScreenManager);
                EncyclopediaPageVM encyclopediaPageVM = (EncyclopediaPageVM)field2.GetValue(encyclopediaData);
                this.selectedHeroPage = (encyclopediaPageVM as EncyclopediaHeroPageVM);

                if (this.selectedHeroPage == null)
                {
                    return;
                }
                this.selectedHero = (this.selectedHeroPage.Obj as Hero);
                if (this.selectedHero == null)
                {
                    return;
                }
                if (this.gauntletLayer == null)
                {
                    this.gauntletLayer = new GauntletLayer(211, "GauntletLayer");
                }

                try
                {
                    if (this.viewModel == null)
                    {
                        this.viewModel = new HeroBuilderVM(this.heroModel, delegate (Hero editHero)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Entering edit appearance for: " + editHero));
                        });
                    }
                    this.viewModel.SetHero(this.selectedHero);
                    this.gauntletMovie = this.gauntletLayer.LoadMovie("HeroEditor", this.viewModel);
                    this.gauntletLayerTopScreen = ScreenManager.TopScreen;
                    this.gauntletLayerTopScreen.AddLayer(this.gauntletLayer);
                    this.gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.MouseButtons);

                    // Refresh
                    this.selectedHeroPage.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error :\n{ex.Message} \n\n{ex.InnerException?.Message}");
                }
            });
        }

        private void AddModels(CampaignGameStarter gameStarter)
        {
            if (gameStarter != null)
            {
                gameStarter.AddModel(this.heroModel = new HeroBuilderModel());
                gameStarter.AddModel(new Models.AgeModel());
            }
        }

        private HeroBuilderVM viewModel;
        private EncyclopediaHeroPageVM selectedHeroPage;
        private HeroBuilderModel heroModel;
        private Hero selectedHero;
        private ScreenBase gauntletLayerTopScreen;
        private GauntletLayer gauntletLayer;
        private GauntletMovie gauntletMovie;

        private bool _isLoaded;
    }
}