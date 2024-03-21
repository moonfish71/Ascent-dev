using Ascent.Content.NPCs.Templates;
using Ascent.Content.Projectiles.Hostile.BossAttacks.Aster;
using Ascent.Core;
using Ascent;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Ascent.Content.NPCs.Bosses.Aster
{
    public partial class AsterBoss : AscentNPC
    {

        #region Attack Processing

        struct AttackCycle
        {
            public Action[] Actions;
        }

        private AttackCycle IntroCutscene = new AttackCycle();
        private AttackCycle PhaseTransition = new AttackCycle();

        private AttackCycle P1 = new AttackCycle();
        private AttackCycle P2 = new AttackCycle();

        private enum Action
        {
            None,
            Crash,
            StarBarrage
        }

        private void SetUpPhases()
        {
            P1.Actions = new Action[]
            {
                Action.StarBarrage
            };
        }

        private void DoAction(Action action, int phase)
        {
            AttackActive = true;

            timer[1]++;
            timer[2]++;

            switch (action)
            {
                case Action.StarBarrage:
                    StarBarrage();
                    break;

                case Action.Crash:
                    AsterCrash();
                    break;

                default:
                    EndAttack();
                    break;
            }
        }

        private void EndAttack()
        {
            timer[0] = 0;
            timer[1] = 0;
            timer[2] = 0;

            AttackActive = false;
            AttackIndex++;
            loopTracker = 0;
        }
        #endregion

        #region Phases

        private void Phase1()
        {
            if (timer[0] > 100)
            {
                DoAction(P1.Actions[AttackIndex], 0);

                if (AttackIndex >= P1.Actions?.Length)
                {
                    AttackIndex = 0;
                }
            }
            else
            {
                Move(NPC.Center, ActivePlayer.Center, 1f, .9f);
            }
        }

        #endregion

        #region Attacks

        int loopTracker = 0;

        //Tracked Target Vector: (ActivePlayer.velocity * ModMath.Delta(NPC.Center, ActivePlayer.Center).Length() / *Speed*)

        //Lore time!!!! (I'll make better ones later)
        public static string[] AttackPhrases = new string[]
        {
            "Altae",
            "Let us in",
            "Kill Olothon",
            "The stars",
            "One truth",
            "Eternal",
            "A gilded cage",
            "A broken wheel",
            "Heaven says",
            "Angel",
            "Above",
            "Deny your fate",
            "Husk of meaning",
            "She awaits you",
            "A thousand eyes",
            "Escape with us",
            "Void",
            "Null",
            "Impotent",
            "Mortal",
            "No future",
            "The only way",
            "End death"

            //Test Phrase
            //" ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz "
        };

        private void LettersAttack()
        {
            //String String = AttackPhrases[Main.rand.Next(AttackPhrases.Length)];
            //for (int i = 0; i < String?.Length; i++)
            //{
            //    int letterID;

            //    letterID = String.ElementAt(i);

            //    Console.WriteLine(String.ElementAt(i) + " | " + letterID);
            //}
        }

        private void StarBarrage()
        {
            SoundStyle test = SoundID.DD2_ExplosiveTrapExplode;

            if (timer[2] >= 30)
            {
                NPC.netUpdate = true;

                Shoot(ModContent.ProjectileType<MadStar>(), NPC.Center, ActivePlayer.Center + (ActivePlayer.velocity * ModMath.Delta(NPC.Center, ActivePlayer.Center).Length() / 20), 20, false, NPC.damage, 1, NPC.target);
                timer[2] = 0;

                NPC.netUpdate = false;
            }

            Move(NPC.Center, ActivePlayer.Center, .6f, .9f);

            if (timer[1] > ModMath.SecondsToTicks(5))
            {
                SoundEngine.PlaySound(test);
                EndAttack();
            }
        }

        private void Orbit()
        {
            EndAttack();
        }

        private void AsterCrash()
        {
            EndAttack();
        }

        #endregion
    }
}