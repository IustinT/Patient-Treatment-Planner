
using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Prism.Magician;
using Prism.Services.Dialogs;
using System.Windows.Input;
using System.Collections.Generic;
using ICU.Data.Models;

namespace ICU.Planner.ViewModels
{
    [ViewModelBase]
    [AutoInitialize]
    public partial class ViewModelBase : BindableBase
    {
        public static SystemConfig SystemConfig { get; set; }

        private const string ButtonTextOK = "OK";
        private const string CaptionError = "Error";

        /// <summary>
        /// Clears the is busy flag on the device main thread.
        /// </summary>
        protected void ClearIsBusy()
        {
            IsBusy = false;
            IsNotBusy = true;
            this.DeviceService.BeginInvokeOnMainThread(() => this.IsBusy = false);
            this.DeviceService.BeginInvokeOnMainThread(() => this.IsNotBusy = true);
        }

        /// <summary>
        /// Invoked when IsBusy changes.
        /// </summary>
        protected virtual void OnIsBusyChanged()
        {
        }

        /// <summary>
        /// Sets the is busy flag on the device main thread.
        /// </summary>
        protected void SetIsBusy()
        {
            IsBusy = true;
            IsNotBusy = false;
            this.DeviceService.BeginInvokeOnMainThread(() => this.IsBusy = true);
            this.DeviceService.BeginInvokeOnMainThread(() => this.IsNotBusy = false);
        }

        /// <summary>
        /// Handles the exception by clearing the IsBusy flag, then displaying an alert with the base exception message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>Task.</returns>
        protected virtual Task HandleException(Exception ex)
        {
            ClearIsBusy();
            try
            {
                var baseException = ex.GetBaseException();
                return PageDialogService.DisplayAlertAsync(CaptionError, baseException.Message, ButtonTextOK);
            }
            catch (Exception e)
            {
                //TODO: log exception

                Debugger.Log(1, "Crash", e.ToString());
                return Task.CompletedTask;
            }
        }

        private DelegateCommand<string> showDialogAsyncCommand;
        public ICommand ShowDialogAsyncCommand => showDialogAsyncCommand ??=
            new DelegateCommand<string>(async name => await ShowDialogAsync(name));
        public virtual Task<IDialogResult> ShowDialogAsync(string dialogName, DialogParameters parameters = null)
        => DialogService.ShowDialogAsync(dialogName, parameters ?? new DialogParameters());

        /// <summary>
        /// Command called when pressing the Close button on a Dialog Header
        /// </summary>
        private ICommand closeDialogButtonPressedCommand;

        public ICommand CloseDialogButtonPressedCommand => closeDialogButtonPressedCommand ??=
            new DelegateCommand(
                async () => await CloseDialogButtonPressedCommandExecute(),
                CloseDialogButtonPressedCommandCanExecute)
            .ObservesProperty(() => IsBusy);

        protected virtual bool CloseDialogButtonPressedCommandCanExecute() => true;
        protected virtual Task CloseDialogButtonPressedCommandExecute() => Task.CompletedTask;

        /// <summary>
        /// Displays an alert dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="buttonText">The button text.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentException">The title or message or buttonText was null or white space.</exception>
        protected async Task DisplayDialog(string title, string message, string buttonText = ButtonTextOK)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(message));
            }
            if (string.IsNullOrWhiteSpace(buttonText))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(buttonText));
            }
            try
            {
                await PageDialogService.DisplayAlertAsync(title, message, buttonText);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        /// <summary>
        /// Displays an alert dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="acceptButtonText">The accept button text.</param>
        /// <param name="cancellationButtonText">The cancellation button text.</param>
        /// <returns>Task&lt;bool&gt;.</returns>
        /// <exception cref="ArgumentException">The title or message or acceptButtonText or cancellationButtonText was null or white space.</exception>
        protected async Task<bool> DisplayDialog(string title, string message, string acceptButtonText, string cancellationButtonText)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(title));
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(message));
            }
            if (string.IsNullOrWhiteSpace(acceptButtonText))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(acceptButtonText));
            }
            if (string.IsNullOrWhiteSpace(cancellationButtonText))
            {
                throw new ArgumentException("Value cannot be null or white space.", nameof(cancellationButtonText));
            }
            try
            {
                return await PageDialogService.DisplayAlertAsync(title, message, acceptButtonText, cancellationButtonText);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
                return false;
            }
        }

    }
}