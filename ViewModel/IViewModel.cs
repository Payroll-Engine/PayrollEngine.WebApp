using System;

namespace PayrollEngine.WebApp.ViewModel;

public interface IViewModel : IEquatable<IViewModel>
{
    int Id { get; set; }
    ObjectStatus Status { get; set; }
    DateTime Created { get; set; }
    DateTime Updated { get; set; }
}