﻿using System.Collections;
using System.ComponentModel;

namespace GpxViewer2.Controls.PropertyGrid.Mvvm;

internal class ValidatableViewModelBase : PropertyChangedBase, INotifyDataErrorInfo
{
    private static readonly string[] NO_ERRORS = [];

    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new();

    /// <inheritdoc />
    public virtual IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == null)
        {
            return NO_ERRORS;
        }

        if (_errorsByPropertyName.TryGetValue(propertyName, out var errorList))
        {
            return errorList;
        }

        return NO_ERRORS;
    }

    /// <inheritdoc />
    public bool HasErrors => _errorsByPropertyName.Count > 0;

    /// <inheritdoc />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    protected virtual void SetError(string propertyName, string error)
    {
        if (_errorsByPropertyName.TryGetValue(propertyName, out var errorList))
        {
            if (!errorList.Contains(error))
            {
                errorList.Add(error);
            }
        }
        else
        {
            _errorsByPropertyName.Add(propertyName, [error]);
        }

        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected virtual void RemoveErrors(string propertyName)
    {
        if (_errorsByPropertyName.ContainsKey(propertyName))
        {
            _errorsByPropertyName.Remove(propertyName);
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
