using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using blocke.circleimageview;

namespace CIV.Sample
{
	[Activity (Label = "CircleImageView-Xamarin", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var profileimage = FindViewById<CircleImageView>(Resource.Id.profile_image);
			var largeprofileimage = FindViewById<CircleImageView>(Resource.Id.large_profile_image);
			var fillimage = FindViewById<CircleImageView>(Resource.Id.fill_image);

			largeprofileimage.SetImageResource(Resource.Drawable.homer);
			largeprofileimage.SetBorderColor(Color.Blue);
			largeprofileimage.SetBorderWidth(20);
			largeprofileimage.SetBorderOverlay(true);

			fillimage.SetImageResource(Resource.Drawable.profile);
			fillimage.SetFillColor (Color.OrangeRed);
			fillimage.SetBorderColor (Color.ForestGreen);
			fillimage.SetBorderWidth (10);
			fillimage.SetBorderOverlay (true);

		}
	}
}


