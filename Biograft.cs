using MelonLoader;
using BTD_Mod_Helper;
using Biograft;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Models.Powers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2Cpp;
using Il2CppSystem.IO;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

[assembly: MelonInfo(typeof(Biograft.Biograft), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Biograft;

public class Biograft : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<Biograft>("Biograft loaded!");
    }
}

public class BiograftHero : ModHero
{
    public override string BaseTower => TowerType.Sauda;

    public override int Cost => 1000;

    public override string DisplayName => "Biograft";
    public override string Title => "Model Zeta";
    public override string Level1Description => "Slashes with both swords at nearby Bloons. ";
    public override bool Use2DModel => true;
    public override string Description =>
        "Slashes with both swords at nearby Bloons.";


    public override string NameStyle => TowerType.Etienne; // Yellow colored
    public override string BackgroundStyle => TowerType.Etienne; // Yellow colored
    public override string GlowStyle => TowerType.Etienne; // Yellow colored
    public override float XpRatio => 1.5f;
    public override int MaxLevel => 11;


    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.GetWeapon().rate = 0.4f;
        towerModel.GetWeapon().projectile.pierce = 1;
        //"Hitting enemies with Dual Swing temporarily increases your attack speed..."
        towerModel.AddBehavior(new DamageBasedAttackSpeedModel("DamageBasedAttackSpeedModel_PowerofBiograft", 1, 120, 0.025f, 150));
        //"Reaching full heat disables Dual Swing for half a second.."
        //towerModel.AddBehavior(new DamageBasedAttackSpeedModel("DamageBasedAttackSpeedModel_Overheat", 100, 30, 999, 1));

        towerModel.GetWeapon().AddBehavior(new AlternateProjectileModel("AlternateProjectileModel_", towerModel.GetWeapon().projectile.Duplicate(), towerModel.GetWeapon().emission.Duplicate(), 5));
        towerModel.GetWeapon().GetBehavior<AlternateProjectileModel>().projectile.RemoveBehavior<DamageModel>();
        //towerModel.ApplyDisplay<TowerModel>(ShrapnelDisplay);
        foreach (var damageModel in towerModel.GetDescendants<DamageModel>().ToArray())
        {
            damageModel.immuneBloonProperties = (BloonProperties)9;
        }
        towerModel.GetBehavior<CreateSoundOnTowerPlaceModel>().heroSound1.assetId = GetAudioSourceReference("OnSwich");
        towerModel.GetBehavior<CreateSoundOnTowerPlaceModel>().heroSound2.assetId = GetAudioSourceReference("OnSwich");
        towerModel.GetDescendant<CreateSoundOnProjectileCreatedModel>().sound1.assetId = GetAudioSourceReference("RightSwing");
        towerModel.GetDescendant<CreateSoundOnProjectileCreatedModel>().sound2.assetId = GetAudioSourceReference("LeftSwing");
        towerModel.GetDescendant<CreateSoundOnProjectileCreatedModel>().sound3.assetId = GetAudioSourceReference("RightSwing");
        towerModel.GetDescendant<CreateSoundOnProjectileCreatedModel>().sound4.assetId = GetAudioSourceReference("LeftSwing");
        towerModel.GetDescendant<CreateSoundOnProjectileCreatedModel>().sound5.assetId = GetAudioSourceReference("RightSwing");
    }
}
public class Levels
{
    public class Level2 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Increased popping power.";

        public override int Level => 2;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetWeapon().projectile.pierce++;
            //towerModel.GetBehavior<DamageBasedAttackSpeedModel>().damageThreshold++;
        }
    }
    public class Level3 : ModHeroLevel<BiograftHero>
    {
        public override string AbilityName => "Slash Storm";

        public override string AbilityDescription =>
            "";

        public override string Description => $"{AbilityName}: {AbilityDescription}";

        public override int Level => 3;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var quincy = Game.instance.model.GetTowerFromId(TowerType.Gwendolin + " 7");
            //var glue = Game.instance.model.GetTowerWithName(TowerType.GlueGunner + "-300");
            //var glueStorm = Game.instance.model.GetTowerWithName(TowerType.GlueGunner + "-040");

            var abilityModel = quincy.GetAbility().Duplicate();
            abilityModel.name = "AbilityModel_SlashStorm";
            abilityModel.displayName = AbilityName;
            abilityModel.addedViaUpgrade = Id;
            abilityModel.icon = GetSpriteReference("SlashStorm");
            abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
            //abilityModel.AddBehavior(glueStorm.GetDescendant<CreateSoundOnAbilityModel>());
            abilityModel.Cooldown = 10;
            var balls = abilityModel.GetDescendant<CreateProjectileOnExhaustFractionModel>().projectile;
            balls.pierce = 60/12;
            balls.GetDescendant<AgeModel>().lifespan = 2f;
            balls.GetDescendant<AgeModel>().Lifespan = 2f;
            towerModel.AddBehavior(abilityModel);
        }
    }
    public class Level4 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Biograft's swords slice through 2 layers of Bloons.";

        public override int Level => 4;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetWeapon().projectile.GetDamageModel().damage++;
            towerModel.GetBehavior<DamageBasedAttackSpeedModel>().damageThreshold += 1;
        }
    }
    public class Level5 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Biograft can detect Camo Bloons.";

        public override int Level => 5;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
        }
    }
    public class Level6 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Increased popping power.";

        public override int Level => 6;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetWeapon().projectile.pierce++;
            //towerModel.GetBehavior<DamageBasedAttackSpeedModel>().damageThreshold += 1;
        }
    }
    public class Level7 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Biograft can now pop Lead Bloons.";

        public override int Level => 7;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var damageModel in towerModel.GetDescendants<DamageModel>().ToArray())
            {
                damageModel.immuneBloonProperties &= ~BloonProperties.Lead;
            }
        }
    }
    public class Level8 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Increased attack range.";

        public override int Level => 8;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range += 15;
        }
    }
    public class Level9 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Increased attack range.";

        public override int Level => 9;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            TowerModel dartling = Game.instance.model.GetTowerFromId(TowerType.DartlingGunner + "-200");
            AddBehaviorToBloonModel electricShock = dartling.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            foreach (var weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                weaponModel.projectile.AddBehavior(electricShock);
                weaponModel.projectile.collisionPasses = new[] { 0, 1 };
            }
        }
    }
    public class Level10 : ModHeroLevel<BiograftHero>
    {
        public override string AbilityName => "Overdrive";

        public override string AbilityDescription =>
            "";

        public override string Description => $"{AbilityName}: {AbilityDescription}";

        public override int Level => 10;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var quincy = Game.instance.model.GetTowerFromId("BoomerangMonkey-040");
            //var glue = Game.instance.model.GetTowerWithName(TowerType.GlueGunner + "-300");
            //var glueStorm = Game.instance.model.GetTowerWithName(TowerType.GlueGunner + "-040");

            var abilityModel = quincy.GetAbility().Duplicate();
            abilityModel.name = "AbilityModel_Overdrive";
            abilityModel.displayName = AbilityName;
            abilityModel.addedViaUpgrade = Id;
            //abilityModel.icon = GetSpriteReference("SlashStorm");
            //abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
            //abilityModel.AddBehavior(glueStorm.GetDescendant<CreateSoundOnAbilityModel>());
            abilityModel.GetDescendant<TurboModel>().Lifespan = 30;
            abilityModel.livesCost = 10;
            abilityModel.Cooldown = 90;
            towerModel.AddBehavior(abilityModel);
            
        }
    }
    public class Level11 : ModHeroLevel<BiograftHero>
    {
        public override string Description => "Increased attack range.";

        public override int Level => 11;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            TowerModel dartling = Game.instance.model.GetTowerFromId(TowerType.DartlingGunner + "-200");
            AddBehaviorToBloonModel electricShock = dartling.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            foreach (var weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                weaponModel.projectile.AddBehavior(electricShock);
                weaponModel.projectile.collisionPasses = new[] { 0, 1 };
            }
        }
    }
}