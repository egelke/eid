using System;
using System.Collections.Generic;
using System.Text;

namespace Egelke.Eid.Client
{
    public class CardEventArgs : EventArgs
    {
        public Card Card { get; private set; }

        public CardEventArgs(Card card)
        {
            this.Card = card;
        }
    }
}
