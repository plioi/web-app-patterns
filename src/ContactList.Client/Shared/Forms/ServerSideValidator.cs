using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ContactList.Client.Shared.Forms;

public class ServerSideValidator : ComponentBase, IDisposable
{
    ValidationMessageStore? _messageStore;

    [CascadingParameter] EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ServerSideValidator)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(ServerSideValidator)} " +
                $"inside an {nameof(EditForm)}.");
        }

        _messageStore = new(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += HandleValidationRequested;
        CurrentEditContext.OnFieldChanged += HandleOnFieldChanged;
    }

    void HandleValidationRequested(object? s, ValidationRequestedEventArgs e)
    {
        _messageStore?.Clear();
    }

    void HandleOnFieldChanged(object? s, FieldChangedEventArgs e)
    {
        _messageStore?.Clear(e.FieldIdentifier);
    }

    public void SetErrors(Dictionary<string, List<string>> errors)
    {
        if (CurrentEditContext is not null)
        {
            foreach (var (key, value) in errors)
                _messageStore?.Add(CurrentEditContext.Field(key), value);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void Dispose()
    {
        if (CurrentEditContext is null)
            return;

        CurrentEditContext.OnValidationRequested -= HandleValidationRequested;
        CurrentEditContext.OnFieldChanged -= HandleOnFieldChanged;
    }
}
