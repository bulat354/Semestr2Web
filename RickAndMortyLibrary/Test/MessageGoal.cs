using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Test
{
    public enum MessageFirstGoal
    {
        None, Player, Timer, Character, Action, Person, Message, Voting,
        Dinner, Turn, GameOver, Round
    }

    public enum MessageSecondGoal
    {
        None, Stop, Add, Remove, Attach, Detach,
        Lose, Win, Show, Ready, Shuffle
    }

    //      None None - пустое сообщение

    //      Person Attach <личность> <игрок> - раскрыть личность игрока
    //      Person Attach <личность> - прикрепляет игроку его личность
    //  Person Show <карта> - показывает верхнюю колоду личностей
    //      Person Add <персонаж> <личность> - раскрывает личность персонажа
    //      Person Detach - открепляет личность от игрока

    //      Round None <игрок> - обозначить первого игрока раунда
    //      Turn None <игрок> - показывает что ходит игрок
    //      Message None <сообщение> - показывает у игрока сообщение
    //      Voting - голосование и ожидание его результата
    //      Player None - игрок должен выбрать другого игрока

    //      Action Show <игрок> <карта> - показывает карту игрока игрок, которую он выбрал в этом раунде
    //      Action Remove - скрывает выбранные карты игроков
    //      Action Attach <карта> - игрок в следующем ходу ходит картой карта
    //      Action None - игрок должен выбрать карту действия
    //      Action Add <действие> - добавляет карту действия игроку в руки

    //      Character None <персонаж1> <персонаж2> ... - игрок должен выбрать одного персонажа из списка
    //      Character Remove <персонаж> - удаляет персонажа из стола
    //      Character Detach <игрока> - открепляет персонажа от игрока
    //      Character Attach <персонаж> <игрок> - прикрепляет к игроку персонажа
    //      Character Add <персонаж> - добавляет персонажа в центр стола
    //  Character Shuffle <персонаж1> <персонаж2> ... - сообщает игроку, что идет перемешивание личностей указанных персонажей

    //      GameOver Lose или Win - сообщение о том, что игрок выиграл или проиграл
}
