import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../features/auth/context/AuthContext';
import { Button } from '../ui/Button';
import { LogOut, Home, User } from 'lucide-react';
import './Navbar.css';

export const Navbar = () => {
    const { user, isAuthenticated, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <nav className="navbar">
            <div className="navbar-container">
                <Link to="/" className="navbar-brand">
                    <Home className="navbar-logo-icon" size={24} />
                    <span className="navbar-logo-text">RoomConnect</span>
                </Link>

                <div className="navbar-links">
                    {isAuthenticated ? (
                        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                            {(user?.role === 'LANDLORD' || user?.role === 'ADMIN') && (
                                <>
                                    <Link to="/landlord/dashboard" style={{ textDecoration: 'none', color: '#555', fontWeight: '500' }}>Dashboard</Link>
                                    <Link to="/landlord/rooms" style={{ textDecoration: 'none', color: '#555', fontWeight: '500' }}>My Rooms</Link>
                                    <Link to="/landlord/bookings" style={{ textDecoration: 'none', color: '#555', fontWeight: '500' }}>Bookings</Link>
                                </>
                            )}
                            <div className="navbar-user-info">
                                <div className="avatar">
                                    {user?.fullName.charAt(0).toUpperCase() || <User size={16} />}
                                </div>
                                <span className="user-name">{user?.fullName}</span>
                            </div>
                            <Button variant="ghost" size="sm" onClick={handleLogout} className="logout-btn">
                                <LogOut size={18} className="mr-2" />
                                Logout
                            </Button>
                        </div>
                    ) : (
                        <>
                            <Link to="/login">
                                <Button variant="ghost">Sign In</Button>
                            </Link>
                            <Link to="/register">
                                <Button variant="primary">Sign Up</Button>
                            </Link>
                        </>
                    )}
                </div>
            </div>
        </nav>
    );
};


