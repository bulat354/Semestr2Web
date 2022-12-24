using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using RickAndMortyLibrary.Common.Game;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RickAndMortyUI.Controls
{
    public class CardCollection : TemplatedControl
    {
        public ObservableCollection<CardInfo> Cards { get; set; } = new ObservableCollection<CardInfo>();

        public void Add(int id, IImage source)
        {
            var cardInfo = new CardInfo();
            cardInfo.Id = id;
            cardInfo.Source = source;

            Cards.Add(cardInfo);
        }

        public void Remove(int id)
        {
            Cards.Remove(new CardInfo() { Id = id });
        }

        public void SetImage(int id, IImage source)
        {
            var cardInfo = new CardInfo();
            cardInfo.Id = id;
            cardInfo.Source = source;

            var index = Cards.IndexOf(cardInfo);
            Cards[index] = cardInfo;
        }

        public void SetCardsClickable(bool isClickable)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];
                card.IsClickable = isClickable;
                Cards[i] = card;
            }
        }

        public class CardInfo
        {
            public int Id { get; set; }
            public IImage Source { get; set; }
            public bool IsVisible { get; set; } = true;
            public bool IsClickable { get; set; } = false;

            public CardInfo() { }
            public CardInfo(int id, IImage source, bool isVisible = true, bool isClickable = false)
            {
                Id = id;
                Source = source;
                IsVisible = isVisible;
                IsClickable = isClickable;
            }

            public override int GetHashCode()
            {
                return Id;
            }

            public override bool Equals(object? obj)
            {
                if (obj is CardInfo card)
                {
                    return card.Id == Id;
                }

                return false;
            }
        }
    }
}
