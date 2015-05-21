using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;



namespace RengarSweg
{
    internal class Program
    {
        public static Menu Menu { get; set; }

        private static Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        private static Orbwalking.Orbwalker orbwalker;

        private static Spell q, w, e;

        private static int combo;

        private static int mixed;
    

    private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += args1 => Game_OnGameLoad(args1);
        }

        private static void Game_OnGameLoad(EventArgs args1)
        {
            throw new NotImplementedException();
        }

        private static void Game_OnGameLoad(EventArgs args, DrawingDraw drawingOnDraw)
        {
            if (Player.ChampionName != "Rengar") return;

            q = new Spell(SpellSlot.Q, 0f);
            w = new Spell(SpellSlot.W, 500f);
            e = new Spell(SpellSlot.E, 950f);

            q.SetTargetted(0.5f, 200f);
            e.SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.SkillshotLine);
            w.SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu mixedMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            mixedMenu.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
            mixedMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));

            Menu.AddToMainMenu();
            Drawing.OnDraw += drawingOnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            // Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

        }

        private static void Interrupter2_OnInterruptableTarget(
            Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && sender.Distance(Player) < q.Range)
            {
                q.CastOnUnit(sender);
            }
        }

        //static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (sender is Obj_AI_Hero && sender.IsEnemy && sender.Distance(Player) < Q.Range)
        //    {
        //        Console.WriteLine("Spell Name: {0}  Channel Duration: {1}",args.SData.Name,args.SData.c);
        //        if(args.SData.ChannelDuration > .35)
        //        {
        //            Q.CastOnUnit(sender);
        //        }
        //    }
        //}


        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Savagery(null);
                Battleroar();
                BolaStrike();
            }

            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Savagery(null);
                Battleroar();
                BolaStrike();
            }

        }

        private static void Savagery(Obj_AI_Base target)
        {
            if (!Menu.Item("useQ").GetValue<bool>()) return;

            if (q.IsReady())
            {
                int enemies = ObjectManager.Get<Obj_AI_Hero>().Count(x => x.IsEnemy && x.Distance(Player, false) < 200);


                if (target.IsValidTarget(q.Range))
                {
                    q.CastOnUnit(target);
                }
            }
        }


        private static void Battleroar()
        {
            if (!Menu.Item("useW").GetValue<bool>()) return;

            if (w.IsReady())
            {
                int enemies =
                    ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && x.Distance(Player, false) < 500).Count();

                if (enemies > 0)
                {
                    w.Cast();
                }
            }
        }


        private static void BolaStrike()
        {
            if (!Menu.Item("useE").GetValue<bool>()) return;

            if (e.IsReady())
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(1000f, TargetSelector.DamageType.Physical);

                if (target.IsValidTarget(e.Range))
                {
                    e.Cast(target.Position);
                }
            }
        }
    }
}

            

 



