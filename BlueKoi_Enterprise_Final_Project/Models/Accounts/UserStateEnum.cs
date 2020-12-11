using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models
{
    /// <summary>
    /// User state to check if the user is new, active, or banned
    /// </summary>
    public enum UserStateEnum
    {
        New = 0,
        Active = 1,
        Banned = 2
    }
}
