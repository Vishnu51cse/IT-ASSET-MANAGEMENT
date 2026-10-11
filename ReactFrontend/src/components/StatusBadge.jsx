import React from 'react';

const StatusBadge = ({ status, type = 'asset' }) => {
  if (!status) return null;

  let badgeClass = 'badge ';
  const lower = status.toLowerCase();

  if (type === 'asset') {
    if (lower === 'available') badgeClass += 'badge-available';
    else if (lower === 'assigned') badgeClass += 'badge-assigned';
    else if (lower === 'undermaintenance') badgeClass += 'badge-maintenance';
    else if (lower === 'retired') badgeClass += 'badge-retired';
    else badgeClass += 'bg-secondary';
  } else if (type === 'request') {
    if (lower === 'pending') badgeClass += 'badge-pending';
    else if (lower === 'approved' || lower === 'completed') badgeClass += 'badge-approved';
    else if (lower === 'rejected') badgeClass += 'badge-rejected';
    else badgeClass += 'bg-secondary';
  } else if (type === 'maintenance') {
    if (lower === 'completed') badgeClass += 'badge-approved';
    else if (lower === 'inprogress' || lower === 'open') badgeClass += 'badge-maintenance';
    else badgeClass += 'bg-secondary';
  }

  // Formatting display text (e.g., UnderMaintenance -> Under Maintenance)
  const displayText = status.replace(/([A-Z])/g, ' $1').trim();

  return (
    <span className={`${badgeClass} px-2.5 py-1 rounded-pill fw-medium`} style={{ fontSize: '0.8rem' }}>
      {displayText}
    </span>
  );
};

export default StatusBadge;
