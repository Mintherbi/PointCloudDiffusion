using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudDiffusion.Models
{
    public interface IModel
    {
        string _Name { get; set }
        int _BatchSize { get; set; }
    }
}
