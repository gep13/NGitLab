using System.Runtime.Serialization;

namespace NGitLab.Models
{
    public enum PackageType
    {
        [EnumMember(Value = "all")]
        All,
        [EnumMember(Value = "conan")]
        Conan,
        [EnumMember(Value = "maven")]
        Maven,
        [EnumMember(Value = "npm")]
        Npm,
        [EnumMember(Value = "pypi")]
        PyPI,
        [EnumMember(Value = "composer")]
        Composer,
        [EnumMember(Value = "nuget")]
        NuGet,
        [EnumMember(Value = "helm")]
        Helm,
        [EnumMember(Value = "terraform_module")]
        TerraformModule,
        [EnumMember(Value = "golang")]
        GoLang,
        [EnumMember(Value = "generic")]
        Generic,
    }
}
