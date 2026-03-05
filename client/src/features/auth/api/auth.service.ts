import api from '../../../services/api';
import type { AuthResponse, LoginCredentials, RegisterCredentials } from '../types/auth';

class AuthService {
    /**
     * Register a new user
     */
    async register(data: RegisterCredentials): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/register', data);
        return response.data;
    }

    /**
     * Login a user
     */
    async login(data: LoginCredentials): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/login', data);
        return response.data;
    }

    /**
     * Ensure user's session is removed
     */
    async logout(): Promise<void> {
        try {
            await api.post('/auth/logout');
        } catch (error) {
            console.error('Logout error', error);
        } finally {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
        }
    }
}

export default new AuthService();


