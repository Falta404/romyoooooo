using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR_Domain.Entities;

namespace QR.Application.DTOs
{
    public class GetAllUsersDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ResturantName { get; set; } = string.Empty;

        public GetAllUsersDto(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            PhoneNumber = user.PhoneNumber;
            IsActive = user.IsActive;
        }
    }
}
