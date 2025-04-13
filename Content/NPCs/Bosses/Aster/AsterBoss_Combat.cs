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
using Ascent.Core.Systems.Particles;

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
            StarBarrage,
            Orbit
        }

        private void SetUpPhases()
        {
            P1.Actions =
            [
                Action.StarBarrage
            ];
        }

        private void DoAction(Action action, int phase)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) 
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

                    case Action.Orbit:
                        Orbit();
                        break;

                    default:
                        EndAttack();
                        break;
                
                }
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
            if (timer[0] > 2000)
            {
                if (!AttackActive) { Speak(); }

                if (AttackIndex >= P1.Actions?.Length)
                {
                    AttackIndex = 0;
                }

                DoAction(P1.Actions[AttackIndex], 0);
            }
            else
            {
                 Move(NPC.Center, ActivePlayer.Center, 1f, 0.99f);
            }
        }

        #endregion

        #region Attacks

        int loopTracker = 0;

        private bool CanFire = true;

        //Tracked Target Vector: (ActivePlayer.velocity * ModMath.Delta(NPC.Center, ActivePlayer.Center).Length() / *Speed Function over distance*)

        private int letterToFrame(int letterID)
        {
            int Letter = 0;
            if(letterID < 91 & letterID > 64)
            {
                Letter = letterID - 64;
            }
            else if (letterID > 96 && letterID < 123)
            {
                Letter = letterID - 96;
            }
            return Letter;
        }

        private void StarBarrage()
        {
            SoundStyle test = SoundID.DD2_ExplosiveTrapExplode;

            if (timer[2] >= 30)
            {
                NPC.netUpdate = true;

                Shoot(ModContent.ProjectileType<MadStar>(), NPC.Center, ActivePlayer.Center + (ActivePlayer.velocity * Vector2.Distance(NPC.Center, ActivePlayer.Center) / 20), -5, false, NPC.damage, 1, NPC.target);
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