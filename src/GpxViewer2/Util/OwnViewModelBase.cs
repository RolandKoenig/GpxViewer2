using CommunityToolkit.Mvvm.ComponentModel;
using GpxViewer2.ViewServices;
using RolandK.AvaloniaExtensions.Mvvm;
using RolandK.AvaloniaExtensions.ViewServices.Base;
using RolandK.InProcessMessaging;

namespace GpxViewer2.Util;

public class OwnViewModelBase : ObservableObject, IAttachableViewModel
{
    private object? _associatedView;
    private IEnumerable<MessageSubscription>? _messageSubscriptions;

    /// <inheritdoc />
    public event EventHandler<CloseWindowRequestEventArgs>? CloseWindowRequest;

    /// <inheritdoc />
    public event EventHandler<ViewServiceRequestEventArgs>? ViewServiceRequest;

    /// <inheritdoc />
    public object? AssociatedView
    {
        get => _associatedView;
        set
        {
            _associatedView = value;
            this.OnAssociatedViewChanged(_associatedView);
        }
    }

    protected T? TryGetViewService<T>()
        where T : class
    {
        var requestViewServiceArgs = new ViewServiceRequestEventArgs(typeof(T));
        this.ViewServiceRequest?.Invoke(this, requestViewServiceArgs);
        return requestViewServiceArgs.ViewService as T;
    }

    protected T GetViewService<T>()
        where T : class
    {
        var viewService = this.TryGetViewService<T>();
        if (viewService == null)
        {
            throw new InvalidOperationException($"ViewService {typeof(T).FullName} not found!");
        }

        return viewService;
    }

    protected void CloseHostWindow(object? dialogResult = null)
    {
        if (this.CloseWindowRequest == null)
        {
            throw new InvalidOperationException("Unable to call Close on host window!");
        }

        this.CloseWindowRequest.Invoke(
            this,
            new CloseWindowRequestEventArgs(dialogResult));
    }

    protected async void WrapWithErrorHandling(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            var srvErrorReporting = this.GetViewService<IErrorReportingViewService>();
            await srvErrorReporting.ShowErrorDialogAsync(ex);
        }
    }

    protected async Task WrapWithErrorHandlingAsync(Func<Task> asyncFunc)
    {
        try
        {
            await asyncFunc();
        }
        catch (Exception ex)
        {
            var srvErrorReporting = this.GetViewService<IErrorReportingViewService>();
            await srvErrorReporting.ShowErrorDialogAsync(ex);
        }
    }

    protected void GetService<TService>(out TService service)
        where TService : notnull
    {
        var srvServiceProvider = this.GetViewService<IServiceProviderViewService>();
        srvServiceProvider.GetService(out service);
    }

    protected IDisposable GetScopedService<TService>(out TService service)
        where TService : notnull
    {
        var srvServiceProvider = this.GetViewService<IServiceProviderViewService>();
        return srvServiceProvider.GetScopedUseCase(out service);
    }

    protected IDisposable GetScopedService<TService1, TService2>(out TService1 service1, out TService2 service2)
        where TService1 : notnull
        where TService2 : notnull
    {
        var srvServiceProvider = this.GetViewService<IServiceProviderViewService>();
        return srvServiceProvider.GetScopedUseCase(out service1, out service2);
    }

    protected virtual void OnAssociatedViewChanged(object? associatedView)
    {
        if (_messageSubscriptions != null)
        {
            _messageSubscriptions.UnsubscribeAll();
            _messageSubscriptions = null;
        }

        if (associatedView != null)
        {
            var srvServiceProvider = GetViewService<IServiceProviderViewService>();
            srvServiceProvider.GetService(out IInProcessMessageSubscriber srvMessageSubscriber);

            _messageSubscriptions = srvMessageSubscriber.SubscribeAllWeak(this);
        }
    }
}
