using System;
using System.Windows.Forms;
using QLTN_LT.GUI.Base;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Base template for Add/Edit forms with common functionality.
    /// </summary>
    public abstract class FormTemplate : BaseForm
    {
        protected bool IsEditMode { get; set; }

        public FormTemplate()
        {
            UIHelper.ApplyFormStyle(this);
        }

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles save button click.
        /// </summary>
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (!ValidateInputs())
                {
                    ShowWarning("Vui lòng kiểm tra lại dữ liệu nhập.");
                    return;
                }

                // Show confirmation
                if (!ShowConfirm(IsEditMode ? "Bạn có chắc muốn cập nhật?" : "Bạn có chắc muốn thêm mới?"))
                {
                    return;
                }

                // Show loading
                Wait(true);

                try
                {
                    // Save data
                    SaveData();

                    // Show success
                    ShowInfo(IsEditMode ? "Cập nhật thành công!" : "Thêm mới thành công!");

                    // Close dialog
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                finally
                {
                    Wait(false);
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                ExceptionHandler.Handle(ex, "SaveData");
            }
        }

        /// <summary>
        /// Handles cancel button click.
        /// </summary>
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (ShowConfirm("Bạn có muốn hủy?"))
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "BtnCancel_Click");
            }
        }

        /// <summary>
        /// Handles delete button click.
        /// </summary>
        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsEditMode)
                {
                    ShowWarning("Chỉ có thể xóa trong chế độ chỉnh sửa.");
                    return;
                }

                if (!ShowConfirm("Bạn có chắc muốn xóa?", "Xác nhận xóa"))
                {
                    return;
                }

                Wait(true);

                try
                {
                    DeleteData();
                    ShowInfo("Xóa thành công!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                finally
                {
                    Wait(false);
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                ExceptionHandler.Handle(ex, "DeleteData");
            }
        }

        /// <summary>
        /// Deletes the data.
        /// </summary>
        protected virtual void DeleteData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes the form for add mode.
        /// </summary>
        protected virtual void InitializeAddMode()
        {
            IsEditMode = false;
            this.Text = "Thêm mới";
        }

        /// <summary>
        /// Initializes the form for edit mode.
        /// </summary>
        protected virtual void InitializeEditMode()
        {
            IsEditMode = true;
            this.Text = "Chỉnh sửa";
        }
    }
}

