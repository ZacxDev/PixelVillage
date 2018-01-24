using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PixelVillage.Events
{

    interface EventListener
    { 
    }

    interface MouseEventListener : EventListener
    {
        void OnClickDown(Rectangle cursor);

        void OnClickUp(Rectangle cursor);

        void onMouseMove(Rectangle cursor);
    }
}
