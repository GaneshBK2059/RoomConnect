import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { User, LoginCredentials } from '../types/auth';
import authService from '../api/auth.service';

interface AuthContextType {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (credentials: LoginCredentials) => Promise<void>;
    logout: () => void;
    setUser: (user: User) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUserState] = useState<User | null>(null);
    const [token, setTokenState] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        // Attempt to load existing user and token from localStorage on initial run
        const storedToken = localStorage.getItem('token');
        const storedUser = localStorage.getItem('user');

        if (storedToken && storedUser) {
            try {
                setTokenState(storedToken);
                setUserState(JSON.parse(storedUser));
            } catch (err) {
                console.error('Failed to parse user from local storage:', err);
                authService.logout();
            }
        }

        setIsLoading(false);
    }, []);

    const login = async (credentials: LoginCredentials) => {
        const res = await authService.login(credentials);
        if (res.success && res.token && res.user) {
            setTokenState(res.token);
            setUserState(res.user);
            localStorage.setItem('token', res.token);
            localStorage.setItem('user', JSON.stringify(res.user));
        } else {
            throw new Error(res.message || 'Login failed');
        }
    };

    const logout = () => {
        authService.logout();
        setTokenState(null);
        setUserState(null);
    };

    const setUser = (newUser: User) => {
        setUserState(newUser);
        localStorage.setItem('user', JSON.stringify(newUser));
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                token,
                isAuthenticated: !!token,
                isLoading,
                login,
                logout,
                setUser,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

