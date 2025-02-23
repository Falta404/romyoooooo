using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QR_Domain.Exceptions;
using QR_Domain.Interfaces;

namespace QR_Domain.Validations
{
    public class UserValidations
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserValidations(IUnitOfWork unitOfWork) =>
            _unitOfWork = unitOfWork;

        public async Task ValidateUser(int userId)
        {
            if (!await _unitOfWork.UserRepository.isExistAsync(u => u.Id == userId))
                throw new BusinessException((int)HttpStatusCode.Conflict, "User is not exist");
        }

    }
}
