import React, { useEffect, useState } from 'react';
import { getDashboardStats, type DashboardStats } from '../../services/landlordApi';
import {
    HomeIcon,
    CheckCircleIcon,
    CalendarIcon,
    ClockIcon
} from '@heroicons/react/24/outline';
import './Landlord.css';

export const Dashboard: React.FC = () => {
    const [stats, setStats] = useState<DashboardStats | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string>('');

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const data = await getDashboardStats();
                setStats(data);
            } catch (err: any) {
                setError(err.response?.data?.message || 'Failed to load dashboard stats');
            } finally {
                setLoading(false);
            }
        };

        fetchStats();
    }, []);

    if (loading) return <div className="landlord-container"><div className="spinner" style={{ margin: '2rem auto' }}></div></div>;
    if (error) return <div className="landlord-container"><div style={{ color: 'var(--error)' }}>Error: {error}</div></div>;

    return (
        <div className="landlord-container">
            <div className="landlord-header">
                <h1 className="landlord-title">Landlord Dashboard</h1>
            </div>

            <div className="dashboard-grid">
                <div className="stat-card">
                    <div className="stat-icon">
                        <HomeIcon width={32} height={32} />
                    </div>
                    <div className="stat-content">
                        <span className="stat-value">{stats?.totalRooms}</span>
                        <span className="stat-label">Total Rooms</span>
                    </div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon success">
                        <CheckCircleIcon width={32} height={32} />
                    </div>
                    <div className="stat-content">
                        <span className="stat-value">{stats?.activeRooms}</span>
                        <span className="stat-label">Active Rooms</span>
                    </div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon">
                        <CalendarIcon width={32} height={32} />
                    </div>
                    <div className="stat-content">
                        <span className="stat-value">{stats?.totalBookings}</span>
                        <span className="stat-label">Total Bookings</span>
                    </div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon warning">
                        <ClockIcon width={32} height={32} />
                    </div>
                    <div className="stat-content">
                        <span className="stat-value">{stats?.pendingBookings}</span>
                        <span className="stat-label">Pending Bookings</span>
                    </div>
                </div>
            </div>
        </div>
    );
};
