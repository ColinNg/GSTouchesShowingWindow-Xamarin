//
//  GSTouchesShowingController.cs
//  GSTouchesShowingController-Xamarin
//
//  Ported by Colin Ng on 2018-11-08. 
//  Copyright © 2018 Orbital Technologies Inc. All rights reserved.
//
//  Original Attribution as follows:
//
//  GSTouchesShowingController.swift
//  GSTouchesShowingWindow-Swift
//
//  Created by Lukas Petr on 8/25/17.
//  Copyright © 2017 Glimsoft. All rights reserved.
//

using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace GSTouchesShowingWindowXamarin
{
    class GSTouchesShowingController
    {

        GSTouchImageViewQueue touchImageViewQueue = new GSTouchImageViewQueue(touchesCount: 8);
        Dictionary<string, UIImageView> touchImgViewsDict = new Dictionary<string, UIImageView>();
        Dictionary<UITouch, DateTime> touchesStartDateMapTable = new Dictionary<UITouch, DateTime>();


        public void TouchBegan(UITouch touch, UIView view)
        {
            var touchImgView = touchImageViewQueue.PopTouchImageView();
            touchImgView.Center = touch.LocationInView(view);
            view.AddSubview(touchImgView);

            touchImgView.Alpha = 0.0f;
            touchImgView.Transform = CGAffineTransform.MakeScale(1.13f, 1.13f);
            SetTouchImageView(touchImgView, touch);

            UIView.Animate(duration: 0.1, animation: () =>
            {
                touchImgView.Alpha = 1.0f;
                touchImgView.Transform = CGAffineTransform.MakeScale(1, 1);
            });

            touchesStartDateMapTable[touch] = DateTime.Now;
        }

        public void TouchMoved(UITouch touch, UIView view)
        {
            TouchImageView(touch).Center = touch.LocationInView(view);
        }

        public void TouchEnded(UITouch touch, UIView view)
        {
            var touchStartDate = this.touchesStartDateMapTable[touch];
            var touchDuration = DateTime.Now - touchStartDate;
            touchesStartDateMapTable.Remove(touch);

            if (touchDuration < Constants.ShortTapTresholdDuration)
            {
                ShowExpandingCircle(touch.LocationInView(view), view);
            }

            var touchImgView = TouchImageView(touch);
            UIView.Animate(duration: 0.5, animation: () =>
            {
                touchImgView.Alpha = 0.0f;
                touchImgView.Transform = CGAffineTransform.MakeScale(1.13f, 1.13f);
            }, completion: () =>
             {
                 touchImgView.RemoveFromSuperview();
                 touchImgView.Alpha = 1.0f;
                 this.touchImageViewQueue.Push(touchImgView);
                 this.RemoveTouchImageView(touch);
             });
        }

        void ShowExpandingCircle(CGPoint position, UIView view)
        {
            var circleLayer = new CAShapeLayer();
            var initialRadius = Constants.ShortTapInitialCircleRadius;
            var finalRadius = Constants.ShortTapFinalCircleRadius;
            circleLayer.Position = new CGPoint(x: position.X - initialRadius, y: position.Y - initialRadius);

            var startPathRect = new CGRect(x: 0, y: 0, width: initialRadius * 2, height: initialRadius * 2);
            var startPath = UIBezierPath.FromRoundedRect(rect: startPathRect, cornerRadius: initialRadius);

            var endPathOrigin = initialRadius - finalRadius;
            var endPathRect = new CGRect(x: endPathOrigin, y: endPathOrigin, width: finalRadius * 2, height: finalRadius * 2);
            var endPath = UIBezierPath.FromRoundedRect(rect: endPathRect, cornerRadius: finalRadius);

            circleLayer.Path = startPath.CGPath;
            circleLayer.FillColor = UIColor.Clear.CGColor;
            circleLayer.StrokeColor = new UIColor(red: 0.0f / 255f, green: 135.0f / 255f, blue: 244.0f / 255f, alpha: 0.8f).CGColor;
            circleLayer.LineWidth = 2.0f;
            view.Layer.AddSublayer(circleLayer);

            CATransaction.Begin();
            CATransaction.CompletionBlock = () =>
            {
                circleLayer.RemoveFromSuperLayer();
            };

            // Expanding animation
            var expandingAnimation = CABasicAnimation.FromKeyPath("path");
            expandingAnimation.From = startPath;
            expandingAnimation.To = endPath;
            expandingAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
            expandingAnimation.Duration = 0.4f;
            expandingAnimation.RepeatCount = 1.0f;
            circleLayer.AddAnimation(expandingAnimation, key: "expandingAnimation");
            circleLayer.Path = endPath.CGPath;

            // Delayed fade out animation
            DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, TimeSpan.FromSeconds(0.20)), action: () =>
            {
                var fadingOutAnimation = CABasicAnimation.FromKeyPath("opacity");
                fadingOutAnimation.From = new NSNumber(1.0f);
                fadingOutAnimation.To = new NSNumber(0.0f);
                fadingOutAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
                fadingOutAnimation.Duration = 0.15f;
                circleLayer.AddAnimation(fadingOutAnimation, key: "fadeOutAnimation");
                circleLayer.Opacity = 0.0f;
            });

            CATransaction.Commit();
        }

        UIImageView TouchImageView(UITouch touch)
        {
            return touchImgViewsDict[$"{touch.GetHashCode()}"] ?? null;
        }

        void SetTouchImageView(UIImageView touchImageView, UITouch touch)
        {
            touchImgViewsDict[$"{touch.GetHashCode()}"] = touchImageView;
        }

        void RemoveTouchImageView(UITouch touch)
        {
            touchImgViewsDict.Remove($"{touch.GetHashCode()}");
        }
    }
}
