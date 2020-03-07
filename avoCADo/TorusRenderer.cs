using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class TorusRenderer
    {




        public void GenerateVertices(int xDivisions, int yDivisions)
        {
            xDivisions = MathHelper.Clamp(xDivisions, 3, 30);
            yDivisions = MathHelper.Clamp(yDivisions, 3, 30);


        }

        private void CalculateVertex(float alpha, float beta)
        {

        }
    }
}
