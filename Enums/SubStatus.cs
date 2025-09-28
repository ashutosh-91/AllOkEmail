using System.ComponentModel;

namespace AllOkEmail.Enums
{
    public enum SubStatus
    {
        [Description("Disposable")]
        Disposable,
        [Description("Blacklisted")]
        Blacklisted,
        [Description("Valid")]
        Valid,
        [Description("InValidEmail")]
        InValidEmail,
        [Description("MxNotFound")]
        MxNotFound
    }
}
