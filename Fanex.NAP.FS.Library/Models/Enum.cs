namespace Fanex.NAP.FS.Library.Models
{
    public enum DeviceFlagged
    {
        /// <summary>
        /// New collected device.
        /// </summary>
        Blue,

        /// <summary>
        /// Normal case.
        /// </summary>
        Green,

        /// <summary>
        /// Device collected from an existing device (due to plugins changes or browser updates...).
        /// </summary>
        Gray,

        /// <summary>
        /// Device collected from Invalid token.
        /// </summary>
        Silver,

        /// <summary>
        /// Device collected from browserless transactions.
        /// </summary>
        Red,

        /// <summary>
        /// Device collected from browserless transactions due to Token is Empty
        /// </summary>
        RedEmptyToken,

        /// <summary>
        /// Device collected from browserless transactions due to Analysis is Empty
        /// </summary>
        RedEmptyAnalysis,

        /// <summary>
        /// Device collected from browserless transactions due to Js is disabled (so, Analysis = {})
        /// </summary>
        RedDisabledJsAnalysis,

        /// <summary>
        /// Device collected from browserless transactions due to Analysis was modified
        /// </summary>
        RedInvalidAnalysis,

        /// <summary>
        /// Device collected from browserless transactions due to Token is invalid
        /// </summary>
        RedInvalidToken,

        /// <summary>
        /// Device collected from browserless transactions due to DeviceCode is invalid
        /// </summary>
        RedInvalidDeviceCode
    }

    public enum TransactionHistoryLength
    {
        Hours24,
        Days7,
        Days30,
        Days180
    }
}