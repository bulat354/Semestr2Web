using RickAndMortyLibrary.Common.Game.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    /// <summary>
    /// Класс для импортирования карт.
    /// </summary>
    internal static class CardsImporter
    {
        private static ActionCard[] allActionCards;
        private static CharacterCard[] allCharacterCards;
        private static PersonalityCard[] allPersonalityCards;

        static CardsImporter()
        {
            allActionCards = new ActionCard[0];
            allCharacterCards = new CharacterCard[0];
            allPersonalityCards = new PersonalityCard[0];

            Import();
        }

        private static void Import()
        {

        }

        public static ICardsPack<ActionCard> GetActionCardsPack()
        {
            throw new NotImplementedException();
        }

        public static ICardsPack<CharacterCard> GetCharacterCardsPack()
        {
            throw new NotImplementedException();
        }

        public static ICardsPack<PersonalityCard> GetPersonalityCardsPack()
        {
            throw new NotImplementedException();
        }

        public static ICardsPack<PersonalityCard> GetPlayerPersonalityCardsPack()
        {
            throw new NotImplementedException();
        }
    }
}
