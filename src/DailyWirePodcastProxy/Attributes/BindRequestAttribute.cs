using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DailyWirePodcastProxy.Attributes;

public class BindRequestAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
{
    public BindingSource BindingSource  => BindingSource.Custom;
    
    public string? Name { get; set; }
}