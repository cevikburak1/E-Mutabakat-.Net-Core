using Business.Abstract;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
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
        private readonly IMailService _mailService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IUserOperationClaimService _userOperationClaimService;
        private readonly IOperationClaimService _operationClaimService;
        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService, IMailService mailService, IMailParameterService mailParameterService, IMailTemplateService mailTemplateService, IUserOperationClaimService userOperationClaimService, IOperationClaimService operationClaimService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
            _mailService = mailService;
            _mailParameterService = mailParameterService;
            _mailTemplateService = mailTemplateService;
            _userOperationClaimService = userOperationClaimService;
            _operationClaimService = operationClaimService;
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

        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userService.GetById(id));
        }

        public IDataResult<User> GetByMailConfirmValue(string value)
        {
            return new SuccessDataResult<User>(_userService.GetByMailConfirmValue(value));
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

        [TransactionScopeAspect]
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
            //Validasyon İşlemleri 
            //ValidationTool.Validate(new UserValidator(), user);
            //ValidationTool.Validate(new CompanyValidation(), company);

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
            SendConfirmEmail(user);

            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Messages.UserRegsiter);
        }

        void SendConfirmEmail(User user)
        {
            string subject = "Kullanıcı Kayıt Onay Maili";
            string body = "Kullanıcı sisteme Kayıt oldu. Kaydı Tamamlamak için aşşagıdaki linke tıklaynız :)";
            string link = "https://localhost:7030/api/Auth/confirmuser?value=" + user.MailConfirmValue;
            string linkDescripiton = "Kaydı Onaylamak İçin Tıklayın";

            var mailtemplate = _mailTemplateService.GetByTemplateName("Kayıt", 3);
            string templatebody = mailtemplate.Data.Value;
            templatebody = templatebody.Replace("{{title}}", subject);
            templatebody = templatebody.Replace("{{message}}", body);
            templatebody = templatebody.Replace("{{link}}", link);
            templatebody = templatebody.Replace("{{linkDescription}}", linkDescripiton);
            var mailparameter = _mailParameterService.Get(3);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailparameter.Data,
                Email = user.Email,
                Subject = "Kullanıcı Kayıt Onay Maili",
                Body = templatebody,

            };
            _mailService.SendMail(sendMailDto);
            user.MailConfirmDate = DateTime.Now;
            _userService.Update(user);
        }

        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password,int companyId)
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
            _companyService.UserCompanyAdd(user.Id, companyId);
            SendConfirmEmail(user);
            return new SuccessDataResult<User>(user, Messages.UserRegsiter);
        }

        public IResult Update(User user)
        {
            _userService.Update(user);
            return new SuccessResult(Messages.UserMailConfirmSuccessful);
        }

        public IResult UserExist(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult(Messages.UserAlready);
            }
            return new SuccessResult();
        }

        IResult IAuthService.SendConfirmEmail(User user)
        {
            if (user.MailConfirm == true)
            {
                return new ErrorResult(Messages.MailAlreadyConfirm);
            }
            DateTime confirmMailDate = user.MailConfirmDate;
            DateTime now = DateTime.Now;
            if(confirmMailDate.ToShortDateString() == now.ToShortDateString())
            {
                if(confirmMailDate.Hour== now.Hour &&confirmMailDate.AddMinutes(5).Minute <= now.Minute)
                {
                    SendConfirmEmail(user);
                    return new SuccessResult(Messages.MailConfirmSendSuccessful);
                }
                else
                {
                    return new ErrorResult(Messages.MailConfirmExpressionNotDone);
                }
            }
            SendConfirmEmail(user);
            return new SuccessResult(Messages.MailConfirmSendSuccessful);
        }

        public IDataResult<UserCompany> GetCompany(int userid)
        {
            return new SuccessDataResult<UserCompany>(_companyService.GetCompany(userid).Data);
        }
    }
}
