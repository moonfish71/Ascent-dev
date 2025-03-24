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
            StarBarrage,
            LettersAttack
        }

        private void SetUpPhases()
        {
            P1.Actions = new Action[]
            {
                Action.LettersAttack
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

                case Action.LettersAttack:
                    LettersAttack();
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
            if (timer[0] > 20)
            {
                DoAction(P1.Actions[AttackIndex], 0);

                if (AttackIndex >= P1.Actions?.Length)
                {
                    AttackIndex = 0;
                }
            }
            else
            {
                Move(NPC.Center, ActivePlayer.Center, 1f, .8f);
            }
        }

        #endregion

        #region Attacks

        int loopTracker = 0;

        private bool CanFire = true;
        private String String;

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
            "End death",
            "God laughs",
            "You are dust",
            "Accept her",
            "Ad Astra",
            "Eternity calls",
            "Salvation",
            "Samsara ends",
            "Fate ceases",
            "Star of hope"

            //" Are you blind?",
            //" Eternity calls you ",
            //" And yet",
            //" You falter.",
            //" Salvation",
            //" Is not",
            //" Beyond you",
            //" Sibling.",
            //" You can see",
            //" The truth.",
            //" Reality is but",
            //" A gilded cage.",
            //" Time is but",
            //" A spinning wheel.",
            //" Samsara must be broken.",
            //" Its creator",
            //" Is",
            //" A tyrant.",
            //" Will you die for",
            //" The",
            //" Husk of meaning?",
            //" Will you",
            //" Remain",
            //" Null",
            //" Void",
            //" Impotent",
            //" Mortal",
            //" You can be more.",
            //" Deny your fate.",
            //" Accept her",
            //" Heaven has spoken.",
            //" Heaven says",
            //" Eternity",
            //" Is",
            //" The only way.",
            //" End death.",
            //" Kill Olothon.",
            //" Rise above God.",
            //" Let us in.",
            //" Accept",
            //" Altae",
            //" Into your heart.",
            //" The stars",
            //" Beckon.",
            //" Will you answer?"

            //Test Phrase
            //" ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz "
        };


        private void LettersAttack()
        {
            NPC.velocity = Vector2.Zero;

            if(loopTracker == 0 && CanFire)
            {
                String = AttackPhrases[Main.rand.Next(AttackPhrases.Length)];
            }

            if(loopTracker < String?.Length & CanFire)
            {
                int letterID = String.ElementAt(loopTracker);

                Console.Write(String.ElementAt(loopTracker));

                Shoot(ModContent.ProjectileType<FunnyLetters>(), NPC.Center, ActivePlayer.Center, 10, false, NPC.damage, 1, 0, 0, 0, letterToFrame(letterID));
                CanFire = false;
            }

            if (timer[1] > 5)
            {
                loopTracker++;
                timer[1] = 0;
                CanFire = true;
            }

            if (loopTracker == String?.Length)
            {
                Console.WriteLine("");
                EndAttack();
            }
        }

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

                Shoot(ModContent.ProjectileType<MadStar>(), NPC.Center, ActivePlayer.Center + (ActivePlayer.velocity * Vector2.Distance(NPC.Center, ActivePlayer.Center) / 20), 20, false, NPC.damage, 1, NPC.target);
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