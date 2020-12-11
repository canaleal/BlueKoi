using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Items
{
    /// <summary>
    /// Item state used to specify who owns the image/ who saved the image to the project
    /// </summary>
    public enum ItemSpecialOwnerEnum
    {
        User = 0,
        Admin = 1,
        Owner = 2
    }
}