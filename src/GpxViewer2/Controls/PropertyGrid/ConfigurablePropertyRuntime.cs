using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GpxViewer2.Controls.PropertyGrid.Mvvm;

namespace GpxViewer2.Controls.PropertyGrid;

internal class ConfigurablePropertyRuntime : ValidatableViewModelBase
{
    private readonly PropertyDescriptor _descriptor;
    private readonly ConfigurablePropertyMetadata _metadata;
    private readonly object _hostObject;

    public ConfigurablePropertyMetadata Metadata => _metadata;

    public object? ValueAccessor
    {
        get => this.GetValue();
        set
        {
            if (value != this.GetValue())
            {
                try
                {
                    this.SetValue(value);
                    this.RaisePropertyChanged(nameof(this.ValueAccessor));
                }
                catch (Exception e)
                {
                    this.SetError(nameof(this.ValueAccessor), e.Message);
                    this.RaisePropertyChanged(nameof(this.ValueAccessor));
                    return;
                }

                this.ValidateCurrentValue();
            }
        }
    }

    internal ConfigurablePropertyRuntime(PropertyDescriptor propertyInfo, object hostObject,
        IPropertyContractResolver? propertyContractResolver)
    {
        _descriptor = propertyInfo;
        _metadata = new ConfigurablePropertyMetadata(propertyInfo, hostObject.GetType(), propertyContractResolver);
        _hostObject = hostObject;

        this.ValidateCurrentValue();
    }

    public override string ToString()
    {
        return _metadata.ToString();
    }

    public object? GetValue()
    {
        return _descriptor.GetValue(_hostObject);
    }

    public void SetValue(object? value)
    {
        if (value == null)
        {
            var targetType = _descriptor.PropertyType;
            if (targetType.IsValueType)
            {
                _descriptor.SetValue(_hostObject, Activator.CreateInstance(targetType));
            }
            else
            {
                _descriptor.SetValue(_hostObject, null);
            }
        }
        else
        {
            var givenType = value.GetType();
            var targetType = _descriptor.PropertyType;
            if (givenType == targetType)
            {
                _descriptor.SetValue(_hostObject, value);
            }
            else
            {
                _descriptor.SetValue(_hostObject, Convert.ChangeType(value, targetType));
            }
        }
    }

    private void ValidateCurrentValue()
    {
        var errorsFound = false;
        var ctx = new ValidationContext(_hostObject);
        ctx.DisplayName = _metadata.PropertyDisplayName;
        ctx.MemberName = _metadata.PropertyName;
        foreach (var actAttrib in _descriptor.Attributes)
        {
            if (!(actAttrib is ValidationAttribute actValidAttrib))
            { continue; }

            var validationResult = actValidAttrib.GetValidationResult(this.ValueAccessor, ctx);
            if (validationResult != null)
            {
                this.SetError(nameof(this.ValueAccessor), validationResult.ErrorMessage ?? "Unknown");
                errorsFound = true;
            }
        }

        if (!errorsFound)
        {
            this.RemoveErrors(nameof(this.ValueAccessor));
        }
    }
}
