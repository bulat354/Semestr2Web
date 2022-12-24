using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI
{
    public class Character
    {
        public CharacterCard Card { get; set; }
        public PersonalityCard Personality { get; set; }
        public CharacterTag Tag { get; set; }
        public IPlayerController? Player;

        public CardColor Color => Card.Color;
        public int CardId => Card.Id;
        public int PersonId => Personality.Id;
        public Person Person => Personality.Person;
        public int PlayerId => Player == null ? -1 : Player.Id;

        public void SwapPersons(Character character)
        {
            if (character.Tag.HasFlag(CharacterTag.Immutable) || Tag.HasFlag(CharacterTag.Immutable))
                return;

            var tmp = character.Personality;
            character.Personality = Personality;
            Personality = tmp;

            if (character.Player != null)
                character.Player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, character.PersonId.ToString(), MessageSecondGoal.Attach));
            if (Player != null)
                Player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, PersonId.ToString(), MessageSecondGoal.Attach));
        }
    }
}
