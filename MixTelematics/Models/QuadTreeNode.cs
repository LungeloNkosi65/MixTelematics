using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MixTelematics.Models
{
    public class QuadTreeNode
    {
        public VehiclePosition Vehicle { get; set; }
        public QuadTreeNode Left { get; set; }
        public QuadTreeNode Right { get; set; }
    }
}
