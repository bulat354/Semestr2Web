using RickAndMortyLibrary.Common.Game;
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
    /// <summary>
    /// Класс для импортирования карт.
    /// </summary>
    internal static class CardsImporter
    {
        private static ActionCard[] allActionCards;
        private static CharacterCard[] allCharacterCards;
        private static PersonalityCard[] allPersonalityCards;
        private static PersonalityCard[] playerPersonalityCards;

        static CardsImporter()
        {
            var actionCardsPath = AppDomain.CurrentDomain.BaseDirectory + "/images/actionCards/";
            allActionCards = new ActionCard[]
            {
                new ActionCard("Мистер Жопосранчик", actionCardsPath + "singleCards/white1.jpg", CardColor.None, 1),
                new ActionCard("Я тоже!", actionCardsPath + "singleCards/white2.jpg", CardColor.None, 1),
                new ActionCard("Я пытался пристрелить Саммер 10 минут назад!", actionCardsPath + "singleCards/white3.jpg", CardColor.None, 1),
                new ActionCard("Я знаю тебя уже 15 лет", actionCardsPath + "singleCards/years15Blue.jpg", CardColor.Blue, 1),
                new ActionCard("Я знаю тебя уже 15 лет", actionCardsPath + "singleCards/years15Green.jpg", CardColor.Green, 1),
                new ActionCard("Я знаю тебя уже 15 лет", actionCardsPath + "singleCards/years15Red.jpg", CardColor.Red, 1),
                new ActionCard("Джерри", actionCardsPath + "doubleCards/jarryBlue.jpg", CardColor.Blue, 2),
                new ActionCard("Джерри", actionCardsPath + "doubleCards/jarryGreen.jpg", CardColor.Green, 2),
                new ActionCard("Джерри", actionCardsPath + "doubleCards/jarryRed.jpg", CardColor.Red, 2),
                new ActionCard("Морти", actionCardsPath + "tripleCards/mortyBlue.jpg", CardColor.Blue, 3),
                new ActionCard("Морти", actionCardsPath + "tripleCards/mortyGreen.jpg", CardColor.Green, 3),
                new ActionCard("Морти", actionCardsPath + "tripleCards/mortyRed.jpg", CardColor.Red, 3),
                new ActionCard("Саммер", actionCardsPath + "tripleCards/summerBlue.jpg", CardColor.Blue, 3),
                new ActionCard("Саммер", actionCardsPath + "tripleCards/summerGreen.jpg", CardColor.Green, 3),
                new ActionCard("Саммер", actionCardsPath + "tripleCards/summerRed.jpg", CardColor.Red, 3),
                new ActionCard("Что-то не так, Бет?", actionCardsPath + "tripleCards/white4.jpg", CardColor.None, 3),
                new ActionCard("Стерва-сеструха", actionCardsPath + "tripleCards/white5.jpg", CardColor.None, 3),
                new ActionCard("Ты не можешь меня убить!", actionCardsPath + "tripleCards/white6.jpg", CardColor.None, 3),
                new ActionCard("Бет", actionCardsPath + "fiveCards/betBlue.jpg", CardColor.Blue, 5),
                new ActionCard("Бет", actionCardsPath + "fiveCards/betGreen.jpg", CardColor.Green, 5),
                new ActionCard("Бет", actionCardsPath + "fiveCards/betRed.jpg", CardColor.Red, 5),
                new ActionCard("Рик", actionCardsPath + "sixCards/rick.jpg", CardColor.None, 6),
            };

            var characterCardsPath = AppDomain.CurrentDomain.BaseDirectory + "/images/characterCards/";
            allCharacterCards = new CharacterCard[]
            {
                new CharacterCard("Мистер Борегар", characterCardsPath + "blueCards/characterBlue1.jpg", CardColor.Blue),
                new CharacterCard("Мистер Жопосранчик", characterCardsPath + "blueCards/characterBlue2.jpg", CardColor.Blue),
                new CharacterCard("Рыболюд", characterCardsPath + "blueCards/characterBlue3.jpg", CardColor.Blue),
                new CharacterCard("Большой робот", characterCardsPath + "blueCards/characterBlue4.jpg", CardColor.Blue),
                new CharacterCard("Мясомурай", characterCardsPath + "blueCards/characterBlue5.jpg", CardColor.Blue),
                new CharacterCard("Мужикорог", characterCardsPath + "blueCards/characterBlue6.jpg", CardColor.Blue),
                new CharacterCard("Призрак в банке", characterCardsPath + "blueCards/characterBlue7.jpg", CardColor.Blue),
                new CharacterCard("Тинклз", characterCardsPath + "blueCards/characterBlue8.jpg", CardColor.Blue),
                new CharacterCard("Бездомная кукла", characterCardsPath + "greenCards/characterGreen1.jpg", CardColor.Green),
                new CharacterCard("Мальчик из бассейна", characterCardsPath + "greenCards/characterGreen2.jpg", CardColor.Green),
                new CharacterCard("Сонный Гэри", characterCardsPath + "greenCards/characterGreen3.jpg", CardColor.Green),
                new CharacterCard("Чудовище Франкенштейна", characterCardsPath + "greenCards/characterGreen4.jpg", CardColor.Green),
                new CharacterCard("Ковбойчик на собаке", characterCardsPath + "greenCards/characterGreen5.jpg", CardColor.Green),
                new CharacterCard("Раптор-фотограф", characterCardsPath + "greenCards/characterGreen6.jpg", CardColor.Green),
                new CharacterCard("Большая утка", characterCardsPath + "greenCards/characterGreen7.jpg", CardColor.Green),
                new CharacterCard("Карандашурик", characterCardsPath + "greenCards/characterGreen8.jpg", CardColor.Green),
                new CharacterCard("Малыш-волшебник", characterCardsPath + "redCards/characterRed1.jpg", CardColor.Red),
                new CharacterCard("Надувной пёс", characterCardsPath + "redCards/characterRed2.jpg", CardColor.Red),
                new CharacterCard("Бородатая дама", characterCardsPath + "redCards/characterRed3.jpg", CardColor.Red),
                new CharacterCard("Кузен Никки", characterCardsPath + "redCards/characterRed4.jpg", CardColor.Red),
                new CharacterCard("Жираф наоборот", characterCardsPath + "redCards/characterRed5.jpg", CardColor.Red),
                new CharacterCard("Дядя Стив", characterCardsPath + "redCards/characterRed6.jpg", CardColor.Red),
                new CharacterCard("Миссис Холодильник", characterCardsPath + "redCards/characterRed7.jpg", CardColor.Red),
                new CharacterCard("Киборг-амиш", characterCardsPath + "redCards/characterRed8.jpg", CardColor.Red),
            };

            var personalityCardsPath = AppDomain.CurrentDomain.BaseDirectory + "/images/personalityCards/";

            allPersonalityCards = new PersonalityCard[]
            {
                new PersonalityCard("Паразит", personalityCardsPath + "enemy.jpg", 18),
                new PersonalityCard("Друг", personalityCardsPath + "friend.jpg", 6),
            };

            playerPersonalityCards = new PersonalityCard[]
            {
                new PersonalityCard("Паразит", personalityCardsPath + "enemy.jpg", 2),
                new PersonalityCard("Друг", personalityCardsPath + "friend.jpg", 4),
            };
        }

        private static List<T> GetFullPack<T>(T[] cards)
            where T : Card
        {
            var pack = new List<T>();
            foreach (var card in cards)
                for (int i = 0; i < card.InPackCount; i++)
                    pack.Add(card);
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
            return new CardsPack<PersonalityCard>().Init(GetFullPack(playerPersonalityCards));
        }
    }
}
