using Business.Abstract;
using Business.Constans;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;
        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
        }

        public IResult CompanytExist(Company company)
        {
            var result = _companyService.CompanyExist(company);
            if (result.Success == false)
            {
                return new ErrorResult(Messages.CompanyAlreadyExist);
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user,int companId)
        {
            var claim = _userService.GetClaims(user, companId);
            var accessToken = _tokenHelper.CreateToken(user, claim, companId);
            return new SuccessDataResult<AccessToken>(accessToken);
        }

        public IDataResult<User> Login(UserForLogin userForLogin)
        {
            var usertoCheck = _userService.GetByMail(userForLogin.Email);
            if (usertoCheck == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userForLogin.Password, usertoCheck.PasswordHash, usertoCheck.PasswordSalt))
            {
                return new  ErrorDataResult<User>(Messages.PasswordError);
            }
            return new  SuccessDataResult<User>(usertoCheck, Messages.SuccessfulLogin);
        }

        public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password,Company company)
        {
            byte[] passwordHash, passweordSalt;
            HashingHelper.CreatePasswordHash(password,out passwordHash, out passweordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue= Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passweordSalt,
                Name = userForRegister.Name,
            };
            _userService.Add(user);
            _companyService.Add(company);
            //bu benim ilişkimi tanımlıyor 
            _companyService.UserCompanyAdd(user.Id, company.Id);
            UserCompanyDto userCompanyDto = new UserCompanyDto()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                AddedAt= user.AddedAt,
                IsActive = user.IsActive,
                CompanyId = company.Id,
                MailConfirm= user.MailConfirm,
                MailConfirmDate= user.MailConfirmDate,
                MailConfirmValue = user.MailConfirmValue,
                PasswordHash=user.PasswordHash,
                PasswordSalt=user.PasswordSalt,
            };
            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Messages.UserRegsiter);
        }

        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password)
        {
            byte[] passwordHash, passweordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passweordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passweordSalt,
                Name = userForRegister.Name,
            };
            _userService.Add(user);
            return new SuccessDataResult<User>(user, Messages.UserRegsiter);
        }

        public IResult UserExist(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult(Messages.UserAlready);
            }
            return new SuccessResult();
        }
    }
}
