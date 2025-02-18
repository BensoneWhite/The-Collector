global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Security.Permissions;
global using System.Runtime.CompilerServices;
global using System.Reflection;
global using System.Text;
global using System.Threading.Tasks;
global using RWCustom;
global using UnityEngine;
global using Mono.Cecil.Cil;
global using MonoMod.Cil;
global using Random = UnityEngine.Random;
global using BepInEx;
global using static SlugBase.Features.FeatureTypes;
global using SlugBase.Features;
global using DressMySlugcat;
global using Menu.Remix.MixedUI;
global using UnityEngine.UIElements.Experimental;
global using SlugBase;
global using static Player;
global using BepInEx.Logging;
global using static TheCollector.Plugin;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]