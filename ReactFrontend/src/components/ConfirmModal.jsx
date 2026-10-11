import React from 'react';

const ConfirmModal = ({ show, title, message, confirmText = 'Confirm', confirmVariant = 'primary', onConfirm, onClose }) => {
  if (!show) return null;

  return (
    <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow">
          <div className="modal-header border-bottom-0 pb-0">
            <h5 className="modal-title fw-semibold">{title}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body py-3">
            <p className="text-secondary mb-0">{message}</p>
          </div>
          <div className="modal-footer border-top-0 pt-0">
            <button type="button" className="btn btn-light text-secondary fw-medium" onClick={onClose}>
              Cancel
            </button>
            <button type="button" className={`btn btn-${confirmVariant} px-4 fw-medium`} onClick={onConfirm}>
              {confirmText}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ConfirmModal;
