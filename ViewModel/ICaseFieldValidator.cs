
namespace PayrollEngine.WebApp.ViewModel;

public interface ICaseFieldValidator
{
    /// <summary>
    /// Validate case field
    /// </summary>
    /// <returns>Valid state if the case field is complete, otherwise return list of broken validation rules</returns>
    CaseObjectValidity Validate();

    /// <summary>
    /// validate case field start
    /// </summary>
    /// <returns>True if the case field start is valid</returns>
    bool ValidateStart();

    /// <summary>
    /// validate case field end
    /// </summary>
    /// <returns>True if the case field end is valid</returns>
    bool ValidateEnd();

    /// <summary>
    /// validate case field value
    /// </summary>
    /// <returns>True if the case field value is valid</returns>
    bool ValidateValue();

    /// <summary>
    /// validate case field attachment
    /// </summary>
    /// <returns>True if the case field attachment is valid</returns>
    bool ValidateAttachment();
}