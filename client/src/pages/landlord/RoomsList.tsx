import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getLandlordRooms, deleteRoom, type Room } from '../../services/landlordApi';
import { useAuth } from '../../features/auth/context/AuthContext';
import { PlusIcon, MapPinIcon, TrashIcon, PencilSquareIcon } from '@heroicons/react/24/outline';
import './Landlord.css';

export const RoomsList: React.FC = () => {
    const { user } = useAuth();
    const [rooms, setRooms] = useState<Room[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string>('');

    useEffect(() => {
        if (user) {
            fetchRooms(Number(user.id));
        }
    }, [user]);

    const fetchRooms = async (userId: number) => {
        try {
            const data = await getLandlordRooms(userId);
            setRooms(data);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to load rooms');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (roomId: string) => {
        if (!window.confirm('Are you sure you want to delete this room?')) return;

        try {
            await deleteRoom(roomId);
            setRooms(rooms.filter(r => r.id !== roomId));
        } catch (err: any) {
            alert(err.response?.data?.message || 'Failed to delete room');
        }
    };

    if (loading) return <div className="landlord-container"><div className="spinner" style={{ margin: '2rem auto' }}></div></div>;
    if (error) return <div className="landlord-container"><div style={{ color: 'var(--error)' }}>Error: {error}</div></div>;

    return (
        <div className="landlord-container">
            <div className="landlord-header">
                <h1 className="landlord-title">My Rooms</h1>
                <Link to="/landlord/rooms/create" className="btn-approve" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', padding: '0.75rem 1.5rem', textDecoration: 'none' }}>
                    <PlusIcon width={20} height={20} />
                    <span>Create New Room</span>
                </Link>
            </div>

            {rooms.length === 0 ? (
                <div style={{ textAlign: 'center', padding: '4rem', background: 'var(--bg-card)', borderRadius: '12px', border: '1px dashed var(--border-color)' }}>
                    <h3 style={{ marginBottom: '1rem', color: 'var(--text-muted)' }}>You haven't listed any rooms yet.</h3>
                    <Link to="/landlord/rooms/create" className="btn-approve" style={{ textDecoration: 'none', display: 'inline-block' }}>Add Your First Room</Link>
                </div>
            ) : (
                <div className="rooms-grid">
                    {rooms.map(room => (
                        <div key={room.id} className="room-card">
                            {room.imageUrl ? (
                                <img src={room.imageUrl} alt={room.title} className="room-image" />
                            ) : (
                                <div className="room-no-image">No Image Provided</div>
                            )}
                            <div className="room-content">
                                <h3 className="room-title" title={room.title}>{room.title}</h3>

                                <div className="room-location">
                                    <MapPinIcon width={16} height={16} />
                                    <span>{room.city}, {room.state}</span>
                                </div>

                                <div className="room-price-row">
                                    <div className="room-price">${room.price} <span style={{ fontSize: '0.875rem', fontWeight: 'normal', color: 'var(--text-muted)' }}>/night</span></div>
                                    <span className={`room-status ${room.isAvailable ? 'available' : 'unavailable'}`}>
                                        {room.isAvailable ? 'Listed' : 'Unlisted'}
                                    </span>
                                </div>

                                <div className="room-actions">
                                    <Link to={`/landlord/rooms/edit/${room.id}`} className="btn-edit" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.25rem' }}>
                                        <PencilSquareIcon width={16} height={16} /> Edit
                                    </Link>
                                    <button onClick={() => handleDelete(room.id)} className="btn-delete" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.25rem' }}>
                                        <TrashIcon width={16} height={16} /> Delete
                                    </button>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};
