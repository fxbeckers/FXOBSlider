using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;

namespace FXOBSliderDemo
{
	[Register("FXOBSlider")]
	public class FXOBSlider : UISlider
	{
		public static float[] DefaultScrubbingSpeeds = new [] { 1f, 0.5f, 0.25f, 0.1f };
		public static float[] DefaultScrubbingSpeedsChangePositions = new [] { 0f, 50f, 100f, 150f };
		
		public float ScrubbingSpeed { get; set; }
		public float[] ScrubbingSpeeds { get; set; }
		public float[] ScrubbingSpeedsChangePositions { get; set; }
		
		protected PointF beganTrackingLocation;
		protected float realPositionValue;
		
		public FXOBSlider (IntPtr ptr) : base (ptr) { SetDefaults (); }
		public FXOBSlider (RectangleF frame) : base(frame) { SetDefaults (); }
		
		protected void SetDefaults ()
		{
			this.ScrubbingSpeeds = DefaultScrubbingSpeeds;
			this.ScrubbingSpeedsChangePositions = DefaultScrubbingSpeedsChangePositions;
			this.ScrubbingSpeed = ScrubbingSpeeds.First();
		}
		
		public override bool BeginTracking (UITouch uitouch, UIEvent uievent)
		{
			bool beginTracking = base.BeginTracking (uitouch, uievent);
			if (beginTracking)
			{
				// Set the beginning tracking location to the centre of the current
				// position of the thumb. This ensures that the thumb is correctly re-positioned
				// when the touch position moves back to the track after tracking in one
				// of the slower tracking zones.
				RectangleF thumbRect = this.ThumbRectForBounds(this.Bounds,this.TrackRectForBounds(this.Bounds),this.Value);
				this.beganTrackingLocation = new PointF
					(
						thumbRect.Location.X + thumbRect.Size.Width / 2.0f, 
						thumbRect.Location.Y + thumbRect.Size.Height / 2.0f
						); 
				this.realPositionValue = this.Value;
			}
			return beginTracking;
		}
		
		public override bool ContinueTracking (UITouch uitouch, UIEvent uievent)
		{
			if (this.Tracking)
			{
				PointF previousLocation = uitouch.PreviousLocationInView(this);
				PointF currentLocation  = uitouch.LocationInView(this);
				float trackingOffset = currentLocation.X - previousLocation.X;
				
				// Find the scrubbing speed that curresponds to the touch's vertical offset
				float verticalOffset = Math.Abs(currentLocation.Y - this.beganTrackingLocation.Y);
				int scrubbingSpeedChangePosIndex = this.IndexOfLowerScrubbingSpeed(this.ScrubbingSpeedsChangePositions,verticalOffset);
				
				if (scrubbingSpeedChangePosIndex == NSRange.NotFound) {
					scrubbingSpeedChangePosIndex = ScrubbingSpeeds.Length;
				}
				this.ScrubbingSpeed = this.ScrubbingSpeeds[scrubbingSpeedChangePosIndex - 1];
				
				RectangleF trackRect = this.TrackRectForBounds(Bounds);
				this.realPositionValue = this.realPositionValue + (this.MaxValue - this.MinValue) * (trackingOffset / trackRect.Size.Width);
				
				float valueAdjustment = this.ScrubbingSpeed * (this.MaxValue - this.MinValue) * (trackingOffset / trackRect.Size.Width);
				float thumbAdjustment = 0.0f;
				if (((beganTrackingLocation.Y < currentLocation.Y) && (currentLocation.Y < previousLocation.Y)) ||
				    ((beganTrackingLocation.Y > currentLocation.Y) && (currentLocation.Y > previousLocation.Y)) )
				{
					// We are getting closer to the slider, go closer to the real location
					thumbAdjustment = (realPositionValue - Value) / (1 + Math.Abs(currentLocation.Y - beganTrackingLocation.Y));
				}
				Value += valueAdjustment + thumbAdjustment;
				
				if (this.Continuous) {
					this.SendActionForControlEvents(UIControlEvent.ValueChanged);
				}
			}
			return this.Tracking;
		}
		
		public override void EndTracking (UITouch uitouch, UIEvent uievent)
		{
			if (this.Tracking) 
			{
				this.ScrubbingSpeed = this.ScrubbingSpeeds.First();
				this.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		
		protected int IndexOfLowerScrubbingSpeed(float[] scrubbingSpeedPositions, float verticalOffset)
		{
			for (int i = 0; i < scrubbingSpeedPositions.Length; i++) {
				float scrubbingSpeedOffset = scrubbingSpeedPositions[i];
				if (verticalOffset < scrubbingSpeedOffset) {
					return i;
				}
			}
			return NSRange.NotFound; 
		}
	}
}

