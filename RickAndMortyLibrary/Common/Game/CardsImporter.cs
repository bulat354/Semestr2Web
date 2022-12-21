using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common.Game
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
            var path = "images/";
            var actionCardsPath = path + "actionCards/";
            allActionCards = new ActionCard[]
            {
                new ActionCard(0,  "Мистер Жопосранчик", actionCardsPath + "singleCards/white1.jpg", CardColor.None, 1),
                new ActionCard(1,  "Я тоже!", actionCardsPath + "singleCards/white2.jpg", CardColor.None, 1),
                new ActionCard(2,  "Я пытался пристрелить Саммер 10 минут назад!", actionCardsPath + "singleCards/white3.jpg", CardColor.None, 1),
                new ActionCard(3,  "Я знаю тебя уже 15 лет", actionCardsPath + "singleCards/years15Blue.jpg", CardColor.Blue, 1),
                new ActionCard(4,  "Я знаю тебя уже 15 лет", actionCardsPath + "singleCards/years15Green.jpg", CardColor.Green, 1),
                new ActionCard(5,  "Я знаю тебя уже 15 лет", actionCardsPath + "singleCards/years15Red.jpg", CardColor.Red, 1),
                new ActionCard(6,  "Джерри", actionCardsPath + "doubleCards/jarryBlue.jpg", CardColor.Blue, 2),
                new ActionCard(7,  "Джерри", actionCardsPath + "doubleCards/jarryGreen.jpg", CardColor.Green, 2),
                new ActionCard(8,  "Джерри", actionCardsPath + "doubleCards/jarryRed.jpg", CardColor.Red, 2),
                new ActionCard(9,  "Морти", actionCardsPath + "tripleCards/mortyBlue.jpg", CardColor.Blue, 3),
                new ActionCard(10, "Морти", actionCardsPath + "tripleCards/mortyGreen.jpg", CardColor.Green, 3),
                new ActionCard(11, "Морти", actionCardsPath + "tripleCards/mortyRed.jpg", CardColor.Red, 3),
                new ActionCard(12, "Саммер", actionCardsPath + "tripleCards/summerBlue.jpg", CardColor.Blue, 3),
                new ActionCard(13, "Саммер", actionCardsPath + "tripleCards/summerGreen.jpg", CardColor.Green, 3),
                new ActionCard(14, "Саммер", actionCardsPath + "tripleCards/summerRed.jpg", CardColor.Red, 3),
                new ActionCard(15, "Что-то не так, Бет?", actionCardsPath + "tripleCards/white4.jpg", CardColor.None, 3),
                new ActionCard(16, "Стерва-сеструха", actionCardsPath + "tripleCards/white5.jpg", CardColor.None, 3),
                new ActionCard(17, "Ты не можешь меня убить!", actionCardsPath + "tripleCards/white6.jpg", CardColor.None, 3),
                new ActionCard(18, "Бет", actionCardsPath + "fiveCards/betBlue.jpg", CardColor.Blue, 5),
                new ActionCard(19, "Бет", actionCardsPath + "fiveCards/betGreen.jpg", CardColor.Green, 5),
                new ActionCard(20, "Бет", actionCardsPath + "fiveCards/betRed.jpg", CardColor.Red, 5),
                new ActionCard(21, "Рик", actionCardsPath + "sixCards/rick.jpg", CardColor.None, 6),
            };

            var characterCardsPath = path + "characterCards/";
            allCharacterCards = new CharacterCard[]
            {
                new CharacterCard(0,  "Мистер Борегар", characterCardsPath + "blueCards/characterBlue1.jpg", CardColor.Blue),
                new CharacterCard(1,  "Мистер Жопосранчик", characterCardsPath + "blueCards/characterBlue2.jpg", CardColor.Blue),
                new CharacterCard(2,  "Рыболюд", characterCardsPath + "blueCards/characterBlue3.jpg", CardColor.Blue),
                new CharacterCard(3,  "Большой робот", characterCardsPath + "blueCards/characterBlue4.jpg", CardColor.Blue),
                new CharacterCard(4,  "Мясомурай", characterCardsPath + "blueCards/characterBlue5.jpg", CardColor.Blue),
                new CharacterCard(5,  "Мужикорог", characterCardsPath + "blueCards/characterBlue6.jpg", CardColor.Blue),
                new CharacterCard(6,  "Призрак в банке", characterCardsPath + "blueCards/characterBlue7.jpg", CardColor.Blue),
                new CharacterCard(7,  "Тинклз", characterCardsPath + "blueCards/characterBlue8.jpg", CardColor.Blue | CardColor.Red | CardColor.Green),
                new CharacterCard(8,  "Бездомная кукла", characterCardsPath + "greenCards/characterGreen1.jpg", CardColor.Green),
                new CharacterCard(9,  "Мальчик из бассейна", characterCardsPath + "greenCards/characterGreen2.jpg", CardColor.Green),
                new CharacterCard(10, "Сонный Гэри", characterCardsPath + "greenCards/characterGreen3.jpg", CardColor.Green),
                new CharacterCard(11, "Чудовище Франкенштейна", characterCardsPath + "greenCards/characterGreen4.jpg", CardColor.Green),
                new CharacterCard(12, "Ковбойчик на собаке", characterCardsPath + "greenCards/characterGreen5.jpg", CardColor.Green),
                new CharacterCard(13, "Раптор-фотограф", characterCardsPath + "greenCards/characterGreen6.jpg", CardColor.Green),
                new CharacterCard(14, "Большая утка", characterCardsPath + "greenCards/characterGreen7.jpg", CardColor.Green),
                new CharacterCard(15, "Карандашурик", characterCardsPath + "greenCards/characterGreen8.jpg", CardColor.Green),
                new CharacterCard(16, "Малыш-волшебник", characterCardsPath + "redCards/characterRed1.jpg", CardColor.Red),
                new CharacterCard(17, "Надувной пёс", characterCardsPath + "redCards/characterRed2.jpg", CardColor.Red),
                new CharacterCard(18, "Бородатая дама", characterCardsPath + "redCards/characterRed3.jpg", CardColor.Red),
                new CharacterCard(19, "Кузен Никки", characterCardsPath + "redCards/characterRed4.jpg", CardColor.Red),
                new CharacterCard(20, "Жираф наоборот", characterCardsPath + "redCards/characterRed5.jpg", CardColor.Red),
                new CharacterCard(21, "Дядя Стив", characterCardsPath + "redCards/characterRed6.jpg", CardColor.Red),
                new CharacterCard(22, "Миссис Холодильник", characterCardsPath + "redCards/characterRed7.jpg", CardColor.Red),
                new CharacterCard(23, "Киборг-амиш", characterCardsPath + "redCards/characterRed8.jpg", CardColor.Red),
            };

            var personalityCardsPath = path + "personalityCards/";

            allPersonalityCards = new PersonalityCard[]
            {
                new PersonalityCard(0, "Паразит", personalityCardsPath + "enemy.jpg", 18, Person.Parasite),
                new PersonalityCard(1, "Друг", personalityCardsPath + "friend.jpg", 6, Person.Friend),
            };
        }

        private static List<int> GetFullPack<T>(T[] cards)
            where T : Card
        {
            var pack = new List<int>();
            for (int j = 0; j < cards.Length; j++)
            {
                var card = cards[j];
                for (int i = 0; i < card.InPackCount; i++)
                    pack.Add(j);
            }
            return pack;
        }

        public static ICardsPack<ActionCard> GetActionCardsPack()
        {
            return new CardsPack<ActionCard>().Init(GetFullPack(allActionCards));
        }

        public static ICardsPack<CharacterCard> GetCharacterCardsPack()
        {
            return new CardsPack<CharacterCard>().Init(GetFullPack(allCharacterCards));
        }

        public static ICardsPack<PersonalityCard> GetPersonalityCardsPack()
        {
            return new CardsPack<PersonalityCard>().Init(GetFullPack(allPersonalityCards));
        }

        public static ICardsPack<PersonalityCard> GetPlayerPersonalityCardsPack()
        {
            var list = new List<int>()
            {
                0, 0,
                1, 1, 1, 1
            };
            return new CardsPack<PersonalityCard>().Init(list);
        }

        private static Card GetAction(int id)
        {
            if (id >= allActionCards.Length || id < 0)
                throw new ArgumentOutOfRangeException();

            return allActionCards[id];
        }

        private static Card GetCharacter(int id)
        {
            if (id >= allActionCards.Length || id < 0)
                throw new ArgumentOutOfRangeException();

            return allCharacterCards[id];
        }

        private static Card GetPersonality(int id)
        {
            if (id >= allActionCards.Length || id < 0)
                throw new ArgumentOutOfRangeException();

            return allPersonalityCards[id];
        }

        public static T GetCard<T>(int id) where T : Card
        {
            if (typeof(T) == typeof(ActionCard))
                return (T)GetAction(id);

            if (typeof(T) == typeof(CharacterCard))
                return (T)GetCharacter(id);

            if (typeof(T) == typeof(PersonalityCard))
                return (T)GetPersonality(id);

            throw new Exception();
        }
    }
}
