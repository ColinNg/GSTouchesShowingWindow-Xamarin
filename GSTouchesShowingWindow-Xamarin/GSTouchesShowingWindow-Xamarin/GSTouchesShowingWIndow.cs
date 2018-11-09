//
//  GSTouchesShowingWindow.cs
//  GSTouchesShowingWindow-Xamarin
//
//  Ported by Colin Ng on 2018-11-08. 
//  Copyright © 2018 Orbital Technologies Inc. All rights reserved.
//
//  Original Attribution as follows:
//
//  GSTouchesShowingWindow.swift
//  GSTouchesShowingWindow-Swift
//
//  Created by Lukas Petr on 6/16/17.
//  Copyright © 2017 Glimsoft. All rights reserved.
//

using UIKit;

namespace GSTouchesShowingWindowXamarin
{
    public class GSTouchesShowingWindow : UIWindow
    {
        readonly GSTouchesShowingController controller = new GSTouchesShowingController();

        override public void SendEvent(UIEvent evt)
        {
            var touches = evt.AllTouches;

            foreach (UITouch touch in touches)
            {
                switch (touch.Phase)
                {
                    case UITouchPhase.Began:
                        controller.TouchBegan(touch, view: this);
                        break;
                    case UITouchPhase.Moved:
                        controller.TouchMoved(touch, view: this);
                        break;
                    case UITouchPhase.Ended:
                        controller.TouchEnded(touch, view: this);
                        break;
                    case UITouchPhase.Cancelled:
                        break;
                }
            }
            base.SendEvent(evt);
        }
    }

}
