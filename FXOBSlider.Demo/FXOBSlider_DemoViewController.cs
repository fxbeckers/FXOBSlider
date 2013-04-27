using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FXOBSliderDemo
{
	[Register("FXOBSlider_DemoViewController")]
	public class FXOBSlider_DemoViewController : UIViewController
	{
		[Outlet] FXOBSlider slider { get; set; }
		[Outlet] UILabel sliderValueLabel { get; set; }
		[Outlet] UILabel scrubberSpeedLabel { get; set; }

		public FXOBSlider_DemoViewController () : base ("FXOBSlider_DemoViewController", null) {}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.UpdateUI();
		}

		[Action("sliderValueDidChange:")] 
		void sliderValueDidChange(NSObject sender)
		{
			this.UpdateUI();
		}

		public void UpdateUI()
		{
			this.sliderValueLabel.Text = string.Format("Value: {0}",this.slider.Value.ToString("P1"));
			this.scrubberSpeedLabel.Text = string.Format("Scrubbing speed: {0}",this.slider.ScrubbingSpeed);
		}

		[Obsolete ("Deprecated in iOS 6.0")]
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			this.slider = null;
			this.sliderValueLabel = null;
			this.scrubberSpeedLabel = null;
		}
	}
}

