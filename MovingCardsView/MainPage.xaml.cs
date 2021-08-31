using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MovingCardsView
{
    public partial class MainPage : ContentPage
    {
        private List<string> imageSourceList;
        private PanGestureRecognizer iosPanGesture;
        private bool? panVertically = null;
        private bool isMovingCard = false;

        public MainPage()
        {
            InitializeComponent();
            imageSourceList = new List<string>
            {
                "https://jorgediegocrespo.files.wordpress.com/2019/11/iphonesafearea-e1574276852279.jpg",
                "https://jorgediegocrespo.files.wordpress.com/2020/11/alertdialogmain-e1605684413393.jpg",
                "https://jorgediegocrespo.files.wordpress.com/2020/11/chromecast-e1604991843219.jpg"
            };
            cvImages.ItemsSource = imageSourceList;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform == Device.iOS)
            {
                iosPanGesture = new PanGestureRecognizer();
                cvImages.GestureRecognizers.Add(iosPanGesture);
                iosPanGesture.PanUpdated += MoveCard;
            }
            else
            {
                cvImages.CustomPanUpdated += MoveCard;
            }
        }

        protected override void OnDisappearing()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                iosPanGesture.PanUpdated -= MoveCard;
            }
            else
            {
                cvImages.CustomPanUpdated -= MoveCard;
            }
            base.OnDisappearing();
        }

        private async void MoveCard(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                case GestureStatus.Running:
                    if (panVertically == null && Math.Abs(e.TotalY) > Math.Abs(e.TotalX))
                        panVertically = true;
                    else if (panVertically == null && Math.Abs(e.TotalY) < Math.Abs(e.TotalX))
                        panVertically = false;

                    if (panVertically == false)
                    {
                        if (!isMovingCard && Device.RuntimePlatform == Device.iOS)
                        {
                            isMovingCard = true;
                            cvImages.OnPanUpdated(new PanUpdatedEventArgs(GestureStatus.Completed, 0, e.TotalX, 0));
                        }
                        await DoCardAnimationAsync(e.TotalX);
                    }
                    break;
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    await FinishCardMovementAsync();
                    panVertically = null;
                    isMovingCard = false;
                    break;
            }
        }

        private async Task DoCardAnimationAsync(double totalX)
        {
            double absTotalX = Math.Abs(totalX);            double safeMargin = 20;            if (absTotalX <= safeMargin)                return;

            BlockCardAnimation(true);
            cvImages.IsCustomizingPanGesture = true;
            double x = absTotalX - safeMargin;            double factor = totalX < 0 ? -1 : 1;            double tanslationX = Device.RuntimePlatform == Device.iOS ?                                 (factor * x) :                                 (factor * x) /*+ frProfile.TranslationX*/;            await frCarousel.TranslateTo(tanslationX, 0, 1);
        }

        private void BlockCardAnimation(bool block)        {            if (Device.RuntimePlatform == Device.iOS)                Device.BeginInvokeOnMainThread(() =>                {                    cvImages.IsVerticalSwipeEnabled = !block;                    cvImages.IsPanInteractionEnabled = !block;                });        }

        private async Task FinishCardMovementAsync()
        {
            cvImages.IsCustomizingPanGesture = false;
            await frCarousel.TranslateTo(0, 0);            BlockCardAnimation(false);
        }
    }
}
