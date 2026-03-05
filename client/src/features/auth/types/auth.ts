export interface User {
    id: number;
    fullName: string;
    email: string;
    phone?: string;
    role: string;
    isActive: boolean;
    emailVerified: boolean;
    avatarUrl?: string;
    bio?: string;
}

export interface AuthResponse {
    success: boolean;
    message: string;
    token?: string;
    user?: User;
}

export interface LoginCredentials {
    email: string;
    password?: string; // made optional if needed, but required for login
}

export interface RegisterCredentials {
    fullName: string;
    email: string;
    password?: string;
    phone?: string;
    role?: string;
}
