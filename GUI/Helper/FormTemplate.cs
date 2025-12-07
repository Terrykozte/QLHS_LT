using System;
using System.Windows.Forms;
using QLTN_LT.GUI.Base;
using System.Diagnostics;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// FormTemplate - Lớp cơ sở cho các form Add/Edit
    /// - Xử lý validation chuẩn
    /// - Quản lý save/cancel/delete
    /// - Ngăn chặn double-close
    /// - Clean code OOP
    /// </summary>
    public abstract class FormTemplate : BaseForm
    {
        #region Properties

        protected bool IsEditMode { get; set; }
        private bool _isSaving = false;

        #endregion

        #region Constructor

        public FormTemplate()
        {
            try
            {
                UIHelper.ApplyFormStyle(this);
                this.CloseOnEsc = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing FormTemplate: {ex.Message}");
            }
        }

        #endregion

        #region Validation & Data Operations

        /// <summary>
        /// Validates all required fields.
        /// </summary>
        protected virtual bool ValidateInputs()
        {
            return true;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        protected virtual void SaveData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads data for edit mode.
        /// </summary>
        protected virtual void LoadData()
        {
            // Optional: override in derived classes
        }

        /// <summary>
        /// Deletes the data.
        /// </summary>
        protected virtual void DeleteData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Handles save button click.
        /// </summary>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            // Ngăn chặn double-click
            if (_isSaving)
                return;

            try
            {
                // Validate inputs
                if (!ValidateInputs())
                {
                    ShowWarning("Vui lòng kiểm tra lại dữ liệu nhập.");
                    return;
                }

                _isSaving = true;
                Wait(true);

                try
                {
                    // Save data
                    SaveData();

                    // Close dialog with OK result
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                finally
                {
                    Wait(false);
                    _isSaving = false;
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                _isSaving = false;
                ExceptionHandler.Handle(ex, "BtnSave_Click");
            }
        }

        /// <summary>
        /// Handles cancel button click.
        /// </summary>
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BtnCancel_Click: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles delete button click.
        /// </summary>
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_isSaving)
                return;

            try
            {
                if (!IsEditMode)
                {
                    ShowWarning("Chỉ có thể xóa trong chế độ chỉnh sửa.");
                    return;
                }

                if (!ShowConfirm("🗑️ Bạn có chắc muốn xóa?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                {
                    return;
                }

                _isSaving = true;
                Wait(true);

                try
                {
                    DeleteData();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                finally
                {
                    Wait(false);
                    _isSaving = false;
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                _isSaving = false;
                ExceptionHandler.Handle(ex, "BtnDelete_Click");
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the form for add mode.
        /// </summary>
        protected virtual void InitializeAddMode()
        {
            IsEditMode = false;
            this.Text = "➕ Thêm mới";
        }

        /// <summary>
        /// Initializes the form for edit mode.
        /// </summary>
        protected virtual void InitializeEditMode()
        {
            IsEditMode = true;
            this.Text = "✏️ Chỉnh sửa";
        }

        #endregion
    }
}

