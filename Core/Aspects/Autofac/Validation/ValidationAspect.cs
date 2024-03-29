﻿using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interception;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect:MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Hatalı Tip");
            }
            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {
           var validator =  (IValidator)Activator.CreateInstance(_validatorType);
           var entityType = _validatorType.BaseType.GetGenericArguments()[0];
            var entity = invocation.Arguments.Where(x => x.GetType() == entityType);
            foreach (var entities in entity)
            {
                ValidationTool.Validate(validator, entities);
            }
        }
    }
}
