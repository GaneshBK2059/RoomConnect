import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../features/auth/context/AuthContext';
import authService from '../features/auth/api/auth.service';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '../components/ui/Card';
import './AuthPages.css';
import { UserPlus } from 'lucide-react';

export const Register = () => {
    const [formData, setFormData] = useState({
        fullName: '',
        email: '',
        password: '',
        confirmPassword: '',
        phone: '',
        role: 'RENTER' // Default role
    });

    const [error, setError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        // Validation
        if (!formData.fullName || !formData.email || !formData.password) {
            setError('Please fill in all required fields');
            return;
        }

        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        try {
            setIsSubmitting(true);
            // First register
            await authService.register({
                fullName: formData.fullName,
                email: formData.email,
                password: formData.password,
                phone: formData.phone,
                role: formData.role
            });

            // Auto login after successful registration
            await login({
                email: formData.email,
                password: formData.password
            });

            navigate('/', { replace: true });
        } catch (err: any) {
            setError(err.message || 'Registration failed. Please try a different email.');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-background register-bg"></div>

            <div className="auth-content register-content">
                <Card className="auth-card">
                    <CardHeader>
                        <div className="auth-icon-wrapper">
                            <UserPlus className="auth-icon" size={28} />
                        </div>
                        <CardTitle>Create an Account</CardTitle>
                        <CardDescription>Enter your details below to get started</CardDescription>
                    </CardHeader>

                    <CardContent>
                        <form onSubmit={handleSubmit} className="auth-form">
                            {error && <div className="auth-alert">{error}</div>}

                            <Input
                                label="Full Name *"
                                name="fullName"
                                placeholder="John Doe"
                                value={formData.fullName}
                                onChange={handleChange}
                                disabled={isSubmitting}
                                required
                            />

                            <Input
                                label="Email *"
                                name="email"
                                type="email"
                                placeholder="m@example.com"
                                value={formData.email}
                                onChange={handleChange}
                                disabled={isSubmitting}
                                required
                            />

                            <div className="form-grid">
                                <Input
                                    label="Password *"
                                    name="password"
                                    type="password"
                                    placeholder="••••••••"
                                    value={formData.password}
                                    onChange={handleChange}
                                    disabled={isSubmitting}
                                    required
                                />

                                <Input
                                    label="Confirm Password *"
                                    name="confirmPassword"
                                    type="password"
                                    placeholder="••••••••"
                                    value={formData.confirmPassword}
                                    onChange={handleChange}
                                    disabled={isSubmitting}
                                    required
                                />
                            </div>

                            <div className="form-grid">
                                <Input
                                    label="Phone Number"
                                    name="phone"
                                    placeholder="555-0123"
                                    value={formData.phone}
                                    onChange={handleChange}
                                    disabled={isSubmitting}
                                />

                                <div className="input-wrapper">
                                    <label className="input-label">I want to *</label>
                                    <select
                                        name="role"
                                        className="input-field"
                                        value={formData.role}
                                        onChange={handleChange}
                                        disabled={isSubmitting}
                                    >
                                        <option value="RENTER">Find a Room (Renter)</option>
                                        <option value="LANDLORD">List a Room (Landlord)</option>
                                    </select>
                                </div>
                            </div>

                            <Button
                                type="submit"
                                fullWidth
                                size="lg"
                                isLoading={isSubmitting}
                                className="mt-4"
                            >
                                Create Account
                            </Button>
                        </form>
                    </CardContent>

                    <CardFooter className="auth-footer">
                        <p className="auth-footer-text">
                            Already have an account?{' '}
                            <Link to="/login" className="auth-link">
                                Sign in
                            </Link>
                        </p>
                    </CardFooter>
                </Card>
            </div>
        </div>
    );
};


