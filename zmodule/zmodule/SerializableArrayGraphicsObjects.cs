using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace zmodule
{
   [Serializable]
     class SerializableArrayGraphicsObjects
    {   
        public SerializableArrayGraphicsObjects()
        {

        }
        
        public List<GraphicsObject>[] mas = new List<GraphicsObject>[12];

        public void setObject(int i, List<GraphicsObject> gObject)
        {
            mas[i] = gObject;
        }
        public List<GraphicsObject> getObject(int i)
        {
            return mas[i];
        }

    

    }
}
