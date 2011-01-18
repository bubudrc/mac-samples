
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.QuartzComposer;

namespace QCBackground
{
	public partial class SharedContentView : MonoMac.AppKit.NSView
	{
		
		NSImageView mover;
		
		#region Constructors

		// Called when created from unmanaged code
		public SharedContentView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SharedContentView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		public override void AwakeFromNib ()
		{
			WantsLayer = true;
			Layer = makeCompositionLayer();
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			if ((theEvent.ModifierFlags & NSEventModifierMask.AlphaShiftKeyMask) == NSEventModifierMask.AlphaShiftKeyMask
			     || (theEvent.ModifierFlags & NSEventModifierMask.ShiftKeyMask) == NSEventModifierMask.ShiftKeyMask)
			{
				NSAnimationContext.BeginGrouping();
				NSAnimationContext.CurrentContext.Duration = 2.0f;
			}
			
			if (theEvent.Characters.ToUpper()[0] == 'A') 
			{
				NSView view = Mover();
				if (view.Superview == null)
				{
					// there is a problem here with AddSubview with animator
					//((NSView)Animator).AddSubview(view);
					this.AddSubview(view);
				}
				else
				{
					((NSView)view.Animator).RemoveFromSuperview();
				}
			}
			else
				base.KeyDown(theEvent);
			
			if ((theEvent.ModifierFlags & NSEventModifierMask.AlphaShiftKeyMask) == NSEventModifierMask.AlphaShiftKeyMask
			     || (theEvent.ModifierFlags & NSEventModifierMask.ShiftKeyMask) == NSEventModifierMask.ShiftKeyMask)
				NSAnimationContext.EndGrouping();
		}
		
		private CALayer makeCompositionLayer()
		{
			QCCompositionRepository repo = QCCompositionRepository.SharedCompositionRepository;
			
			QCComposition composition = repo.GetComposition("/moving shapes");
			QCCompositionLayer compLayer = QCCompositionLayer.Create(composition);
			//CGColor cgColor = new CGColor(0.25f, 0.675f, 0.1f, 1.0f);
			NSColor nsColor = NSColor.FromCalibratedRgba(0.25f, 0.675f, 0.1f, 1.0f);
			
			string path = String.Format("patch.{0}.value",QCComposition.InputPrimaryColorKey);
			compLayer.SetValueForKeyPath(nsColor,(NSString)path);
			
			path = String.Format("patch.{0}.value",QCComposition.InputPaceKey);
			compLayer.SetValueForKeyPath(NSNumber.FromFloat(5.0f),(NSString)path);
			
			
			return compLayer;
		}
		
		private NSImageView Mover()
		{
			if (mover == null) 
			{
				
				mover = new NSImageView(Bounds);
				mover.ImageScaling = NSImageScale.AxesIndependently;
				mover.Image = NSImage.ImageNamed("photo.jpg");
				
				float xInset = 0.25f * Bounds.Width;
				float yInset = 0.25f * Bounds.Height;
				
				RectangleF moverFrame = Bounds.Inset(xInset,yInset);
				PointF origin = new PointF(Bounds.GetMidX() - moverFrame.Width / 2.0f,
				                           Bounds.GetMidY() - moverFrame.Height / 2.0f);
				moverFrame.Location = origin;
				
				mover.Frame = moverFrame;
				mover.AlphaValue = 0.5f;
				
			}
			return mover;
		}
		
	}
}

