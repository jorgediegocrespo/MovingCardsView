using Android.Content;
using Android.Views;
using PanCardView.Droid;
using Xamarin.Forms;
using MovingCardsView;

[assembly: ExportRenderer(typeof(CustomCarouselView), typeof(MovingCardsView.Droid.CustomCarouselViewRenderer))]
namespace MovingCardsView.Droid
{
    public class CustomCarouselViewRenderer : CardsViewRenderer
    {
        private float? startX;
        private float? startY;

        public CustomCarouselViewRenderer(Context context) : base(context)
        { }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var args = GetPanUpdateEventArgs(e);
            if (args != null)
                (Element as CustomCarouselView)?.OnCustomPanUpdated(args);

            if ((Element as CustomCarouselView)?.IsCustomizingPanGesture ?? false)
                return true;
            else
                return base.OnTouchEvent(e);
        }

        private PanUpdatedEventArgs GetPanUpdateEventArgs(MotionEvent e)
        {
            var gesture = GetGestureStatus(e.ActionMasked);
            if (gesture == null)
                return null;

            if (gesture.Value == GestureStatus.Started)
            {
                startX = e.GetX();
                startY = e.GetY();
            }

            if (gesture.Value == GestureStatus.Completed)
            {
                startX = null;
                startY = null;
            }

            return new PanUpdatedEventArgs(gesture.Value, 0, GetTotalX(e) + Element.TranslationX, GetTotalY(e) + Element.TranslationY);
        }

        private GestureStatus? GetGestureStatus(MotionEventActions actionMasked)
        {
            switch (actionMasked)
            {
                case MotionEventActions.Down:
                    return GestureStatus.Started;
                case MotionEventActions.Move:
                    return GestureStatus.Running;
                case MotionEventActions.Up:
                    return GestureStatus.Completed;
                default:
                    return null;
            }
        }

        private float GetTotalX(MotionEvent ev) => (ev.GetX() - startX.GetValueOrDefault()) / Context.Resources.DisplayMetrics.Density;
        private float GetTotalY(MotionEvent ev) => (ev.GetY() - startY.GetValueOrDefault()) / Context.Resources.DisplayMetrics.Density;
    }
}
