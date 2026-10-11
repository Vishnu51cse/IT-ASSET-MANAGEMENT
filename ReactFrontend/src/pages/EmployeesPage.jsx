import React, { useEffect, useState } from 'react';
import API from '../services/api';
import ConfirmModal from '../components/ConfirmModal';
import { UserPlus, Search, Edit2, UserX, CheckCircle, Mail, Phone, Building } from 'lucide-react';

const EmployeesPage = () => {
  const [employees, setEmployees] = useState([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);
  
  // Modal State
  const [showModal, setShowModal] = useState(false);
  const [editingEmp, setEditingEmp] = useState(null);
  const [formData, setFormData] = useState({
    employeeCode: '',
    name: '',
    email: '',
    department: '',
    designation: '',
    phone: '',
    status: 'Active'
  });
  const [errorMsg, setErrorMsg] = useState('');

  // Deactivate Modal State
  const [deactivateId, setDeactivateId] = useState(null);

  useEffect(() => {
    fetchEmployees();
  }, []);

  const fetchEmployees = async (query = '') => {
    try {
      setLoading(true);
      const res = await API.get('/employees', { params: { search: query } });
      setEmployees(res.data);
    } catch (err) {
      console.error('Failed to fetch employees', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (e) => {
    setSearch(e.target.value);
    fetchEmployees(e.target.value);
  };

  const handleOpenAdd = () => {
    setEditingEmp(null);
    setFormData({
      employeeCode: `EMP-00${employees.length + 1}`,
      name: '',
      email: '',
      department: 'Engineering',
      designation: 'Software Engineer',
      phone: '',
      status: 'Active'
    });
    setErrorMsg('');
    setShowModal(true);
  };

  const handleOpenEdit = (emp) => {
    setEditingEmp(emp);
    setFormData({
      employeeCode: emp.employeeCode,
      name: emp.name,
      email: emp.email,
      department: emp.department,
      designation: emp.designation,
      phone: emp.phone,
      status: emp.status
    });
    setErrorMsg('');
    setShowModal(true);
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      if (editingEmp) {
        await API.put(`/employees/${editingEmp.id}`, formData);
      } else {
        await API.post('/employees', formData);
      }
      setShowModal(false);
      fetchEmployees(search);
    } catch (err) {
      setErrorMsg(err.response?.data?.message || 'Error saving employee data.');
    }
  };

  const handleDeactivate = async () => {
    if (!deactivateId) return;
    try {
      await API.delete(`/employees/${deactivateId}`);
      setDeactivateId(null);
      fetchEmployees(search);
    } catch (err) {
      alert('Error deactivating employee');
    }
  };

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h4 className="fw-bold mb-1">Employee Directory</h4>
          <p className="text-muted mb-0" style={{ fontSize: '0.9rem' }}>Manage company employees and asset assignments</p>
        </div>
        <button onClick={handleOpenAdd} className="btn btn-primary d-flex align-items-center gap-2 fw-medium px-3 py-2 rounded-3">
          <UserPlus size={18} />
          <span>Add New Employee</span>
        </button>
      </div>

      {/* Search Bar */}
      <div className="card-custom p-3 mb-4">
        <div className="input-group">
          <span className="input-group-text bg-light border-end-0 text-muted">
            <Search size={18} />
          </span>
          <input
            type="text"
            className="form-control bg-light border-start-0 ps-0"
            placeholder="Search employees by name, code, department, email..."
            value={search}
            onChange={handleSearchChange}
          />
        </div>
      </div>

      {/* Employees Table */}
      <div className="card-custom p-4">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.875rem' }}>
            <thead className="table-light">
              <tr>
                <th>Code</th>
                <th>Employee Name</th>
                <th>Department</th>
                <th>Designation</th>
                <th>Phone</th>
                <th>Active Assets</th>
                <th>Status</th>
                <th className="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-muted">Loading employees...</td>
                </tr>
              ) : employees.length > 0 ? (
                employees.map(emp => (
                  <tr key={emp.id}>
                    <td><span className="badge bg-light text-dark border font-monospace">{emp.employeeCode}</span></td>
                    <td>
                      <div className="fw-semibold text-dark">{emp.name}</div>
                      <div className="text-muted" style={{ fontSize: '0.75rem' }}>{emp.email}</div>
                    </td>
                    <td>{emp.department}</td>
                    <td>{emp.designation}</td>
                    <td>{emp.phone || <span className="text-muted">—</span>}</td>
                    <td>
                      <span className="badge bg-primary-subtle text-primary px-2.5 py-1 rounded-pill">
                        {emp.activeAssignedAssetsCount} Assets
                      </span>
                    </td>
                    <td>
                      {emp.status === 'Active' ? (
                        <span className="badge badge-available px-2.5 py-1 rounded-pill">Active</span>
                      ) : (
                        <span className="badge badge-retired px-2.5 py-1 rounded-pill">Deactivated</span>
                      )}
                    </td>
                    <td className="text-end">
                      <button onClick={() => handleOpenEdit(emp)} className="btn btn-light btn-sm text-secondary me-1" title="Edit">
                        <Edit2 size={16} />
                      </button>
                      {emp.status === 'Active' && (
                        <button onClick={() => setDeactivateId(emp.id)} className="btn btn-light btn-sm text-danger" title="Deactivate">
                          <UserX size={16} />
                        </button>
                      )}
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="8" className="text-center py-4 text-muted">No employees found matching criteria.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Add / Edit Modal */}
      {showModal && (
        <div className="modal fade show d-block" style={{ backgroundColor: 'rgba(15, 23, 42, 0.5)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered modal-lg">
            <div className="modal-content border-0 shadow">
              <div className="modal-header border-bottom">
                <h5 className="modal-title fw-bold">{editingEmp ? 'Edit Employee Details' : 'Add New Employee'}</h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)}></button>
              </div>
              <form onSubmit={handleSave}>
                <div className="modal-body p-4">
                  {errorMsg && <div className="alert alert-danger py-2">{errorMsg}</div>}
                  <div className="row g-3">
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Employee Code</label>
                      <input
                        type="text"
                        className="form-control"
                        value={formData.employeeCode}
                        onChange={(e) => setFormData({ ...formData, employeeCode: e.target.value })}
                        disabled={!!editingEmp}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Full Name</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="John Doe"
                        value={formData.name}
                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Email Address</label>
                      <input
                        type="email"
                        className="form-control"
                        placeholder="john@company.com"
                        value={formData.email}
                        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Phone Number</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="+1-555-0199"
                        value={formData.phone}
                        onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Department</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Engineering"
                        value={formData.department}
                        onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                        required
                      />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label fw-semibold">Designation</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Senior Developer"
                        value={formData.designation}
                        onChange={(e) => setFormData({ ...formData, designation: e.target.value })}
                        required
                      />
                    </div>
                    {editingEmp && (
                      <div className="col-12 col-md-6">
                        <label className="form-label fw-semibold">Status</label>
                        <select
                          className="form-select"
                          value={formData.status}
                          onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                        >
                          <option value="Active">Active</option>
                          <option value="Deactivated">Deactivated</option>
                        </select>
                      </div>
                    )}
                  </div>
                </div>
                <div className="modal-footer border-top p-3">
                  <button type="button" className="btn btn-light" onClick={() => setShowModal(false)}>Cancel</button>
                  <button type="submit" className="btn btn-primary px-4">Save Employee</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Deactivate Confirm Modal */}
      <ConfirmModal
        show={!!deactivateId}
        title="Deactivate Employee"
        message="Are you sure you want to deactivate this employee? Their active login session and assignment privileges will be revoked."
        confirmText="Deactivate"
        confirmVariant="danger"
        onConfirm={handleDeactivate}
        onClose={() => setDeactivateId(null)}
      />
    </div>
  );
};

export default EmployeesPage;
