using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace zmodule
{   [Serializable]
    class SerializableGraphicsObject
    {
        public string type;
        public int x1, y1, x2, y2;
    }
}
