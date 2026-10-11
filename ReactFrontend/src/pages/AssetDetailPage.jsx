import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import API from '../services/api';
import StatusBadge from '../components/StatusBadge';
import { ArrowLeft, Laptop, Calendar, DollarSign, ShieldCheck, MapPin, History, User } from 'lucide-react';

const AssetDetailPage = () => {
  const { id } = useParams();
  const [asset, setAsset] = useState(null);
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchAssetDetails();
  }, [id]);

  const fetchAssetDetails = async () => {
    try {
      setLoading(true);
      const [assetRes, histRes] = await Promise.all([
        API.get(`/assets/${id}`),
        API.get(`/asset-history/asset/${id}`)
      ]);
      setAsset(assetRes.data);
      setHistory(histRes.data);
    } catch (err) {
      console.error('Error fetching asset details', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="text-center py-5 text-muted">Loading asset specification and audit trail...</div>;
  }

  if (!asset) {
    return (
      <div className="text-center py-5">
        <h5>Asset not found</h5>
        <Link to="/admin/assets" className="btn btn-link">Back to Catalog</Link>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-4">
        <Link to="/admin/assets" className="text-muted text-decoration-none fw-medium d-inline-flex align-items-center gap-1 mb-2">
          <ArrowLeft size={16} /> Back to Assets Catalog
        </Link>
        <div className="d-flex align-items-center justify-content-between">
          <div className="d-flex align-items-center gap-3">
            <div className="bg-primary text-white p-3 rounded-3">
              <Laptop size={28} />
            </div>
            <div>
              <h4 className="fw-bold mb-1">{asset.assetName}</h4>
              <div className="d-flex align-items-center gap-2">
                <span className="badge bg-light text-dark border font-monospace">{asset.assetTag}</span>
                <span className="text-muted">•</span>
                <span className="text-secondary">{asset.brand} {asset.model}</span>
              </div>
            </div>
          </div>
          <StatusBadge status={asset.status} type="asset" />
        </div>
      </div>

      <div className="row g-4 mb-4">
        {/* Specs Overview */}
        <div className="col-12 col-lg-5">
          <div className="card-custom p-4 h-100">
            <h6 className="fw-bold mb-3 border-bottom pb-2">Asset Specifications</h6>
            
            <div className="d-flex flex-column gap-3" style={{ fontSize: '0.9rem' }}>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Category:</span>
                <span className="fw-semibold">{asset.categoryName}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Serial Number:</span>
                <span className="font-monospace fw-semibold">{asset.serialNumber}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Purchase Date:</span>
                <span className="fw-semibold">{new Date(asset.purchaseDate).toLocaleDateString()}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Purchase Cost:</span>
                <span className="fw-semibold">${asset.purchaseCost.toFixed(2)}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Warranty Expiry:</span>
                <span className="fw-semibold">
                  {asset.warrantyExpiry ? new Date(asset.warrantyExpiry).toLocaleDateString() : 'N/A'}
                </span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Location:</span>
                <span className="fw-semibold">{asset.location || 'N/A'}</span>
              </div>
              <div className="d-flex justify-content-between">
                <span className="text-muted">Currently Assigned To:</span>
                <span className="fw-semibold text-primary">
                  {asset.currentEmployeeName || 'Not Assigned'}
                </span>
              </div>
              {asset.description && (
                <div className="pt-2 border-top">
                  <span className="text-muted d-block mb-1">Description / Notes:</span>
                  <p className="text-secondary mb-0" style={{ fontSize: '0.85rem' }}>{asset.description}</p>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Chronological Audit Trail Timeline */}
        <div className="col-12 col-lg-7">
          <div className="card-custom p-4 h-100">
            <div className="d-flex align-items-center justify-content-between mb-4 border-bottom pb-2">
              <h6 className="fw-bold mb-0">Asset Audit & Assignment History</h6>
              <span className="badge bg-secondary-subtle text-secondary">{history.length} Events Logged</span>
            </div>

            <div className="timeline">
              {history.length > 0 ? (
                history.map(h => (
                  <div key={h.historyId} className="timeline-item">
                    <div className="timeline-dot"></div>
                    <div className="d-flex align-items-center justify-content-between mb-1">
                      <span className="fw-bold text-dark">{h.action}</span>
                      <small className="text-muted">{new Date(h.date).toLocaleString()}</small>
                    </div>
                    <div className="text-secondary mb-1" style={{ fontSize: '0.875rem' }}>{h.description}</div>
                    <div className="text-muted" style={{ fontSize: '0.75rem' }}>
                      <User size={12} className="me-1" /> Performed by: <strong>{h.performedBy}</strong>
                    </div>
                  </div>
                ))
              ) : (
                <div className="text-center py-4 text-muted">No audit history recorded yet.</div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AssetDetailPage;
