using QR_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Application.DTOs
{
    public class SearchByUserNameDto
    {
        public int Id {  get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ResturantName {  get; set; } = string.Empty;
        public SearchByUserNameDto(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            PhoneNumber = user.PhoneNumber;
        }

    }

    
}
