﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Fasetto.Word.Core;

namespace Fasetto.Word
{
    /// <summary>
    /// The base class for any content that is being used inside of a <see cref="DialogWindow"/>
    /// </summary>
    public class BaseDialogUserControl : UserControl
    {
        #region Private Members
        /// <summary>
        /// The dialog window we will be contained within
        /// </summary>
        private DialogWindow _dialogWindow;

        #endregion
        #region Public Properties

        /// <summary>
        /// The minimum width of this dialog
        /// </summary>
        public int WindowMinimumWidth { get; set; } = 250;
        /// <summary>
        /// The minimum height of this dialog
        /// </summary>
        public int WindowMinimumHeight { get; set; } = 100;

        /// <summary>
        /// The height of the title bar
        /// </summary>
        public int TitleHeight { get; set; } = 30;

        /// <summary>
        /// The title of this dialog
        /// </summary>
        public string Title { get; set; }
        #endregion
        #region Public Commands

        public ICommand CloseCommand { get; private set; }

        #endregion
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDialogUserControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //Create a new dialog
                _dialogWindow = new DialogWindow();
                _dialogWindow.ViewModel = new DialogWindowViewModel(_dialogWindow);

                //Create close command
                CloseCommand = new RelayCommand(()=>_dialogWindow.Close());
            }
        }
        #endregion
        #region Public Dialog Show Methods
        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">the view mode</param>
        /// <typeparam name="T">The viewmodel type for this control</typeparam>
        /// <returns></returns>
        public Task ShowDialog<T>(T viewModel)
            where T : BaseDialogViewModel
        {
            //Create a task to await the dialog closing
            var tcs = new TaskCompletionSource<bool>();

            //Run on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    //Match controls expected sizes to the dialog windows view model
                    _dialogWindow.ViewModel.WindowMinimumWidth = WindowMinimumWidth;
                    _dialogWindow.ViewModel.WindowMinimumHeight = WindowMinimumHeight;
                    _dialogWindow.ViewModel.TitleHeight = TitleHeight;
                    _dialogWindow.ViewModel.Title = string.IsNullOrEmpty(viewModel.Title)  ? Title : viewModel.Title;

                    //Set this control to the dialog window content
                    _dialogWindow.ViewModel.Content = this;

                    //Setup this controls data context binding to the view model
                    DataContext = viewModel;

                    //Show dialog
                    _dialogWindow.ShowDialog();
                }
                finally
                {
                    //Let caller know we finished
                    tcs.TrySetResult(true);
                }
            });
            return tcs.Task;
        }
        #endregion
    }
}