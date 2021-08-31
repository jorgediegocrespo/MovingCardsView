using System;
using Xamarin.Forms;

namespace MovingCardsView
{
    public class CustomCarouselView : PanCardView.CarouselView
    {
        public event EventHandler<PanUpdatedEventArgs> CustomPanUpdated;
        public bool IsCustomizingPanGesture { get; set; }

        public void OnCustomPanUpdated(PanUpdatedEventArgs args)
        {
            CustomPanUpdated?.Invoke(this, args);
        }
    }
}
