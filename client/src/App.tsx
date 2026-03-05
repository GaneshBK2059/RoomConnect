import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { AuthProvider } from './features/auth/context/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Navbar } from './components/layout/Navbar';
import { Footer } from './components/layout/Footer';
import { Home } from './pages/Home';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/landlord/Dashboard';
import { RoomsList } from './pages/landlord/RoomsList';
import { RoomForm } from './pages/landlord/RoomForm';
import { BookingsList } from './pages/landlord/BookingsList';
import './App.css';

// Create a layout component to handle conditional rendering based on route
const AppLayout = () => {
  const location = useLocation();
  const isLandlordRoute = location.pathname.startsWith('/landlord');

  return (
    <div className="app-container" style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', width: '100%' }}>
      <Navbar />
      <main className="main-content" style={{ flex: 1, width: '100%', maxWidth: '100%' }}>
        <Routes>
          {/* Public Routes */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          {/* Protected Routes */}
          <Route element={<ProtectedRoute roles={['LANDLORD', 'ADMIN']} />}>
            <Route path="/landlord/dashboard" element={<Dashboard />} />
            <Route path="/landlord/rooms" element={<RoomsList />} />
            <Route path="/landlord/rooms/create" element={<RoomForm />} />
            <Route path="/landlord/rooms/edit/:id" element={<RoomForm />} />
            <Route path="/landlord/bookings" element={<BookingsList />} />
          </Route>

          {/* Catch-all */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
      {/* Hide footer on landlord dashboard/management pages to maximize screen space */}
      {!isLandlordRoute && <Footer />}
    </div>
  );
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <AppLayout />
      </Router>
    </AuthProvider>
  );
}

export default App;
