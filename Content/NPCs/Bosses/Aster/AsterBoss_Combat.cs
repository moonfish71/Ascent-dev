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

                timer[1]++;
                timer[2]++;
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
            if (timer[0] > ModMath.SecondsToTicks(5))
            {
                if (!AttackActive) { Speak(); timer[1] = 0; }

                if (AttackIndex >= P1.Actions?.Length)
                {
                    AttackIndex = 0;
                }

                DoAction(P1.Actions[AttackIndex], 0);
            }
            else
            {
                if (timer[1] < 15)
                {
                    CamBias = MathHelper.Lerp(CamBias, 10, ModMath.easeInOutQuad(timer[1] / 15));
                }

                Move(NPC.Center, ActivePlayer.Center, 0.5f, 0.95f);
            }
        }

        #endregion

        #region Attacks

        int loopTracker = 0;

        private bool CanFire = true;

        Vector2 Start;
        Vector2 End;
        float ZVel = 0;

        float orbitDir = 1;

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

            if(CamBias < 50)
            {
                CamBias++;
            }

            if (timer[2] > 61)
            {
                NPC.netUpdate = true;

                Shoot(ModContent.ProjectileType<MadStar>(), NPC.Center, ActivePlayer.Center, -10, false, NPC.damage / 5, 1, NPC.target);
                timer[2] = 0;

                Speak("LO");

                orbitDir *= -1;

                loopTracker += 1;

                NPC.netUpdate = false;
            }
            else
            {
                if (timer[2] <= 1)
                {
                    NPC.netUpdate = true;
                    Start = NPC.Center;

                    if (loopTracker <= 9)
                    {
                        End = ActivePlayer.Center - 400 * Vector2.Normalize(Start - ActivePlayer.Center).RotatedByRandom(1 * Math.PI / 2);
                    }
                    else
                    {
                        Vector2 offset = Vector2.One;
                        if(Start.X - ActivePlayer.Center.X < 0)
                        {
                            offset = new Vector2((float)(1 /Math.Sqrt(2)), (float)(-1/Math.Sqrt(2)));
                        }
                        else
                        {
                            offset = new Vector2((float)(-1 / Math.Sqrt(2)), (float)(-1 / Math.Sqrt(2)));
                        }

                        End = ActivePlayer.Center + 400 * offset;
                    }
                    NPC.netUpdate = false;
                }
                else
                {

                    End += ActivePlayer.velocity;

                    NPC.velocity = .5f * (float)Math.PI / 120f * (float)Math.Sin(Math.PI * (timer[2] - 1) / 60) * (End - Start);

                    NPC.Center = Vector2.Lerp(Start, End, (float)-(Math.Cos(Math.PI * (timer[2] - 1) / 60) - 1) / 2);
                    Z = orbitDir * -Vector2.Distance(Start, End) * (float)Math.Sin(Math.PI * (timer[2] - 1) / 60);
                    ZVel = orbitDir * -Vector2.Distance(Start, End) * (float)(Math.PI / 60f) * (float)Math.Cos(Math.PI * (timer[2] - 1) / 60);
                }
            }

            if (loopTracker > 10)
            {
                SoundEngine.PlaySound(test);
                Z = 0;
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