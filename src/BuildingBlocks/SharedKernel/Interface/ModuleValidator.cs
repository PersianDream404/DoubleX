//using FluentValidation;
//using System.Globalization;
//using System.Resources;

//namespace SharedKernel.Interface;

//public abstract class ModuleValidator<T> : AbstractValidator<T>
//{
//    private readonly ResourceManager _resourceManager;

//    protected ModuleValidator()
//    {
//        var assembly = typeof(T).Assembly;

//        var resourceName = $"{assembly.GetName().Name}.Resources.Validation";

//        _resourceManager = new ResourceManager(resourceName, assembly);
//    }

//    protected string Msg(string key)
//    {
//        return _resourceManager.GetString(key, CultureInfo.CurrentUICulture)
//               ?? $"[[{key}]]";
//    }

//    protected IRuleBuilderOptions<T, TProperty> Required<TProperty>(
//        IRuleBuilder<T, TProperty> rule)
//    {
//        return rule.NotEmpty()
//                   .WithMessage(Msg("NotEmpty"));
//    }

//    protected IRuleBuilderOptions<T, string> Email(
//        IRuleBuilder<T, string> rule)
//    {
//        return rule.EmailAddress()
//                   .WithMessage(Msg("Email"));
//    }

//    protected IRuleBuilderOptions<T, string> MaxLength(
//        IRuleBuilder<T, string> rule, int length)
//    {
//        return rule.MaximumLength(length)
//                   .WithMessage(Msg("MaximumLength"));
//    }
//}
