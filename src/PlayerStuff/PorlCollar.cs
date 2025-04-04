﻿namespace TheCollector;

public class PorlCollar  
{
    public static void Init()
    {
        On.Player.GrabUpdate += GrabTheGoddamnPorlOrDontYourChoice;
        On.Player.GraphicsModuleUpdated += UpdatePorlGraphicsModuleYeBloodyEejit;
        //On.Player.ctor += Player_ctor;
    }

    //private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    //{
    //    orig(self, abstractCreature, world);
    //}

    private static void GrabTheGoddamnPorlOrDontYourChoice(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        orig(self, eu);
        try
        {
            if(!self.IsTheCollector()) return;
        }
        catch (Exception err)
        {
            DebugError("Oh crap, another issue occured while checking for scug name while updating grab!!");
            DebugError(err);
            return;
        }
        for (int index = 0; index < self.grasps.Length; index++)
        {
            if (self.grasps[index]?.grabbed is IPlayerEdible) return;
        }
        if (self.Yippee().porlztorage != null)
        {
            self.Yippee().porlztorage.increment = self.input[0].pckp;
            self.Yippee().porlztorage.Update(eu, self);
        }

    }
    private static void UpdatePorlGraphicsModuleYeBloodyEejit(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
    {
        try
        {
            if (!self.IsTheCollector())
            {
                orig(self, actuallyViewed, eu);
                return;
            }
        }
        catch (Exception err)
        {
            orig(self, actuallyViewed, eu);
            DebugWarning("Oh crap, another issue occured while checking for scug name while updating graphics module!");
            DebugError(err);
            return;
        }
        self.Yippee().porlztorage?.GraphicsModuleUpdated(actuallyViewed, eu);
        orig(self, actuallyViewed, eu);
    }

}