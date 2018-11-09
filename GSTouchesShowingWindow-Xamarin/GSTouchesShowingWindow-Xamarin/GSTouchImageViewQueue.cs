using System.Collections.Generic;
using UIKit;
using System.Linq;

namespace GSTouchesShowingWindowXamarin
{
    public class GSTouchImageViewQueue
    {
        List<UIImageView> backingArray;

        public GSTouchImageViewQueue(int touchesCount)
        {
            backingArray = new List<UIImageView>();
            for (int i = 0; i < touchesCount; i++)
            {
                backingArray.Add(new UIImageView(new UIImage(Constants.TouchImageName)));
            }
        }

        public UIImageView PopTouchImageView()
        {
            var result = backingArray.First();
            backingArray.RemoveAt(0);
            return result;
        }

        public void Push(UIImageView touchImageView)
        {
            backingArray.Add(touchImageView);
        }
    }
}