import React, { useEffect, useState } from 'react';
import { getLandlordBookings, approveBooking, rejectBooking, type Booking } from '../../services/landlordApi';
import { CheckIcon, XMarkIcon } from '@heroicons/react/24/outline';
import './Landlord.css';

export const BookingsList: React.FC = () => {
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string>('');

    useEffect(() => {
        fetchBookings();
    }, []);

    const fetchBookings = async () => {
        try {
            const data = await getLandlordBookings();
            setBookings(data);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to load bookings');
        } finally {
            setLoading(false);
        }
    };

    const handleApprove = async (id: number) => {
        try {
            await approveBooking(id);
            setBookings(bookings.map(b => b.id === id ? { ...b, status: 'APPROVED' } : b));
        } catch (err: any) {
            alert(err.response?.data?.message || 'Failed to approve booking');
        }
    };

    const handleReject = async (id: number) => {
        if (!window.confirm('Are you sure you want to reject this booking?')) return;
        try {
            await rejectBooking(id);
            setBookings(bookings.map(b => b.id === id ? { ...b, status: 'REJECTED' } : b));
        } catch (err: any) {
            alert(err.response?.data?.message || 'Failed to reject booking');
        }
    };

    if (loading) return <div className="landlord-container"><div className="spinner" style={{ margin: '2rem auto' }}></div></div>;
    if (error) return <div className="landlord-container"><div style={{ color: 'var(--error)' }}>Error: {error}</div></div>;

    const getStatusClass = (status: string) => {
        switch (status) {
            case 'APPROVED': return 'badge-approved';
            case 'REJECTED': return 'badge-rejected';
            case 'CANCELLED': return 'badge-cancelled';
            default: return 'badge-pending';
        }
    };

    return (
        <div className="landlord-container">
            <div className="landlord-header">
                <h1 className="landlord-title">Booking Management</h1>
            </div>

            {bookings.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '4rem', background: 'var(--bg-card)', borderRadius: '12px', border: '1px dashed var(--border-color)' }}>
                    <h3 style={{ color: 'var(--text-muted)' }}>No bookings found for your rooms yet.</h3>
                </div>
            ) : (
                <div className="table-container">
                    <table className="modern-table">
                        <thead>
                            <tr>
                                <th>Booking ID</th>
                                <th>Room ID</th>
                                <th>Check-in</th>
                                <th>Check-out</th>
                                <th>Guests</th>
                                <th>Total Price</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {bookings.map(booking => (
                                <tr key={booking.id}>
                                    <td>#{booking.id}</td>
                                    <td title={booking.roomId} style={{ color: 'var(--text-muted)', fontFamily: 'monospace' }}>
                                        {booking.roomId.substring(0, 8)}...
                                    </td>
                                    <td>{new Date(booking.checkInDate).toLocaleDateString()}</td>
                                    <td>{new Date(booking.checkOutDate).toLocaleDateString()}</td>
                                    <td>{booking.numberOfGuests}</td>
                                    <td style={{ fontWeight: '600' }}>${booking.totalPrice}</td>
                                    <td>
                                        <span className={`status-badge ${getStatusClass(booking.status)}`}>
                                            {booking.status}
                                        </span>
                                    </td>
                                    <td>
                                        {booking.status === 'PENDING' && (
                                            <div className="action-buttons">
                                                <button onClick={() => handleApprove(booking.id)} className="btn-approve" style={{ display: 'flex', alignItems: 'center', gap: '0.25rem' }}>
                                                    <CheckIcon width={16} height={16} /> Approve
                                                </button>
                                                <button onClick={() => handleReject(booking.id)} className="btn-reject" style={{ display: 'flex', alignItems: 'center', gap: '0.25rem' }}>
                                                    <XMarkIcon width={16} height={16} /> Reject
                                                </button>
                                            </div>
                                        )}
                                        {booking.status !== 'PENDING' && (
                                            <span style={{ color: 'var(--text-muted)', fontSize: '0.85rem' }}>No actions</span>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};
