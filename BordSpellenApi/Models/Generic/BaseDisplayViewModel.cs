using Core.Domain.Dto;

namespace BordSpellen.Models.Generic;

public abstract class BaseViewModel<T, TViewModel> where T: DtoBase where TViewModel: BaseViewModel<T, TViewModel>
{
    public int Id { get; set; }
    
    public abstract T ToDto(TViewModel model);
}