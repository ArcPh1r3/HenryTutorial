# Overview
This tutorial will go over custom character creation for Risk of Rain 2. It assumes a basic understanding of Unity and C# at the very least. If you don't have that and get stuck, look up tutorials! I can't teach you everything here.

Disclaimer: I've never written tutorials before and am a horrible teacher so things may not be explained properly.

## Getting Started
Creating a character is a daunting task. Between modeling, animation, coding, skill icons, sounds, VFX and more there's a whole lot that goes into it. Don't expect it to be an easy process.

The first step would be cloning this repo and opening up `HenryMod.sln` in `Visual Studio`.

The recommended starting point is skill creation. [This](https://github.com/risk-of-thunder/R2Wiki/wiki/Custom-Skill-Creation-&-Adding-skills "Custom Skill Creation & Adding Skills") tutorial is a good place to start, HOWEVER- step 1 goes over adding the skill to a body, which is already streamlined with Henry. I recommend starting from step 2 and working with his existing skills to make what you want.

Creating skills is a pretty specific process that involves a lot of hard coding that greatly differs depending on what you want the skill to do, so there's not a whole lot that can be explained here. 3 basic types of attacks, OverlapAttacks (melee), BulletAttacks (hitscan) and Projectiles have been included as examples. 

Once you have skills down, then it's time to move on to creating assets. Of course you can always start with the assets if that's what you want.

### Model Import
The software generally used to create and animate models is [Blender](https://www.blender.org/)- even Hopoo himself uses it. There's lots of tutorials out there so Blender is the one I recommend. However if you're comfortable with something else feel free to use that.

When creating a character model, it's important to note due to reasons explained below that you're limited to having one material per mesh. There is, however, no limit to the amount of meshes you can have on your model.
 
When importing your character model into Unity, it's important to get the scaling right so things like teleporter/on fire particles and item displays will all be scaled properly.

The recommended method for this is exporting as FBX with these import/export settings:

![](https://cdn.discordapp.com/attachments/757459787117101096/809764180089503754/unknown.png) 


## Setting up your character prefab in Unity

You're gonna want Unity version 2018.4.16f, the one Risk of Rain 2 currently uses. Download [here](https://download.unity3d.com/download_unity/e6e9ca02b32a/Windows64EditorInstaller/UnitySetup64-2018.4.16f1.exe)

Once you have this, if you haven't already, grab Henry's Unity project from the repo. This is recommended over creating a new one so that you can use Henry as a reference as well as add several scripts I've included to make things easier.

Once that's done, follow these steps:

  1. Import your model. Drag it into the scene, right click it in the hierarchy and click `Unpack Prefab`. Then (optionally) rename the `GameObject` to "mdl" + whatever your character's name is. It doesn't have to follow the naming convention but it's good practice to do so.
  2. Add an `Animator`. I highly recommend copying Henry's animator and pasting your own animations in.
  3. Add a `ChildLocator` component to your object.
  4. Add a empty `GameObject` as a child of your object. Call this "MainHurtbox" and add a `Capsule Collider`. Scale it to fit your character and make sure the pivot of the object is set around the center of your character. This determines the `corePosition`, where enemies will aim at and where effects like `Berzerker's Pauldron` will show up.
  5. In your `ChildLocator`, create a new entry called "MainHurtbox" and drag your hurtbox object into it. Once you've done this the code will handle the rest.
  6. Create more entries in your `ChildLocator` for every mesh you have in your model and drag those in where necessary. The names don't matter because you'll fill in that part in the code.
  8. Drag your `GameObject` into the `Project` window to create a new prefab
  9. Once again, drag it into the `Project` window to create another prefab, and when prompted make a `Prefab Variant`. Then change the name of this to "MyCharacterNameDisplay" and change the `Animator` component if you want. This will be your display prefab used in the lobby.
  10. Once this is all done, open the AssetBundle Browser and create a new AssetBundle. Drag whatever assets you're using into there and build it. I like to have everything I need in a single folder and just drag that to keep things simple.
  11. Put the AssetBundle you've just created into your C# project and change the name in the Assets module to whatever you named it. There's plenty of tutorials on this so I won't be going over it.

If this was done right, you should be able to navigate to where you built the AssetBundle and see a game ready bundle. Next step is hooking it all up in your code.

## Setting up your character prefab in code

Assuming you've cloned Henry for this one, and have a basic understanding of how to navigate a C# project. Before you start, there's a few components that you're gonna be working with that it's important to understand.

#### CharacterBody

`CharacterBody` is the main component that handles most things relating to a character. This handles things like base stats, crosshairs, item interactions, etc. There's way too many things for me to go over so it's best to learn about this through experience. Henry's code takes care of all of the setup for this when creating your own body so don't stress over it too much.

#### HealthComponent

`HealthComponent` is pretty self-explanatory. It keeps track of character health and handles all instances of damage being dealt. When creating on-hit effects and such this is where you'll generally be hooking.

#### CharacterModel

CharacterModel is the component attached to every character model. It uses something called 'baseRendererInfos' to handle things like overlays and skins.

A `RendererInfo` consists of mainly a `renderer` (MeshRenderer, SkinnedMeshRenderer, etc. all work) and a `defaultMaterial`. The `ignoreOverlays` field (self-explanatory what this does) should be set to false if you encounter issues like weird shield outlines.

There is a `mainRenderer` field as well and in all honesty I haven't looked into what it's used for.

#### ChildLocator
 
A simple component that's used to easily reference child objects. This is useful for item displays, particle effects, muzzle flashes, and more. I make use of this component a lot to cleanly create characters.

#### InputBankTest

The `InputBank` is used to cache inputs from players as well as fake inputs from AI. You shouldn't need to use this much aside from a few skill interactions, like primary attack combos.

* * *

There's plenty more to go over but these are the main ones you need to know. Reading through the game code with `dnSpy` always helps. Now it's time to move on to Henry's code.


## Step 1 - Initial Setup

The first thing you wanna do is go through and change the name of the character to your own.

In the Solution Explorer, go through and rename every instance of `Henry` to something else. Then open up `HenryPlugin.cs`

![](https://cdn.discordapp.com/attachments/469291841859092488/814488315981725696/unknown.png) 

Navigate to this section: 

![](https://cdn.discordapp.com/attachments/469291841859092488/814488992040615936/unknown.png) 

Standard naming convention is `com.AuthorName.YourModName` so go ahead and fill in the fields accordingly. For the version, it's good practice to follow [Semantic Versioning](https://semver.org/). This isn't enforced but you'll probably be made fun of if you don't follow it.

Now after this, open up `Tokens.cs`

![](https://cdn.discordapp.com/attachments/469291841859092488/814513794398945320/unknown.png) 

Inside this class all our name tokens are stored for organization. Change any of these to your character and skill names and make sure to use the prefixes to avoid mod conflicts.

![](https://cdn.discordapp.com/attachments/469291841859092488/814513934970126436/unknown.png) 

Next you're going to want to create a class for the survivor you're creating. `SurvivorBase` is an abstract class that you can inherit from that takes care of most of the work for you. Included is an example of this, `MyCharacter`.

![](https://cdn.discordapp.com/attachments/469291841859092488/829080247047159828/unknown.png) 

Open up this (hopefully renamed) class and you'll see a bunch of fields and information about your character that you need to fill out.


## Step 2 - CharacterBody Setup

![](https://cdn.discordapp.com/attachments/469291841859092488/829080957264330782/unknown.png) 

This code right here is what sets up your entire `CharacterBody` prefab. You can check out the Prefabs module if you're curious to see how it all works, but this `CreatePrefab` method exists to make it pain free. Everything in it is fairly self-explanatory, base stats, crosshair prefab, survivor pod, name tokens...

A few things to note:

* `Assets.LoadCharacterIcon` assumes the name for your character icon follows the same naming convention of `texCharacterNameIcon`
* The names of all the crosshair prefabs can be found [here](https://github.com/risk-of-thunder/R2Wiki/wiki/Resources-Paths)
* Damage and regen scaling aren't listed here as there's a standard for those that should be followed, but there's nothing stopping you from changing it manually
* `sortPosition` controls where your character is placed in the character select menu
* Having duplicate body names tends to break the entire game so make sure the name you choose is something original. It's something the player never sees so it can be complete nonsense if you want.

If this is done properly, you'll now have a functioning character body! However there's still a few more steps.

## Step 3 - CharacterModel Setup

This step is crucial if you want your character to look good and authentic ingame. All that `ChildLocator` stuff from before, linking all the meshes, comes into play here.

![](https://cdn.discordapp.com/attachments/469291841859092488/829081977666666506/unknown.png) 

Pretty simple, we use `Assets.CreateMaterial` to create materials with Hopoo's shader based on our own materials made in Unity. Make sure the name matches here or it'll just use Commando's material.

To add emission and normal/bump maps, all you have to do is `Assets.CreateMaterial(name, i, color, j)`.

	name = Whatever the material is called in your Unity project
    i = Emission power, not sure what the limit here is but feel free to mess around with high numbers
    color = The color of the emission, generally just go with Color.white
	j = Normal strength
    
`mainRendererIndex` is used to keep track of which `RendererInfo` stores the info for your main body. Set this to 0 if your body is the only renderer, and try to keep the body last as that's what teleporter particles come from. Not hugely important but it's a small detail.

After you have some materials created, what follows is the magic code that adds a `CharacterModel` component with proper `RendererInfos` to your character.

`CustomRendererInfo` is a class defined in the `Prefabs` modules that's designed to store basic information that's used to create your `RendererInfos`. You're gonna need one for every renderer you linked in your `ChildLocator`.

`childName` would be the name of the transform pair while `material` would be whatever material you created just before this.

So, for example, if you had a character with a body mesh and a helmet mesh, you would do the following:

```
Modules.Prefabs.SetupCharacterModel(characterPrefab, new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "HelmetModel",
                    material = helmetMat,
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = characterMat,
                }}, bodyRendererIndex);
```

The `mainRendererIndex` at the end is used to set the `mainSkinnedMeshRenderer` field, you can ignore that. As the character model is second here, the renderer index for it would be set to `1`.

With all that taken care of, you should now have a working character that can be selected and played with ingame! If you run into any issues at this point be sure to go back through and make sure you've followed every step properly.

## Step 4 - Skill Setup

After all this only a few things remain to have a truly complete character.

`InitializeHitboxes` is there to create melee hitboxes.

![](https://cdn.discordapp.com/attachments/469291841859092488/829082643404423182/unknown.png) 

I'll admit it's not the cleanest and there's surely a way I can streamline this further. But from here, if you have a melee hitbox you want to add then you just need to set it up in the `ChildLocator` as you did with all the other things before. Change up some of this code to accommodate your needs and it should be good. See Henry's prefab for an example of what hitboxes are supposed to look like.

`InitializeSkills` method contains all the code for setting up your character's `SkillDefs`. It should be fairly self-explanatory and easy to work with.

![](https://cdn.discordapp.com/attachments/469291841859092488/814505978523418714/unknown.png) 

So this is about as simple as it can get. Using `Skills.CreateSkillDef` you create the `SkillDef` for your skill and fill in all the values with what you need.
- `activationState` is the state you're activating when you use the skill
- `activationStateMachineName` is a bit complicated, simply put you want `Weapon` if it's a skill that allows you to move while using it and `Body` if you don't want that
- `baseMaxStock` is the stock you start with
- `baseRechargeInterval` is the skill cooldown
- `beginSkillCooldownOnSkillEnd` determines whether the skill cooldown starts when the skill starts or ends
- `canceledFromSprinting` determines whether the skill is cancelled by sprint inputs
- `forceSprintDuringState` determines whether your character is forced to sprint during the skill
- `fullRestockOnAssign` should just be set to true
- `interruptPriority` will be explained down below
- `resetCooldownTimerOnUse` is for ammo primaries like Bandit's and Visions of Heresy, resets the cooldown to 0 on use
- `isCombatSkill` determines whether out of combat effects like `Red Whip` are cancelled on use
- `mustKeyPress` determines whether the skill can be activated by tapping or holding- false is recommended in most cases
- `cancelSprintingOnActivation` determines whether the skill cancels sprint on use
- `rechargeStock` is how many stocks you regain after every cooldown
- `requiredStock` is how many stocks are required to use the skill
- `stockToConsume` is how many stocks this skill loses on use
- `keywordTokens` is an array of strings that determines what keywords the skill has

So once you've created your `SkillDef`, `Skills.AddSecondarySkill()` will add it to the character pain free. You can add as many skills as you want with this method. For the rest of the skills you just need to do the same thing, but with `AddUtilitySkill()` and `AddSpecialSkill()`.

## Step 5 - Skins


Skin creation isn't streamlined as much as I'd like but it's not too bad to work with as of now. Inside the `CreateSkins` method, you'll see this code:

![](https://cdn.discordapp.com/attachments/469291841859092488/814510796918489098/unknown.png) 

Most of it can be ignored. It's just grabbing the necessary components and adding the `ModelSkinController` as well as creating a `List` of our `SkinDefs`. The line at the end is for `GameObjectActivations` field, which serves as an example of how to use it but should be removed for your character.

Not too keen on explaining it all since it's so poorly written but the rest of this method should be fairly easy to understand. If not feel free to harass me with your questions.

## Step 6 - Item Displays

Ah yes, who doesn't love 2400 lines of grueling, tedious code, doing nothing but copy pasting blocks and blocks of code and then typing in values.

Thankfully `KingEnderBrine` has made a tool that streamlines this process to an absurd degree, so no one else has to ever experience that pain again.

[ItemDisplayPlacementHelper](https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/)

All the code for the item displays is already written in `CreateItemDisplays`, so all you need to do is install this tool, run the game, press `F2` on the main menu, select your character and start dragging items around.

Seriously, this section would've been a lot longer without that tool.

## Step 7 - Vengeance Doppelganger

![](https://cdn.discordapp.com/attachments/469291841859092488/829083472084860988/unknown.png) 

Yeah. I haven't streamlined anything for AI creation beyond copying an existing AI.

If you're looking for actual AI creation then [`Regigigas`](https://github.com/ArcPh1r3/RegigigasMod/blob/master/RegigigasMod/Modules/Enemies/Regigigas.cs#L247) serves as a decent reference for now.

## Step 8 - Unlockables

I'll elaborate on this more sometime but unlockables are basically just hooking a game event like `onClientGameOverGlobal` for example and checking if you've met the proper conditions for the unlock.


There's plenty of examples of this in most of my repos, so I recommend checking those out. One thing to note is that on kill achievements don't network properly, only the host can unlock them. So when thinking of unlocks if possible try and avoid those for now.


Unlocks are optional I suppose, people like having them, but you don't need them.

--------------------

That should cover just about everything. If there's anything I'm missing or anything that doesn't make sense feel free to let me know in the modding discord.